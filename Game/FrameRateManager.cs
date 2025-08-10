using UnityEngine;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    // Sets a fixed frame rate for the application to ensure consistent gameplay experience
    public class FrameRateManager
    {
        private static FrameRateManager _instance;

        public static FrameRateManager Instance => _instance ??= new FrameRateManager();

        public void Init()
        {
            Application.targetFrameRate = 60;
        }
    }
}
