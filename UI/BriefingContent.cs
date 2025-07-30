using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class BriefingContent : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI briefingContent;
        [SerializeField] MissionData missionData;
        [SerializeField] GameDurations gameDurations;
        [SerializeField] StoryData storyData;

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChange;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChange;
        }

        private void Start()
        {
            storyData.InitializeMissionTips();
        }

        private void MenuChangeService_OnMenuChange(object sender, EventArgs e)
        {
            if (MenuChangeService.Instance.curMenu == MenuType.NewGame)
            {
                SetBriefingContent();
            }
        }

        private void SetBriefingContent()
        {
            briefingContent.text = storyData.storyContent[LevelManager.Instance.level - 1];
        }
    }
}
