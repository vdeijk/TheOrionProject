using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    /// <summary>
    /// Handles movement actions for a unit, including pathfinding and movement logic.
    /// </summary>
    public class ActionMoveController : ActionBaseController<MoveActionData>
    {
        private void FixedUpdate()
        {
            if (!Data.IsActive) return;

            ActionMoveService.Instance.UpdateMovement();
        }
    }
}