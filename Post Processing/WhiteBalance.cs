using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TurnBasedStrategy
{
    public class WhiteBalanceController : Singleton<WhiteBalanceController>
    {
        [SerializeField] float defaultTemperature = 10f;
        [SerializeField] float mapViewTemperature = -10f;
        [SerializeField] float defaultTint = 10f;
        [SerializeField] float mapViewTint = -10f;
        [SerializeField] GameDurations gameDurations;
        [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] Volume postProcessVolume;

        private WhiteBalance whiteBalance;
        protected override void Awake()
        {
            Instance = SetSingleton();
            postProcessVolume.profile.TryGet(out whiteBalance);
        }

        private void OnEnable()
        {
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
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

            if (CameraChangeService.Instance.curCamera == CameraType.Map)
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

            while (elapsedTime < gameDurations.cameraBlendDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = elapsedTime / gameDurations.cameraBlendDuration;
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