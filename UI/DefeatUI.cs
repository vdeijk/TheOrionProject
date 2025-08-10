using TMPro;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class DefeatUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI tipText;

        private GameTipsData gameTipsData => GameTipsData.Instance;

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
                int randomIndex = Random.Range(0, gameTipsData.MissionTips.Count);
                tipText.text = "<b>Tip:</b> " + gameTipsData.MissionTips[randomIndex];
            }
        }
    }
}
