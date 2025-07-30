using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TurnBasedStrategy
{
    public class HueShiftController : Singleton<HueShiftController>
    {
        [Serializable]
        public class HueShiftSettings
        {
            [Range(-180f, 180f)]
            public float hueShift = 0f;
        }

        [SerializeField] private GameDurations gameDurations;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private HueShiftSettings successSettings;
        [SerializeField] private HueShiftSettings failureSettings;
        [SerializeField] private HueShiftSettings defaultSettings;

        private ColorAdjustments colorAdjustments;
        protected override void Awake()
        {
            Instance = SetSingleton();
            postProcessVolume.profile.TryGet(out colorAdjustments);
            // Initialize hue shift to default value
            colorAdjustments.hueShift.value = defaultSettings.hueShift;
        }

        // Alters hue shift based on success/failure, then returns to default after a delay
        public void Alter(bool isSuccessful)
        {
            StopAllCoroutines();
            if (isSuccessful)
            {
                StartCoroutine(Lerp(successSettings));
            }
            else
            {
                StartCoroutine(Lerp(failureSettings));
            }
            StartCoroutine(DelayLerpShift());
        }

        // Waits, then transitions hue shift back to default
        private IEnumerator DelayLerpShift()
        {
            yield return new WaitForSecondsRealtime(gameDurations.materialFadeDuration);
            StartCoroutine(Lerp(defaultSettings));
        }

        // Smoothly transitions hue shift to target value
        private IEnumerator Lerp(HueShiftSettings target)
        {
            float startHue = colorAdjustments.hueShift.value;
            float elapsed = 0f;
            while (elapsed < gameDurations.materialFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = transitionCurve.Evaluate(elapsed / gameDurations.materialFadeDuration);
                colorAdjustments.hueShift.value = Mathf.Lerp(startHue, target.hueShift, t);
                yield return null;
            }
            colorAdjustments.hueShift.value = target.hueShift;
        }
    }
}
