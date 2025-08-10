using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TurnBasedStrategy.Infra
{
    public class ImageFadeMonobService : SingletonBaseService<ImageFadeMonobService>
    {
        private Color32 color = new Color32();

        public IEnumerator Fade(float intialValue, float targetValue, Image image, float duration)
        {
            float time = 0;

            if (targetValue == 1)
            {
                image.gameObject.SetActive(true);
            }

            while (time < duration)
            {
                float alpha = Mathf.SmoothStep(intialValue, targetValue, time / duration);
                color = new Color(image.color.r, image.color.b, image.color.g, alpha);
                image.color = color;

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            color = new Color(image.color.r, image.color.b, image.color.g, targetValue);
            image.color = color;

            if (targetValue == 0)
            {
                image.gameObject.SetActive(false);
            }
        }
    }
}