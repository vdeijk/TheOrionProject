using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class OverlayClickHandlers : MonoBehaviour
    {
        [SerializeField] Transform continueButtonTransform;
        [SerializeField] TextMeshProUGUI dateAndLevelText;

        private void OnEnable()
        {
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        private void Start()
        {
            SetContinueButton();
        }

        public void Continue()
        {
            TurnBasedStrategy.Game.ControlModeManager.Instance.EnterBriefingMode(false);
        }

        public void NewRun()
        {
            TurnBasedStrategy.Game.ControlModeManager.Instance.EnterBriefingMode(true);
        }

        public void EnterOptions()
        {
        }

        public void EnterCredits()
        {
            MenuChangeMonobService.Instance.ToCreditsMenu();
        }

        public void Quit()
        {
            Application.Quit();
        }

        private void MenuChangeService_OnMenuChanged(object sender, System.EventArgs e)
        {
            SetContinueButton();
            SetDateAndLevel();
        }

        private void SetDateAndLevel()
        {
            int level = TurnBasedStrategy.Game.LevelManager.Instance.level;
            int date = 4732 + level * 3;
            dateAndLevelText.text = "Starcycle " + date + ", " + "Battle " + level;
        }

        private void SetContinueButton()
        {
            if (MenuChangeMonobService.Instance.curMenu != MenuType.Main) return;

            if (TurnBasedStrategy.Game.LevelManager.Instance.level == 1)
            {
                continueButtonTransform.gameObject.SetActive(false);
            }
            else
            {
                continueButtonTransform.gameObject.SetActive(true);
            }
        }
    }
}