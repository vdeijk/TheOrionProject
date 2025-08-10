using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Game;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class GameSpeedUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI gameSpeedText;

        private void OnEnable()
        {
            TimeScaleManager.OnGameSpeedSet += TimeScaleManager_OnGameSpeedSet;
        }

        private void OnDisable()
        {
            TimeScaleManager.OnGameSpeedSet -= TimeScaleManager_OnGameSpeedSet;
        }

        private void Start()
        {
            SetGameSpeedText();
        }

        private void TimeScaleManager_OnGameSpeedSet(object sender, EventArgs e)
        {
            SetGameSpeedText();
        }

        private void SetGameSpeedText()
        {
            gameSpeedText.text = Time.timeScale.ToString() + "X";
        }

    }
}

