using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class CameraSmoothingService : Singleton<CameraSmoothingService>
    {
        public float targetZoom { get; private set; }

        [SerializeField] Transform trackingTargetTransform;
        [SerializeField] float CenteringSmoothConstant = 600;
        [SerializeField] float EnemyFollowSmoothTime = 0.2f;
        [SerializeField] float ZoomSmoothTime = .2f;
        [SerializeField] CinemachineThirdPersonFollow cinemachineThirdPersonFollow;

        private Coroutine centeringCoroutine;
        private Vector3 velocityRef = Vector3.zero;
        private bool inMission => ControlModeManager.Instance.gameControlType == GameControlType.Mission;

        private void OnEnable()
        {
            PhaseManager.OnPhaseChanged += PhaseManager_OnPhaseChanged;
        }

        private void OnDisable()
        {
            PhaseManager.OnPhaseChanged -= PhaseManager_OnPhaseChanged;
        }

        private void Start()
        {
            trackingTargetTransform.rotation = Quaternion.Euler(0, -45, 0);
        }

        private void Update()
        {
            if (!inMission || PhaseManager.Instance.isPlayerPhase) return;
            Unit unit = UnitSelectService.Instance.selectedUnit;
            if (unit != null)
            {
                trackingTargetTransform.position = Vector3.SmoothDamp(
                    trackingTargetTransform.position,
                    unit.unitBodyTransform.position,
                    ref velocityRef,
                    EnemyFollowSmoothTime
                );
            }
        }

        // Smoothly centers camera on target position
        public void StartCentering(Vector3 targetPos)
        {
            if (!PhaseManager.Instance.isPlayerPhase) return;
            if (centeringCoroutine != null)
            {
                StopCoroutine(centeringCoroutine);
            }
            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(targetPos);
            Vector3 finalTargetPos = LevelGrid.Instance.GetWorldPosition(gridPosition);
            centeringCoroutine = StartCoroutine(SmoothCentering(finalTargetPos));
        }

        private void PhaseManager_OnPhaseChanged(object sender, System.EventArgs e)
        {
            if (PhaseManager.Instance.isPlayerPhase) return;
            StartCoroutine(SmoothZoom(CameraControlsService.Instance.DEFAULT_ZOOM));
        }

        // Smoothly zooms camera to target value
        private IEnumerator SmoothZoom(float targetZoomValue)
        {
            float startVerticalArm = cinemachineThirdPersonFollow.VerticalArmLength;
            float startCameraDistance = cinemachineThirdPersonFollow.CameraDistance;
            float elapsedTime = 0;
            while (elapsedTime < ZoomSmoothTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsedTime / ZoomSmoothTime);
                float smoothT = Mathf.SmoothStep(0, 1, t);
                float verticalArmLength = Mathf.Lerp(startVerticalArm, targetZoomValue, smoothT);
                float cameraDistance = Mathf.Lerp(startCameraDistance, targetZoomValue, smoothT);
                CameraControlsService.Instance.UpdateCinemachineCam(verticalArmLength, cameraDistance);
                yield return null;
            }
            CameraControlsService.Instance.UpdateCinemachineCam(targetZoomValue, targetZoomValue);
        }

        // Smoothly moves camera to target position
        private IEnumerator SmoothCentering(Vector3 targetPosition)
        {
            Vector3 startPosition = trackingTargetTransform.position;
            float distance = Vector3.Distance(startPosition, targetPosition);
            float duration = distance / CenteringSmoothConstant;
            duration = Mathf.Clamp(duration, 0.3f, 1.5f);
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                float smoothT = Mathf.SmoothStep(0, 1, t);
                trackingTargetTransform.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    smoothT
                );
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            trackingTargetTransform.position = targetPosition;
        }
    }
}
