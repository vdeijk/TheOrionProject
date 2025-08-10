using System;
using System.Collections;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Infra;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TurnBasedStrategy
{
    public class WhiteBalanceMonobService: SingletonBaseService<WhiteBalanceMonobService>
    {
        [SerializeField] float defaultTemperature = 10f;
        [SerializeField] float mapViewTemperature = -10f;
        [SerializeField] float defaultTint = 10f;
        [SerializeField] float mapViewTint = -10f;
        [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] Volume postProcessVolume;

        private WhiteBalance whiteBalance;

        private DurationData durationData => DurationData.Instance;

        protected override void Awake()
        {
            Instance = SetSingleton();
            postProcessVolume.profile.TryGet(out whiteBalance);
        }

        private void OnEnable()
        {
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        // Updates white balance based on camera type
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            UpdateWhiteBalance();
        }

        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            UpdateWhiteBalance();
        }

        // Determines target temperature/tint and starts transition
        private void UpdateWhiteBalance()
        {
            float targetTemp;
            float targetTint;

            if (CameraChangeMonobService.Instance.curCamera == Data.CameraType.Map)
            {
                targetTemp = mapViewTemperature;
                targetTint = mapViewTint;
            }
            else
            {
                targetTemp = defaultTemperature;
                targetTint = defaultTint;
            }

            StopAllCoroutines();
            StartCoroutine(LerpWhiteBalance(targetTemp, targetTint));
        }

        // Smoothly transitions white balance parameters to target values
        private IEnumerator LerpWhiteBalance(float targetTemp, float targetTint)
        {
            float startTemp = whiteBalance.temperature.value;
            float startTint = whiteBalance.tint.value;
            float elapsedTime = 0f;

            while (elapsedTime < durationData.CameraBlendDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = elapsedTime / durationData.CameraBlendDuration;
                float curveT = transitionCurve.Evaluate(t);

                whiteBalance.temperature.value = Mathf.Lerp(startTemp, targetTemp, curveT);
                whiteBalance.tint.value = Mathf.Lerp(startTint, targetTint, curveT);

                yield return null;
            }

            whiteBalance.temperature.value = targetTemp;
            whiteBalance.tint.value = targetTint;
        }
    }
}