using UnityEngine;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Represents an action that the enemy AI can take, including the target grid position, action value, and the action itself.
    /// Used for AI decision making to evaluate and select the best possible action.
    /// </summary>
    public class UnitAIData
    {
        public Vector2Int GridPosition { get; set; }
        public int ActionValue { get; set; } // The value/score of the action for AI evaluation
        public BaseActionData BaseActionData { get; set; } // The action to be performed
    }
}
