using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class SalvageWeaponUI : SalvageDisplayUI
    {
        [SerializeField] TextMeshProUGUI weaponTypeText;
        [SerializeField] TextMeshProUGUI heatCostText;
        [SerializeField] TextMeshProUGUI ammoCostText;
        [SerializeField] TextMeshProUGUI rangeText;
        [SerializeField] TextMeshProUGUI damageText;

        protected override void Setup()
        {
            PartType partType = SalvageService.Instance.Data.curSalvageType;
            if (partType == PartType.WeaponPrimary || partType == PartType.WeaponSecondary)
            {
                UnitSingleController selectedTarget = UnitSelectService.Instance.Data.SelectedTarget;
                WeaponSO weaponData = (WeaponSO)selectedTarget.Data.PartsData[partType];

                weaponTypeText.text = "<b>Weapon Type:</b> " + weaponData.weaponType.ToString();
                ammoCostText.text = "<b>Max Ammo:</b> " + weaponData.MaxAmmo.ToString();
                heatCostText.text = "<b>Heat Cost:</b> " + weaponData.HeatCost.ToString();
                rangeText.text = "<b>Shooting Range:</b> " + weaponData.Range.ToString();
                damageText.text = "<b>Base Damage:</b> " + weaponData.Damage.ToString();
            }

        }
    }
}