using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class AssemblyTorsoUI : AssemblyPartUI
    {
        [SerializeField] TextMeshProUGUI partNames;
        [SerializeField] TextMeshProUGUI shieldText;
        [SerializeField] TextMeshProUGUI heatText;
        [SerializeField] TextMeshProUGUI synergyText;
        [SerializeField] TextMeshProUGUI salvageBoostText;
        [SerializeField] TextMeshProUGUI heatVenttText;

        protected override void SetTexts(PartSO partData)
        {
            if (!(partData is TorsoSO newPartData)) return;

            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            PartType curPartType = AssemblyService.Instance.Data.CurPartType;
            TorsoSO oldPartData = (TorsoSO)unit.Data.PartsData[curPartType];

            partNames.text = oldPartData.Name + symbol + "<br>" + newPartData.Name;
            shieldText.text = "<b>Max Shield</b>: " + oldPartData.MaxShield + symbol + newPartData.MaxShield;
            heatText.text = "<b>Max Heat:</b> " + oldPartData.MaxHeat + symbol + newPartData.MaxHeat;
            synergyText.text = "<b>Weapon Affinity:</b> " + oldPartData.Synergy + symbol + newPartData.Synergy;
            salvageBoostText.text = "<b>Scavenge Boost:</b> " + oldPartData.SalvageBoost + symbol + newPartData.SalvageBoost;
            heatVenttText.text = "<b>Heat Vent:</b> " + oldPartData.HeatVent + symbol + newPartData.HeatVent;
        }
    }
}

