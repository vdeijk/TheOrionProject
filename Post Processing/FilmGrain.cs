using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TurnBasedStrategy
{
    public class FilmGrainController : Singleton<FilmGrainController>
    {
        [SerializeField] float defaultIntensity = 0.2f;
        [SerializeField] float mapViewIntensity = 0.4f;
        [SerializeField] float battlePrepIntensity = 0.4f;
        [SerializeField] GameDurations gameDurations;
        [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] Volume postProcessVolume;

        private FilmGrain filmGrain;
        protected override void Awake()
        {
            Instance = SetSingleton();
            postProcessVolume.profile.TryGet(out filmGrain);
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

        // Updates film grain intensity based on camera and game mode
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            UpdateFilmGrain();
        }

        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            UpdateFilmGrain();
        }

        // Determines target intensity and starts transition
        private void UpdateFilmGrain()
        {
            float targetIntensity = defaultIntensity;
            switch (CameraChangeService.Instance.curCamera)
            {
                case CameraType.Map:
                    targetIntensity = mapViewIntensity;
                    break;
                case CameraType.Overhead:
                    if (ControlModeManager.Instance.gameControlType == GameControlType.Outside)
                    {
                        targetIntensity = battlePrepIntensity;
                    }
                    break;
            }
            StopAllCoroutines();
            StartCoroutine(LerpFilmGrain(targetIntensity));
        }

        // Smoothly transitions film grain intensity to target value
        private IEnumerator LerpFilmGrain(float targetIntensity)
        {
            float startIntensity = filmGrain.intensity.value;
            float elapsedTime = 0f;
            while (elapsedTime < gameDurations.cameraBlendDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = elapsedTime / gameDurations.cameraBlendDuration;
                float curveT = transitionCurve.Evaluate(t);
                filmGrain.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, curveT);
                yield return null;
            }
            filmGrain.intensity.value = targetIntensity;
        }
    }
}