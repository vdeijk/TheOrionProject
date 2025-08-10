using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    public class GridUnitService
    {
        private GridData gridData => GridCoordinatorService.Instance.Data;

        private static GridUnitService _instance;

        public static GridUnitService Instance => _instance ??= new GridUnitService();

        public event EventHandler OnUnitMovedGridPosition;

        // Adds a unit to the specified grid cell
        public void AddUnit(Vector2Int gridPosition, UnitSingleController unit)
        {
            GridObject gridObject = gridData.Objects[gridPosition.x, gridPosition.y];
            gridObject.AddUnit(unit);
        }

        // Removes a unit from the specified grid cell
        public void RemoveUnit(Vector2Int gridPosition, UnitSingleController unit)
        {
            GridObject gridObject = gridData.Objects[gridPosition.x, gridPosition.y];
            gridObject.RemoveUnit(unit);
        }

        // Removes all allied units from the grid
        public void RemoveAlliedUnits()
        {
            foreach (UnitSingleController unit in UnitCategoryService.Instance.Data.Allies)
            {
                Vector2Int gridPosition = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);
                RemoveUnit(gridPosition, unit);
            }
        }

        // Moves a unit from one grid cell to another and notifies listeners
        public void UnitMovedGridPosition(UnitSingleController unit, Vector2Int fromGridPosition, Vector2Int toGridPosition)
        {
            if (fromGridPosition == toGridPosition) return;
            RemoveUnit(fromGridPosition, unit);
            AddUnit(toGridPosition, unit);
            OnUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
        }

        // Returns the first unit at a grid cell, or null if empty
        public UnitSingleController GetUnit(Vector2Int gridPosition)
        {
            GridObject gridObject = gridData.Objects[gridPosition.x, gridPosition.y];
            return gridObject.GetUnit();
        }

        public bool GetIsEnemy(UnitSingleController selectedUnit, UnitSingleController targetUnit)
        {
            if (targetUnit == null) return false;

            UnitFactionController targetFaction = targetUnit.Data.UnitEntityTransform.GetComponent<UnitFactionController>();
            UnitFactionController selectedUnitFaction = selectedUnit.Data.UnitEntityTransform.GetComponent<UnitFactionController>();

            if (targetFaction.IS_ENEMY == selectedUnitFaction.IS_ENEMY) return false;

            return true;
        }
    }
}

