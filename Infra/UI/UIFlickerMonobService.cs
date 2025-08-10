using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Infra
{
    public class UIFlickerMonobService : SingletonBaseService<UIFlickerMonobService>
    {
        [SerializeField] private float baseFlickerSpeed = 20f;
        [SerializeField] private float flickerSpeedVariation = 10f; 
        [SerializeField] private float minAlpha = 0.2f;
        [SerializeField] private float maxAlpha = 1.0f;
        [SerializeField] private float alphaNoiseStrength = 0.15f; 
        [SerializeField] Color flickerColor = Color.cyan;
        [SerializeField] private float microFlickerChance = 0.05f; 
        [SerializeField] private float microFlickerDuration = 0.05f;

        private DurationData durationData => DurationData.Instance;

        public IEnumerator FlickerElements(Image[] circuitBorderElements)
        {
            Color[] originalColors = new Color[circuitBorderElements.Length];
            for (int i = 0; i < circuitBorderElements.Length; i++)
            {
                originalColors[i] = circuitBorderElements[i].color;
            }

            float[] speedVariations = new float[circuitBorderElements.Length];
            for (int i = 0; i < circuitBorderElements.Length; i++)
            {
                speedVariations[i] = Random.Range(-flickerSpeedVariation, flickerSpeedVariation);
            }

            float elapsedTime = 0f;
            float microFlickerEndTime = 0f;
            bool inMicroFlicker = false;

            while (elapsedTime < durationData.CircuitryFlickerDuration)
            {
                if (!inMicroFlicker && Random.value < microFlickerChance)
                {
                    inMicroFlicker = true;
                    microFlickerEndTime = elapsedTime + microFlickerDuration;
                }

                for (int i = 0; i < circuitBorderElements.Length; i++)
                {
                    float elementSpeed = baseFlickerSpeed + speedVariations[i];

                    float baseFlicker = (Mathf.Sin(elapsedTime * elementSpeed) + 1f) * 0.5f;

                    float noise = Random.Range(-alphaNoiseStrength, alphaNoiseStrength);
                    float flickerValue = Mathf.Clamp(
                        Mathf.Lerp(minAlpha, maxAlpha, baseFlicker) + noise,
                        minAlpha,
                        maxAlpha
                    );

                    if (inMicroFlicker)
                    {
                        if (Random.value < 0.5f)
                        {
                            flickerValue = Random.value < 0.7f ? minAlpha : maxAlpha;
                        }
                    }

                    float colorIntensity = baseFlicker * (0.3f + Random.Range(-0.1f, 0.1f));
                    Color elementColor = Color.Lerp(
                        originalColors[i],
                        flickerColor,
                        colorIntensity
                    );

                    elementColor.a = flickerValue;
                    circuitBorderElements[i].color = elementColor;
                }

                if (inMicroFlicker && elapsedTime >= microFlickerEndTime)
                {
                    inMicroFlicker = false;
                }

                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            for (int i = 0; i < circuitBorderElements.Length; i++)
            {
                circuitBorderElements[i].color = originalColors[i];
            }
        }
    }
}