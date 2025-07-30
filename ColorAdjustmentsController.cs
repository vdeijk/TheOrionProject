using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TurnBasedStrategy
{
    // Controls post-processing color adjustments, especially saturation, based on camera state
    public class ColorAdjustmentsController : Singleton<ColorAdjustmentsController>
    {
        [Serializable]
        public class SaturationSettings
        {
            [Range(-100f, 100f)]
            public float saturation = 0f;
        }

        [SerializeField] private GameDurations gameDurations;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private SaturationSettings defaultSaturationSettings;
        [SerializeField] private SaturationSettings salvageCamSaturationSettings;

        private ColorAdjustments colorAdjustments;

        // Initializes color adjustments and sets default saturation
        protected override void Awake()
        {
            Instance = SetSingleton();
            postProcessVolume.profile.TryGet(out colorAdjustments);
            colorAdjustments.saturation.value = defaultSaturationSettings.saturation;
        }

        private void OnEnable()
        {
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        // Handles camera change event to alter saturation
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e) => Alter();

        // Alters saturation based on camera type
        private void Alter()
        {
            SaturationSettings target;

            switch (CameraChangeService.Instance.curCamera)
            {
                case CameraType.Salvage:
                    target = salvageCamSaturationSettings;
                    break;
                default:
                    target = defaultSaturationSettings;
                    break;
            }

            StopAllCoroutines();
            StartCoroutine(Lerp(target));
        }

        // Smoothly transitions saturation to target value
        private IEnumerator Lerp(SaturationSettings target)
        {
            float startSaturation = colorAdjustments.saturation.value;
            float elapsed = 0f;

            while (elapsed < gameDurations.cameraBlendDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = transitionCurve.Evaluate(elapsed / gameDurations.cameraBlendDuration);

                colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, target.saturation, t);

                yield return null;
            }

            colorAdjustments.saturation.value = target.saturation;
        }
    }
}