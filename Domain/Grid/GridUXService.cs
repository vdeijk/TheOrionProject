using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    public class GridUXService
    {
        private static GridUXService _instance;

        public static GridUXService Instance => _instance ??= new GridUXService();
        private GridData gridData => GridCoordinatorService.Instance.Data;

        public void UpdateGrid()
        {
            var gridPositionOrNull = UnitSelectService.Instance.Data.SelectedGridPosition;
            UnitSingleController selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;

            Hide();

            // No unit or grid square is selected
            if (gridPositionOrNull == null || ActionCoordinatorService.IsAnyActionActive()) return;

            Vector2Int gridPosition = (Vector2Int)UnitSelectService.Instance.Data.SelectedGridPosition;

            //No unit is selected but a single grid square is
            if (selectedUnit == null)
            {
                gridData.GridSystemArray[gridPosition.x, gridPosition.y].Show(GetMaterialType(GridVisualType.YellowFullAlpha));
                return;
            }

            //Highlight grid square that belongs to the selected unit
            bool isEnemy = selectedUnit.Data.UnitEntityTransform.GetComponent<UnitFactionController>().IS_ENEMY;
            GridVisualType gridVisualType = isEnemy ? GridVisualType.RedFullAlpha : GridVisualType.BlueFullAlpha;

            gridData.GridSystemArray[gridPosition.x, gridPosition.y].Show(GetMaterialType(gridVisualType));

            // If the selected unit has no action points left, return early
            UnitActionController actionSystem = selectedUnit.Data.UnitMindTransform.GetComponent<UnitActionController>();
            if (actionSystem.actionPoints <= 0)
            {
                return;
            }

            //Update valid grid positions based on the selected action
            BaseActionData selectedActionData = ActionCoordinatorService.Instance.SelectedActionData;
            if (selectedActionData.ActionType == ActionType.Move || selectedActionData.ActionType == ActionType.Pass)
            {
                gridVisualType = GridVisualType.YellowFullAlpha;
            }
            else
            {
                gridVisualType = GridVisualType.RedFullAlpha;
            }

            foreach (Vector2Int gridPos in gridData.ValidGridPositions)
            {
                gridData.GridSystemArray[gridPos.x, gridPos.y].Show(GetMaterialType(gridVisualType));
            }
        }

        // Hides all grid visuals except those in the exclude list
        public void Hide(HashSet<Vector2Int> excludePositions = null)
        {
            for (int x = 0; x < gridData.Width; x++)
            {
                for (int z = 0; z < gridData.Height; z++)
                {
                    Vector2Int current = new Vector2Int(x, z);

                    if (excludePositions != null && excludePositions.Contains(current))
                        continue;

                    gridData.GridSystemArray[x, z].Hide();
                }
            }
        }

        // Gets the material for a given grid visual type
        private Material GetMaterialType(GridVisualType gridVisualType)
        {
            foreach (GridMaterialType materialType in gridData.materialTypes)
            {
                if (materialType.gridVisualType == gridVisualType) return materialType.material;
            }
            return null;
        }
    }
}

/*

        // Updates grid visuals based on the currently selected grid square
        public void UpdateSelectedGridSquare()
        {
            GridPosition gridPosition = (GridPosition)GridSquareInfoService.Instance.selectedGridPosition;
            HashSet<GridPosition> visiblePositions = new HashSet<GridPosition>
            {
                gridPosition
            };

            Hide(visiblePositions);

            gridData.GridSystemArray[gridPosition.x, gridPosition.z].Show(GetMaterialType(GridVisualType.YellowFullAlpha));


        }

        // Updates grid visuals based on the currently selected unit and action
        public void UpdateSelectedUnit()
        {
            Unit selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
            BaseActionData selectedActionData = ActionStateService.Instance.SelectedActionData;

            if (selectedUnit == null || selectedActionData == null || ActionStateService.IsAnyActionActive())
            {
                Hide();
                return;
            }

            UnitFaction unitFaction = selectedUnit.unitEntityTransform.GetComponent<UnitFaction>();
            GridPosition gridPosition = GridUtilityService.Instance.GetGridPosition(selectedUnit.unitBodyTransform.position);

            HashSet<GridPosition> visiblePositions = new HashSet<GridPosition>
            {
                gridPosition
            };

            Hide(visiblePositions);

            GridVisualType gridVisualType = GridVisualType.BlueFullAlpha;
            if (selectedUnit.unitEntityTransform.GetComponent<UnitFaction>().IS_ENEMY)
            {
                gridVisualType = GridVisualType.RedFullAlpha;
            }
            Show(gridPosition, gridVisualType);

            ActionSystem actionSystem = selectedUnit.unitMindTransform.GetComponent<ActionSystem>();
            if (actionSystem.actionPoints <= 0) return;

            foreach (GridPosition gridPos in gridData.ValidGridPositions)
            {
                gridData.GridSystemArray[gridPos.x, gridPos.z].Show(GetMaterialType(gridVisualType));
            }
        }*/