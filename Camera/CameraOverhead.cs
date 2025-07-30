using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class CameraOverhead : Singleton<CameraOverhead>
    {
        [field:SerializeField] public Transform overheadCamTransform { get; private set; }

        private void OnEnable()
        {
            UnitSelectService.OnUnitSelected += UnitSelectionSystem_OnUnitSelected;
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            UnitSelectService.OnUnitSelected -= UnitSelectionSystem_OnUnitSelected;
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        private void Start()
        {
            overheadCamTransform.gameObject.SetActive(false);
        }

        // Centers camera on selected unit if not player's phase
        private void UnitSelectionSystem_OnUnitSelected(object sender, EventArgs e)
        {
            if (PhaseManager.Instance.isPlayerPhase) return;

            Vector3 targetPos = UnitSelectService.Instance.selectedUnit.transform.position;
            CameraSmoothingService.Instance.StartCentering(targetPos);
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            if (CameraChangeService.Instance.curCamera != CameraType.Overhead) return;

            overheadCamTransform.gameObject.SetActive(true);
        }
    }
}

