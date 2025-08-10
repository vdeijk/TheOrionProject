using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class SalvageTorsoUI : SalvageDisplayUI
    {
        [SerializeField] TextMeshProUGUI shieldText;
        [SerializeField] TextMeshProUGUI heatText;
        [SerializeField] TextMeshProUGUI synergyText;
        [SerializeField] TextMeshProUGUI salvageBoostText;
        [SerializeField] TextMeshProUGUI heatVentText;

        protected override void Setup()
        {
            PartType partType = SalvageService.Instance.Data.curSalvageType;
            if (partType == PartType.Torso)
            {
                UnitSingleController selectedTarget = UnitSelectService.Instance.Data.SelectedTarget;
                TorsoSO torsoData = (TorsoSO)selectedTarget.Data.PartsData[PartType.Torso];

                shieldText.text = "<b>Max Shield:</b> " + torsoData.MaxShield.ToString();
                heatText.text = "<b>Max Heat:</b> " + torsoData.MaxHeat.ToString();
                synergyText.text = "<b>Weapon Affinity:</b> " + torsoData.Synergy;
                salvageBoostText.text = "<b>Salvage Boost:</b> " + torsoData.SalvageBoost;
                heatVentText.text = "<b>Heat Vent:</b> " + torsoData.HeatVent;
            }
        }
    }
}
