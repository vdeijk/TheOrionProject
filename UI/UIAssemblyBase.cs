using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class UIAssemblyBase : UIAssemblyPart
    {
        [SerializeField] TextMeshProUGUI partNames;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI armorText;
        [SerializeField] TextMeshProUGUI rangeText;
        [SerializeField] TextMeshProUGUI unitTypeText;

        protected override void SetTexts(PartData partData)
        {
            if (!(partData is BaseData newPartData)) return;

            Unit unit = UnitSelectService.Instance.selectedUnit;
            PartType curPartType = UnitAssemblyService.Instance.curPartType;
            BaseData oldPartData = (BaseData)unit.partsData[curPartType];

            partNames.text = oldPartData.Name + symbol + "<br>" + newPartData.Name;
            healthText.text = "<b>Max Health:</b> " + oldPartData.MaxHealth + symbol + newPartData.MaxHealth;
            armorText.text = "<b>Max Armor:</b> " + oldPartData.MaxArmor + symbol + newPartData.MaxArmor;
            rangeText.text = "<b>Movement Range:</b> " + oldPartData.Range + symbol + newPartData.Range;
            unitTypeText.text = "<b>Frame Type:</b> " + oldPartData.unitType + symbol + newPartData.unitType;
        }
    }
}