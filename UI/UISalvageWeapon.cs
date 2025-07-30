using System;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class UISalvageWeapon : UISalvageDisplay
    {
        [SerializeField] TextMeshProUGUI weaponTypeText;
        [SerializeField] TextMeshProUGUI heatCostText;
        [SerializeField] TextMeshProUGUI ammoCostText;
        [SerializeField] TextMeshProUGUI rangeText;
        [SerializeField] TextMeshProUGUI damageText;

        protected override void Setup()
        {
            PartType partType = UnitSalvageService.Instance.curSalvageType;
            if (partType == PartType.WeaponPrimary || partType == PartType.WeaponSecondary)
            {
                Unit selectedTarget = UnitSelectService.Instance.selectedTarget;
                WeaponData weaponData = (WeaponData)selectedTarget.partsData[partType];

                weaponTypeText.text = "<b>Weapon Type:</b> " + weaponData.weaponType.ToString();
                ammoCostText.text = "<b>Max Ammo:</b> " + weaponData.MaxAmmo.ToString();
                heatCostText.text = "<b>Heat Cost:</b> " + weaponData.HeatCost.ToString();
                rangeText.text = "<b>Shooting Range:</b> " + weaponData.Range.ToString();
                damageText.text = "<b>Base Damage:</b> " + weaponData.Damage.ToString();
            }

        }
    }
}