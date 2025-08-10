using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class StatBarUI : MonoBehaviour
    {
        [field: SerializeField] public Image barImage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI valueText { get; private set; }

        private void OnEnable()
        {
            ActionBaseService.OnActionStarted += BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted += BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected += UnitActionSystem_OnUnitSelected;
            CameraChangeMonobService.OnCameraChanged += ControlModeManager_OnGameModeChanged;
        }

        private void OnDisable()
        {
            ActionBaseService.OnActionStarted -= BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted -= BaseAction_OnActionCompleted;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
            UnitSelectService.OnUnitSelected -= UnitActionSystem_OnUnitSelected;
            CameraChangeMonobService.OnCameraChanged -= ControlModeManager_OnGameModeChanged;
        }

        public virtual void UpdateBar()
        {

        }

        protected bool CheckCannnotExecute()
        {
            return UnitSelectService.Instance.Data.SelectedUnit == null;
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

