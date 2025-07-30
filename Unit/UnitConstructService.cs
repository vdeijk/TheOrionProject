using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Constructs unit parts and spawns units in the scene
    public class UnitConstructService : Singleton<UnitConstructService>
    {
        [SerializeField] Transform cameraPrefab;

        // Builds the transform hierarchy for a unit's parts
        public Dictionary<PartType, Transform> GetTransformData(BaseData baseData, TorsoData torsoData, WeaponData weaponPrimaryData, WeaponData weaponSecondaryData, Unit unit)
        {
            Transform baseTransform = AddPart(baseData.Prefab, unit.unitBodyTransform, PartType.Base.ToString());
            Transform torsoTransform = AddPart(torsoData.Prefab, baseTransform.GetComponent<UnitSlots>().slots[0], PartType.Torso.ToString());
            Transform weaponPrimarytransform = AddPart(weaponPrimaryData.Prefab, torsoTransform.GetComponent<UnitSlots>().slots[0], PartType.WeaponPrimary.ToString());
            Transform weaponSecondarytransform = AddPart(weaponSecondaryData.Prefab, torsoTransform.GetComponent<UnitSlots>().slots[1], PartType.WeaponSecondary.ToString());

            return new Dictionary<PartType, Transform>
            {
                { PartType.Base, baseTransform},
                { PartType.Torso, torsoTransform},
                { PartType.WeaponPrimary, weaponPrimarytransform},
                { PartType.WeaponSecondary, weaponSecondarytransform },
            };
        }

        // Spawns units at specified spawn points
        public List<Unit> SpawnUnits(SpawnData spawnData)
        {
            List<Unit> units = new List<Unit>();

            for (int i = 0; i < spawnData.mechs.Count; i++)
            {
                Transform spawnPoint = spawnData.spawnPoints[i];

                GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(spawnPoint.position);

                if (LevelGrid.Instance.GetUnitAtGridPosition(gridPosition) != null) break;

                Unit unit = Instantiate(spawnData.prefab, spawnPoint.position, spawnData.rotation).GetComponent<Unit>();

                Mech mechData = spawnData.mechs[i];
                BaseData baseData = (BaseData)mechData.mechData[PartType.Base];
                TorsoData torsoData = (TorsoData)mechData.mechData[PartType.Torso];
                WeaponData weaponPrimaryData = (WeaponData)mechData.mechData[PartType.WeaponPrimary];
                WeaponData weaponSecondaryData = (WeaponData)mechData.mechData[PartType.WeaponSecondary];

                Dictionary<PartType, Transform> transformData = GetTransformData(baseData, torsoData, weaponPrimaryData, weaponSecondaryData, unit);

                unit.Init(mechData.mechData, spawnData, transformData);

                LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, unit);

                units.Add(unit);
            }

            return units;
        }

        // Instantiates a part prefab and sets its tag and transform
        private Transform AddPart(Transform Prefab, Transform parent, string tagToSet)
        {
            Transform partTransform = Instantiate(Prefab, Vector3.zero, Quaternion.identity);

            SetTagRecursively(partTransform.gameObject, tagToSet);

            partTransform.SetParent(parent);
            partTransform.localPosition = Vector3.zero;
            partTransform.localRotation = Quaternion.identity;
            partTransform.localScale = Vector3.one;

            return partTransform;
        }

        // Recursively sets the tag for a GameObject and its children
        private void SetTagRecursively(GameObject obj, string tag)
        {
            obj.tag = tag;
            foreach (Transform child in obj.transform)
            {
                SetTagRecursively(child.gameObject, tag);
            }
        }
    }
}


