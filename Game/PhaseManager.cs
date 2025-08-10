using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    public class PhaseManager
    {
        private static PhaseManager _instance;

        public static PhaseManager Instance => _instance ??= new PhaseManager();

        public int turnNumber { get; private set; } = 1;
        public bool isPlayerPhase { get; private set; } = true;

        public static event EventHandler OnPhaseChanged;

        // Resets phase to initial state for a new game or scenario
        public void ResetPhase()
        {
            turnNumber = 1;
            isPlayerPhase = true;
        }

        // Starts the first phase and notifies listeners
        public void StartFirstPhase()
        {
            isPlayerPhase = true;

            if (isPlayerPhase)
            {
                turnNumber = 1;
            }

            SFXMonobService.Instance.PlaySFX(SFXType.StartPhase);

            OnPhaseChanged?.Invoke(this, EventArgs.Empty);
        }

        // Advances to the next phase, toggling player/enemy and updating turn number
        public void SetNextPhase()
        {
            isPlayerPhase = !isPlayerPhase;

            if (isPlayerPhase)
            {
                turnNumber++;

                List<UnitSingleController> allies = UnitCategoryService.Instance.Data.Allies;
                if (allies.Count > 0)
                {
                    SelectModeManager.Instance.DefaultActionSelected(UnitCategoryService.Instance.Data.Allies[0]);
                    CameraSmoothingMonobService.Instance.StartCentering(allies[0].transform.position);
                    TimeScaleManager.Instance.SetTimeScaleNormal();
                    SFXMonobService.Instance.PlaySFX(SFXType.StartPhase);
                }

                InputManager.Instance.ToggleControls(true);
            }
            else
            {
                // Enemy phase: speed up time and disable controls
                TimeScaleManager.Instance.SetTimeScaleFast();
                InputManager.Instance.ToggleControls(false);
                // UnitSelectService.Instance.DeselectUnit(); // Optionally deselect unit
                SFXMonobService.Instance.PlaySFX(SFXType.StartPhase);
            }

            OnPhaseChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

