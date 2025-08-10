using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class ShootActionButtonUI : ActionButtonUI
    {
        [SerializeField] protected TextMeshProUGUI actionHeatCostField;
        [SerializeField] protected TextMeshProUGUI actionAmmoCostField;
        [SerializeField] protected TextMeshProUGUI actionDamageField;

        private ShootActionData data => ActionShootService.Instance.Data;


        // Sets up the button for a specific action type and updates UI fields
        public override void UpdateButton()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;

            isSelected = data == ActionCoordinatorService.Instance.SelectedActionData;
            button.interactable = selectedUnit.Data.UnitMindTransform.GetComponent<UnitActionController>().actionPoints > 0;

            ShootActionData shootActionData = ActionShootService.Instance.Data;
            TorsoSO torsoData = (TorsoSO)shootActionData.Unit.Data.PartsData[PartType.Torso];
            WeaponSO weaponData = (WeaponSO)shootActionData.Unit.Data.PartsData[PartType.WeaponPrimary];
            WeaponType weaponType = weaponData.weaponType;
            UnitAmmoController ammoSystem = UnitAmmoService.Instance.GetAmmoSystem(PartType.WeaponPrimary);
            float damage = weaponData.Damage;

            if (torsoData.Synergy == weaponType) damage *= 1.5f; // Synergy bonus

            actionNameField.text = "SHOOT";
            actionTypeField.text = "<b>Type:</b> " + weaponData.weaponType.ToString();
            actionRangeField.text = "<b>Range:</b> " + shootActionData.partSO.Range.ToString();
            actionDamageField.text = "<b>Damage:</b> " + damage.ToString();

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
                    actionHeatCostField.enabled = true;
                    break;
                case WeaponType.Missile:
                    actionAmmoCostField.text = "<b>Ammo:</b> " + ammoSystem.ammo.ToString();
                    actionAmmoCostField.enabled = true;
                    break;
            }
        }
    }
}