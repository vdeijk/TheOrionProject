using UnityEngine;
using System;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    public class TimeScaleManager
    {
        private static TimeScaleManager _instance;
        private GameController _controller;

        public static TimeScaleManager Instance => _instance ??= new TimeScaleManager();

        // Event for notifying listeners when game speed changes
        public static event EventHandler OnGameSpeedSet;

        public void Init(GameController controller)
        {
            _controller = controller;
        }

        // Pauses the game by setting time scale to zero
        public void SetTimeScaleToZero()
        {
            Time.timeScale = 0;
        }

        // Sets time scale to fast for enemy phase or accelerated gameplay
        public void SetTimeScaleFast()
        {
            Time.timeScale = _controller.FastScale;
            OnGameSpeedSet?.Invoke(this, EventArgs.Empty);
        }

        // Sets time scale to normal for standard gameplay
        public void SetTimeScaleNormal()
        {
            Time.timeScale = _controller.NormalScale;
            OnGameSpeedSet?.Invoke(this, EventArgs.Empty);
        }
    }
}

