using UnityEngine;
using System.Collections.Generic;
using System;

namespace TurnBasedStrategy
{
    public class GridSystemVisuals : Singleton<GridSystemVisuals>
    {
        [SerializeField] private Transform gridSystemVisualSinglePrefab;
        [SerializeField] List<GridVisualMaterialType> gridVisualMaterialTypes = new List<GridVisualMaterialType>();
        [SerializeField] LayerMask terrainLayer;
        [SerializeField] GridData gridData;
        [SerializeField] Transform gridObjectDebugPrefab;

        private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

        public enum GridVisualType
        {
            White,
            Red,
            Bue,
            YellowFullAlpha,
            RedFullAlpha,
            BlueFullAlpha,
        }

        [Serializable]
        public struct GridVisualMaterialType
        {
            public GridVisualType gridVisualType;
            public Material material;
        }

        protected override void Awake()
        {
            Instance = SetSingleton();
            // Initialize grid visuals array based on grid dimensions
            gridSystemVisualSingleArray = new GridSystemVisualSingle[
                gridData.gridWidth,
                gridData.gridHeight
            ];
        }

        private void OnDestroy()
        {
            // Unsubscribe from all events to prevent memory leaks
            UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
            BaseAction.OnActionStarted -= BaseAction_OnActionStarted;
            UnitActionSystem.Instance.OnActionCompleted -= BaseAction_OnActionCompleted;
            PhaseManager.OnPhaseChanged -= TurnSystem_OnPhaseChanged;
            UnitCategoryService.OnUnitRemoved -= UnitManager_OnUnitRemoved;
            UnitSelectService.OnUnitDeselected -= UnitSelectionSystem_OnUnitDeselected;
            GridSquareInfoController.OnGridSquareClicked -= GridSquareInfoController_OnGridSquareClicked;
            GridSquareInfoController.OnClickedOutsideGrid -= GridSquareInfoController_OnClickedOutsideGrid;
            UnitSelectService.OnUnitSelected -= UnitSelectionSystem_OnUnitSelected;
            CameraChangeService.OnCameraChanged -= ControlModeManager_OnGameModeChanged;
        }

        private void Start()
        {
            // Subscribe to relevant game events for grid visual updates
            UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
            BaseAction.OnActionStarted += BaseAction_OnActionStarted;
            UnitActionSystem.Instance.OnActionCompleted += BaseAction_OnActionCompleted;
            PhaseManager.OnPhaseChanged += TurnSystem_OnPhaseChanged;
            UnitCategoryService.OnUnitRemoved += UnitManager_OnUnitRemoved;
            UnitSelectService.OnUnitDeselected += UnitSelectionSystem_OnUnitDeselected;
            GridSquareInfoController.OnGridSquareClicked += GridSquareInfoController_OnGridSquareClicked;
            GridSquareInfoController.OnClickedOutsideGrid += GridSquareInfoController_OnClickedOutsideGrid;
            UnitSelectService.OnUnitSelected += UnitSelectionSystem_OnUnitSelected;
            CameraChangeService.OnCameraChanged += ControlModeManager_OnGameModeChanged;

            // Instantiate grid visuals for each cell
            for (int x = 0; x < gridData.gridWidth; x++)
            {
                for (int z = 0; z < gridData.gridHeight; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);

                    Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(gridPosition);
                    Vector3 newWorldPos = new Vector3(worldPos.x, GetTerrainY(worldPos.x, worldPos.z), worldPos.z);
                    Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, newWorldPos, Quaternion.identity);

                    gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
                }
            }

