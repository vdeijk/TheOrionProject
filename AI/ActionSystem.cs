using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Manages the action points and available actions for a unit.
    /// Handles resetting action points on phase/menu changes and provides access to unit actions.
    /// </summary>
    public class ActionSystem : MonoBehaviour
    {
        [SerializeField] Unit unit;
        [field: SerializeField] public int MAX_ACTION_POINTS { get; private set; }

        public int actionPoints { get; private set; } // Current action points
        public BaseAction[] baseActions { get; private set; } // Actions available to the unit

        public static event EventHandler OnActionPointsChanged;

        private void Awake()
        {
            actionPoints = MAX_ACTION_POINTS;
            baseActions = unit.unitEntityTransform.GetComponents<BaseAction>();
        }

        private void OnEnable()
        {
            // Subscribe to phase and menu change events
            PhaseManager.OnPhaseChanged += PhaseManager_OnPhaseChanged;
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            PhaseManager.OnPhaseChanged -= PhaseManager_OnPhaseChanged;
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        /// <summary>
        /// Returns the first action of type T available to the unit.
        /// </summary>
        public T GetAction<T>() where T : BaseAction
        {
            foreach (BaseAction baseAction in baseActions)
            {
                if (baseAction is T)
                {
                    return (T)baseAction;
                }
            }
            return null;
        }

        /// <summary>
        /// Deducts action points when an action is performed and triggers the change event.
        /// </summary>
        public void SpendActionPoints(BaseAction selectedAction)
        {
            actionPoints -= 1;

            OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Resets action points when returning to the main menu.
        /// </summary>
        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            if (MenuChangeService.Instance.curMenu == MenuType.Main)
            {
                actionPoints = MAX_ACTION_POINTS;
                OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Resets action points at the start of a new phase.
        /// </summary>
        private void PhaseManager_OnPhaseChanged(object sender, EventArgs e)
        {
            actionPoints = MAX_ACTION_POINTS;

            OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
