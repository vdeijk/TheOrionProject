using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    public class GridUtilityService
    {
        private static GridUtilityService _instance;

        public static GridUtilityService Instance => _instance ??= new GridUtilityService();

        private GridData Data => GridCoordinatorService.Instance.Data;

        // Checks if a grid cell is walkable (not blocked, not steep, not occupied)
        public bool IsWalkable(Vector2Int gridPosition)
        {
            if (gridPosition.x < 0 || gridPosition.x >= Data.Width ||
                gridPosition.y < 0 || gridPosition.y >= Data.Height)
                return false;

            UnitSingleController unitAtPos = GridUnitService.Instance.GetUnit(gridPosition);
            if (unitAtPos != null)
            {
                return false;
            }

            GridObject gridObject = Data.Objects[gridPosition.x, gridPosition.y];
            if (gridObject.gridSquareType == GridSquareType.Inaccessible ||
                gridObject.gridSquareType == GridSquareType.Steep)
                return false;

            if (gridObject.GetUnit() != null)
                return false;

            return true;
        }

        // Converts grid coordinates to world position
        public Vector3 GetWorldPosition(Vector2Int gridPosition)
        {
            return new Vector3(gridPosition.x, 0, gridPosition.y) * Data.CellSize;
        }

        // Checks if grid coordinates are within bounds
        public bool IsValidPosition(Vector2Int gridPosition)
        {
            bool isXIndexValid = gridPosition.x >= 0 && gridPosition.x < Data.Width;
            bool isZIndexValid = gridPosition.y >= 0 && gridPosition.y < Data.Height;
            return isXIndexValid && isZIndexValid;
        }

        // Converts world position to grid coordinates
        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            return new Vector2Int(Mathf.RoundToInt(worldPosition.x / Data.CellSize), Mathf.RoundToInt(worldPosition.z / Data.CellSize));
        }


        // Gets terrain height at a given position using raycast
        public float GetTerrainY(float x, float z)
        {
            if (Physics.Raycast(new Vector3(x, 500, z), Vector3.down, out RaycastHit raycastHit, float.MaxValue, Data.TerrainLayer))
            {
                return raycastHit.point.y;
            }
            return 0;
        }
    }
}

