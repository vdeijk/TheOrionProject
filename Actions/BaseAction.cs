using System.Collections.Generic;
using UnityEngine;
using System;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Abstract base class for all unit actions (move, shoot, pass, etc.).
    /// </summary>
    public abstract class BaseAction : MonoBehaviour
    {
        public class OnActionCompletedEventArgs : EventArgs
        {
            public BaseAction baseAction;
        }

        public class OnActionStartedEventArgs : EventArgs
        {
            public BaseAction baseAction;
        }

        [field: SerializeField] public ActionType actionType { get; protected set; }
        [field: SerializeField] public Unit unit { get; protected set; }
        [field: SerializeField] public bool hasMaxRange { get; protected set; }
        public GridPositionType GRID_POSITION_TYPE { get; protected set; }
        public bool isActive { get; protected set; } = false;
        public string actionName { get; protected set; }
        public int Range { get; protected set; }
        public int APCost { get; protected set; } = 1;

        public static event EventHandler<OnActionStartedEventArgs> OnActionStarted;
        public static event EventHandler<OnActionCompletedEventArgs> OnActionCompleted;

        protected virtual void Awake() { }

        public abstract void TakeAction(GridPosition gridPosition);

        protected abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

        /// <summary>
        /// Returns the best EnemyAIAction based on action value for all valid grid positions.
        /// </summary>
        public virtual EnemyAIAction GetBestEnemyAIAction()
        {
            GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(unit.unitBodyTransform.position);

            List<EnemyAIAction> enemyAIActions = new List<EnemyAIAction>();
            List<GridPosition> validGridPositions = LevelGrid.Instance.GetValidPositions(unit, Range, GRID_POSITION_TYPE, unitGridPosition);

            foreach (GridPosition gridPosition in validGridPositions)
                enemyAIActions.Add(GetEnemyAIAction(gridPosition));

            if (enemyAIActions.Count > 0)
            {
                enemyAIActions.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
                return enemyAIActions[0];
            }

            return null;
        }

        /// <summary>
        /// Marks the action as started and invokes the OnActionStarted event.
        /// </summary>
        protected void ActionStart()
        {
            isActive = true;

            OnActionStarted?.Invoke(this, new OnActionStartedEventArgs
            {
                baseAction = this,
            });
        }

        /// <summary>
        /// Marks the action as completed and invokes the OnActionCompleted event.
        /// </summary>
        protected void ActionComplete()
        {
            isActive = false;

            OnActionCompleted?.Invoke(this, new OnActionCompletedEventArgs
            {
                baseAction = this,
            });
        }
    }
}
