using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    public class GridActionService
    {
        private static GridActionService _instance;

        public static GridActionService Instance => _instance ??= new GridActionService();

        private GridData Data => GridCoordinatorService.Instance.Data;


        // Returns valid grid positions for a unit based on range, type, and terrain
        public List<Vector2Int> GetPositions(BaseActionData data)
        {
            List<Vector2Int> validGridPositionList = new List<Vector2Int>();

            if (data == null) return validGridPositionList;

            Vector2Int gridPosition = GridUtilityService.Instance.GetGridPosition(data.Unit.Data.UnitBodyTransform.position);

            for (int x = -data.partSO.Range; x <= data.partSO.Range; x++)
            {
                for (int z = -data.partSO.Range; z <= data.partSO.Range; z++)
                {
                    Vector2Int offsetGridPosition = new Vector2Int(x, z);
                    Vector2Int testGridPosition = gridPosition + offsetGridPosition;

                    if (!GridUtilityService.Instance.IsValidPosition(testGridPosition)) continue;
                    if (Mathf.Abs(x) + Mathf.Abs(z) > data.partSO.Range) continue;

                    GridObject gridObject = Data.Objects[testGridPosition.x, testGridPosition.y];
                    if (gridObject.gridSquareType == GridSquareType.Inaccessible) continue;

                    BaseSO baseData = (BaseSO)data.Unit.Data.PartsData[PartType.Base];
                    bool isMech = baseData.UnitType == UnitType.Ground;
                    bool isSteep = gridObject.gridSquareType == GridSquareType.Steep;
                    if (isMech && isSteep) continue;

                    UnitSingleController targetUnit = GridUnitService.Instance.GetUnit(testGridPosition);

                    switch (data.ActionType)
                    {
                        case ActionType.ShootPrimary:
                        case ActionType.ShootSecondary:
                            if (GridUnitService.Instance.GetIsEnemy(data.Unit, targetUnit)) validGridPositionList.Add(testGridPosition);
                            break;
                        case ActionType.Move:
                            if (targetUnit == null) validGridPositionList.Add(testGridPosition);
                            break;
                        default:
                            validGridPositionList.Add(testGridPosition);
                            break;
                    }
                }
            }

            return validGridPositionList;
        }

        // Returns valid grid positions for the selected action
        public void UpdatePositions(UnitSingleController unit)
        {
            Data.ValidGridPositions = GetPositions(ActionCoordinatorService.Instance.SelectedActionData);
        }
    }
}
