using UnityEngine;
using TurnBasedStrategy.Game;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class GameOverClickHandlers : MonoBehaviour
    {
        public void Continue()
        {
            ControlModeManager.Instance.EnterMenuMode();
        }
    }
}