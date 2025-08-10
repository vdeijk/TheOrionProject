using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    // Handles selection and deselection of units, including cycling and targeting
    public class UnitSelectService
    {
        public UnitSelectData Data { get; private set; } = new(null, null, null);

        private static UnitSelectService _instance;

        public static UnitSelectService Instance => _instance ??= new UnitSelectService();

        public static event EventHandler OnUnitSelected;
        public static event EventHandler OnUnitDeselected;
        public static event EventHandler OnGridSquareSelected;
        public static event EventHandler OnGridSquareDeselected;

        // Handles unit removal and updates selection
        public void TrySelectFirstAlly()
        {
            List<UnitSingleController> alliedUnits = UnitCategoryService.Instance.Data.Allies;
            if (alliedUnits.Count <= 0)
            {
                SelectModeManager.Instance.NothingSelected();
            }
            else
            {
                SelectModeManager.Instance.DefaultActionSelected(alliedUnits[0]);
            }
        }

        // Handles mouse selection of units on the grid
        public void TryHandleUnitSelection()
        {
            Vector2Int gridPos = GridUtilityService.Instance.GetGridPosition(MouseWorldService.Instance.GetPosition());
            UnitSingleController unit = GridUnitService.Instance.GetUnit(gridPos);
            if (unit)
            {
                if (unit == Data.SelectedUnit) return;

                SelectModeManager.Instance.DefaultActionSelected(unit);
            }
            else if (gridPos.x == 0 && gridPos.y == 0)
            {
                SelectModeManager.Instance.NothingSelected();
            }
            else
            {
                SelectModeManager.Instance.GridSquareSelected(gridPos);
            }
        }

        // Selects a grid square based on world position and notifies listeners
        public void SelectGridSquare(Vector2Int gridPosition)
        {
            Data.SelectedGridPosition = gridPosition;
            OnGridSquareSelected?.Invoke(this, EventArgs.Empty);
        }


        // Sets the current target unit
        public void SelectTarget(UnitSingleController unit)
        {
            Data.SelectedTarget = unit;
        }

        // Selects a unit and triggers selection event
        public void SelectUnit(UnitSingleController unit)
        {
            if (unit == Data.SelectedUnit) return;

            Data.PreviousSelectedUnit = Data.SelectedUnit;
            Data.SelectedUnit = unit;
            SFXMonobService.Instance.PlaySFX(SFXType.SelectUnit);

            OnUnitSelected?.Invoke(this, EventArgs.Empty);
        }

        // Deselects the current unit and triggers deselection event
        public void DeselectUnit()
        {
            Data.PreviousSelectedUnit = Data.SelectedUnit;
            Data.SelectedUnit = null;

            OnUnitDeselected?.Invoke(this, EventArgs.Empty);
        }

        // Handles right mouse input for deselection
        public void HandleInputMouseRight(bool rightMouseButtonInput)
        {
            if (!rightMouseButtonInput || Data.SelectedUnit == null || ActionCoordinatorService.IsAnyActionActive()) return;

            SelectModeManager.Instance.NothingSelected();
        }

        // Cycles to the next unit in the list
        public void SelectNextUnit(List<UnitSingleController> units)
        {
            int newIndex = units.IndexOf(Data.SelectedUnit);
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

            UnitSingleController unit = units[newIndex];
            SelectModeManager.Instance.DefaultActionSelected(unit);
        }

        // Cycles to the previous unit in the list
        public void SelectPrevUnit(List<UnitSingleController> units)
        {
            int newIndex = units.IndexOf(Data.SelectedUnit);
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

            UnitSingleController unit = units[newIndex];
            SelectModeManager.Instance.DefaultActionSelected(unit);
        }


        // Deselects the current grid square and notifies listeners
        public void DeselectGridSquare()
        {
            Data.SelectedGridPosition = null;
            OnGridSquareDeselected?.Invoke(this, EventArgs.Empty);
        }
    }
}
