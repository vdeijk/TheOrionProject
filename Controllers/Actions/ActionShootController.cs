using System;
using UnityEngine;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    [Serializable]
    /// <summary>
    /// Handles shooting actions for a unit, including targeting, animation, and projectile creation.
    /// </summary>
    public class ActionShootController : ActionBaseController<ShootActionData>
    {
        public static event EventHandler OnStartedShooting;
        public static event EventHandler OnStoppedShooting;

        private void FixedUpdate()
        {
            if (!TypedData.IsActive || !TypedData.selectedTarget) return;

            ActionShootService.Instance.ExecuteUpdate();
        }
    }
}

