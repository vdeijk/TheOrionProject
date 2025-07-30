using System;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class UISalvageBase : UISalvageDisplay
    {
        [SerializeField] TextMeshProUGUI partTypeText;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI armorCostText;
        [SerializeField] TextMeshProUGUI rangeText;

        protected override void Setup()
        {
            PartType partType = UnitSalvageService.Instance.curSalvageType;
            if (partType == PartType.Base)
            {
                Unit selectedTarget = UnitSelectService.Instance.selectedTarget;
                BaseData baseData = (BaseData)selectedTarget.partsData[PartType.Base];

                partTypeText.text = "<b>Frame Type:</b> " + baseData.unitType.ToString();
                healthText.text = "<b>Max Health:</b> " + baseData.MaxHealth.ToString();
                armorCostText.text = "<b>Max Armor:</b> " + baseData.MaxArmor.ToString();
                rangeText.text = "<b>Movement Range:</b> " + baseData.Range.ToString();
            }
        }
    }
}

