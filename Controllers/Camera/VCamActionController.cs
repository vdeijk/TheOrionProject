using Unity.Cinemachine;
using UnityEngine;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class VCamActionController : MonoBehaviour
    {
        [SerializeField] Transform actionCamTransform;
        [SerializeField] CinemachineCamera cinemachineActionCamera;
        [SerializeField] float shootActionShoulderOffset = 0.5f;

        private void OnEnable()
        {
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        private void Start()
        {
            actionCamTransform.gameObject.SetActive(false);
        }

        // Sets up the action camera to follow shooter and look at target
        private void Show()
        {
            Transform shooterTransform = UnitSelectService.Instance.Data.SelectedUnit.transform;
            Transform targetTransform = UnitSelectService.Instance.Data.SelectedTarget.transform;
            Vector3 camDir = targetTransform.position;
            cinemachineActionCamera.Target.TrackingTarget = shooterTransform;
            cinemachineActionCamera.Target.LookAtTarget = targetTransform;
        }

        private void Hide()
        {
            actionCamTransform.gameObject.SetActive(false);
        }

        private void CameraChangeService_OnCameraChanged(object sender, System.EventArgs e)
        {
            if (CameraChangeMonobService.Instance.curCamera == Data.CameraType.Action)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }
}

