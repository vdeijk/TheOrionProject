using System;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    [Serializable]
    /// <summary>
    /// Pure data container for shooting-related configuration and runtime state.
    /// </summary>
    public class ShootActionData: BaseActionData
    {
        /// <summary>
        /// Calculates the appropriate weapon part type based on action type.
        /// </summary>
        public PartType WeaponPartType => ActionType switch
        {
            ActionType.ShootPrimary => PartType.WeaponPrimary,
            ActionType.ShootSecondary => PartType.WeaponSecondary,
            _ => PartType.WeaponPrimary
        };

        /// <summary>
        /// Returns the weapon type (laser, bullet, etc.).
        /// </summary>
        public WeaponSO WeaponSO => (WeaponSO)partSO;
        public WeaponType WeaponType => WeaponSO.weaponType;
        public UnitSingleController selectedTarget => UnitSelectService.Instance.Data.SelectedTarget;
    }
}
