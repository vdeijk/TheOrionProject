
using UnityEngine;
using System.Collections;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Infra
{
    public class MenuFadeMonobService : SingletonBaseService<MenuFadeMonobService>
    {
        private DurationData durationData => DurationData.Instance;

        public void DisableImmediately(CanvasGroup[] canvasGroups)
        {
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(false);
            }
        }

        public void Fade(bool isUIVisible, CanvasGroup[] canvasGroups)
        {
            if (isUIVisible)
            {
                StartCoroutine(WaitToFadein(canvasGroups));
            }
            else
            {
                foreach (CanvasGroup canvasGroup in canvasGroups)
                {
                    StartCoroutine(Fade(canvasGroup, 0));
                }
            }
        }

        public IEnumerator WaitToFadein(CanvasGroup[] canvasGroups)
        {
            yield return new WaitForSecondsRealtime(durationData.UiTransitionuration);

            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                StartCoroutine(Fade(canvasGroup, 1));
            }
        }

        public IEnumerator Fade(CanvasGroup canvasGroup, float targetValue)
        {
            float time = 0;
            float intialValue = canvasGroup.alpha;

            if (targetValue == 1)
            {
                canvasGroup.gameObject.SetActive(true);
            }

            while (time < durationData.UiTransitionuration)
            {
                canvasGroup.alpha = Mathf.SmoothStep(intialValue, targetValue, time / durationData.UiTransitionuration);

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            canvasGroup.alpha = targetValue;

            if (targetValue == 0)
            {
                canvasGroup.gameObject.SetActive(false);
            }
        }
    }
}

