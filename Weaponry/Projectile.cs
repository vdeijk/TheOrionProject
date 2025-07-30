using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Base class for all projectile types
    public class Projectile : MonoBehaviour
    {
        [SerializeField] GridData gridData;
        protected Unit unit;
        protected Unit targetUnit;
        protected Vector3 targetPos;
        public event EventHandler OnHit;

        // Subscribes to game mode change event
        protected virtual void OnEnable()
        {
            ControlModeManager.OnGameModeChanged += ControlModeManager_OnGameModeChanged;
        }

        // Unsubscribes from game mode change event
        protected virtual void OnDisable()
        {
            ControlModeManager.OnGameModeChanged -= ControlModeManager_OnGameModeChanged;
        }

        // Sets up projectile with source and target units
        public virtual void Setup(Unit unit, Unit targetUnit)
        {
            targetPos = targetUnit.transformData[PartType.Torso].position;
            this.unit = unit;
            // this.targetUnit = targetUnit;
        }

        // Applies damage to the target unit
        protected void Damage()
        {
            BaseAction baseAction = UnitActionSystem.Instance.selectedAction;
            WeaponData weaponData = (WeaponData)unit.partsData[SetWeaponType(baseAction.actionType)];
            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(unit.unitBodyTransform.position);
            GridObject gridObject = gridData.gridObjectArray[gridPosition.x, gridPosition.z];
            float damage = weaponData.Damage;
            BaseData baseData = (BaseData)unit.partsData[PartType.Base];
            bool isGridSquareHigh = gridObject.gridSquareType == GridSquareType.High;
            bool isUitGroundOrHover = baseData.unitType == UnitType.Ground ||
                baseData.unitType == UnitType.Hover;
            if (isGridSquareHigh && isUitGroundOrHover) damage *= 1.5f;
            targetUnit.unitMindTransform.GetComponent<HealthSystem>().TakeDamage(damage, unit);
            OnHit?.Invoke(this, EventArgs.Empty);
        }

        // Determines which weapon part type is used for the action
        private PartType SetWeaponType(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.ShootPrimary:
                    return PartType.WeaponPrimary;
                case ActionType.ShootSecondary:
                    return PartType.WeaponSecondary;
                default:
                    return PartType.WeaponPrimary;
            }
        }

        // Destroys projectile if game mode changes away from Mission
        private void ControlModeManager_OnGameModeChanged(object sender, EventArgs e)
        {
            if (ControlModeManager.Instance.gameControlType == GameControlType.Mission) return;
            Destroy(gameObject);
        }
    }
}
