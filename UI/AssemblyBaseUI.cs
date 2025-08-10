using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class AssemblyBaseUI : AssemblyPartUI
    {
        [SerializeField] TextMeshProUGUI partNames;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI armorText;
        [SerializeField] TextMeshProUGUI rangeText;
        [SerializeField] TextMeshProUGUI unitTypeText;

        protected override void SetTexts(PartSO partData)
        {
            if (!(partData is BaseSO newPartData)) return;

            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            PartType curPartType = AssemblyService.Instance.Data.CurPartType;
            BaseSO oldPartData = (BaseSO)unit.Data.PartsData[curPartType];

            partNames.text = oldPartData.Name + symbol + "<br>" + newPartData.Name;
            healthText.text = "<b>Max Health:</b> " + oldPartData.MaxHealth + symbol + newPartData.MaxHealth;
            armorText.text = "<b>Max Armor:</b> " + oldPartData.MaxArmor + symbol + newPartData.MaxArmor;
            rangeText.text = "<b>Movement Range:</b> " + oldPartData.Range + symbol + newPartData.Range;
            unitTypeText.text = "<b>Frame Type:</b> " + oldPartData.UnitType + symbol + newPartData.UnitType;
        }
    }
}