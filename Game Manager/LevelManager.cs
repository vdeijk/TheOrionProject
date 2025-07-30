using UnityEngine;

namespace TurnBasedStrategy
{
    public class LevelManager : Singleton<LevelManager>
    {
        public int level { get; private set; } = 1;


        public PlayerProgress InitPlayerProgress()
        {
            PlayerProgress playerProgress = new PlayerProgress();
            playerProgress.level = level = 1;

            return playerProgress;
        }

        public void SetPlayerProgress(PlayerProgress playerProgress)
        {
            level = playerProgress.level;
        }

        public PlayerProgress GetPlayerProgress()
        {
            PlayerProgress playerProgress = new PlayerProgress();
            playerProgress.level = level;
            return playerProgress;
        }

        public void SetLevel(int newLevel)
        {
            if (newLevel > 5)
            {
                level = 5;
                PopupFading.Instance.FadeIn(TutorialType.LevelCap);
            }
            else
            {
                level = newLevel;
            }
        }
    }
}