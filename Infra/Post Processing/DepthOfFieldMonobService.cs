using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Infra
{
    public class DepthOfFieldMonobService: SingletonBaseService<DepthOfFieldMonobService>
    {
        [Serializable]
        public class DepthOfFieldSettings
        {
            public float focusDistance = 10f;
            public float aperture = 5.6f;
            public float focalLength = 50f;
        }

        [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] Volume postProcessVolume;
        [SerializeField] DepthOfFieldSettings defaultSettings;
        [SerializeField] DepthOfFieldSettings actionCamSettings;
        [SerializeField] DepthOfFieldSettings salvageCamSettings;
        [SerializeField] DepthOfFieldSettings assembleCamSettings;
        [SerializeField] float MaxAperture;
        [SerializeField] float MinAperture;

        private DepthOfField dof;

        private DurationData durationData => DurationData.Instance;

        protected override void Awake()
        {
            Instance = SetSingleton();
            postProcessVolume.profile.TryGet(out dof);

            // Initialize depth of field to default settings
            dof.focusDistance.value = defaultSettings.focusDistance;
            dof.aperture.value = defaultSettings.aperture;
            dof.focalLength.value = defaultSettings.focalLength;
        }

        private void OnEnable()
        {
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        // Updates depth of field settings based on camera type
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e) => UpdateDepthOfField();

        private void UpdateDepthOfField()
        {
            DepthOfFieldSettings target;

            switch (CameraChangeMonobService.Instance.curCamera)
            {
                case Data.CameraType.Action:
                    target = actionCamSettings;
                    break;
                case Data.CameraType.Assembly:
                    target = assembleCamSettings;
                    break;
                case Data.CameraType.Salvage:
                    target = salvageCamSettings;
                    break;
                default:
                    target = defaultSettings;
                    break;
            }

            StopAllCoroutines();
            StartCoroutine(LerpDepthOfField(target));
        }

        // Smoothly transitions depth of field parameters to target values
        private IEnumerator LerpDepthOfField(DepthOfFieldSettings target)
        {
            float startFocus = dof.focusDistance.value;
            float startAperture = dof.aperture.value;
            float startFocalLength = dof.focalLength.value;

            float elapsed = 0f;

            while (elapsed < durationData.CameraBlendDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = transitionCurve.Evaluate(elapsed / durationData.CameraBlendDuration);

                dof.focusDistance.value = Mathf.Lerp(startFocus, target.focusDistance, t);
                dof.aperture.value = Mathf.Lerp(startAperture, target.aperture, t);
                dof.focalLength.value = Mathf.Lerp(startFocalLength, target.focalLength, t);

                yield return null;
            }

            dof.focusDistance.value = target.focusDistance;
            dof.aperture.value = target.aperture;
            dof.focalLength.value = target.focalLength;
        }
    }
}
