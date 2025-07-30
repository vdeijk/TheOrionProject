using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
    public class BarAnimationService : Singleton<BarAnimationService>
    {
        [SerializeField] GameDurations gameDurations;
        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        // Animates a UI bar's fill amount with a smooth transition and handles visibility
        public IEnumerator AnimateBarRoutine(Image bar, float targetValue, float maxValue)
        {
            if (maxValue <= 0)
            {
                // Hide parent UI if max value is zero (e.g. no resource to show)
                bar.transform.parent.parent.gameObject.SetActive(false);
            }
            else
            {
                bar.transform.parent.parent.gameObject.SetActive(true);
            }

            float startValue = bar.fillAmount;
            float elapsed = 0;

            // Animate fill amount using curve for smoothness
            while (elapsed < gameDurations.uiTransitionuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / gameDurations.uiTransitionuration);
                float easedT = animationCurve.Evaluate(t);
                bar.fillAmount = Mathf.Lerp(startValue, targetValue, easedT);
                yield return null;
            }

            bar.fillAmount = targetValue;

            // Hide bar if target value is zero (e.g. depleted)
            if (targetValue <= 0)
            {
                bar.gameObject.SetActive(false);
            }
        }
    }
}