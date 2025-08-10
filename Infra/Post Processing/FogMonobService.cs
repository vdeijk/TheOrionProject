using UnityEngine;
using System.Collections;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Infra
{
    public class FogMonobService : SingletonBaseService<FogMonobService>
    {
        [field: SerializeField] public float MAX_DENSITY { get; private set; }
        [field: SerializeField] public float MIN_DENSITY { get; private set; }

        // Sets fog density to maximum with smooth transition
        public void SetToMaxDensity()
        {
            StopAllCoroutines();
            StartCoroutine(Lerp(RenderSettings.fogDensity, MAX_DENSITY, DurationData.Instance.CameraBlendDuration));
        }

        // Sets fog density to minimum with smooth transition
        public void SetToMinDensity()
        {
            StopAllCoroutines();
            StartCoroutine(Lerp(RenderSettings.fogDensity, MIN_DENSITY, DurationData.Instance.CameraBlendDuration));
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
