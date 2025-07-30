using System;
using UnityEngine;
using System.Collections;

namespace TurnBasedStrategy
{
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

