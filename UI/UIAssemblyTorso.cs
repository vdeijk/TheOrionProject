using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class UIAssemblyTorso : UIAssemblyPart
    {
        [SerializeField] TextMeshProUGUI partNames;
        [SerializeField] TextMeshProUGUI shieldText;
        [SerializeField] TextMeshProUGUI heatText;
        [SerializeField] TextMeshProUGUI synergyText;
        [SerializeField] TextMeshProUGUI salvageBoostText;
        [SerializeField] TextMeshProUGUI heatVenttText;

        protected override void SetTexts(PartData partData)
        {
            if (!(partData is TorsoData newPartData)) return;

            Unit unit = UnitSelectService.Instance.selectedUnit;
            PartType curPartType = UnitAssemblyService.Instance.curPartType;
            TorsoData oldPartData = (TorsoData)unit.partsData[curPartType];

            partNames.text = oldPartData.Name + symbol + "<br>" + newPartData.Name;
            shieldText.text = "<b>Max Shield</b>: " + oldPartData.MaxShield + symbol + newPartData.MaxShield;
            heatText.text = "<b>Max Heat:</b> " + oldPartData.MaxHeat + symbol + newPartData.MaxHeat;
            synergyText.text = "<b>Weapon Affinity:</b> " + oldPartData.Synergy + symbol + newPartData.Synergy;
            salvageBoostText.text = "<b>Scavenge Boost:</b> " + oldPartData.SalvageBoost + symbol + newPartData.SalvageBoost;
            heatVenttText.text = "<b>Heat Vent:</b> " + oldPartData.HeatVent + symbol + newPartData.HeatVent;
        }
    }
}

