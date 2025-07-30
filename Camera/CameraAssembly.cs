using System;
using Unity.Cinemachine;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class CameraAssembly : Singleton<CameraAssembly>
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
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        // Sets the camera to follow and look at the selected unit
        public void SetTarget()
        {
            target = UnitSelectService.Instance.selectedUnit.transform;
            virtualCamera.Follow = null;
            virtualCamera.LookAt = null;
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            if (CameraChangeService.Instance.curCamera == CameraType.Assembly)
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
