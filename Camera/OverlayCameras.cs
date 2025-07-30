using System;
using UnityEngine;
using System.Collections;

namespace TurnBasedStrategy
{
    public class OverlayCameras : Singleton<OverlayCameras>
    {
        [SerializeField] GameDurations gameDurations;
        [SerializeField] Transform iconCamera;
        [SerializeField] Transform billboardCamera;

        private void OnEnable()
        {
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        private void Start()
        {
            iconCamera.gameObject.SetActive(false);
            billboardCamera.gameObject.SetActive(false);
        }

        // Fades cameras in/out based on camera mode
        protected void FadeWithDelay(bool isActivated, Transform camera)
        {
            float delay = gameDurations.cameraBlendDuration;
            StartCoroutine(Fade(isActivated, delay, camera));
        }

        protected void FadeWithoutDelay(bool isActivated, Transform camera)
        {
            StartCoroutine(Fade(isActivated, 0, camera));
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            StopAllCoroutines();
            switch (CameraChangeService.Instance.curCamera)
            {
                case CameraType.Map:
                    FadeWithDelay(true, iconCamera);
                    FadeWithoutDelay(false, billboardCamera);
                    break;
                case CameraType.Assembly:
                    FadeWithoutDelay(false, iconCamera);
                    FadeWithoutDelay(false, billboardCamera);
                    break;
                case CameraType.Overhead:
                    FadeWithoutDelay(false, billboardCamera);
                    FadeWithoutDelay(false, iconCamera);
                    FadeWithDelay(true, billboardCamera);
                    break;
                case CameraType.Action:
                    FadeWithoutDelay(false, billboardCamera);
                    FadeWithDelay(true, billboardCamera);
                    FadeWithoutDelay(false, iconCamera);
                    break;
                default:
                    FadeWithoutDelay(false, billboardCamera);
                    FadeWithoutDelay(false, iconCamera);
                    break;
            }
        }

        // Coroutine to activate/deactivate camera after delay
        private IEnumerator Fade(bool isActivated, float delay, Transform camera)
        {
            yield return new WaitForSecondsRealtime(delay);
            camera.gameObject.SetActive(isActivated);
        }
    }
}

