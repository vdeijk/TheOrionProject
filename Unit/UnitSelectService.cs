using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles selection and deselection of units, including cycling and targeting
    public class UnitSelectService : Singleton<UnitSelectService>
    {
        public Unit selectedUnit { get; private set; }
        public Unit prevSelectedUnit { get; private set; }
        public Unit selectedTarget { get; private set; }
        [SerializeField] LayerMask unitLayermask;
        public static event EventHandler OnUnitSelected;
        public static event EventHandler OnUnitDeselected;

        private void OnEnable()
        {
            UnitCategoryService.OnUnitRemoved += UnitManager_OnUnitRemoved;
        }

        private void OnDestroy()
        {
            UnitCategoryService.OnUnitRemoved -= UnitManager_OnUnitRemoved;
        }

        // Handles mouse selection of units on the grid
        public bool TryHandleUnitSelection()
        {
            bool wasUnitSelected = selectedUnit != null;
            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            if (unit)
            {
                GridSquareInfoController.Instance.SelectGridSquare(MouseWorld.GetPosition());
                if (unit == selectedUnit) return true;
                SelectUnit(unit);
                return true;
            }
            if (wasUnitSelected) DeselectUnit();
            return false;
        }

        // Sets the current target unit
        public void SelectTarget(Unit unit)
        {
            selectedTarget = unit;
        }

        // Selects a unit and triggers selection event
        public void SelectUnit(Unit unit)
        {
            if (unit == selectedUnit) return;
            prevSelectedUnit = selectedUnit;
            selectedUnit = unit;
            SFXController.Instance.PlaySFX(SFXType.SelectUnit);
            OnUnitSelected?.Invoke(this, EventArgs.Empty);
        }

        // Deselects the current unit and triggers deselection event
        public void DeselectUnit()
        {
            prevSelectedUnit = selectedUnit;
            selectedUnit = null;
            OnUnitDeselected?.Invoke(this, EventArgs.Empty);
        }

        // Handles right mouse input for deselection
        public void HandleInputMouseRight(bool rightMouseButtonInput)
        {
            bool inProgress = UnitActionSystem.Instance.inProgress;
            if (!rightMouseButtonInput || selectedUnit == null || inProgress) return;
            DeselectUnit();
        }

        // Cycles to the next unit in the list
        public void SelectNextUnit(List<Unit> units)
        {
            int newIndex = units.IndexOf(selectedUnit);
            int totalUnitsInFactionMinusOne = units.Count - 1;
            for (int i = 0; i < totalUnitsInFactionMinusOne; i++)
            {
                if (newIndex < totalUnitsInFactionMinusOne)
                {
                    newIndex += 1;
                }
                else
                {
                    newIndex = 0;
                }
            }
            SelectUnit(units[newIndex]);
        }

        // Cycles to the previous unit in the list
        public void SelectPrevUnit(List<Unit> units)
        {
            int newIndex = units.IndexOf(selectedUnit);
            int totalUnitsInFactionMinusOne = units.Count - 1;
            for (int i = 0; i < totalUnitsInFactionMinusOne; i++)
            {
                if (newIndex >= 1)
                {
                    newIndex -= 1;
                }
                else
                {
                    newIndex = totalUnitsInFactionMinusOne;
                }
            }
            SelectUnit(units[newIndex]);
        }

        // Handles unit removal and updates selection
        private void UnitManager_OnUnitRemoved(object sender, EventArgs e)
        {
            List<Unit> alliedUnits = UnitCategoryService.Instance.allies;
            if (!UnitCategoryService.Instance.all.Contains(selectedUnit) && alliedUnits.Count > 0)
            {
                selectedUnit = alliedUnits[0];
            }
            else if (alliedUnits.Count <= 0)
            {
                selectedUnit = null;
            }
        }
    }
}
