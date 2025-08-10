using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class PhysicalCamController : MonoBehaviour
    {
        [field: SerializeField] public PhysicalCameraData Data { get; private set; }

        private void OnEnable()
        {
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
            AssemblyService.OnPartSelected += UnitAssemblyService_OnPartSelected;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
            AssemblyService.OnPartSelected -= UnitAssemblyService_OnPartSelected;
        }

        private void Start()
        {
            PhysicalCameraMonobService.Instance.Init(Data);
            PhysicalCameraMonobService.Instance.SetupActionCam();
        }

        private void UnitAssemblyService_OnPartSelected(object sender, System.EventArgs e)
        {
            PhysicalCameraMonobService.Instance.SetupInventoryCam();
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            PhysicalCameraMonobService.Instance.SetupActionCam();
            PhysicalCameraMonobService.Instance.SetupBillboardCam();
        }
    }
}

