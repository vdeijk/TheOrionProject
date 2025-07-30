using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class MissionOutcomeManager : Singleton<MissionOutcomeManager>
    {
        // Checks if the player has been defeated and triggers the appropriate game state
        public void CheckHasBeenDefeated()
        {
            if (ControlModeManager.Instance.gameControlType == GameControlType.Outside) return;

            // If no allies remain, transition to defeat mode
            if (UnitCategoryService.Instance.allies.Count <= 0)
            {
                StopAllCoroutines(); // Ensure no conflicting coroutines are running
                StartCoroutine(ControlModeManager.Instance.EnterDefeatMode());
            }
            else
            {
                // Otherwise, continue the mission
                ControlModeManager.Instance.EnterMissionMode();
            }
        }
    }
}