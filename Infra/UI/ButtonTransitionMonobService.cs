using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Infra
{
    public class ButtonTransitionMonobService : SingletonBaseService<ButtonTransitionMonobService>
    {
        private float duration => DurationData.Instance.UiTransitionuration;

        public IEnumerator LerpColor(Image image, Color32 targetColor)
        {
            float time = 0;
            Color32 initialColor = image.color;

            while (time < duration)
            {
                image.color = Color32.Lerp(initialColor, targetColor, time / duration);

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            image.color = targetColor;
        }
    }
}