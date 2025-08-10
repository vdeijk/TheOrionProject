using UnityEngine;
using TurnBasedStrategy.UI;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    public class LevelManager
    {
        private static LevelManager _instance;

        public static LevelManager Instance => _instance ??= new LevelManager();

        public int level { get; private set; } = 1;


        public PlayerProgressState InitPlayerProgress()
        {
            PlayerProgressState playerProgress = new PlayerProgressState();
            playerProgress.level = level = 1;

            return playerProgress;
        }

        public void SetPlayerProgress(PlayerProgressState playerProgress)
        {
            level = playerProgress.level;
        }

        public PlayerProgressState GetPlayerProgress()
        {
            PlayerProgressState playerProgress = new PlayerProgressState();
            playerProgress.level = level;
            return playerProgress;
        }

        public void SetLevel(int newLevel)
        {
            if (newLevel > 5)
            {
                level = 5;
                PopupFadingUI.Instance.FadeIn(TutorialType.LevelCap);
            }
            else
            {
                level = newLevel;
            }
        }
    }
}