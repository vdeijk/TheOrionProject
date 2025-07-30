using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class DefeatUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI tipText;
        [SerializeField] MissionData missionData;

        private void OnEnable()
        {
            ControlModeManager.OnGameModeChanged += ControlModeManager_OnGameModeChanged;
        }

        private void OnDisable()
        {
            ControlModeManager.OnGameModeChanged -= ControlModeManager_OnGameModeChanged;
        }

        private void ControlModeManager_OnGameModeChanged(object sender, System.EventArgs e)
        {
            if (ControlModeManager.Instance.gameControlType == GameControlType.Outside)
            {
                int level = LevelManager.Instance.level;
                int randomIndex = Random.Range(0, missionData.missionTips.Count);
                tipText.text = "<b>Tip:</b> " + missionData.missionTips[randomIndex];
            }
        }
    }
}
