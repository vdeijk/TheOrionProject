using System;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class VCamSalvageController : MonoBehaviour
    {
        [field: SerializeField] public CameraSettings baseSettings { get; private set; }
        [field: SerializeField] public CameraSettings torsoSettings { get; private set; }
        [field: SerializeField] public CameraSettings weaponPrimarySettings { get; private set; }
        [field: SerializeField] public CameraSettings weaponSecondarySettings { get; private set; }
        [field: SerializeField] public CinemachineCamera baseCamera { get; private set; }
        [field: SerializeField] public CinemachineCamera torsoCamera { get; private set; }
        [field: SerializeField] public CinemachineCamera primaryWeaponCamera { get; private set; }
        [field: SerializeField] public CinemachineCamera secondaryWeaponCamera { get; private set; }
        [field: SerializeField] public float distance { get; private set; } = 10;

        DurationData durationData => DurationData.Instance;

        private void OnEnable()
        {
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        private void Awake()
        {
            CamerasSalvageService.Instance.Init(SetupData());
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            if (CameraChangeMonobService.Instance.curCamera == Data.CameraType.Salvage) return;

            CamerasSalvageService.Instance.DisableCameras();
        }

        // Handles camera blend timing
        public IEnumerator SetIsBlending()
        {
            CamerasSalvageService.Instance.SetIsBlending(true);

            yield return new WaitForSecondsRealtime(durationData.CameraBlendDuration);

            CamerasSalvageService.Instance.SetIsBlending(false);
        }

        private SalvageCamerasData SetupData()
        {
            return new SalvageCamerasData
            {
                SalvageCamera = this,
                BaseSettings = baseSettings,
                TorsoSettings = torsoSettings,
                WeaponPrimarySettings = weaponPrimarySettings,
                WeaponSecondarySettings = weaponSecondarySettings,
                BaseCamera = baseCamera,
                TorsoCamera = torsoCamera,
                PrimaryWeaponCamera = primaryWeaponCamera,
                SecondaryWeaponCamera = secondaryWeaponCamera,
                Distance = distance
            };
        }
    }
}