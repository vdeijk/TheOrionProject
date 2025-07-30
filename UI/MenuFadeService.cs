
using UnityEngine;
using System.Collections;

namespace TurnBasedStrategy
{
    public class MenuFadeService : Singleton<MenuFadeService>
    {
        [SerializeField] GameDurations gameDurations;

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
            yield return new WaitForSecondsRealtime(gameDurations.uiTransitionuration);

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

            while (time < gameDurations.uiTransitionuration)
            {
                canvasGroup.alpha = Mathf.SmoothStep(intialValue, targetValue, time / gameDurations.uiTransitionuration);

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

