using UnityEngine;
using System;

namespace TurnBasedStrategy
{
    public class TimeScaleManager : Singleton<TimeScaleManager>
    {
        [SerializeField] float FastScale;
        [SerializeField] float NormalScale;

        // Event for notifying listeners when game speed changes
        public static event EventHandler OnGameSpeedSet;

        // Pauses the game by setting time scale to zero
        public void SetTimeScaleToZero()
        {
            Time.timeScale = 0;
        }

        // Sets time scale to fast for enemy phase or accelerated gameplay
        public void SetTimeScaleFast()
        {
            Time.timeScale = FastScale;
            OnGameSpeedSet?.Invoke(this, EventArgs.Empty);
        }

        // Sets time scale to normal for standard gameplay
        public void SetTimeScaleNormal()
        {
            Time.timeScale = NormalScale;
            OnGameSpeedSet?.Invoke(this, EventArgs.Empty);
        }
    }
}

