using Unity.Cinemachine;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class CameraVictory : Singleton<CameraVictory>
    {
        [SerializeField] Transform victoryCamTransform;
        [SerializeField] CinemachineCamera cinemachineVictoryCamera;
        [SerializeField] Transform target;

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
            victoryCamTransform.gameObject.SetActive(false);
        }

        // Sets up and shows the victory camera
        private void ShowVictoryCamera()
        {
            cinemachineVictoryCamera.Target.TrackingTarget = target;
            cinemachineVictoryCamera.Target.LookAtTarget = target;

            victoryCamTransform.gameObject.SetActive(true);
        }

        private void HideVictoryCamera()
        {
            victoryCamTransform.gameObject.SetActive(false);
        }

        private void CameraChangeService_OnCameraChanged(object sender, System.EventArgs e)
        {
            if (CameraChangeService.Instance.curCamera == CameraType.Victory)
            {
                ShowVictoryCamera();
            }
            else
            {
                HideVictoryCamera();
            }
        }
    }
}

