using System;
using UnityEngine;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class TileInfoFadingUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup gridSquareInfoCanvasGroup;

        private bool isCameraOverhead => CameraChangeMonobService.Instance.curCamera == Data.CameraType.Overhead;
        private bool isMenuNone => MenuChangeMonobService.Instance.curMenu == MenuType.None;

        private void Awake()
        {
            gridSquareInfoCanvasGroup.alpha = 0;
            gridSquareInfoCanvasGroup.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            UnitSelectService.OnGridSquareSelected += GridSquareInfoController_OnClickedOutsideGrid;
            UnitSelectService.OnGridSquareDeselected += GridSquareInfoController_OnGridSquareClicked;
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
            ActionBaseService.OnActionStarted += BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted += UnitActionSystem_OnActionCompleted;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            UnitSelectService.OnGridSquareSelected -= GridSquareInfoController_OnClickedOutsideGrid;
            UnitSelectService.OnGridSquareDeselected -= GridSquareInfoController_OnGridSquareClicked;
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
            ActionBaseService.OnActionStarted -= BaseAction_OnActionStarted;
            ActionBaseService.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
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

            bool isTileSelected = UnitSelectService.Instance.Data.SelectedGridPosition != null;

            if (isCameraOverhead && isMenuNone && isTileSelected && !ActionCoordinatorService.IsAnyActionActive())
            {
                StartCoroutine(MenuFadeMonobService.Instance.Fade(gridSquareInfoCanvasGroup, 1));
            }
            else
            {
                StartCoroutine(MenuFadeMonobService.Instance.Fade(gridSquareInfoCanvasGroup, 0));
            }
        }
    }
}

