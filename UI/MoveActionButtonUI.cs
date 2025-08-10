using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class MoveActionButtonUI : ActionButtonUI
    {
        private MoveActionData data => ActionMoveService.Instance.Data;

        // Sets up the button for a specific action type and updates UI fields
        public override void UpdateButton()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
            var baseSO = data.partSO as BaseSO;

            isSelected = data == ActionCoordinatorService.Instance.SelectedActionData;
            button.interactable = selectedUnit.Data.UnitMindTransform.GetComponent<UnitActionController>().actionPoints > 0;

            actionNameField.text = "MOVE";
            actionRangeField.text = "<b>Range:</b> " + data.partSO.Range.ToString();
            actionTypeField.text = "<b>Type:</b> " + baseSO.UnitType.ToString();
        }
    }
}