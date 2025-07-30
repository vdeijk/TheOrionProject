using UnityEngine;
using System.Collections;

namespace TurnBasedStrategy
{
    public class FogController : Singleton<FogController>
    {
        [field: SerializeField] public float MAX_DENSITY { get; private set; }
        [field: SerializeField] public float MIN_DENSITY { get; private set; }
        [field: SerializeField] public float DURATION { get; private set; }

        // Sets fog density to maximum with smooth transition
        public void SetToMaxDensity()
        {
            StopAllCoroutines();
            StartCoroutine(Lerp(RenderSettings.fogDensity, MAX_DENSITY, DURATION));
        }

        // Sets fog density to minimum with smooth transition
        public void SetToMinDensity()
        {
            StopAllCoroutines();
            StartCoroutine(Lerp(RenderSettings.fogDensity, MIN_DENSITY, DURATION));
        }

        // Smoothly interpolates fog density over time
        private IEnumerator Lerp(float intialValue, float targetValue, float duration)
        {
            float time = 0;
            while (time < duration)
            {
                RenderSettings.fogDensity = Mathf.SmoothStep(intialValue, targetValue, time / duration);
                time += Time.unscaledDeltaTime;
                yield return null;
            }
            RenderSettings.fogDensity = targetValue;
        }
    }
}
