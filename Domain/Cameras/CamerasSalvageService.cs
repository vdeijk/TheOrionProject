using Unity.Cinemachine;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    public class CamerasSalvageService
    {
        public SalvageCamerasData data;

        private UnitSingleController salvagableUnit;
        private static CamerasSalvageService _instance;

        public static CamerasSalvageService Instance => _instance ??= new CamerasSalvageService();

        public void SetIsBlending(bool isBlending)
        {
            data.IsBlending = isBlending;
        }

        public void Init(SalvageCamerasData data)
        {
            this.data = data;

            DisableCameras();
        }

        // Activates the camera for the selected part type
        public void SetTarget(PartType curSalvageType)
        {
            CinemachineCamera targetCamera = GetCamera(curSalvageType);
            if (targetCamera != null)
            {
                DisableCameras();
                targetCamera.gameObject.SetActive(true);
                data.SalvageCamera.StartCoroutine(data.SalvageCamera.SetIsBlending());
            }
        }

        public void DisableCameras()
        {
            data.BaseCamera.gameObject.SetActive(false);
            data.TorsoCamera.gameObject.SetActive(false);
            data.PrimaryWeaponCamera.gameObject.SetActive(false);
            data.SecondaryWeaponCamera.gameObject.SetActive(false);
        }

        public void SetSalvagableUnit(UnitSingleController unit)
        {
            salvagableUnit = unit;
            ConfigureCameras();
        }

        private void ConfigureCameras()
        {
            ConfigureCamera(PartType.Base, data.BaseCamera, data.BaseSettings);
            ConfigureCamera(PartType.Torso, data.TorsoCamera, data.TorsoSettings);
            ConfigureCamera(PartType.WeaponPrimary, data.PrimaryWeaponCamera, data.WeaponPrimarySettings);
            ConfigureCamera(PartType.WeaponSecondary, data.SecondaryWeaponCamera, data.WeaponSecondarySettings);
        }

        // Positions and rotates camera for a part
        private void ConfigureCamera(PartType partType, CinemachineCamera camera, CameraSettings settings)
        {
            if (salvagableUnit == null || !salvagableUnit.Data.TransformData.TryGetValue(partType, out var targetTransform))
                return;
            Quaternion orbitRotation = salvagableUnit.Data.UnitBodyTransform.rotation * Quaternion.Euler(0f, 180f + settings.angle, 0f);
            Vector3 offset = orbitRotation * new Vector3(0, settings.height, -data.Distance);
            Vector3 cameraPosition = targetTransform.position + offset;
            camera.transform.position = cameraPosition;
            camera.transform.rotation = Quaternion.LookRotation(targetTransform.position - cameraPosition, targetTransform.up);
            camera.LookAt = targetTransform;
        }

        public CinemachineCamera GetCamera(PartType partType)
        {
            return partType switch
            {
                PartType.Base => data.BaseCamera,
                PartType.Torso => data.TorsoCamera,
                PartType.WeaponPrimary => data.PrimaryWeaponCamera,
                PartType.WeaponSecondary => data.SecondaryWeaponCamera,
                _ => data.BaseCamera
            };
        }
    }
}