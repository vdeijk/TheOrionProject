using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
    public class ButtonTransitionService : Singleton<ButtonTransitionService>
    {
        [SerializeField] GameDurations gameDurations;

        private float duration => gameDurations.uiTransitionuration;

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