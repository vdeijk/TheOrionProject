using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Controls unit animation states
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] Unit unit;
        private UnitAnimationType animationState = UnitAnimationType.Idle;

        private void OnEnable()
        {
            UnitCategoryService.OnUnitAdded += UnitManager_OnUnitAdded;
        }

        private void OnDisable()
        {
            UnitCategoryService.OnUnitAdded -= UnitManager_OnUnitAdded;
        }

        public void AnimateShootAction()
        {
            Animator unitAnimator = unit.unitBodyTransform.GetComponentInChildren<Animator>();
            unitAnimator.SetTrigger("Shoot");
        }

        public void SetStateIdle()
        {
            Animator unitAnimator = unit.unitBodyTransform.GetComponentInChildren<Animator>();
            unitAnimator.SetBool("Idle", true);
        }

        // Handles movement animation transitions
        public void AnimateMoveAction(UnitAnimationType unitAnimationState)
        {
            if (animationState == unitAnimationState) return;
            Animator unitAnimator = unit.unitBodyTransform.GetComponentInChildren<Animator>();
            foreach (AnimatorControllerParameter parameter in unitAnimator.parameters)
            {
                unitAnimator.SetBool(parameter.name, false);
            }
            switch (unitAnimationState)
            {
                case UnitAnimationType.WalkFwd:
                    unitAnimator.SetBool("Walk", true);
                    break;
                case UnitAnimationType.TurnLeft:
                    unitAnimator.SetBool("Walk Left", true);
                    break;
                case UnitAnimationType.TurnRight:
                    unitAnimator.SetBool("Walk Right", true);
                    break;
                default:
                    unitAnimator.SetBool("Idle", true);
                    break;
            }
            animationState = unitAnimationState;
        }

        public void AnimateDeath()
        {
            Animator unitAnimator = unit.unitBodyTransform.GetComponentInChildren<Animator>();
            unitAnimator.SetTrigger("Death");
        }

        // Sets unit to idle when added to category
        private void UnitManager_OnUnitAdded(object sender, UnitCategoryService.OnUnitAddedArgs e)
        {
            if (e.unit == unit)
            {
                Animator unitAnimator = unit.unitBodyTransform.GetComponentInChildren<Animator>();
                unitAnimator.SetBool("Idle", true);
                UnitCategoryService.OnUnitAdded -= UnitManager_OnUnitAdded;
            }
        }
    }
}