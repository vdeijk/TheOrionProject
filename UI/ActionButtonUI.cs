using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class ActionButtonUI : BaseButtonUI
    {
        [field: SerializeField] public ActionType actionType { get; private set; }
        [SerializeField] TextMeshProUGUI actionNameField;
        [SerializeField] TextMeshProUGUI actionRangeField;
        [SerializeField] TextMeshProUGUI actionTypeField;
        [SerializeField] TextMeshProUGUI actionHeatCostField;
        [SerializeField] TextMeshProUGUI actionAmmoCostField;
        [SerializeField] TextMeshProUGUI actionDamageField;

        private BaseAction baseAction;

        protected override void Start()
        {
            base.Start();
            // Display action type as button label
            actionNameField.text = actionType.ToString().ToUpper();
        }

        // Updates button visuals based on current selection and unit state
        public void UpdateSelectedVisual()
        {
            BaseAction selectedBaseAction = UnitActionSystem.Instance.selectedAction;
            Unit selectedUnit = UnitSelectService.Instance.selectedUnit;
            if (selectedBaseAction == null || selectedUnit == null || baseAction == null) return;
            isSelected = baseAction.actionType == selectedBaseAction.actionType;
            button.interactable = selectedUnit.unitMindTransform.GetComponent<ActionSystem>().actionPoints > 0;
            SetButtonState(GetButtonState(), true);
        }

        // Sets up the button for a specific action type and updates UI fields
        public void SetBaseAction(BaseAction baseAction)
        {
            this.baseAction = baseAction;
            switch (baseAction)
            {
                case MoveAction:
                    SetMoveAction();
                    break;
                case ShootAction:
                    SetShootAction(((ShootAction)baseAction).weaponPartType);
                    break;
                case PassAction:
                    SetPassAction();
                    break;
            }
        }

        // Handles button click to select the action in the system
        protected override void OnButtonClicked()
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        }

        // Configures UI for move actions
        private void SetMoveAction()
        {
            BaseData baseData = (BaseData)baseAction.unit.partsData[PartType.Base];
            actionNameField.text = "MOVE";
            actionTypeField.text = "<b>Type:</b> " + baseData.unitType.ToString();
            actionRangeField.text = "<b>Range:</b> " + baseAction.Range.ToString();
            actionRangeField.enabled = true;
            actionTypeField.enabled = true;
            actionAmmoCostField.enabled = false;
            actionHeatCostField.enabled = false;
            actionDamageField.enabled = false;
        }

        // Configures UI for shoot actions, including ammo, heat, and synergy bonus
        private void SetShootAction(PartType partType)
        {
            TorsoData torsoData = (TorsoData)baseAction.unit.partsData[PartType.Torso];
            WeaponData weaponData = (WeaponData)baseAction.unit.partsData[partType];
            WeaponType weaponType = weaponData.weaponType;
            AmmoSystem ammoSystem = UnitAmmoService.Instance.GetAmmoSystem(partType);
            actionNameField.text = "SHOOT";
            float damage = weaponData.Damage;
            if (torsoData.Synergy == weaponType) damage *= 1.5f; // Synergy bonus
            switch (weaponType)
            {
                case WeaponType.Bullet:
                    actionAmmoCostField.text = "<b>Ammo:</b> " + ammoSystem.ammo.ToString();
                    actionHeatCostField.text = "<b>Heat:</b> +" + weaponData.HeatCost.ToString();
                    actionAmmoCostField.enabled = true;
                    actionHeatCostField.enabled = true;
                    break;
                case WeaponType.Laser:
                    actionHeatCostField.text = "<b>Heat:</b> +" + weaponData.HeatCost.ToString();
                    actionAmmoCostField.enabled = false;
                    actionHeatCostField.enabled = true;
                    break;
                case WeaponType.Missile:
                    actionAmmoCostField.text = "<b>Ammo:</b> " + ammoSystem.ammo.ToString();
                    actionAmmoCostField.enabled = true;
                    actionHeatCostField.enabled = false;
                    break;
            }
            actionTypeField.text = "<b>Type:</b> " + weaponData.weaponType.ToString();
            actionRangeField.text = "<b>Range:</b> " + baseAction.Range.ToString();
            actionDamageField.text = "<b>Damage:</b> " + damage.ToString();
            actionRangeField.enabled = true;
            actionTypeField.enabled = true;
            actionDamageField.enabled = true;
        }

        // Configures UI for pass actions (heat vent)
        private void SetPassAction()
        {
            TorsoData torsoData = (TorsoData)baseAction.unit.partsData[PartType.Torso];
            actionNameField.text = "PASS";
            if (colorPalette != null)
            {
                actionNameField.color = colorPalette.colorBlue;
            }
            actionRangeField.text = "<b>Range:</b> " + 1;
            actionHeatCostField.text = "<b>Heat:</b> -" + (torsoData.HeatVent).ToString();
            actionRangeField.enabled = true;
            actionHeatCostField.enabled = true;
            actionAmmoCostField.enabled = false;
            actionTypeField.enabled = false;
            actionDamageField.enabled = false;
        }
    }
}