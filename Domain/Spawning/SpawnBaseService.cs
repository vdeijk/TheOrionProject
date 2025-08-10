using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    public class SpawnBaseService
    {
        // Gets a list of free spawn points
        public List<Transform> GetFreeSpawnPoints(Transform[] spawnPoints, int requiredCount)
        {
            var freePoints = new List<Transform>();
            foreach (var point in spawnPoints)
            {
                Vector2Int gridPos = GridUtilityService.Instance.GetGridPosition(point.position);
                if (GridUnitService.Instance.GetUnit(gridPos) == null)
                {
                    freePoints.Add(point);
                    if (freePoints.Count == requiredCount)
                        break;
                }
            }
            return freePoints;
        }

        // Returns available spawn points for a faction
        public Transform[] SetSpawnPoints(SpawnBaseData data)
        {
            List<Transform> allSpawnPoints = new List<Transform>();

            foreach (Transform child in data.FactionTransform)
            {
                allSpawnPoints.Add(child);
            }

            List<Transform> freeSpawnPoints = new List<Transform>();
            foreach (var point in allSpawnPoints)
            {
                Vector2Int gridPos = GridUtilityService.Instance.GetGridPosition(point.position);
                if (GridUnitService.Instance.GetUnit(gridPos) == null)
                {
                    freeSpawnPoints.Add(point);
                    if (freeSpawnPoints.Count == data.Mechs.Count)
                        break;
                }
            }

            return freeSpawnPoints.ToArray();
        }

        // Spawns units and adds them to the category service
        protected void SpawnUnits(SpawnBaseData data)
        {
            List<UnitSingleController> units = UnitConstructService.Instance.SpawnUnits(data);

            foreach (UnitSingleController unit in units)
            {
                UnitCategoryService.Instance.AddUnit(unit);
            }
        }
    }
}
