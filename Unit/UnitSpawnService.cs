using System.Collections.Generic;
using UnityEngine;
using System;

namespace TurnBasedStrategy
{
    // Handles spawning, teleporting, and refilling units for both factions
    public class UnitSpawnService : Singleton<UnitSpawnService>
    {
        [SerializeField] Transform faction0;
        [SerializeField] Transform faction1;
        [SerializeField] Transform prefabAlly;
        [SerializeField] Transform prefabEnemy;

        private int maxAllies = 6;

        public List<Mech> alliedMechs { get; private set; } = new List<Mech>();
        public List<Mech> enemyMechs { get; private set; } = new List<Mech>();

        public static event EventHandler OnUnitsTeleported;

        // Initializes player mechs with starter parts
        public List<Mech> InitPlayerMechs()
        {
            alliedMechs = new List<Mech>();

            for (int i = 0; i < maxAllies; i++)
            {
                var mechData = new Dictionary<PartType, PartData>
            {
                { PartType.Base, PartsManager.Instance.starterBase },
                { PartType.Torso, PartsManager.Instance.starterTorso },
                { PartType.WeaponPrimary, PartsManager.Instance.starterWeapon },
                { PartType.WeaponSecondary, PartsManager.Instance.starterWeapon  }
            };

                alliedMechs.Add(new Mech { mechData = mechData });
            }

            return alliedMechs;
        }

        public void SetPlayerMechs(List<Mech> mechs)
        {
            alliedMechs = mechs;
        }

        // Refills missing allies and spawns them
        public void RefillAllies()
        {
            alliedMechs.Clear();
            int currentCount = UnitCategoryService.Instance.allies.Count;
            int refillCount = maxAllies - currentCount;
            if (refillCount < 0) refillCount = 0;

            for (int i = 0; i < refillCount; i++)
            {
                var mechData = new Dictionary<PartType, PartData>
                {
                    { PartType.Base, PartsManager.Instance.starterBase },
                    { PartType.Torso, PartsManager.Instance.starterTorso },
                    { PartType.WeaponPrimary, PartsManager.Instance.starterWeapon },
                    { PartType.WeaponSecondary, PartsManager.Instance.starterWeapon }
                };
                alliedMechs.Add(new Mech { mechData = mechData });
            }

            if (refillCount > 0)
            {
                int startIndex = alliedMechs.Count - refillCount;
                var newMechs = alliedMechs.GetRange(startIndex, refillCount);

                SpawnData spawnDataAllies = SetSpawnData(0, faction0, Quaternion.identity, prefabAlly, refillCount);
                spawnDataAllies.mechs = newMechs;
                SpawnUnits(spawnDataAllies);
            }
        }

        // Teleports units to free spawn points
        public void TeleportToSpawnPoint(List<Unit> units)
        {
            foreach (var unit in units)
            {
                GridPosition currentGridPos = LevelGrid.Instance.GetGridPosition(unit.transform.position);
                LevelGrid.Instance.RemoveUnitAtGridPosition(currentGridPos, unit);
            }

            Transform[] allSpawnPoints = SetSpawnPoints(new SpawnData
            {
                factionTransform = faction0,
                count = maxAllies
            });

            var freeSpawnPoints = GetFreeSpawnPoints(allSpawnPoints, units.Count);

            for (int i = 0; i < units.Count && i < freeSpawnPoints.Count; i++)
            {
                units[i].transform.position = freeSpawnPoints[i].position;
                units[i].transform.rotation = freeSpawnPoints[i].rotation;

                GridPosition toGridPosition = LevelGrid.Instance.GetGridPosition(freeSpawnPoints[i].position);
                LevelGrid.Instance.AddUnitAtGridPosition(toGridPosition, units[i]);
            }

            OnUnitsTeleported?.Invoke(this, EventArgs.Empty);
        }

        // Spawns all allied units
        public void SpawnAllies()
        {
            int NumberOfAllies = alliedMechs.Count;
            SpawnData spawnDataAllies = SetSpawnData(0, faction0, Quaternion.identity, prefabAlly, NumberOfAllies);
            SpawnUnits(spawnDataAllies);
        }

        // Spawns all enemy units
        public void SpawnEnemies()
        {
            int NumberOfEnemies = MissionSetupManager.Instance.numberOfEnemies;
            SpawnData spawnDataEnemies = SetSpawnData(1, faction1, Quaternion.Euler(new Vector3(0, 180, 0)), prefabEnemy, NumberOfEnemies);
            SpawnUnits(spawnDataEnemies);
        }

        // Prepares spawn data for a faction
        private SpawnData SetSpawnData(int factionSide, Transform factionTransform, Quaternion rot, Transform prefab, int count)
        {
            SpawnData spawnData = new SpawnData();

            if (factionSide == 0)
            {
                spawnData.mechs = alliedMechs;
            }
            else
            {
                spawnData.mechs = InitEnemyMechs(count);
            }

            spawnData.factionSide = factionSide;
            spawnData.factionTransform = factionTransform;
            spawnData.rotation = rot;
            spawnData.prefab = prefab;
            spawnData.count = count;
            spawnData.spawnPoints = SetSpawnPoints(spawnData);

            return spawnData;
        }

        // Initializes enemy mechs with enemy parts
        public List<Mech> InitEnemyMechs(int count)
        {
            enemyMechs = new List<Mech>();

            for (int i = 0; i < count; i++)
            {
                var mechData = new Dictionary<PartType, PartData>
            {
                { PartType.Base, PartsManager.Instance.GetEnemyPart(PartType.Base) },
                { PartType.Torso, PartsManager.Instance.GetEnemyPart(PartType.Torso) },
                { PartType.WeaponPrimary, PartsManager.Instance.GetEnemyPart(PartType.WeaponPrimary) },
                { PartType.WeaponSecondary, PartsManager.Instance.GetEnemyPart(PartType.WeaponSecondary) }
            };

                enemyMechs.Add(new Mech { mechData = mechData });
            }

            return enemyMechs;
        }

        // Spawns units and adds them to the category service
        private void SpawnUnits(SpawnData spawnData)
        {
            List<Unit> units = UnitConstructService.Instance.SpawnUnits(spawnData);

            foreach (Unit unit in units)
            {
                UnitCategoryService.Instance.AddUnit(unit);
            }
        }

        // Returns available spawn points for a faction
        private Transform[] SetSpawnPoints(SpawnData spawnData)
        {
            List<Transform> allSpawnPoints = new List<Transform>();

            foreach (Transform child in spawnData.factionTransform)
            {
                allSpawnPoints.Add(child);
            }

            List<Transform> freeSpawnPoints = new List<Transform>();
            foreach (var point in allSpawnPoints)
            {
                GridPosition gridPos = LevelGrid.Instance.GetGridPosition(point.position);
                if (LevelGrid.Instance.GetUnitAtGridPosition(gridPos) == null)
                {
                    freeSpawnPoints.Add(point);
                    if (freeSpawnPoints.Count == spawnData.count)
                        break;
                }
            }

            return freeSpawnPoints.ToArray();
        }

        // Gets a list of free spawn points
        private List<Transform> GetFreeSpawnPoints(Transform[] spawnPoints, int requiredCount)
        {
            var freePoints = new List<Transform>();
            foreach (var point in spawnPoints)
            {
                GridPosition gridPos = LevelGrid.Instance.GetGridPosition(point.position);
                if (LevelGrid.Instance.GetUnitAtGridPosition(gridPos) == null)
                {
                    freePoints.Add(point);
                    if (freePoints.Count == requiredCount)
                        break;
                }
            }
            return freePoints;
        }
    }
}
