using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Domain
{
    /// <summary>
    /// Provides utility methods to check if any action is selected or active for the currently selected unit.
    /// </summary>
    public class ActionCoordinatorService
    {
        public BaseActionData SelectedActionData;

        private static ActionCoordinatorService _instance;

        public static ActionCoordinatorService Instance => _instance ??= new ActionCoordinatorService();

        public readonly List<ActionBaseService> AllActionServices = new List<ActionBaseService>();

        public void SelectMoveAction()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
            ActionMoveController action = selectedUnit.Data.UnitEntityTransform.GetComponent<ActionMoveController>();
            SelectedActionData = action.TypedData;

            GridActionService.Instance.UpdatePositions(action.TypedData.Unit);
            GridUXService.Instance.UpdateGrid();
        }

        public void SelectShootAction()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
            ActionShootController action = selectedUnit.Data.UnitEntityTransform.GetComponent<ActionShootController>();
            SelectedActionData = action.TypedData;

            GridActionService.Instance.UpdatePositions(action.TypedData.Unit);
            GridUXService.Instance.UpdateGrid();
        }

        public void SelectPassAction()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
            ActionPassController action = selectedUnit.Data.UnitEntityTransform.GetComponent<ActionPassController>();
            SelectedActionData = action.TypedData;

            GridActionService.Instance.UpdatePositions(action.TypedData.Unit);
            GridUXService.Instance.UpdateGrid();
        }

        public void DeselectAction()
        {
            SelectedActionData = null;

            GridActionService.Instance.UpdatePositions(null);
            GridUXService.Instance.UpdateGrid();
        }

        /// <summary>
        /// Returns true if any action is active for the currently selected unit.
        /// </summary>
        public static bool IsAnyActionActive()
        {
            var selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
            if (selectedUnit == null) return false;

            var actionSystem = selectedUnit.Data.UnitMindTransform.GetComponent<UnitActionController>();

            return actionSystem.baseActions.Any(a => a.Data != null && a.Data.IsActive);
        }

    }
}
