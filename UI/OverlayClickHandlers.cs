using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class OverlayClickHandlers : MonoBehaviour
    {
        [SerializeField] MissionData missionData;
        [SerializeField] Transform continueButtonTransform;
        [SerializeField] TextMeshProUGUI dateAndLevelText;

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        private void Start()
        {
            SetContinueButton();
        }

        public void Continue()
        {
            ControlModeManager.Instance.EnterBriefingMode(false);
        }

        public void NewRun()
        {
            ControlModeManager.Instance.EnterBriefingMode(true);
        }

        public void EnterOptions()
        {
        }

        public void EnterCredits()
        {
            MenuChangeService.Instance.ToCreditsMenu();
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
            int level = LevelManager.Instance.level;
            int date = 4732 + level * 3;
            dateAndLevelText.text = "Starcycle " + date + ", " + "Battle " + level;
        }

        private void SetContinueButton()
        {
            if (MenuChangeService.Instance.curMenu != MenuType.Main) return;

            if (LevelManager.Instance.level == 1)
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