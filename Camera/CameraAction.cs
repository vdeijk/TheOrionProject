using Unity.Cinemachine;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class CameraAction : Singleton<CameraAction>
    {
        [SerializeField] Transform actionCamTransform;
        [SerializeField] CinemachineCamera cinemachineActionCamera;
        [SerializeField] float shootActionShoulderOffset = 0.5f;

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
            actionCamTransform.gameObject.SetActive(false);
        }

        // Sets up the action camera to follow shooter and look at target
        private void ShowActionCamera()
        {
            Transform shooterTransform = UnitSelectService.Instance.selectedUnit.transform;
            Transform targetTransform = UnitSelectService.Instance.selectedTarget.transform;
            Vector3 camDir = targetTransform.position;
            cinemachineActionCamera.Target.TrackingTarget = shooterTransform;
            cinemachineActionCamera.Target.LookAtTarget = targetTransform;
        }

        private void HideActionCamera()
        {
            actionCamTransform.gameObject.SetActive(false);
        }

        private void CameraChangeService_OnCameraChanged(object sender, System.EventArgs e)
        {
            if (CameraChangeService.Instance.curCamera == CameraType.Action)
            {
                ShowActionCamera();
            }
            else
            {
                HideActionCamera();
            }
        }
    }
}

