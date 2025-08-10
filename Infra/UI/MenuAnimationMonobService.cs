using UnityEngine;
using System.Collections;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Infra
{
    public class MenuAnimationMonobService : SingletonBaseService<MenuAnimationMonobService>
    {
        [SerializeField] Vector2 startOffset = new Vector2(0, 18f);
        [SerializeField] AnimationCurve driftCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private DurationData durationData => DurationData.Instance;

        public IEnumerator RevealAll(CanvasGroup[] canvasGroups)
        {
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 0;
            }

            yield return new WaitForSecondsRealtime(durationData.UiTransitionuration);

            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                StartCoroutine(Reveal(canvasGroup));
            }
        }

        private IEnumerator Reveal(CanvasGroup canvasGroup)
        {
            RectTransform rectTransform = canvasGroup.GetComponent<RectTransform>();

            Vector3 targetPosition = rectTransform.localPosition;
            Vector3 startingPos = targetPosition + new Vector3(startOffset.x, startOffset.y, 0);
            rectTransform.localPosition = startingPos;
            canvasGroup.alpha = 0;
            float elapsed = 0;

            while (elapsed < durationData.UiTransitionuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalizedTime = Mathf.Clamp01(elapsed / durationData.UiTransitionuration);

                rectTransform.localPosition = Vector3.Lerp(
                    startingPos,
                    targetPosition,
                    driftCurve.Evaluate(normalizedTime)
                );

                canvasGroup.alpha = Mathf.Lerp(0, 1, driftCurve.Evaluate(normalizedTime));

                yield return null;
            }

            rectTransform.localPosition = targetPosition;
            canvasGroup.alpha = 1;
        }
    }
}
