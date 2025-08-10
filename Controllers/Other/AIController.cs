using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    /// <summary>
    /// Controls the enemy AI turn logic, including decision making for actions (shoot, move, pass).
    /// Handles phase changes and action completion events to manage AI state transitions.
    /// </summary>
    public class AIController: MonoBehaviour
    {
        private float timer = 1; // Timer for AI action pacing
        private State state = State.Busy; // Current AI state

        private enum State { TakingTurn, Busy, }

        private bool isInMission => ControlModeManager.Instance.gameControlType == GameControlType.Mission;
        private DurationData durationData => DurationData.Instance;

        private void OnEnable()
        {
            PhaseManager.OnPhaseChanged += ControlModeManager_OnPhaseChanged;
            ActionBaseService.OnActionCompleted += UnitActionSystem_OnActionCompleted;
        }

        private void OnDisable()
        {
            PhaseManager.OnPhaseChanged -= ControlModeManager_OnPhaseChanged;
            ActionBaseService.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
        }

        private void Update()
        {
            // Only act during enemy phase and mission mode
            if (PhaseManager.Instance.isPlayerPhase || !isInMission) return;

            timer -= Time.deltaTime;

            // If it's time to act, try to take actions or end phase
            if (state == State.TakingTurn && timer < 0)
            {
                if (UnitAIService.Instance.TryTakeAction()) state = State.Busy;
                else PhaseManager.Instance.SetNextPhase();
            }
        }

        /// <summary>
        /// Called when the phase changes, sets AI to take turn if appropriate.
        /// </summary>
        private void ControlModeManager_OnPhaseChanged(object sender, EventArgs e)
        {
            SetStateTakeTurn();
        }

        /// <summary>
        /// Called when a unit action is completed, sets AI to take turn if appropriate.
        /// </summary>
        private void UnitActionSystem_OnActionCompleted(object sender, EventArgs e)
        {
            SetStateTakeTurn();
        }

        /// <summary>
        /// Prepares the AI to take its turn by resetting timer and state.
        /// </summary>
        private void SetStateTakeTurn()
        {
            if (PhaseManager.Instance.isPlayerPhase) return;

            timer = durationData.ActionDuration;

            state = State.TakingTurn;
        }
    }
}

