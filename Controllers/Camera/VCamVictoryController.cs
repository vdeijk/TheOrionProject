using Unity.Cinemachine;
using UnityEngine;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class VCamVictoryController : SingletonBaseService<VCamVictoryController>
    {
        [SerializeField] Transform victoryCamTransform;
        [SerializeField] CinemachineCamera cinemachineVictoryCamera;
        [SerializeField] Transform target;

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
            victoryCamTransform.gameObject.SetActive(false);
        }

        // Sets up and shows the victory camera
        private void Show()
        {
            cinemachineVictoryCamera.Target.TrackingTarget = target;
            cinemachineVictoryCamera.Target.LookAtTarget = target;

            victoryCamTransform.gameObject.SetActive(true);
        }

        private void Hide()
        {
            victoryCamTransform.gameObject.SetActive(false);
        }

        private void CameraChangeService_OnCameraChanged(object sender, System.EventArgs e)
        {
            if (CameraChangeMonobService.Instance.curCamera == Data.CameraType.Victory)
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

