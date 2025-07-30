using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class LevelGrid : Singleton<LevelGrid>
    {
        [field: SerializeField] public MeshRenderer levelMeshRenderer { get; private set; }
        [field: SerializeField] public LayerMask levelLayers { get; private set; }
        [field: SerializeField] public Transform parent { get; private set; }
        [field: SerializeField] public GridData gridData { get; private set; }

        [SerializeField] Transform gridObjectDebugPrefab;

        public event EventHandler OnUnitMovedGridPosition;

        // Returns valid grid positions for a unit based on range, type, and terrain
        public List<GridPosition> GetValidPositions(Unit unit, int range, GridPositionType gridPositionType, GridPosition gridPosition)
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = gridPosition + offsetGridPosition;

                    if (!IsValidGridPosition(testGridPosition)) continue;
                    if (Mathf.Abs(x) + Mathf.Abs(z) > range) continue;

                    GridObject gridObject = gridData.gridObjectArray[testGridPosition.x, testGridPosition.z];
                    if (gridObject.gridSquareType == GridSquareType.Inaccessible) continue;

                    BaseData baseData = (BaseData)unit.partsData[PartType.Base];
                    bool isMech = baseData.unitType == UnitType.Ground;
                    bool isSteep = gridObject.gridSquareType == GridSquareType.Steep;
                    if (isMech && isSteep) continue;

                    Unit targetUnit = GetUnitAtGridPosition(testGridPosition);

                    UnitFaction targetFaction;
                    UnitFaction unitFaction;

                    // Filter positions based on requested type
                    switch (gridPositionType)
                    {
                        case GridPositionType.Allies:
                            if (targetUnit == null) continue;
                            targetFaction = targetUnit.unitEntityTransform.GetComponent<UnitFaction>();
                            unitFaction = unit.unitEntityTransform.GetComponent<UnitFaction>();
                            if (targetFaction.IS_ENEMY == unitFaction.IS_ENEMY) validGridPositionList.Add(testGridPosition);
                            break;
                        case GridPositionType.Enemies:
                            if (targetUnit == null) continue;
                            targetFaction = targetUnit.unitEntityTransform.GetComponent<UnitFaction>();
                            unitFaction = unit.unitEntityTransform.GetComponent<UnitFaction>();
                            if (targetFaction.IS_ENEMY != unitFaction.IS_ENEMY) validGridPositionList.Add(testGridPosition);
                            break;
                        case GridPositionType.Unoccupied:
                            if (targetUnit == null) validGridPositionList.Add(testGridPosition);
                            break;
                        default:
                            validGridPositionList.Add(testGridPosition);
                            break;
                    }
                }
            }
            return validGridPositionList;
        }

        // Adds a unit to the specified grid cell
        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = gridData.gridObjectArray[gridPosition.x, gridPosition.z];
            gridObject.AddUnit(unit);
        }

        // Removes a unit from the specified grid cell
        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = gridData.gridObjectArray[gridPosition.x, gridPosition.z];
            gridObject.RemoveUnit(unit);
        }

        // Removes all allied units from the grid
        public void RemoveAlliedUnits()
        {
            foreach (Unit unit in UnitCategoryService.Instance.allies)
            {
                GridPosition gridPosition = GetGridPosition(unit.unitBodyTransform.position);
                RemoveUnitAtGridPosition(gridPosition, unit);
            }
        }

        // Checks if a grid cell is walkable (not blocked, not steep, not occupied)
        public bool IsWalkable(GridPosition gridPosition)
        {
            if (gridPosition.x < 0 || gridPosition.x >= gridData.gridWidth ||
                gridPosition.z < 0 || gridPosition.z >= gridData.gridHeight)
                return false;

            Unit unitAtPos = GetUnitAtGridPosition(gridPosition);
            if (unitAtPos != null)
            {
                return false;
            }

            GridObject gridObject = gridData.gridObjectArray[gridPosition.x, gridPosition.z];
            if (gridObject.gridSquareType == GridSquareType.Inaccessible ||
                gridObject.gridSquareType == GridSquareType.Steep)
                return false;

            if (gridObject.GetUnit() != null)
                return false;

            return true;
        }

        // Moves a unit from one grid cell to another and notifies listeners
        public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            if (fromGridPosition == toGridPosition) return;
            RemoveUnitAtGridPosition(fromGridPosition, unit);
            AddUnitAtGridPosition(toGridPosition, unit);
            OnUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
        }

        // Returns the first unit at a grid cell, or null if empty
        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridData.gridObjectArray[gridPosition.x, gridPosition.z];
            return gridObject.GetUnit();
        }

        // Converts world position to grid coordinates
        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return new GridPosition(Mathf.RoundToInt(worldPosition.x / gridData.cellSize), Mathf.RoundToInt(worldPosition.z / gridData.cellSize));
        }

        // Converts grid coordinates to world position
        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return new Vector3(gridPosition.x, 0, gridPosition.z) * gridData.cellSize;
        }

        // Checks if grid coordinates are within bounds
        private bool IsValidGridPosition(GridPosition gridPosition)
        {
            bool isXIndexValid = gridPosition.x >= 0 && gridPosition.x < gridData.gridWidth;
            bool isZIndexValid = gridPosition.z >= 0 && gridPosition.z < gridData.gridHeight;
            return isXIndexValid && isZIndexValid;
        }
    }
}

