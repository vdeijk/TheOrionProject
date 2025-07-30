using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class PopupFading : Singleton<PopupFading>
    {
        [SerializeField] CanvasGroup[] canvasGroupspopup;
        [SerializeField] TextMeshProUGUI popupContent;
        [SerializeField] TextMeshProUGUI popupTitle;
        [SerializeField] TutorialData tutorialData;
        [SerializeField] GameDurations gameDurations;

        private List<TutorialType> tutorialsViewed = new List<TutorialType>();

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
                SaveDataManager.Instance.gameData.haveTutorialsBeenViwed) return;

            tutorialsViewed.Add(tutorialType);

            var content = tutorialData.tutorialContent[tutorialType];
            popupTitle.text = content.Title;
            popupContent.text = content.Body;

            if (tutorialsViewed.Count >= Enum.GetValues(typeof(TutorialType)).Length)
            {
                SaveDataManager.Instance.gameData.haveTutorialsBeenViwed = true;
                SaveDataManager.Instance.SaveGameData();
            }

            StartCoroutine(Delay());
        }

        public void FadeOut()
        {
            StopAllCoroutines();

            MenuFadeService.Instance.Fade(false, canvasGroupspopup);
        }

        public IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(gameDurations.cameraBlendDuration);

            MenuFadeService.Instance.Fade(true, canvasGroupspopup);
            SFXController.Instance.PlaySFX(SFXType.Popup);
        }
    }
}