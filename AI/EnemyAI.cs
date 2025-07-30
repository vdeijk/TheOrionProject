using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Controls the enemy AI turn logic, including decision making for actions (shoot, move, pass).
    /// Handles phase changes and action completion events to manage AI state transitions.
    /// </summary>
    public class EnemyAI : Singleton<EnemyAI>
    {
        private float timer = 1; // Timer for AI action pacing
        private State state = State.Busy; // Current AI state

        [SerializeField] GameDurations gameDurations; 

        private enum State { TakingTurn, Busy, }

        private bool isInMission => ControlModeManager.Instance.gameControlType == GameControlType.Mission;

        private void OnEnable()
        {
            PhaseManager.OnPhaseChanged += ControlModeManager_OnPhaseChanged;
            UnitActionSystem.Instance.OnActionCompleted += UnitActionSystem_OnActionCompleted;
        }

        private void OnDisable()
        {
            PhaseManager.OnPhaseChanged -= ControlModeManager_OnPhaseChanged;
            UnitActionSystem.Instance.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
        }

        private void Update()
        {
            // Only act during enemy phase and mission mode
            if (PhaseManager.Instance.isPlayerPhase || !isInMission) return;

            timer -= Time.deltaTime;

            // If it's time to act, try to take actions or end phase
            if (state == State.TakingTurn && timer < 0)
            {
                if (TryTakeActions()) state = State.Busy;
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

            timer = gameDurations.actionDuration;

            state = State.TakingTurn;
        }

        /// <summary>
        /// Attempts to make each enemy unit perform an action. Returns true if any action was taken.
        /// </summary>
        private bool TryTakeActions()
        {
            foreach (Unit enemyUnit in UnitCategoryService.Instance.enemies)
            {
                ActionSystem actionSystem = enemyUnit.unitMindTransform.GetComponent<ActionSystem>();
                if (actionSystem.actionPoints <= 0) continue;
                if (TryTakeAction(enemyUnit)) return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to perform shoot, move, or pass actions for the given unit.
        /// </summary>
        private bool TryTakeAction(Unit unit)
        {
            if (TryHandleShootAction(unit)) return true;
            if (TryHandleMoveAction(unit)) return true;
            if (TryHandlePassAction(unit)) return true;

            return false;
        }

        /// <summary>
        /// Tries to find and execute the best shoot action for the unit.
        /// </summary>
        private bool TryHandleShootAction(Unit unit)
        {
            EnemyAIAction bestShootAIAction = null;
            ActionSystem actionSystem = unit.unitMindTransform.GetComponent<ActionSystem>();

            foreach (BaseAction baseAction in actionSystem.baseActions)
            {
                if (baseAction is ShootAction shootAction)
                {
                    var shootAI = shootAction.GetBestEnemyAIAction();
                    if (shootAI != null && shootAI.actionValue > 0)
                    {
                        if (bestShootAIAction == null || shootAI.actionValue > bestShootAIAction.actionValue)
                        {
                            bestShootAIAction = shootAI;
                        }
                    }
                }
            }

            if (bestShootAIAction != null)
            {
                UnitActionSystem.Instance.HandleSelectedActionAI(bestShootAIAction, unit);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to find and execute the best move action for the unit.
        /// </summary>
        private bool TryHandleMoveAction(Unit unit)
        {
            EnemyAIAction bestEnemyAIAction = null;
            ActionSystem actionSystem = unit.unitMindTransform.GetComponent<ActionSystem>();

            foreach (BaseAction baseAction in actionSystem.baseActions)
            {
                if (baseAction is MoveAction)
                {
                    var aiAction = baseAction.GetBestEnemyAIAction();
                    if (aiAction != null && (bestEnemyAIAction == null || aiAction.actionValue > bestEnemyAIAction.actionValue))
                    {
                        bestEnemyAIAction = aiAction;
                    }
                }
            }

            if (bestEnemyAIAction != null)
            {
                UnitActionSystem.Instance.HandleSelectedActionAI(bestEnemyAIAction, unit);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to execute a pass action for the unit if available.
        /// </summary>
        private bool TryHandlePassAction(Unit unit)
        {
            ActionSystem actionSystem = unit.unitMindTransform.GetComponent<ActionSystem>();
            PassAction passAction = actionSystem.GetAction<PassAction>();

            if (passAction != null && actionSystem.actionPoints > 0)
            {
                EnemyAIAction passAIAction = passAction.GetBestEnemyAIAction();
                UnitActionSystem.Instance.HandleSelectedActionAI(passAIAction, unit);
                return true;
            }

            return false;
        }
    }
}

