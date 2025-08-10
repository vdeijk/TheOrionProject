using System;
using Unity.Cinemachine;
using UnityEngine;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Infra
{
    public class CameraControlsMonobService : SingletonBaseService<CameraControlsMonobService>
    {
        public float targetZoom { get; private set; }

        [field: SerializeField] public float MAX_ZOOM { get; private set; }
        [field: SerializeField] public float DEFAULT_ZOOM { get; private set; }

        [SerializeField] CinemachineThirdPersonFollow cinemachineThirdPersonFollow;
        [SerializeField] Transform trackingTargetTransform;
        [SerializeField] Transform overheadCamTransform;
        [SerializeField] MeshRenderer terrainMesh;
        [SerializeField] float ZOOM_SPEED_STRENGHTH;
        [SerializeField] float MIN_ZOOM;
        [SerializeField] float MOVE_SPEED;

        private float MAX_PAN_X;
        private float MAX_PAN_Z;

        private void Start()
        {
            MAX_PAN_X = terrainMesh.bounds.size.x;
            MAX_PAN_Z = terrainMesh.bounds.size.z;
            DEFAULT_ZOOM = (MAX_ZOOM + MIN_ZOOM) / 2;
            targetZoom = DEFAULT_ZOOM;
            cinemachineThirdPersonFollow.VerticalArmLength = DEFAULT_ZOOM;
            cinemachineThirdPersonFollow.CameraDistance = DEFAULT_ZOOM;

            trackingTargetTransform.rotation = Quaternion.Euler(0, -45, 0);

            overheadCamTransform.gameObject.SetActive(false);
        }

        // Centers camera on selected unit if input is given
        public void UpdateCentering(bool cameraCenteringInput)
        {
            if (cameraCenteringInput)
            {
                Vector3 targetPos = UnitSelectService.Instance.Data.SelectedUnit.transform.position;
                CameraSmoothingMonobService.Instance.StartCentering(targetPos);
            }
        }

        // Moves camera based on input, clamps to terrain bounds
        public void UpdatePosition(Vector3 cameraMoveInputs)
        {
            if (cameraMoveInputs == Vector3.zero || targetZoom >= MAX_ZOOM) return;

            float moveSpeed = Mathf.Sqrt((MOVE_SPEED * targetZoom) / DEFAULT_ZOOM);
            Vector3 moveVector = trackingTargetTransform.forward * cameraMoveInputs.z + trackingTargetTransform.right * cameraMoveInputs.x;
            Vector3 newPos = trackingTargetTransform.position + moveVector * moveSpeed * Time.unscaledDeltaTime;
            float x = Mathf.Clamp(newPos.x, 0, MAX_PAN_X);
            float z = Mathf.Clamp(newPos.z, 0, MAX_PAN_Z);

            trackingTargetTransform.position = new Vector3(x, trackingTargetTransform.position.y, z);
        }

        public void UpdateCinemachineCam(float verticalArmLength, float cameraDistance)
        {
            cinemachineThirdPersonFollow.VerticalArmLength = verticalArmLength;
            cinemachineThirdPersonFollow.CameraDistance = cameraDistance;
        }

        // Handles zoom input and toggles camera mode if needed
        public void UpdateZoom(float cameraZoomInput)
        {
            if (cameraZoomInput != 0)
            {
                bool isInverted = OptionsUIMonobService.Instance.cameraInvert;
                if (isInverted) cameraZoomInput = (-cameraZoomInput);

                float zoomChange = cameraZoomInput * Time.unscaledDeltaTime * ZOOM_SPEED_STRENGHTH;
                targetZoom = Mathf.Clamp(cinemachineThirdPersonFollow.CameraDistance + zoomChange, MIN_ZOOM, MAX_ZOOM);
                cinemachineThirdPersonFollow.VerticalArmLength = targetZoom;
                cinemachineThirdPersonFollow.CameraDistance = targetZoom;

                CameraChangeMonobService.Instance.ToggleMapAndOverheadCameras(targetZoom);
            }
        }
    }
}

