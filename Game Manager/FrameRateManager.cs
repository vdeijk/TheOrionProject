using UnityEngine;

namespace TurnBasedStrategy
{
    public class FrameRateManager : MonoBehaviour
    {
        // Sets a fixed frame rate for the application to ensure consistent gameplay experience
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}
