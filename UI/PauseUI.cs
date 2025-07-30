using System;
using UnityEngine;

namespace TurnBasedStrategy
{
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
