using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class ScrapContainerFading : MonoBehaviour
    {
        [SerializeField] CanvasGroup[] canvasGroups;
        private bool isMenuNone => MenuChangeService.Instance.curMenu == MenuType.None;

        private void Awake()
        {
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
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

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            UpdateHideableUI();
        }

        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            UpdateHideableUI();
        }

        private void BaseAction_OnActionStarted(object sender, EventArgs e)
        {
            UpdateHideableUI();
        }

        private void UnitSelectionSystem_OnUnitDeselected(object sender, EventArgs e)
        {
            UpdateHideableUI();
        }

        private void UnitSelectionSystem_OnUnitSelected(object sender, EventArgs e)
        {
            UpdateHideableUI();
        }

        private void UnitActionSystem_OnActionCompleted(object sender, EventArgs e)
        {
            UpdateHideableUI();
        }

        private void UpdateHideableUI()
        {
            StopAllCoroutines();

            bool inMission = ControlModeManager.Instance.gameControlType == GameControlType.Mission;
            bool isOverheadCamera = CameraChangeService.Instance.curCamera == CameraType.Overhead;

            BaseAction selectedAction = UnitActionSystem.Instance.selectedAction;
            bool isActive = selectedAction != null && selectedAction.isActive;

            if (PhaseManager.Instance.isPlayerPhase && isMenuNone && inMission &&
                isOverheadCamera && !isActive)
            {
                foreach (CanvasGroup canvasGroup in canvasGroups)
                {
                    StartCoroutine(MenuFadeService.Instance.Fade(canvasGroup, 1));
                }
            }
            else
            {
                foreach (CanvasGroup canvasGroup in canvasGroups)
                {
                    StartCoroutine(MenuFadeService.Instance.Fade(canvasGroup, 0));
                }
            }
        }
    }
}
