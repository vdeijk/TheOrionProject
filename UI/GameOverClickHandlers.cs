using TurnBasedStrategy;
using UnityEngine;

public class GameOverClickHandlers : MonoBehaviour
{
    [SerializeField] MissionData missionData;

    public void Continue()
    {
        ControlModeManager.Instance.EnterMenuMode();
    }
}
