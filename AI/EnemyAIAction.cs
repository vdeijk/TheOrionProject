using UnityEngine;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Represents an action that the enemy AI can take, including the target grid position, action value, and the action itself.
    /// Used for AI decision making to evaluate and select the best possible action.
    /// </summary>
    public class EnemyAIAction
    {
        public GridPosition gridPosition;
        public int actionValue; // The value/score of the action for AI evaluation
        public BaseAction baseAction; // The action to be performed
    }
}
