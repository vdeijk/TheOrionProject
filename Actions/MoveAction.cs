using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Handles movement actions for a unit, including pathfinding and movement logic.
    /// </summary>
    public class MoveAction : BaseAction
    {
        public enum MovementType { Rotate, Move, Stop }

        [SerializeField] float StoppingDistance;
        [SerializeField] UnitMovement unitMovement;
        [SerializeField] Rigidbody rb;
        [SerializeField] LayerMask terrainLayer;
        [SerializeField] HeatSystem heatSystem;

        private Vector3 targetPosition;
        private Vector3 targetDirection;
        private Vector3 curPos;
        private Vector3 curFwd;
        private List<Vector3> pathWaypoints;
        private int currentWaypointIndex;

        protected override void Awake()
        {
            base.Awake();
            actionName = "Move";
            targetPosition = transform.position;
            GRID_POSITION_TYPE = GridPositionType.Unoccupied;
        }

        private void OnEnable()
        {
            UnitCategoryService.OnUnitAdded += UnitManager_OnUnitAdded;
            AssemblyPartInfo.OnPartAssembled += AssemblyPartInfo_OnPartAssembled;
        }

        private void OnDisable()
        {
            UnitCategoryService.OnUnitAdded -= UnitManager_OnUnitAdded;
            AssemblyPartInfo.OnPartAssembled -= AssemblyPartInfo_OnPartAssembled;
        }

        private void Start()
        {
            InitData();
        }

        private void FixedUpdate()
        {
            if (!isActive) return;

            // If following a path, move through waypoints
            if (pathWaypoints != null && currentWaypointIndex < pathWaypoints.Count)
            {
                targetPosition = pathWaypoints[currentWaypointIndex];
                targetDirection = (targetPosition - rb.transform.position).normalized;
                curPos = new Vector3(rb.transform.position.x, targetPosition.y, rb.transform.position.z);
                curFwd = new Vector3(rb.transform.forward.x, targetDirection.y, rb.transform.forward.z);

                switch (SetMovementType())
                {
                    case MovementType.Rotate:
                        unitMovement.UpdateRotation(curFwd, targetDirection);
                        break;

                    case MovementType.Move:
                        unitMovement.UpdateRotation(curFwd, targetDirection);
                        unitMovement.UpdateMovement(targetDirection);
                        break;

                    case MovementType.Stop:
                        currentWaypointIndex++;
                        if (currentWaypointIndex < pathWaypoints.Count)
                        {
                            targetPosition = pathWaypoints[currentWaypointIndex];
                        }
                        else
                        {
                            unitMovement.StopMovement();
                            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
                            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, unit);
                            TorsoData torsoData = (TorsoData)unit.partsData[PartType.Torso];
                            BaseData baseData = (BaseData)unit.partsData[PartType.Base];
                            // Only hover units generate heat on movement completion
                            if (baseData.unitType == UnitType.Hover) heatSystem.GenerateHeat(torsoData.MaxHeat / 10);
                            ActionComplete();
                        }
                        break;
                }
            }
            else
            {
                // If not following a path, move directly to target
                targetDirection = (targetPosition - rb.transform.position).normalized;
                curPos = new Vector3(rb.transform.position.x, targetPosition.y, rb.transform.position.z);
                curFwd = new Vector3(rb.transform.forward.x, targetDirection.y, rb.transform.forward.z);

                switch (SetMovementType())
                {
                    case MovementType.Rotate:
                        unitMovement.UpdateRotation(curFwd, targetDirection);
                        break;

                    case MovementType.Move:
                        unitMovement.UpdateRotation(curFwd, targetDirection);
                        unitMovement.UpdateMovement(targetDirection);
                        break;

                    case MovementType.Stop:
                        unitMovement.StopMovement();
                        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(targetPosition);
                        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, unit);
                        TorsoData torsoData = (TorsoData)unit.partsData[PartType.Torso];
                        heatSystem.GenerateHeat(torsoData.MaxHeat / 10);
                        ActionComplete();
                        break;
                }
            }
        }

        /// <summary>
        /// Returns the best AI action for movement, prioritizing proximity to player units.
        /// </summary>
        public override EnemyAIAction GetBestEnemyAIAction()
        {
            GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(unit.unitBodyTransform.position);

            bool playerInRange = false;
            int minEngageRange = Range * 2;
            int maxEngageRange = Range * 3;
            int engageRange = UnityEngine.Random.Range(minEngageRange, maxEngageRange);

            // Check if any player unit is within range
            foreach (Unit playerUnit in UnitCategoryService.Instance.allies)
            {
                GridPosition playerPos = LevelGrid.Instance.GetGridPosition(playerUnit.unitBodyTransform.position);
                int dist = Mathf.Abs(playerPos.x - unitGridPosition.x) + Mathf.Abs(playerPos.z - unitGridPosition.z);
                if (dist <= 10)
                {
                    playerInRange = true;
                    break;
                }
            }
            if (!playerInRange) return null;

            List<EnemyAIAction> enemyAIActions = new List<EnemyAIAction>();
            List<GridPosition> validGridPositions = LevelGrid.Instance.GetValidPositions(unit, Range, GRID_POSITION_TYPE, unitGridPosition);

            int gridWidth = LevelGrid.Instance.gridData.gridWidth;
            int gridHeight = LevelGrid.Instance.gridData.gridHeight;
            int curMinDistToPlayer = int.MaxValue;

            // Evaluate all valid positions and select the one closest to a player unit
            foreach (GridPosition gridPosition in validGridPositions)
            {
                var path = AStarPathfinder.FindPath(
                    unitGridPosition,
                    gridPosition,
                    pos => LevelGrid.Instance.IsWalkable(pos),
                    gridWidth,
                    gridHeight
                );

                if (path == null) continue;

                int actionValue;
                int minDistToPlayer = int.MaxValue;
                foreach (Unit playerUnit in UnitCategoryService.Instance.allies)
                {
                    GridPosition playerPos = LevelGrid.Instance.GetGridPosition(playerUnit.unitBodyTransform.position);
                    int dist = Mathf.Abs(playerPos.x - gridPosition.x) + Mathf.Abs(playerPos.z - gridPosition.z);
                    if (dist < minDistToPlayer) minDistToPlayer = dist;
                }

                if (minDistToPlayer >= curMinDistToPlayer) continue;

                actionValue = 100 - minDistToPlayer;

                enemyAIActions.Add(new EnemyAIAction
                {
                    gridPosition = gridPosition,
                    actionValue = actionValue,
                    baseAction = this
                });
            }

            if (enemyAIActions.Count > 0)
            {
                // Return the highest value action
                enemyAIActions.Sort((a, b) => b.actionValue - a.actionValue);
                return enemyAIActions[0];
            }

            return null;
        }

        /// <summary>
        /// Initiates movement to the target grid position, using pathfinding if possible.
        /// </summary>
        public override void TakeAction(GridPosition targetGridPosition)
        {
            BaseData baseData = (BaseData)unit.partsData[PartType.Base];
            TorsoData torsoData = (TorsoData)unit.partsData[PartType.Torso];

            targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
            GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(unit.transform.position);

            LevelGrid.Instance.RemoveUnitAtGridPosition(unitGridPosition, unit);

            var path = AStarPathfinder.FindPath(
                unitGridPosition,
                targetGridPosition,
                pos => LevelGrid.Instance.IsWalkable(pos),
                LevelGrid.Instance.gridData.gridWidth,
                LevelGrid.Instance.gridData.gridHeight
            );

            if (path != null && path.Count > 0)
            {
                pathWaypoints = path.Select(p => LevelGrid.Instance.GetWorldPosition(p)).ToList();
                currentWaypointIndex = 0;
                targetPosition = pathWaypoints[currentWaypointIndex];
            }
            else
            {
                pathWaypoints = null;
                targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
            }

            unitMovement.StartMovement(true);
            ActionStart();
        }

        /// <summary>
        /// Returns an AI action for a specific grid position, based on enemy positions.
        /// </summary>
        protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            WeaponData weaponData = (WeaponData)unit.partsData[PartType.WeaponPrimary];

            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = LevelGrid.Instance.GetValidPositions(unit, weaponData.Range, GridPositionType.Enemies, gridPosition).Count * 10,
                baseAction = this,
            };
        }

        private void AssemblyPartInfo_OnPartAssembled(object sender, EventArgs e)
        {
            if (UnitSelectService.Instance.selectedUnit == unit)
            {
                InitData();
            }
        }

        private void UnitManager_OnUnitAdded(object sender, UnitCategoryService.OnUnitAddedArgs e)
        {
            if (UnitSelectService.Instance.selectedUnit == unit)
            {
                InitData();
            }
        }

        /// <summary>
        /// Initializes movement data from unit parts.
        /// </summary>
        private void InitData()
        {
            BaseData baseData = (BaseData)unit.partsData[PartType.Base];
            Range = baseData.Range;
        }

        /// <summary>
        /// Determines the current movement type (rotate, move, stop) based on position and direction.
        /// </summary>
        private MovementType SetMovementType()
        {
            Vector3 unitForward = unit.unitBodyTransform.forward;
            unitForward.y = 0;

            if ((targetPosition - curPos).sqrMagnitude < StoppingDistance)
            {
                return MovementType.Stop;
            }
            else if (Vector3.Dot(curFwd, targetDirection) <= .99f)
            {
                return MovementType.Rotate;
            }

            return MovementType.Move;
        }
    }
}