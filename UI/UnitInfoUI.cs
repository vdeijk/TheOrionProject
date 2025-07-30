using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
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
            BaseAction.OnActionStarted += BaseAction_OnActionStarted;
            BaseAction.OnActionCompleted += BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected += UnitActionSystem_OnUnitSelected;
            PhaseManager.OnPhaseChanged += ControlModeManager_OnPhaseChanged;
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            BaseAction.OnActionStarted -= BaseAction_OnActionStarted;
            BaseAction.OnActionCompleted -= BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected -= UnitActionSystem_OnUnitSelected;
            PhaseManager.OnPhaseChanged -= ControlModeManager_OnPhaseChanged;
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
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
            return UnitSelectService.Instance.selectedUnit == null;
        }

        private void UpdateAP()
        {
            ActionSystem actionSystem = UnitSelectService.Instance.selectedUnit.unitMindTransform.GetComponent<ActionSystem>();
            Unit unit = UnitSelectService.Instance.selectedUnit;
            TorsoData torsoData = (TorsoData)unit.partsData[PartType.Torso];
            actionPointsTextMeshPro.text = "<b>Action Points:</b> " + actionSystem.actionPoints;
            salvageChanceField.text = "<b>Scavenge Chance:</b> " + (20 + torsoData.SalvageBoost) + "%";
        }

        private void UpdateNameAndStats()
        {
            Unit selectedUnit = UnitSelectService.Instance.selectedUnit;
            HealthSystem healthSystem = selectedUnit.unitMindTransform.GetComponent<HealthSystem>();

            int curHealth = (int)healthSystem.health;
            int curArmor = (int)healthSystem.armor;
            int curShield = (int)healthSystem.shield;
            int cutHeat = (int)selectedUnit.unitMindTransform.GetComponent<HeatSystem>().heat;

            healthText.text = curHealth.ToString();
            armorText.text = curArmor.ToString();
            shieldText.text = curShield.ToString();
            ammoText.text = cutHeat.ToString();
        }
        private void UpdateBars()
        {
            Unit selectedUnit = UnitSelectService.Instance.selectedUnit;

            HealthSystem healthSystem = selectedUnit.unitMindTransform.GetComponent<HealthSystem>();
            healthBar.fillAmount = healthSystem.GetHealthNormalized();
            armorBar.fillAmount = healthSystem.GetArmorNormalized();
            shieldBar.fillAmount = healthSystem.GetShieldNormalized();

            HeatSystem heatSystem = selectedUnit.unitMindTransform.GetComponent<HeatSystem>();
            ammoBar.fillAmount = heatSystem.GetHeatNormalized();
        }
    }
}
