using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class SalvageBaseUI : SalvageDisplayUI
    {
        [SerializeField] TextMeshProUGUI partTypeText;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI armorCostText;
        [SerializeField] TextMeshProUGUI rangeText;

        protected override void Setup()
        {
            PartType partType = SalvageService.Instance.Data.curSalvageType;
            if (partType == PartType.Base)
            {
                UnitSingleController selectedTarget = UnitSelectService.Instance.Data.SelectedUnit;
                BaseSO baseData = (BaseSO)selectedTarget.Data.PartsData[PartType.Base];

                partTypeText.text = "<b>Frame Type:</b> " + baseData.UnitType.ToString();
                healthText.text = "<b>Max Health:</b> " + baseData.MaxHealth.ToString();
                armorCostText.text = "<b>Max Armor:</b> " + baseData.MaxArmor.ToString();
                rangeText.text = "<b>Movement Range:</b> " + baseData.Range.ToString();
            }
        }
    }
}

