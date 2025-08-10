using System;
using UnityEngine;
using TurnBasedStrategy.Game;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class PauseUI : MonoBehaviour
    {
        public void ToggleOptions()
        {
            Debug.Log("ToggleOptions");
        }

        public void Resume()
        {
            ControlModeManager.Instance.EnterMissionMode();
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
