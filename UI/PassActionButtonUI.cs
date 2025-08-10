using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class PassActionButtonUI : ActionButtonUI
    {
        [SerializeField] protected TextMeshProUGUI actionHeatVentField;

        private ShootActionData data => ActionShootService.Instance.Data;

        // Sets up the button for a specific action type and updates UI fields
        public override void UpdateButton()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;

            isSelected = data == ActionCoordinatorService.Instance.SelectedActionData;
            button.interactable = selectedUnit.Data.UnitMindTransform.GetComponent<UnitActionController>().actionPoints > 0;
            SetButtonState(GetButtonState(), true);

            actionNameField.text = "PASS";
            actionRangeField.text = "<b>Range:</b> " + 1;
            actionTypeField.text = "<b>Type:</b> " + "Vent Heat";

            PassActionData passActionData = ActionPassService.Instance.Data;
            TorsoSO torsoData = (TorsoSO)passActionData.Unit.Data.PartsData[PartType.Torso];
        
            actionHeatVentField.text = "<b>Heat:</b> -" + (torsoData.HeatVent).ToString();
            actionHeatVentField.enabled = true;
        }
    }
}