            CreateDebugObjects(gridObjectDebugPrefab);
            UpdateGridVisualWithSelectedUnit();
        }

        // Instantiates debug objects for each grid cell
        public void CreateDebugObjects(Transform debugPrefab)
        {
            for (int x = 0; x < gridData.gridWidth; x++)
            {
                for (int z = 0; z < gridData.gridHeight; z++)
                {
                    GridPosition gridPosition = new GridPosition(x, z);

                    Transform debugTransform = Instantiate(debugPrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                    GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                    gridDebugObject.SetGridObject(gridData.gridObjectArray[gridPosition.x, gridPosition.z]);
                }
            }
        }

        // Hides all grid visuals except those in the exclude list
        private void HideGrid(HashSet<GridPosition> excludePositions = null)
        {
            for (int x = 0; x < gridData.gridWidth; x++)
            {
                for (int z = 0; z < gridData.gridHeight; z++)
                {
                    GridPosition current = new GridPosition(x, z);

                    if (excludePositions != null && excludePositions.Contains(current))
                        continue;

                    gridSystemVisualSingleArray[x, z].Hide();
                }
            }
        }

        // Shows grid visuals for a list of positions with a specific material
        private void ShowGridPositions(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
        {
            foreach (GridPosition gridPosition in gridPositionList)
            {
                gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualMaterialType(gridVisualType));
            }
        }

        // Shows grid visual for a single position
        private void ShowGridPosition(GridPosition gridPosition, GridVisualType gridVisualType)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualMaterialType(gridVisualType));
        }

        // Gets terrain height at a given position using raycast
        private float GetTerrainY(float x, float z)
        {
            if (Physics.Raycast(new Vector3(x, 500, z), Vector3.down, out RaycastHit raycastHit, float.MaxValue, terrainLayer))
            {
                return raycastHit.point.y;
            }
            return 0;
        }

        // Event handlers for updating grid visuals based on game state changes
        private void UnitSelectionSystem_OnUnitSelected(object sender, EventArgs e)
        {
            UpdateGridVisualWithSelectedUnit();
        }

        private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
        {
            UpdateGridVisualWithSelectedUnit();
        }

        private void BaseAction_OnActionStarted(object sender, EventArgs e)
        {
            UpdateGridVisualWithSelectedUnit();
        }

        private void BaseAction_OnActionCompleted(object sender, EventArgs e)
        {
            UpdateGridVisualWithSelectedUnit();
        }

        private void TurnSystem_OnPhaseChanged(object sender, EventArgs e)
        {
            UpdateGridVisualWithSelectedUnit();
        }

        private void UnitSelectionSystem_OnUnitDeselected(object sender, EventArgs e)
        {
            UpdateGridVisualWithSelectedUnit();
        }

        private void UnitManager_OnUnitRemoved(object sender, EventArgs e)
        {
            UpdateGridVisualWithSelectedUnit();
        }

        private void ControlModeManager_OnGameModeChanged(object sender, EventArgs e)
        {
            UpdateGridVisualWithSelectedUnit();
        }

        private void GridSquareInfoController_OnGridSquareClicked(object sender, EventArgs e)
        {
            if (UnitSelectService.Instance.selectedUnit != null) return;
            UpdateGridVisualWithSelectedGridSquare();
        }

        private void GridSquareInfoController_OnClickedOutsideGrid(object sender, EventArgs e)
        {
            HideGrid();
        }

        // Updates grid visuals based on the currently selected unit and action
        private void UpdateGridVisualWithSelectedUnit()
        {
            Unit selectedUnit = UnitSelectService.Instance.selectedUnit;
            BaseAction selectedAction = UnitActionSystem.Instance.selectedAction;

            if (selectedUnit == null) return;

            if (selectedAction != null && selectedAction.isActive)
            {
                HideGrid();
                return;
            }

            UnitFaction unitFaction = selectedUnit.unitEntityTransform.GetComponent<UnitFaction>();
            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(selectedUnit.unitBodyTransform.position);

            HashSet<GridPosition> visiblePositions = new HashSet<GridPosition>
            {
                gridPosition
            };

            HideGrid(visiblePositions);

            GridVisualType gridVisualType = GridVisualType.BlueFullAlpha;
            if (selectedUnit.unitEntityTransform.GetComponent<UnitFaction>().IS_ENEMY)
            {
                gridVisualType = GridVisualType.RedFullAlpha;
            }
            ShowGridPosition(gridPosition, gridVisualType);

            if (selectedAction == null) return;

            ActionSystem actionSystem = selectedUnit.unitMindTransform.GetComponent<ActionSystem>();
            if (actionSystem.actionPoints <= 0) return;

            switch (selectedAction)
            {
                case MoveAction:
                    ShowGridPositions(UnitActionSystem.Instance.validGridPositions, GridVisualType.YellowFullAlpha);
                    break;

                case ShootAction:
                    ShowGridPositions(UnitActionSystem.Instance.validGridPositions, GetGridVisualTypeShootAction(selectedUnit));
                    break;
            }
        }

        // Updates grid visuals based on the currently selected grid square
        private void UpdateGridVisualWithSelectedGridSquare()
        {
            GridPosition gridPosition = (GridPosition)GridSquareInfoController.Instance.selectedGridPosition;
            HashSet<GridPosition> visiblePositions = new HashSet<GridPosition>
            {
                gridPosition
            };

            HideGrid(visiblePositions);

            List<GridPosition> gridPositions = new List<GridPosition>();
            gridPositions.Add(gridPosition);
            ShowGridPositions(gridPositions, GridVisualType.YellowFullAlpha);
        }

        // Determines grid visual type for shoot actions based on unit faction
        private GridVisualType GetGridVisualTypeShootAction(Unit selectedUnit)
        {
            UnitFaction unitFaction = selectedUnit.unitEntityTransform.GetComponent<UnitFaction>();
            if (unitFaction.IS_ENEMY) return GridVisualType.BlueFullAlpha;
            return GridVisualType.RedFullAlpha;
        }

        // Gets the material for a given grid visual type
        private Material GetGridVisualMaterialType(GridVisualType gridVisualType)
        {
            foreach (GridVisualMaterialType gridVisualMaterialType in gridVisualMaterialTypes)
            {
                if (gridVisualMaterialType.gridVisualType == gridVisualType) return gridVisualMaterialType.material;
            }
            return null;
        }
    }
}

