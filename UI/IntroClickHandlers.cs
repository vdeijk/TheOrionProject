using UnityEngine;
using TurnBasedStrategy.Game;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
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