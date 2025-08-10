using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    /// <summary>
    /// Service for instantiating and setting up projectiles based on weapon type.
    /// </summary>
    public class ProjectileService
    {
        private static ProjectileService _instance;

        public ProjectileData Data { get; private set; }

        public static ProjectileService Instance => _instance ??= new ProjectileService();

        public void Init(ProjectileData data)
        {
            Data = data;
        }

        /// <summary>
        /// Creates and launches a projectile for the given ShootAction.
        /// </summary>
        public void Create(ShootActionData shootActionData)
        {
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            UnitSingleController targetUnit = UnitSelectService.Instance.Data.SelectedTarget;

            Transform weaponTransform = unit.Data.TransformData[shootActionData.WeaponPartType];
            UnitSlotsController unitSlots = weaponTransform.GetComponent<UnitSlotsController>();

            // Select projectile type based on weapon
            switch (shootActionData.WeaponType)
            {
                case WeaponType.Bullet:
                    SetupBullet(unit, targetUnit);
                    break;

                case WeaponType.Missile:
                    SetupMissile(unit, targetUnit);
                    break;

                case WeaponType.Laser:
                    SetupLaser(unit, targetUnit);
                    break;
            }
        }

        /// <summary>
        /// Instantiates and sets up a bullet projectile.
        /// </summary>
        private void SetupBullet(UnitSingleController unit, UnitSingleController targetUnit)
        {
            BaseActionData selectedActionData = ActionShootService.Instance.Data;
            PartType weaponPartType = GetPartTypeFromActionType(selectedActionData.ActionType);
            WeaponSO weaponData = (WeaponSO)unit.Data.PartsData[weaponPartType];
            Transform slotTransform = unit.Data.TransformData[weaponPartType];

            Transform projectileTransform = Data.Controller.InstantiateProjectile(slotTransform.position);
            ProjectileBulletController bulletProjectile = projectileTransform.GetComponent<ProjectileBulletController>();
            bulletProjectile.Setup(unit, targetUnit);
        }

        /// <summary>
        /// Instantiates and sets up a laser projectile.
        /// </summary>
        private void SetupLaser(UnitSingleController unit, UnitSingleController targetUnit)
        {
            BaseActionData selectedActionData = ActionShootService.Instance.Data;
            PartType weaponPartType = GetPartTypeFromActionType(selectedActionData.ActionType);
            WeaponSO weaponData = (WeaponSO)unit.Data.PartsData[weaponPartType];
            Transform slotTransform = unit.Data.TransformData[weaponPartType];

            Transform projectileTransform = Data.Controller.InstantiateBeam(slotTransform.position);
            ProjectileLaserController laserProjectile = projectileTransform.GetComponent<ProjectileLaserController>();
            laserProjectile.Setup(unit, targetUnit);
        }

        /// <summary>
        /// Instantiates and sets up a missile projectile.
        /// </summary>
        private void SetupMissile(UnitSingleController unit, UnitSingleController targetUnit)
        {
            BaseActionData selectedActionData = ActionShootService.Instance.Data;
            PartType weaponPartType = GetPartTypeFromActionType(selectedActionData.ActionType);
            WeaponSO weaponData = (WeaponSO)unit.Data.PartsData[weaponPartType];
            Transform slotTransform = unit.Data.TransformData[weaponPartType];

            Transform projectileTransform = Data.Controller.InstantiateMissile(slotTransform.position);
            ProjectileMissileController missileProjectile = projectileTransform.GetComponent<ProjectileMissileController>();
            missileProjectile.Setup(unit, targetUnit);
        }

        /// <summary>
        /// Maps an action type to the corresponding part type.
        /// </summary>
        private PartType GetPartTypeFromActionType(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.ShootPrimary => PartType.WeaponPrimary,
                ActionType.ShootSecondary => PartType.WeaponSecondary,
                ActionType.Move => PartType.Base,
                _ => PartType.Base
            };
        }
    }
}