using UnityEngine;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy.Data
{
    /// <summary>
    /// Pure data container for shooting-related configuration and runtime state.
    /// </summary>
    /// 
    public class BaseActionData
    {
        [field: SerializeField] public bool HasMaxRange { get; private set; }
        [field: SerializeField] public string ActionName { get; private set; }
        [field: SerializeField] public ActionType ActionType { get; private set; }
        [field: SerializeField] public UnitSingleController Unit { get; private set; }
        [field: SerializeField] public UnitAnimationController UnitAnimator { get; private set; }
        [field: SerializeField] public UnitMovementController UnitMovement { get; private set; }
        [field: SerializeField] public Rigidbody Rb { get; private set; }
        [field: SerializeField] public ActionBaseController BaseAction { get; private set; }

        public bool IsActive { get; set; } = false;

        public PartSO partSO => Unit.Data.PartsData[PartType];
        public PartType PartType => ActionType switch
        {
            ActionType.Move => PartType.Base,
            ActionType.ShootPrimary => PartType.WeaponPrimary,
            ActionType.ShootSecondary => PartType.WeaponSecondary,
            ActionType.Pass => PartType.Torso,
            _ => PartType.Base
        };

        // Determines which weapon part type is used for the selected action
        public PartType GetWeaponPartType()
        {
            return ActionType switch
            {
                ActionType.ShootPrimary => PartType.WeaponPrimary,
                ActionType.ShootSecondary => PartType.WeaponSecondary,
                _ => PartType.WeaponPrimary
            };
        }
    }
}
