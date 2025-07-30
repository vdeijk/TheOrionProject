using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
    public class StatBar : MonoBehaviour
    {
        [field: SerializeField] public Image barImage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI valueText { get; private set; }

        private void OnEnable()
        {
            BaseAction.OnActionStarted += BaseAction_OnActionStarted;
            BaseAction.OnActionCompleted += BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected += UnitActionSystem_OnUnitSelected;
            CameraChangeService.OnCameraChanged += ControlModeManager_OnGameModeChanged;
        }

        private void OnDisable()
        {
            BaseAction.OnActionStarted -= BaseAction_OnActionStarted;
            BaseAction.OnActionCompleted -= BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected -= UnitActionSystem_OnUnitSelected;
            CameraChangeService.OnCameraChanged -= ControlModeManager_OnGameModeChanged;
        }

        public virtual void UpdateBar()
        {

        }

        protected bool CheckCannnotExecute()
        {
            return UnitSelectService.Instance.selectedUnit == null;
        }

        private void UnitActionSystem_OnUnitSelected(object sender, EventArgs e)
        {
            UpdateBar();
        }

        private void ControlModeManager_OnGameModeChanged(object sender, EventArgs e)
        {
            UpdateBar();
        }

        private void BaseAction_OnActionStarted(object sender, EventArgs e)
        {
            UpdateBar();
        }

        private void BaseAction_OnActionCompleted(object sender, EventArgs e)
        {
            UpdateBar();
        }

        private void UnitSelectionSystem_OnUnitDeselected(object sender, EventArgs e)
        {
            UpdateBar();
        }
    }
}

