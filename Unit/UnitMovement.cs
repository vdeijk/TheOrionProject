using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Controls movement, rotation, and animation for a unit
    public class UnitMovement : MonoBehaviour
    {
        [field: SerializeField] public Unit unit { get; private set; }
        [field: SerializeField] public Rigidbody rb { get; private set; }
        public float MoveSpeed { get; private set; }
        public float RotateSpeed { get; private set; }
        [SerializeField] UnitAnimator unitAnimator;
        [SerializeField] AudioSource footsteps;
        [SerializeField] AudioSource rotServo;
        private UnitAnimationType curAnimationState = UnitAnimationType.Idle;
        private BaseData baseData;

        private void OnEnable()
        {
            AssemblyPartInfo.OnPartAssembled += AssemblyPartInfo_OnPartAssembled;
            UnitSpawnService.OnUnitsTeleported += UnitSpawnService_OnUnitsTeleported;
        }

        private void OnDisable()
        {
            AssemblyPartInfo.OnPartAssembled -= AssemblyPartInfo_OnPartAssembled;
            UnitSpawnService.OnUnitsTeleported -= UnitSpawnService_OnUnitsTeleported;
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
                StartCoroutine(AudioFading.Instance.Fade(footsteps.volume, 1, footsteps, .5f));
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
            StartCoroutine(AudioFading.Instance.Fade(footsteps.volume, 0, footsteps, .5f));
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
        private void UnitSpawnService_OnUnitsTeleported(object sender, System.EventArgs e)
        {
            InitData();
        }

        private void AssemblyPartInfo_OnPartAssembled(object sender, EventArgs e)
        {
            if (UnitSelectService.Instance.selectedUnit == unit)
            {
                InitData();
            }
        }

        // Initializes movement speed and rotation from base data
        private void InitData()
        {
            UnitMovementService.Instance.InitPos(rb, unit);
            baseData = (BaseData)unit.partsData[PartType.Base];
            MoveSpeed = baseData.MoveSpeed;
            RotateSpeed = baseData.RotateSpeed;
        }
    }
}
