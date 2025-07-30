using UnityEngine;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Service for instantiating and setting up projectiles based on weapon type.
    /// </summary>
    public class ProjectileService : Singleton<ProjectileService>
    {
        [SerializeField] Transform bulletPrefab;
        [SerializeField] Transform missileProjectilePrefab;
        [SerializeField] Transform laserPrefab;

        /// <summary>
        /// Creates and launches a projectile for the given ShootAction.
        /// </summary>
        public void Create(ShootAction shootAction)
        {
            Unit unit = UnitSelectService.Instance.selectedUnit;
            Unit targetUnit = UnitSelectService.Instance.selectedTarget;

            Transform weaponTransform = unit.transformData[shootAction.weaponPartType];
            UnitSlots unitSlots = weaponTransform.GetComponent<UnitSlots>();

            // Select projectile type based on weapon
            switch (shootAction.weaponType)
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
        private void SetupBullet(Unit unit, Unit targetUnit)
        {
            BaseAction baseAction = UnitActionSystem.Instance.selectedAction;
            PartType weaponPartType = GetPartTypeFromActionType(baseAction.actionType);
            WeaponData weaponData = (WeaponData)unit.partsData[weaponPartType];
            Transform slotTransform = unit.transformData[weaponPartType];

            Transform projectileTransform = Instantiate(bulletPrefab, slotTransform.position, Quaternion.identity);
            BulletProjectile bulletProjectile = projectileTransform.GetComponent<BulletProjectile>();
            bulletProjectile.Setup(unit, targetUnit);
        }

        /// <summary>
        /// Instantiates and sets up a laser projectile.
        /// </summary>
        private void SetupLaser(Unit unit, Unit targetUnit)
        {
            BaseAction baseAction = UnitActionSystem.Instance.selectedAction;
            PartType weaponPartType = GetPartTypeFromActionType(baseAction.actionType);
            WeaponData weaponData = (WeaponData)unit.partsData[weaponPartType];
            Transform slotTransform = unit.transformData[weaponPartType];

            Transform projectileTransform = Instantiate(laserPrefab, slotTransform.position, Quaternion.identity);
            LaserProjectile laserProjectile = projectileTransform.GetComponent<LaserProjectile>();
            laserProjectile.Setup(unit, targetUnit);
        }

        /// <summary>
        /// Instantiates and sets up a missile projectile.
        /// </summary>
        private void SetupMissile(Unit unit, Unit targetUnit)
        {
            BaseAction baseAction = UnitActionSystem.Instance.selectedAction;
            PartType weaponPartType = GetPartTypeFromActionType(baseAction.actionType);
            WeaponData weaponData = (WeaponData)unit.partsData[weaponPartType];
            Transform slotTransform = unit.transformData[weaponPartType];

            Transform projectileTransform = Instantiate(missileProjectilePrefab, slotTransform.position, Quaternion.identity);
            MissileProjectile missileProjectile = projectileTransform.GetComponent<MissileProjectile>();
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