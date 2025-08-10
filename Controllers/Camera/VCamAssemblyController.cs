using System;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;
using Unity.Cinemachine;
using UnityEngine;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class VCamAssemblyController : SingletonBaseService<VCamAssemblyController>
    {
        [SerializeField] CinemachineCamera virtualCamera;
        private Transform target;

        protected override void Awake()
        {
            Instance = SetSingleton();
            virtualCamera.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        // Sets the camera to follow and look at the selected unit
        public void SetTarget()
        {
            target = UnitSelectService.Instance.Data.SelectedUnit.transform;
            virtualCamera.Follow = null;
            virtualCamera.LookAt = null;
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            if (CameraChangeMonobService.Instance.curCamera == Data.CameraType.Assembly)
            {
                SetTarget();
                virtualCamera.gameObject.SetActive(true);
            }
            else
            {
                virtualCamera.gameObject.SetActive(false);
            }
        }
    }
}
