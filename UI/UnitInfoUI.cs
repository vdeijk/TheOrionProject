using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class UnitInfoUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI actionPointsTextMeshPro;
        [SerializeField] TextMeshProUGUI salvageChanceField;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI armorText;
        [SerializeField] TextMeshProUGUI shieldText;
        [SerializeField] TextMeshProUGUI ammoText;
        [SerializeField] Image healthBar;
        [SerializeField] Image armorBar;
        [SerializeField] Image shieldBar;
        [SerializeField] Image ammoBar;

        private void OnEnable()
        {
            ActionBaseService.OnActionStarted += BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted += BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected += UnitActionSystem_OnUnitSelected;
            PhaseManager.OnPhaseChanged += ControlModeManager_OnPhaseChanged;
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            ActionBaseService.OnActionStarted -= BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted -= BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected -= UnitActionSystem_OnUnitSelected;
            PhaseManager.OnPhaseChanged -= ControlModeManager_OnPhaseChanged;
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        private void UnitActionSystem_OnUnitSelected(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void ControlModeManager_OnPhaseChanged(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void BaseAction_OnActionStarted(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void BaseAction_OnActionCompleted(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void UnitSelectionSystem_OnUnitDeselected(object sender, EventArgs e)
        {
            UpdateStats();
        }

        private void UpdateStats()
        {
            if (CheckCannnotExecute()) return;
            UpdateAP();
            UpdateNameAndStats();
            UpdateBars();
        }

        private bool CheckCannnotExecute()
        {
            return UnitSelectService.Instance.Data.SelectedUnit == null;
        }

        private void UpdateAP()
        {
            UnitActionController actionSystem = UnitSelectService.Instance.Data.SelectedUnit.Data.UnitMindTransform.GetComponent<UnitActionController>();
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            TorsoSO torsoData = (TorsoSO)unit.Data.PartsData[PartType.Torso];
            actionPointsTextMeshPro.text = "<b>Action Points:</b> " + actionSystem.actionPoints;
            salvageChanceField.text = "<b>Scavenge Chance:</b> " + (20 + torsoData.SalvageBoost) + "%";
        }

        private void UpdateNameAndStats()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
            UnitHealthController healthSystem = selectedUnit.Data.UnitMindTransform.GetComponent<UnitHealthController>();

            int curHealth = (int)healthSystem.health;
            int curArmor = (int)healthSystem.armor;
            int curShield = (int)healthSystem.shield;
            int cutHeat = (int)selectedUnit.Data.UnitMindTransform.GetComponent<HeatSystemController>().heat;

            healthText.text = curHealth.ToString();
            armorText.text = curArmor.ToString();
            shieldText.text = curShield.ToString();
            ammoText.text = cutHeat.ToString();
        }
        private void UpdateBars()
        {
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;

            UnitHealthController healthSystem = selectedUnit.Data.UnitMindTransform.GetComponent<UnitHealthController>();
            healthBar.fillAmount = healthSystem.GetHealthNormalized();
            armorBar.fillAmount = healthSystem.GetArmorNormalized();
            shieldBar.fillAmount = healthSystem.GetShieldNormalized();

            HeatSystemController heatSystem = selectedUnit.Data.UnitMindTransform.GetComponent<HeatSystemController>();
            ammoBar.fillAmount = heatSystem.GetHeatNormalized();
        }
    }
}
