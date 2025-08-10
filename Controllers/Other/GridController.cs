using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class GridController : MonoBehaviour
    {
        [field: SerializeField] public GridData Data { get; private set; }

        [SerializeField] bool createLevelOnPlay;

        private void Start()
        {
            Data.InitHeightAndWidth();
            GridCoordinatorService.Instance.Init(Data);

            if (createLevelOnPlay)
            {
                DeleteObjects();
                LevelGeneratorService.Instance.CreateGrid();
                LevelGeneratorService.Instance.PlaceObjects();
            }
        }

        // Removes all child objects from the grid parent
        public void DeleteObjects()
        {
            for (int i = Data.GridParent.childCount; i > 0; --i)
            {
                DestroyImmediate(Data.GridParent.GetChild(0).gameObject);
            }
        }

        public Transform InstantiateGridSquare(GridSquareSO gridDataSquareType, Vector3 spawnPosition)
        {
            int randomPrefab = UnityEngine.Random.Range(0, gridDataSquareType.prefabTransforms.Length);
            int randomRotation = UnityEngine.Random.Range(0, 360);

            return Instantiate(gridDataSquareType.prefabTransforms[randomPrefab], spawnPosition, Quaternion.Euler(new Vector3(0, randomRotation, 0)));
        }

        public Transform InstantiateGridSquare(Vector3 pos)
        {
            return Instantiate(Data.GridSquarePrefab, pos, Quaternion.identity);
        }
    }
}


/*

        // Event handlers for updating grid visuals based on game state changes
        private void UnitSelectionSystem_OnUnitSelected(object sender, EventArgs e)
        {
            GridSystemService.Instance.UpdateWithSelectedUnit();
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            GridSystemService.Instance.UpdateWithSelectedUnit();
        }

        private void BaseAction_OnActionStarted(object sender, EventArgs e)
        {
            GridSystemService.Instance.UpdateWithSelectedUnit();
        }

        private void BaseAction_OnActionCompleted(object sender, EventArgs e)
        {
            GridSystemService.Instance.UpdateWithSelectedUnit();
        }

        private void TurnSystem_OnPhaseChanged(object sender, EventArgs e)
        {
            GridSystemService.Instance.UpdateWithSelectedUnit();
        }

        private void UnitSelectionSystem_OnUnitDeselected(object sender, EventArgs e)
        {
            GridSystemService.Instance.UpdateWithSelectedUnit();
        }

        private void UnitManager_OnUnitRemoved(object sender, EventArgs e)
        {
            GridSystemService.Instance.UpdateWithSelectedUnit();
        }

        private void ControlModeManager_OnGameModeChanged(object sender, EventArgs e)
        {
            GridSystemService.Instance.UpdateWithSelectedUnit();
        }

        private void GridSquareInfoController_OnGridSquareClicked(object sender, EventArgs e)
        {
            if (UnitSelectService.Instance.Data.SelectedUnit != null) return;

            GridSystemService.Instance.UpdateWithSelectedGridSquare();
        }

        private void GridSquareInfoController_OnClickedOutsideGrid(object sender, EventArgs e)
        {
            GridSystemService.Instance.Hide();
        }
        private void OnEnable()
        {
            BaseActionService.OnActionSelected += UnitActionSystem_OnSelectedActionChanged;
            BaseActionService.OnActionStarted += BaseAction_OnActionStarted;
            BaseActionService.OnActionCompleted += BaseAction_OnActionCompleted;
            PhaseManager.OnPhaseChanged += TurnSystem_OnPhaseChanged;
            UnitCategoryService.OnUnitRemoved += UnitManager_OnUnitRemoved;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
            GridSquareInfoService.OnGridSquareClicked += GridSquareInfoController_OnGridSquareClicked;
            GridSquareInfoService.OnClickedOutsideGrid += GridSquareInfoController_OnClickedOutsideGrid;
            UnitSelectService.OnUnitSelected += UnitSelectionSystem_OnUnitSelected;
            CameraChangeService.OnCameraChanged += ControlModeManager_OnGameModeChanged;
        }

        private void OnDestroy()
        {
            BaseActionService.OnActionSelected -= UnitActionSystem_OnSelectedActionChanged;
            BaseActionService.OnActionStarted -= BaseAction_OnActionStarted;
            BaseActionService.OnActionCompleted -= BaseAction_OnActionCompleted;
            PhaseManager.OnPhaseChanged -= TurnSystem_OnPhaseChanged;
            UnitCategoryService.OnUnitRemoved -= UnitManager_OnUnitRemoved;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
            GridSquareInfoService.OnGridSquareClicked -= GridSquareInfoController_OnGridSquareClicked;
            GridSquareInfoService.OnClickedOutsideGrid -= GridSquareInfoController_OnClickedOutsideGrid;
            UnitSelectService.OnUnitSelected -= UnitSelectionSystem_OnUnitSelected;
            CameraChangeService.OnCameraChanged -= ControlModeManager_OnGameModeChanged;
        }
*/