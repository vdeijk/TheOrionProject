using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class TileInfoFading : MonoBehaviour
    {
        [SerializeField] private CanvasGroup gridSquareInfoCanvasGroup;

        private bool isCameraOverhead => CameraChangeService.Instance.curCamera == CameraType.Overhead;
        private bool isMenuNone => MenuChangeService.Instance.curMenu == MenuType.None;

        private void Awake()
        {
            gridSquareInfoCanvasGroup.alpha = 0;
            gridSquareInfoCanvasGroup.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            GridSquareInfoController.OnClickedOutsideGrid += GridSquareInfoController_OnClickedOutsideGrid;
            GridSquareInfoController.OnGridSquareClicked += GridSquareInfoController_OnGridSquareClicked;
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
            BaseAction.OnActionStarted += BaseAction_OnActionStarted;
            UnitActionSystem.Instance.OnActionCompleted += UnitActionSystem_OnActionCompleted;
        }

        private void OnDisable()
        {
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            GridSquareInfoController.OnClickedOutsideGrid -= GridSquareInfoController_OnClickedOutsideGrid;
            GridSquareInfoController.OnGridSquareClicked -= GridSquareInfoController_OnGridSquareClicked;
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
            BaseAction.OnActionStarted -= BaseAction_OnActionStarted;
            UnitActionSystem.Instance.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
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

        private void GridSquareInfoController_OnClickedOutsideGrid(object sender, EventArgs e)
        {
            UpdateHideableUI();
        }

        private void GridSquareInfoController_OnGridSquareClicked(object sender, EventArgs e)
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

            bool isTileSelected = GridSquareInfoController.Instance.selectedGridPosition != null;

            BaseAction selectedAction = UnitActionSystem.Instance.selectedAction;
            bool isActive = selectedAction != null && selectedAction.isActive;

            if (isCameraOverhead && isMenuNone && isTileSelected && !isActive)
            {
                StartCoroutine(MenuFadeService.Instance.Fade(gridSquareInfoCanvasGroup, 1));
            }
            else
            {
                StartCoroutine(MenuFadeService.Instance.Fade(gridSquareInfoCanvasGroup, 0));
            }
        }
    }
}

