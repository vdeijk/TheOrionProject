using System;
using UnityEngine;
using TurnBasedStrategy.UI;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Manages health, armor, and shield for a unit
    public class UnitHealthController : MonoBehaviour
    {
        public class OnUnitDeadEventArgs : EventArgs
        {
            public UnitSingleController unit;
        }

        public float health { get; private set; }
        public float armor { get; private set; }
        public float shield { get; private set; }

        [SerializeField] UnitSingleController unit;
        [SerializeField] UnitExplosionController unitExplosion;
        [SerializeField] HealthBarWorldSpaceUI healthBarWorldSpace;

        private GridData gridData => GridCoordinatorService.Instance.Data;

        private BaseSO baseData => (BaseSO)unit.Data.PartsData[PartType.Base];
        private TorsoSO torsoData => (TorsoSO)unit.Data.PartsData[PartType.Torso];

        public static event EventHandler OnDamaged;
        public static event EventHandler<OnUnitDeadEventArgs> OnunitDead;

        public float MaxHealth => baseData.MaxHealth;
        public float MaxArmor => baseData.MaxArmor;
        public float MaxShield => torsoData.MaxShield;

        private void OnEnable()
        {
            AssemblyPartInfo.OnPartAssembled += AssemblyPartInfo_OnPartAssembled;
        }

        private void OnDisable()
        {
            AssemblyPartInfo.OnPartAssembled -= AssemblyPartInfo_OnPartAssembled;
        }

        private void Start()
        {
            SetMax();
            healthBarWorldSpace.UpdateBar();
        }

        public void SetMax()
        {
            health = MaxHealth;
            armor = MaxArmor;
            shield = MaxShield;
        }

        // Repairs the specified type
        public void Repair(DamageType type, float amount)
        {
            switch (type)
            {
                case DamageType.Health:
                    float maxHealth = baseData.MaxHealth;
                    float newHealth = Mathf.Min(health + amount, maxHealth);
                    if (newHealth > health)
                    {
                        health = newHealth;
                    }
                    break;
                case DamageType.Armor:
                    float maxArmor = baseData.MaxArmor;
                    float newArmor = Mathf.Min(armor + amount, maxArmor);
                    if (newArmor > armor)
                    {
                        armor = newArmor;
                    }
                    break;
                case DamageType.Shield:
                    float maxShield = torsoData.MaxShield;
                    float newShield = Mathf.Min(shield + amount, maxShield);
                    if (newShield > shield)
                    {
                        shield = newShield;
                    }
                    break;
            }
        }

        // Applies damage to shield, armor, and health in order
        public void TakeDamage(float baseDamage, UnitSingleController shooterUnit)
        {
            ActionType actionType = ActionShootService.Instance.Data.ActionType;
            UnitHealthService healthService = UnitHealthService.Instance;
            WeaponSO weaponData = (WeaponSO)shooterUnit.Data.PartsData[GetWeaponPartType(actionType)];
            TorsoSO torsoData = (TorsoSO)shooterUnit.Data.PartsData[PartType.Torso];

            float adjustedBaseDamage = baseDamage;
            if (torsoData.Synergy == weaponData.weaponType) adjustedBaseDamage *= 1.5f;
            Vector2Int gridPosition = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);
            GridObject gridObject = gridData.Objects[gridPosition.x, gridPosition.y];
            float terrainMultiplier = healthService.CalculateTileTypeMultiplier(
                gridObject.gridSquareType, unit);
            adjustedBaseDamage *= terrainMultiplier;
            float shieldMultiplier = healthService.CalculateWeaponTypeMultiplier(DamageType.Shield, weaponData.weaponType);
            float shieldDamage = adjustedBaseDamage * shieldMultiplier;
            float damageRemainingAfterShield = DamageShield(shieldDamage);
            float armorMultiplier = healthService.CalculateWeaponTypeMultiplier(DamageType.Armor, weaponData.weaponType);
            float throughShieldPercent = shieldDamage > 0 ? damageRemainingAfterShield / shieldDamage : 1f;
            float armorDamage = adjustedBaseDamage * throughShieldPercent * armorMultiplier;
            float damageRemainingAfterArmor = DamageArmor(armorDamage);
            float throughArmorPercent = armorDamage > 0 ? damageRemainingAfterArmor / armorDamage : 1f;
            float healthDamage = adjustedBaseDamage * throughShieldPercent * throughArmorPercent;
            DamageHealth(healthDamage);
        }

        public float GetHealthNormalized()
        {
            return baseData.MaxHealth > 0 ? (float)health / baseData.MaxHealth : 0f;
        }

        public float GetArmorNormalized()
        {
            return baseData.MaxArmor > 0 ? (float)armor / baseData.MaxArmor : 0f;
        }

        public float GetShieldNormalized()
        {
            return torsoData.MaxShield > 0 ? (float)shield / torsoData.MaxShield : 0f;
        }

        // Calculates percent missing for repair cost
        public int CalculateCost(DamageType type)
        {
            float missing = 0f;
            float max = 0f;
            switch (type)
            {
                case DamageType.Health:
                    missing = MaxHealth - health;
                    max = MaxHealth;
                    break;
                case DamageType.Armor:
                    missing = MaxArmor - armor;
                    max = MaxArmor;
                    break;
            }
            float percentMissing = (max > 0) ? (missing / max) * 100f : 0f;
            return Mathf.CeilToInt(percentMissing);
        }

        public void Die()
        {
            Vector2Int unitGridPosition = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);
            GridUnitService.Instance.RemoveUnit(unitGridPosition, unit);
            unitExplosion.ExplodeUnit();
            OnunitDead?.Invoke(this, new OnUnitDeadEventArgs
            {
                unit = unit,
            });
            unit.gameObject.SetActive(false);
        }

        private void AssemblyPartInfo_OnPartAssembled(object sender, EventArgs e)
        {
            if (UnitSelectService.Instance.Data.SelectedUnit == unit)
            {
                SetMax();
            }
        }

        private PartType GetWeaponPartType(ActionType actionType)
        {
            return actionType switch
            {
                ActionType.ShootPrimary => PartType.WeaponPrimary,
                ActionType.ShootSecondary => PartType.WeaponSecondary,
                _ => PartType.WeaponPrimary
            };
        }

        private float DamageShield(float damageAmount)
        {
            if (shield <= 0) return damageAmount;
            float originalShield = shield;
            shield = Mathf.Max(shield - damageAmount, 0);
            return damageAmount > originalShield ? damageAmount - originalShield : 0;
        }

        private float DamageArmor(float damageAmount)
        {
            if (armor <= 0) return damageAmount;
            float originalArmor = armor;
            armor = Mathf.Max(armor - damageAmount, 0);
            return damageAmount > originalArmor ? damageAmount - originalArmor : 0;
        }

        private void DamageHealth(float damageAmount)
        {
            health = Mathf.Max(health - damageAmount, 0);
            OnDamaged?.Invoke(this, EventArgs.Empty);
            if (health <= 0)
            {
                if (unit.Data.UnitEntityTransform.GetComponent<UnitFactionController>().IS_ENEMY)
                {
                    ControlModeManager.Instance.EnterSalvageMode(unit);
                }
                else
                {
                    Die();
                }
            }
        }
    }
}
