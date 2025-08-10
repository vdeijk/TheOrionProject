using System;
using UnityEngine;
using System.Collections;
using TurnBasedStrategy.Game;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class BriefingClickHandlers : MonoBehaviour
    {
        public void StartPrep()
        {
            StartCoroutine(ControlModeManager.Instance.EnterPrepMode(true));
        }

        public void Back()
        {
            ControlModeManager.Instance.EnterMenuMode();
        }
    }
}

