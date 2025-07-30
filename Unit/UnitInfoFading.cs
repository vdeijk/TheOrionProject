using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles fading in/out of unit info UI based on game state
    public class UnitInfoFading : MonoBehaviour
    {
        [SerializeField] private CanvasGroup unitInfoCanvasGroup;

        private bool isCameraOverhead => CameraChangeService.Instance.curCamera == CameraType.Overhead;
        private bool isMenuNone => MenuChangeService.Instance.curMenu == MenuType.None;

        private void Awake()
        {
            unitInfoCanvasGroup.alpha = 0;
            unitInfoCanvasGroup.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // Subscribe to relevant events for UI updates
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
            BaseAction.OnActionStarted += BaseAction_OnActionStarted;
            UnitActionSystem.Instance.OnActionCompleted += UnitActionSystem_OnActionCompleted;
            UnitSelectService.OnUnitSelected += UnitSelectionSystem_OnUnitSelected;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
        }

        private void OnDisable()
        {
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
            BaseAction.OnActionStarted -= BaseAction_OnActionStarted;
            UnitActionSystem.Instance.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
            UnitSelectService.OnUnitSelected -= UnitSelectionSystem_OnUnitSelected;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
        }

        // Event handlers trigger UI update
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e) { UpdateHideableUI(); }
        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e) { UpdateHideableUI(); }
        private void BaseAction_OnActionStarted(object sender, EventArgs e) { UpdateHideableUI(); }
        private void UnitSelectionSystem_OnUnitDeselected(object sender, EventArgs e) { UpdateHideableUI(); }
        private void UnitSelectionSystem_OnUnitSelected(object sender, EventArgs e) { UpdateHideableUI(); }
        private void UnitActionSystem_OnActionCompleted(object sender, EventArgs e) { UpdateHideableUI(); }

        // Shows or hides the unit info UI based on current game state
        private void UpdateHideableUI()
        {
            StopAllCoroutines();
            bool isUnitSelected = UnitSelectService.Instance.selectedUnit != null;
            BaseAction selectedAction = UnitActionSystem.Instance.selectedAction;
            bool isActive = selectedAction != null && selectedAction.isActive;
            if (isCameraOverhead && isMenuNone && isUnitSelected && !isActive)
            {
                StartCoroutine(MenuFadeService.Instance.Fade(unitInfoCanvasGroup, 1));
            }
            else
            {
                StartCoroutine(MenuFadeService.Instance.Fade(unitInfoCanvasGroup, 0));
            }
        }
    }
}

