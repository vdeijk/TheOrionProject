using System;
using TurnBasedStrategy.Game;
using UnityEngine;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class PreparationClickHandlers : MonoBehaviour
    {
        public event Action OnToggleOptionsTriggered;

        public void StartBattle()
        {
            ControlModeManager.Instance.EnterMissionMode();
            PhaseManager.Instance.StartFirstPhase();
        }
        
        public void ToggleViewRepairMode()
        {
            ControlModeManager.Instance.EnterRepairMode();
        }

        public void ToggleViewBattleField()
        {
            ControlModeManager.Instance.EnterPreviewMode();
        }

        public void ToggleAssembleMechs()
        {
            ControlModeManager.Instance.EnterAssemblyMode();
        }

        public void Back()
        {
            ControlModeManager.Instance.EnterBriefingMode(false);
        }
    }
}