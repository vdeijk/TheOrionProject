using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    // Handles movement and positioning logic for units
    public class UnitMovementService
    {
        private UnitMovementData Data;
        private static UnitMovementService _instance;

        public static UnitMovementService Instance => _instance ??= new UnitMovementService();

        public static event EventHandler OnUnitsTeleported;

        public void Init(UnitMovementData data)
        {
            Data = data;
        }

        // Initializes the position of a unit's Rigidbody based on terrain
        public void InitPos(Rigidbody rb, UnitSingleController unit)
        {
            rb.gameObject.SetActive(false);

            Vector2Int unitGridPosition = GridUtilityService.Instance.GetGridPosition(rb.position);
            Vector3 newPos = GridUtilityService.Instance.GetWorldPosition(unitGridPosition);

            Vector3 raycastPos = new Vector3(newPos.x, 0f, newPos.z);
            float terrainHeight = GetTerrainYRaycast(raycastPos);
            float currentHeight = terrainHeight + GetHeightOffset(unit);

            rb.transform.position = new Vector3(newPos.x, currentHeight, newPos.z);

            rb.gameObject.SetActive(true);
        }

        // Updates unit movement based on speed and terrain
        public void UpdateMovement(UnitMovementController unitMovement)
        {
            Vector3 currentPos = unitMovement.rb.transform.position;
            float currentHeight = GetTerrainYRaycast(currentPos) + GetHeightOffset(unitMovement.unit);
            Rigidbody rb = unitMovement.rb;
            float MoveSpeed = unitMovement.MoveSpeed;
            Vector3 nextPos = rb.transform.position + rb.transform.forward * MoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(new Vector3(nextPos.x, currentHeight, nextPos.z));
        }

        // Updates unit rotation and maintains correct height
        public void UpdateRotation(UnitMovementController unitMovement, Vector3 curFwd, Vector3 targetDirection)
        {
            Rigidbody rb = unitMovement.rb;
            float rotateSpeed = unitMovement.RotateSpeed;

            Quaternion currentRotation = rb.rotation;

            Vector3 flatDirection = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);

            Quaternion newRotation = Quaternion.Slerp(
                currentRotation,
                targetRotation,
                rotateSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(newRotation);

            Vector3 pos = rb.transform.position;
            float currentHeight = GetTerrainYRaycast(pos) + GetHeightOffset(unitMovement.unit);
            rb.MovePosition(new Vector3(pos.x, currentHeight, pos.z));
        }

        // Stops unit movement and snaps to grid position
        public void StopMovement(UnitMovementController unitMovement)
        {
            UnitSingleController unit = unitMovement.unit;
            Rigidbody rb = unitMovement.rb;

            Vector3 worldPos = unit.Data.UnitBodyTransform.position;
            Vector2Int gridPosition = GridUtilityService.Instance.GetGridPosition(worldPos);
            Vector3 finalPos = GridUtilityService.Instance.GetWorldPosition(gridPosition);

            Vector3 pos = rb.transform.position;
            float currentHeight = GetTerrainYRaycast(pos) + GetHeightOffset(unit);
            rb.position = new Vector3(finalPos.x, currentHeight, finalPos.z);
        }


        // Teleports units to free spawn points
        public void TeleportToSpawnPoint(List<UnitSingleController> units)
        {
            foreach (var unit in units)
            {
                Vector2Int currentGridPos = GridUtilityService.Instance.GetGridPosition(unit.transform.position);
                GridUnitService.Instance.RemoveUnit(currentGridPos, unit);
            }

            Transform[] allSpawnPoints = SpawnAllyService.Instance.SetSpawnPoints(SpawnAllyService.Instance.Data);

            var freeSpawnPoints = SpawnAllyService.Instance.GetFreeSpawnPoints(allSpawnPoints, units.Count);

            for (int i = 0; i < units.Count && i < freeSpawnPoints.Count; i++)
            {
                units[i].transform.position = freeSpawnPoints[i].position;
                units[i].transform.rotation = freeSpawnPoints[i].rotation;

                Vector2Int toGridPosition = GridUtilityService.Instance.GetGridPosition(freeSpawnPoints[i].position);
                GridUnitService.Instance.AddUnit(toGridPosition, units[i]);
            }

            OnUnitsTeleported?.Invoke(this, EventArgs.Empty);
        }

        // Gets terrain height at a position using raycast
        private float GetTerrainYRaycast(Vector3 nextPos)
        {
            var gridData = GridCoordinatorService.Instance.Data;
            if (Physics.Raycast(new Vector3(nextPos.x, nextPos.y + 100, nextPos.z), Vector3.down, out RaycastHit hit, float.MaxValue, gridData.TerrainLayer))
            {
                return hit.point.y;
            }
            return nextPos.y;
        }

        // Returns height offset based on unit type
        private float GetHeightOffset(UnitSingleController unit)
        {
            BaseSO baseData = (BaseSO)unit.Data.PartsData[PartType.Base];

            switch (baseData.UnitType)
            {
                case UnitType.Hover:
                    return Data.HoverHeightOffset;
                case UnitType.Air:
                    return Data.AirHeightOffset;
                default:
                    return 0f;
            }
        }
    }
}
