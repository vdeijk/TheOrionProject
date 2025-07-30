using System;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class UISalvageTorso : UISalvageDisplay
    {
        [SerializeField] TextMeshProUGUI shieldText;
        [SerializeField] TextMeshProUGUI heatText;
        [SerializeField] TextMeshProUGUI synergyText;
        [SerializeField] TextMeshProUGUI salvageBoostText;
        [SerializeField] TextMeshProUGUI heatVentText;

        protected override void Setup()
        {
            PartType partType = UnitSalvageService.Instance.curSalvageType;
            if (partType == PartType.Torso)
            {
                Unit selectedTarget = UnitSelectService.Instance.selectedTarget;
                TorsoData torsoData = (TorsoData)selectedTarget.partsData[PartType.Torso];

                shieldText.text = "<b>Max Shield:</b> " + torsoData.MaxShield.ToString();
                heatText.text = "<b>Max Heat:</b> " + torsoData.MaxHeat.ToString();
                synergyText.text = "<b>Weapon Affinity:</b> " + torsoData.Synergy;
                salvageBoostText.text = "<b>Salvage Boost:</b> " + torsoData.SalvageBoost;
                heatVentText.text = "<b>Heat Vent:</b> " + torsoData.HeatVent;
            }
        }
    }
}
