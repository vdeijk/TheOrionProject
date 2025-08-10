using System;
using UnityEngine;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    // Handles fading in/out of unit info UI based on game state
    public class UnitInfoFadingUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup unitInfoCanvasGroup;

        private bool isCameraOverhead => CameraChangeMonobService.Instance.curCamera == Data.CameraType.Overhead;
        private bool isMenuNone => MenuChangeMonobService.Instance.curMenu == MenuType.None;

        private void Awake()
        {
            unitInfoCanvasGroup.alpha = 0;
            unitInfoCanvasGroup.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // Subscribe to relevant events for UI updates
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
            ActionBaseService.OnActionStarted += BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted += UnitActionSystem_OnActionCompleted;
            UnitSelectService.OnUnitSelected += UnitSelectionSystem_OnUnitSelected;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
            ActionBaseService.OnActionStarted -= BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
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

            if (isCameraOverhead && isMenuNone && !ActionCoordinatorService.IsAnyActionActive())
            {
                StartCoroutine(MenuFadeMonobService.Instance.Fade(unitInfoCanvasGroup, 1));
            }
            else
            {
                StartCoroutine(MenuFadeMonobService.Instance.Fade(unitInfoCanvasGroup, 0));
            }
        }
    }
}

