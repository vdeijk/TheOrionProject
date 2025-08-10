using System;
using TurnBasedStrategy.Game;
using UnityEngine;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class VCamOverheadController : SingletonBaseService<VCamOverheadController>
    {
        [field:SerializeField] public Transform overheadCamTransform { get; private set; }

        private void OnEnable()
        {
            UnitSelectService.OnUnitSelected += UnitSelectionSystem_OnUnitSelected;
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            UnitSelectService.OnUnitSelected -= UnitSelectionSystem_OnUnitSelected;
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        private void Start()
        {
            overheadCamTransform.gameObject.SetActive(false);
        }

        // Centers camera on selected unit if not player's phase
        private void UnitSelectionSystem_OnUnitSelected(object sender, EventArgs e)
        {
            if (PhaseManager.Instance.isPlayerPhase) return;

            Vector3 targetPos = UnitSelectService.Instance.Data.SelectedUnit.transform.position;
            CameraSmoothingMonobService.Instance.StartCentering(targetPos);
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            if (CameraChangeMonobService.Instance.curCamera != Data.CameraType.Overhead) return;

            overheadCamTransform.gameObject.SetActive(true);
        }
    }
}

