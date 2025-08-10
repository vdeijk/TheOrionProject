using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    // Controls fading in/out of general info UI elements based on game state and events
    public class GeneralInfoFadingUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup[] canvasGroups;

        // Indicates if the game is in a preparation phase
        private bool isPreparing => ControlModeManager.Instance.isPreparing;

        private void Awake()
        {
            // Hide all canvas groups initially
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            // Subscribe to relevant events to update UI visibility
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
            ActionBaseService.OnActionStarted += BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted += UnitActionSystem_OnActionCompleted;
            UnitSelectService.OnUnitSelected += UnitSelectionSystem_OnUnitSelected;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
            PhaseManager.OnPhaseChanged += PhaseManager_OnPhaseChanged;
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
            ActionBaseService.OnActionStarted -= BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
            UnitSelectService.OnUnitSelected -= UnitSelectionSystem_OnUnitSelected;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
            PhaseManager.OnPhaseChanged -= PhaseManager_OnPhaseChanged;
        }

        // Event handlers trigger UI update
        private void PhaseManager_OnPhaseChanged(object sender, EventArgs e) { UpdateHideableUI(); }
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e) { UpdateHideableUI(); }
        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e) { UpdateHideableUI(); }
        private void BaseAction_OnActionStarted(object sender, EventArgs e) { UpdateHideableUI(); }
        private void UnitSelectionSystem_OnUnitDeselected(object sender, EventArgs e) { UpdateHideableUI(); }
        private void UnitSelectionSystem_OnUnitSelected(object sender, EventArgs e) { UpdateHideableUI(); }
        private void UnitActionSystem_OnActionCompleted(object sender, EventArgs e) { UpdateHideableUI(); }

        // Shows or hides UI elements based on phase, camera, and action state
        private void UpdateHideableUI()
        {
            StopAllCoroutines();
            bool inMission = ControlModeManager.Instance.gameControlType == GameControlType.Mission;
            bool isOverheadCamera = CameraChangeMonobService.Instance.curCamera == Data.CameraType.Overhead;
            if (PhaseManager.Instance.isPlayerPhase && isOverheadCamera &&
                !ActionCoordinatorService.IsAnyActionActive() && !isPreparing)
            {
                foreach (CanvasGroup canvasGroup in canvasGroups)
                {
                    StartCoroutine(MenuFadeMonobService.Instance.Fade(canvasGroup, 1)); // Fade in
                }
            }
            else
            {
                foreach (CanvasGroup canvasGroup in canvasGroups)
                {
                    StartCoroutine(MenuFadeMonobService.Instance.Fade(canvasGroup, 0)); // Fade out
                }
            }
        }
    }
}
