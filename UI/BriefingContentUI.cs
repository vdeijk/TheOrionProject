using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class BriefingContentUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI briefingContent;

        private void OnEnable()
        {
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChange;
        }

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChange;
        }

        private void Start()
        {
            StoryData.Instance.InitStoryContent();
        }

        private void MenuChangeService_OnMenuChange(object sender, EventArgs e)
        {
            if (MenuChangeMonobService.Instance.curMenu == MenuType.NewGame)
            {
                SetBriefingContent();
            }
        }

        private void SetBriefingContent()
        {
            briefingContent.text = StoryData.Instance.StoryContent[LevelManager.Instance.level - 1];
        }
    }
}
