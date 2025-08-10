using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    /// <summary>
    /// AIService class for managing AI-related functionalities in the game.
    /// This class is responsible for initializing and updating AI behaviors.
    /// </summary>
    public class UnitAIService
    {
        public delegate void TakeActionDelegate(Vector2Int gridPosition);

        public static UnitAIService Instance => _instance ??= new UnitAIService();

        private static UnitAIService _instance;

        /// <summary>
        /// Attempts to make each enemy unit perform an action. Returns true if any action was taken.
        /// </summary>
        public bool TryTakeAction()
        {
            foreach (UnitSingleController unit in UnitCategoryService.Instance.Data.Enemies)
            {
                UnitActionController actionSystem = unit.Data.UnitMindTransform.GetComponent<UnitActionController>();
                if (actionSystem.actionPoints <= 0) continue;

                if (TryTakeShootAction(unit)) return true;
                if (TryTakeMoveAction(unit)) return true;
                if (TryTakePassAction(unit)) return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to find and execute the best shoot action for the unit.
        /// </summary>
        private bool TryTakeShootAction(UnitSingleController unit)
        {
            UnitAIData bestAction = null;
            UnitActionController actionSystem = unit.Data.UnitMindTransform.GetComponent<UnitActionController>();

            foreach (ActionBaseController baseAction in actionSystem.baseActions)
            {
                if (baseAction is ActionShootController shootAction)
                {
                    var shootAI = ActionShootService.Instance.GetBestEnemyAction();
                    if (shootAI != null && shootAI.ActionValue > 0)
                    {
                        if (bestAction == null || shootAI.ActionValue > bestAction.ActionValue)
                        {
                            bestAction = shootAI;
                        }
                    }
                }
            }

            if (bestAction != null)
            {
                SelectModeManager.Instance.ShootActionSelected(unit);
                ActionShootService.Instance.TakeAction(bestAction.GridPosition);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to find and execute the best move action for the unit.
        /// </summary>
        private bool TryTakeMoveAction(UnitSingleController unit)
        {
            UnitAIData bestAction = null;
            UnitActionController actionSystem = unit.Data.UnitMindTransform.GetComponent<UnitActionController>();

            foreach (ActionBaseController baseAction in actionSystem.baseActions)
            {
                if (baseAction is ActionMoveController)
                {
                    var aiAction = ActionMoveService.Instance.GetBestEnemyAction();
                    if (aiAction != null && (bestAction == null || aiAction.ActionValue > bestAction.ActionValue))
                    {
                        bestAction = aiAction;
                    }
                }
            }

            if (bestAction != null)
            {
                SelectModeManager.Instance.DefaultActionSelected(unit);
                ActionMoveService.Instance.TakeAction(bestAction.GridPosition);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to execute a pass action for the unit if available.
        /// </summary>
        private bool TryTakePassAction(UnitSingleController unit)
        {
            UnitActionController actionSystem = unit.Data.UnitMindTransform.GetComponent<UnitActionController>();
            ActionPassController passAction = actionSystem.GetAction<ActionPassController>();

            if (passAction != null && actionSystem.actionPoints > 0)
            {
                var bestAction = ActionPassService.Instance.GetBestEnemyAction();
                SelectModeManager.Instance.PassActionSelected(unit);
                ActionPassService.Instance.TakeAction(bestAction.GridPosition);

                return true;
            }

            return false;
        }
    }
}