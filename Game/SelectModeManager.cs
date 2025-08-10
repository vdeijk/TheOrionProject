using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    public class SelectModeManager
    {
        private static SelectModeManager _instance;

        public static SelectModeManager Instance => _instance ??= new SelectModeManager();

        public void NothingSelected()
        {
            UnitSelectService.Instance.DeselectGridSquare();
            UnitSelectService.Instance.DeselectUnit();
            ActionCoordinatorService.Instance.DeselectAction();
        }

        public void GridSquareSelected(Vector2Int gridPos)
        {
            UnitSelectService.Instance.SelectGridSquare(gridPos);
            UnitSelectService.Instance.DeselectUnit();
            ActionCoordinatorService.Instance.DeselectAction();
        }

        public void UnitSelected(UnitSingleController unit)
        {
            Vector2Int gridPos = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);
            UnitSelectService.Instance.SelectGridSquare(gridPos);
            UnitSelectService.Instance.SelectUnit(unit);
            ActionCoordinatorService.Instance.DeselectAction();
        }

        public void DefaultActionSelected(UnitSingleController unit)
        {
            Vector2Int gridPos = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);
            UnitSelectService.Instance.SelectGridSquare(gridPos);
            UnitSelectService.Instance.SelectUnit(unit);
            ActionCoordinatorService.Instance.SelectMoveAction();
        }

        public void ShootActionSelected(UnitSingleController unit)
        {
            Vector2Int gridPos = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);
            UnitSelectService.Instance.SelectGridSquare(gridPos);
            UnitSelectService.Instance.SelectUnit(unit);
            ActionCoordinatorService.Instance.SelectShootAction();
        }

        public void PassActionSelected(UnitSingleController unit)
        {
            Vector2Int gridPos = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);
            UnitSelectService.Instance.SelectGridSquare(gridPos);
            UnitSelectService.Instance.SelectUnit(unit);
            ActionCoordinatorService.Instance.SelectMoveAction();
        }
    }
}