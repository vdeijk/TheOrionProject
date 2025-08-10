using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    /// <summary>
    /// Handles movement actions for a unit, including pathfinding and movement logic.
    /// </summary>
    public class ActionMoveService : BaseActionService<MoveActionData>
    {
        private static ActionMoveService _instance;
        private List<Vector3> pathWaypoints;
        private int curWaypointIndex;
        private Vector3 targetPos;
        private Vector3 curPos;

        public override MoveActionData Data
        {
            get
            {
                var selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
                if (selectedUnit == null) return null;
                var action = selectedUnit.Data.UnitEntityTransform.GetComponent<ActionMoveController>();
                return action?.TypedData;
            }
        }

        public static ActionMoveService Instance => _instance ??= new ActionMoveService();
        private int Range => Data.partSO.Range;
        private HeatSystemController heatSystem => Data.HeatSystem;

        public void UpdateMovement()
        {
            // If following a path, move through waypoints
            if (pathWaypoints != null && curWaypointIndex < pathWaypoints.Count)
            {
                targetPos = pathWaypoints[curWaypointIndex];
                targetDirection = (targetPos - rb.transform.position).normalized;
                curPos = new Vector3(rb.transform.position.x, targetPos.y, rb.transform.position.z);
                curForward = new Vector3(rb.transform.forward.x, targetDirection.y, rb.transform.forward.z);

                switch (SetMovementType())
                {
                    case MovementType.Rotate:
                        unitMovement.UpdateRotation(curForward, targetDirection);
                        break;

                    case MovementType.Move:
                        unitMovement.UpdateRotation(curForward, targetDirection);
                        unitMovement.UpdateMovement(targetDirection);
                        break;

                    case MovementType.Stop:
                        curWaypointIndex++;
                        if (curWaypointIndex < pathWaypoints.Count)
                        {
                            targetPos = pathWaypoints[curWaypointIndex];
                        }
                        else
                        {
                            unitMovement.StopMovement();
                            Vector2Int gridPosition = GridUtilityService.Instance.GetGridPosition(targetPos);
                            GridUnitService.Instance.AddUnit(gridPosition, unit);
                            TorsoSO torsoData = (TorsoSO)unit.Data.PartsData[PartType.Torso];
                            BaseSO baseData = (BaseSO)unit.Data.PartsData[PartType.Base];
                            // Only hover units generate heat on movement completion
                            if (baseData.UnitType == UnitType.Hover) heatSystem.GenerateHeat(torsoData.MaxHeat / 10);
                            CompleteAction();
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// Returns the best AI action for movement, prioritizing proximity to player units.
        /// </summary>
        public override UnitAIData GetBestEnemyAction()
        {
            Vector2Int unitGridPosition = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);

            bool playerInRange = false;
            int minEngageRange = Range * 2;
            int maxEngageRange = Range * 3;
            int engageRange = Random.Range(minEngageRange, maxEngageRange);

            // Check if any player unit is within range
            foreach (UnitSingleController playerUnit in UnitCategoryService.Instance.Data.Allies)
            {
                Vector2Int playerPos = GridUtilityService.Instance.GetGridPosition(playerUnit.Data.UnitBodyTransform.position);
                int dist = Mathf.Abs(playerPos.x - unitGridPosition.x) + Mathf.Abs(playerPos.y - unitGridPosition.y);
                if (dist <= 10)
                {
                    playerInRange = true;
                    break;
                }
            }
            if (!playerInRange) return null;

            List<UnitAIData> enemyAIActions = new List<UnitAIData>();
            List<Vector2Int> validGridPositions = GridActionService.Instance.GetPositions(Data);

            int gridWidth = GridCoordinatorService.Instance.Data.Width;
            int gridHeight = GridCoordinatorService.Instance.Data.Height;
            int curMinDistToPlayer = int.MaxValue;

            // Evaluate all valid positions and select the one closest to a player unit
            foreach (Vector2Int gridPosition in validGridPositions)
            {
                var path = UnitPathService.FindPath(
                    unitGridPosition,
                    gridPosition,
                    pos => GridUtilityService.Instance.IsWalkable(pos),
                    gridWidth,
                    gridHeight
                );

                if (path == null) continue;

                int actionValue;
                int minDistToPlayer = int.MaxValue;
                foreach (UnitSingleController playerUnit in UnitCategoryService.Instance.Data.Allies)
                {
                    Vector2Int playerPos = GridUtilityService.Instance.GetGridPosition(playerUnit.Data.UnitBodyTransform.position);
                    int dist = Mathf.Abs(playerPos.x - gridPosition.x) + Mathf.Abs(playerPos.y - gridPosition.y);
                    if (dist < minDistToPlayer) minDistToPlayer = dist;
                }

                if (minDistToPlayer >= curMinDistToPlayer) continue;

                actionValue = 100 - minDistToPlayer;

                enemyAIActions.Add(new UnitAIData
                {
                    GridPosition = gridPosition,
                    ActionValue = actionValue,
                    BaseActionData = Data,
                });
            }

            if (enemyAIActions.Count > 0)
            {
                // Return the highest value action
                enemyAIActions.Sort((a, b) => b.ActionValue - a.ActionValue);
                return enemyAIActions[0];
            }

            return null;
        }

        /// <summary>
        /// Initiates movement to the target grid position, using pathfinding if possible.
        /// </summary>
        public override void TakeAction(Vector2Int targetGridPosition)
        {
            BaseSO baseData = (BaseSO)unit.Data.PartsData[PartType.Base];
            TorsoSO torsoData = (TorsoSO)unit.Data.PartsData[PartType.Torso];

            targetPos = GridUtilityService.Instance.GetWorldPosition(targetGridPosition);
            Vector2Int unitGridPosition = GridUtilityService.Instance.GetGridPosition(unit.transform.position);

            GridUnitService.Instance.RemoveUnit(unitGridPosition, unit);

            var path = UnitPathService.FindPath(
                unitGridPosition,
                targetGridPosition,
                pos => GridUtilityService.Instance.IsWalkable(pos),
                GridCoordinatorService.Instance.Data.Width,
                GridCoordinatorService.Instance.Data.Height
            );

            if (path != null && path.Count > 0)
            {
                pathWaypoints = path.Select(p => GridUtilityService.Instance.GetWorldPosition(p)).ToList();
                curWaypointIndex = 0;
                targetPos = pathWaypoints[curWaypointIndex];
            }
            else
            {
                pathWaypoints = null;
                targetPos = GridUtilityService.Instance.GetWorldPosition(targetGridPosition);
            }

            unitMovement.StartMovement(true);
        }

        /// <summary>
        /// Returns an AI action for a specific grid position, based on enemy positions.
        /// </summary>
        protected override UnitAIData GetEnemyAction(Vector2Int gridPosition)
        {
            WeaponSO weaponData = (WeaponSO)unit.Data.PartsData[PartType.WeaponPrimary];

            return new UnitAIData
            {
                GridPosition = gridPosition,
                ActionValue = GridActionService.Instance.GetPositions(Data).Count * 10,
                BaseActionData = Data,
            };
        }

        /// <summary>
        /// Determines the current movement type (rotate, move, stop) based on position and direction.
        /// </summary>
        private MovementType SetMovementType()
        {
            Vector3 unitForward = unit.Data.UnitBodyTransform.forward;
            unitForward.y = 0;

            if ((targetPos - curPos).sqrMagnitude < Data.StoppingDistance)
            {
                return MovementType.Stop;
            }
            else if (Vector3.Dot(curForward, targetDirection) <= .99f)
            {
                return MovementType.Rotate;
            }

            return MovementType.Move;
        }
    }
}