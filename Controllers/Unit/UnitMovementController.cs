using System;
using UnityEngine;
using TurnBasedStrategy.UI;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Controls movement, rotation, and animation for a unit
    public class UnitMovementController : MonoBehaviour
    {
        [field: SerializeField] public UnitSingleController unit { get; private set; }
        [field: SerializeField] public Rigidbody rb { get; private set; }
        public float MoveSpeed { get; private set; }
        public float RotateSpeed { get; private set; }

        [SerializeField] UnitAnimationController unitAnimator;
        [SerializeField] AudioSource footsteps;
        [SerializeField] AudioSource rotServo;
        [SerializeField] float hoverHeightOffset = 2f;
        [SerializeField] float airHeightOffset = 4f;
        [SerializeField] float heightSmoothTime = 0.2f;

        private UnitAnimationType curAnimationState = UnitAnimationType.Idle;
        private BaseSO baseData;

        private void OnEnable()
        {
            AssemblyPartInfo.OnPartAssembled += AssemblyPartInfo_OnPartAssembled;
            UnitMovementService.OnUnitsTeleported += UnitSpawnService_OnUnitsTeleported;
        }

        private void OnDisable()
        {
            AssemblyPartInfo.OnPartAssembled -= AssemblyPartInfo_OnPartAssembled;
            UnitMovementService.OnUnitsTeleported -= UnitSpawnService_OnUnitsTeleported;
        }

        private void Start()
        {
            InitData();
        }

        // Starts movement and plays audio/animation
        public void StartMovement(bool withAudio)
        {
            if (withAudio)
            {
                StopAllCoroutines();
                float duration = DurationData.Instance.MaterialFadeDuration;
                StartCoroutine(AudioFadeMonobService.Instance.Fade(footsteps.volume, 1, footsteps, duration));
            }
            else
            {
                rotServo.Play();
            }
            curAnimationState = UnitAnimationType.WalkFwd;
            unitAnimator.AnimateMoveAction(curAnimationState);
        }

        // Stops rotation and sets idle animation
        public void StopRotation()
        {
            curAnimationState = UnitAnimationType.Idle;
            unitAnimator.AnimateMoveAction(curAnimationState);
        }

        // Stops movement, fades audio, and sets idle animation
        public void StopMovement()
        {
            UnitMovementService.Instance.StopMovement(this);
            StopAllCoroutines();
            float duration = DurationData.Instance.MaterialFadeDuration;
            StartCoroutine(AudioFadeMonobService.Instance.Fade(footsteps.volume, 0, footsteps, duration));
            curAnimationState = UnitAnimationType.Idle;
            unitAnimator.AnimateMoveAction(curAnimationState);
        }

        // Updates movement using movement service
        public void UpdateMovement(Vector3 targetDirection)
        {
            UnitMovementService.Instance.UpdateMovement(this);
        }

        // Updates rotation using movement service
        public void UpdateRotation(Vector3 curFwd, Vector3 targetDirection)
        {
            UnitMovementService.Instance.UpdateRotation(this, curFwd, targetDirection);
        }

        // Re-initializes movement data after teleport or assembly
        private void UnitSpawnService_OnUnitsTeleported(object sender, EventArgs e)
        {
            InitData();
        }

        private void AssemblyPartInfo_OnPartAssembled(object sender, EventArgs e)
        {
            if (UnitSelectService.Instance.Data.SelectedUnit == unit)
            {
                InitData();
            }
        }

        // Initializes movement speed and rotation from base data
        private void InitData()
        {
            UnitMovementService.Instance.InitPos(rb, unit);
            baseData = (BaseSO)unit.Data.PartsData[PartType.Base];
            MoveSpeed = baseData.MoveSpeed;
            RotateSpeed = baseData.RotateSpeed;
        }
    }
}
