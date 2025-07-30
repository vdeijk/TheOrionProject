using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class PhaseManager : Singleton<PhaseManager>
    {
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

            SFXController.Instance.PlaySFX(SFXType.StartPhase);

            OnPhaseChanged?.Invoke(this, EventArgs.Empty);
        }

        // Advances to the next phase, toggling player/enemy and updating turn number
        public void SetNextPhase()
        {
            isPlayerPhase = !isPlayerPhase;

            if (isPlayerPhase)
            {
                turnNumber++;

                List<Unit> allies = UnitCategoryService.Instance.allies;
                if (allies.Count > 0)
                {
                    UnitSelectService.Instance.SelectUnit(allies[0]);
                    CameraSmoothingService.Instance.StartCentering(allies[0].transform.position);
                    TimeScaleManager.Instance.SetTimeScaleNormal();
                    SFXController.Instance.PlaySFX(SFXType.StartPhase);
                }

                InputManager.Instance.ToggleControls(true);
            }
            else
            {
                // Enemy phase: speed up time and disable controls
                TimeScaleManager.Instance.SetTimeScaleFast();
                InputManager.Instance.ToggleControls(false);
                // UnitSelectService.Instance.DeselectUnit(); // Optionally deselect unit
                SFXController.Instance.PlaySFX(SFXType.StartPhase);
            }

            OnPhaseChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

