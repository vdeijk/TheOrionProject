using UnityEngine;

namespace TurnBasedStrategy
{
    public class CameraBillboard : MonoBehaviour
    {
        [SerializeField] Camera billboardCam;

        private void OnEnable()
        {
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        private void Awake()
        {
            billboardCam.farClipPlane = 8000;
        }

        // Adjusts far clip plane based on camera mode
        private void CameraChangeService_OnCameraChanged(object sender, System.EventArgs e)
        {
            if (CameraChangeService.Instance.curCamera == CameraType.Action)
            {
                billboardCam.farClipPlane = 80;
            }
            else
            {
                billboardCam.farClipPlane = 8000;
            }
        }
    }
}

