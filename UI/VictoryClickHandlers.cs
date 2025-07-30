using TurnBasedStrategy;
using UnityEngine;

public class VictoryClickHandlers : MonoBehaviour
{
    [SerializeField] MissionData missionData;

    public void Continue()
    {
        ControlModeManager.Instance.EnterMenuMode();
    }
}
