using System;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

namespace TurnBasedStrategy
{
    public class SalvageCamera : Singleton<SalvageCamera>
    {
        [System.Serializable]
        public class CameraSettings
        {
            public float angle;
            public float height;
        }

        public bool isBlending { get; private set; } = false;

        [SerializeField] CameraSettings baseSettings;
        [SerializeField] CameraSettings torsoSettings;
        [SerializeField] CameraSettings weaponPrimarySettings;
        [SerializeField] CameraSettings weaponSecondarySettings;
        [SerializeField] CinemachineCamera baseCamera;
        [SerializeField] CinemachineCamera torsoCamera;
        [SerializeField] CinemachineCamera primaryWeaponCamera;
        [SerializeField] CinemachineCamera secondaryWeaponCamera;
        [SerializeField] float distance = 10;
        [SerializeField] GameDurations gameDurations;

        private Unit salvagableUnit;

        protected override void Awake()
        {
            Instance = SetSingleton();
            DisableAllCameras();
        }

        private void OnEnable()
        {
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            if (CameraChangeService.Instance.curCamera == CameraType.Salvage) return;
            DisableAllCameras();
        }

        private void DisableAllCameras()
        {
            baseCamera.gameObject.SetActive(false);
            torsoCamera.gameObject.SetActive(false);
            primaryWeaponCamera.gameObject.SetActive(false);
            secondaryWeaponCamera.gameObject.SetActive(false);
        }

        public void SetSalvagableUnit(Unit unit)
        {
            salvagableUnit = unit;
            ConfigureAllCameras();
        }

        // Activates the camera for the selected part type
        public void SetCameraTarget(PartType curSalvageType)
        {
            CinemachineCamera targetCamera = GetCameraForPartType(curSalvageType);
            if (targetCamera != null)
            {
                DisableAllCameras();
                targetCamera.gameObject.SetActive(true);
                StartCoroutine(SetIsBlending());
            }
        }

        private void ConfigureAllCameras()
        {
            ConfigureCamera(PartType.Base, baseCamera, baseSettings);
            ConfigureCamera(PartType.Torso, torsoCamera, torsoSettings);
            ConfigureCamera(PartType.WeaponPrimary, primaryWeaponCamera, weaponPrimarySettings);
            ConfigureCamera(PartType.WeaponSecondary, secondaryWeaponCamera, weaponSecondarySettings);
        }

        // Positions and rotates camera for a part
        private void ConfigureCamera(PartType partType, CinemachineCamera camera, CameraSettings settings)
        {
            if (salvagableUnit == null || !salvagableUnit.transformData.TryGetValue(partType, out var targetTransform))
                return;
            Quaternion orbitRotation = salvagableUnit.unitBodyTransform.rotation * Quaternion.Euler(0f, 180f + settings.angle, 0f);
            Vector3 offset = orbitRotation * new Vector3(0, settings.height, -distance);
            Vector3 cameraPosition = targetTransform.position + offset;
            camera.transform.position = cameraPosition;
            camera.transform.rotation = Quaternion.LookRotation(targetTransform.position - cameraPosition, targetTransform.up);
            camera.LookAt = targetTransform;
        }

        private CinemachineCamera GetCameraForPartType(PartType partType)
        {
            return partType switch
            {
                PartType.Base => baseCamera,
                PartType.Torso => torsoCamera,
                PartType.WeaponPrimary => primaryWeaponCamera,
                PartType.WeaponSecondary => secondaryWeaponCamera,
                _ => baseCamera
            };
        }

        // Handles camera blend timing
        private IEnumerator SetIsBlending()
        {
            isBlending = true;
            yield return new WaitForSecondsRealtime(gameDurations.cameraBlendDuration);
            isBlending = false;
        }
    }
}