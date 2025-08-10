using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class PopupFadingUI : SingletonBaseService<PopupFadingUI>
    {
        [SerializeField] CanvasGroup[] canvasGroupspopup;
        [SerializeField] TextMeshProUGUI popupContent;
        [SerializeField] TextMeshProUGUI popupTitle;

        private List<TutorialType> tutorialsViewed = new List<TutorialType>();

        private DurationData durationData => DurationData.Instance;

        private void Start()
        {
            foreach (CanvasGroup canvasGroup in canvasGroupspopup)
            {
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(false);
            }
        }

        public void FadeIn(TutorialType tutorialType)
        {
            if (tutorialsViewed.Contains(tutorialType) ||
                SaveDataManager.Instance.gameSaveData.haveTutorialsBeenViwed) return;

            tutorialsViewed.Add(tutorialType);

            var content = TutorialData.TutorialContent[tutorialType];
            popupTitle.text = content.Title;
            popupContent.text = content.Body;

            if (tutorialsViewed.Count >= Enum.GetValues(typeof(TutorialType)).Length)
            {
                SaveDataManager.Instance.gameSaveData.haveTutorialsBeenViwed = true;
                SaveDataManager.Instance.SaveGameData();
            }

            StartCoroutine(Delay());
        }

        public void FadeOut()
        {
            StopAllCoroutines();

            MenuFadeMonobService.Instance.Fade(false, canvasGroupspopup);
        }

        public IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(durationData.CameraBlendDuration);

            MenuFadeMonobService.Instance.Fade(true, canvasGroupspopup);
            SFXMonobService.Instance.PlaySFX(SFXType.Popup);
        }
    }
}