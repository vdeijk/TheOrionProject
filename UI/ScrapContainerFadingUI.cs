using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class ScrapContainerFadingUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup[] canvasGroups;
        private bool isMenuNone => MenuChangeMonobService.Instance.curMenu == MenuType.None;

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
            bool isOverheadCamera = CameraChangeMonobService.Instance.curCamera == Data.CameraType.Overhead;

            if (PhaseManager.Instance.isPlayerPhase && isMenuNone && inMission &&
                isOverheadCamera && !ActionCoordinatorService.IsAnyActionActive())
            {
                foreach (CanvasGroup canvasGroup in canvasGroups)
                {
                    StartCoroutine(MenuFadeMonobService.Instance.Fade(canvasGroup, 1));
                }
            }
            else
            {
                foreach (CanvasGroup canvasGroup in canvasGroups)
                {
                    StartCoroutine(MenuFadeMonobService.Instance.Fade(canvasGroup, 0));
                }
            }
        }
    }
}
