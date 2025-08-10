using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    // Controls post-processing color adjustments, especially saturation, based on camera state
    public class ColorAdjustmentsController : SingletonBaseService<ColorAdjustmentsController>
    {
        [Serializable]
        public class SaturationSettings
        {
            [Range(-100f, 100f)]
            public float saturation = 0f;
        }

        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private SaturationSettings defaultSaturationSettings;
        [SerializeField] private SaturationSettings salvageCamSaturationSettings;

        private ColorAdjustments colorAdjustments;

        DurationData durationData => DurationData.Instance;

        // Initializes color adjustments and sets default saturation
        protected override void Awake()
        {
            Instance = SetSingleton();
            postProcessVolume.profile.TryGet(out colorAdjustments);
            colorAdjustments.saturation.value = defaultSaturationSettings.saturation;
        }

        private void OnEnable()
        {
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        // Handles camera change event to alter saturation
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e) => Alter();

        // Alters saturation based on camera type
        private void Alter()
        {
            SaturationSettings target;

            switch (CameraChangeMonobService.Instance.curCamera)
            {
                case Data.CameraType.Salvage:
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

            while (elapsed < durationData.CameraBlendDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = transitionCurve.Evaluate(elapsed / durationData.CameraBlendDuration);

                colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, target.saturation, t);

                yield return null;
            }

            colorAdjustments.saturation.value = target.saturation;
        }
    }
}