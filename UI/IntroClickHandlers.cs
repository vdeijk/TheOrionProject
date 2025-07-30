using UnityEngine;

namespace TurnBasedStrategy
{
    public class IntroClickHandlers : MonoBehaviour
    {
        public void Continue()
        {
            StartCoroutine(ControlModeManager.Instance.EnterPrepMode(true));
        }

        public void Back()
        {
            ControlModeManager.Instance.EnterMenuMode();
        }
    }
}