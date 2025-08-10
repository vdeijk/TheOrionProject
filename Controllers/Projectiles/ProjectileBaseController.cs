using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Base class for all projectile types
    public class ProjectileBaseController : MonoBehaviour
    {
        protected UnitSingleController unit;
        protected UnitSingleController targetUnit;
        protected Vector3 targetPos;

        public event EventHandler OnHit;

        private GridData gridData => GridCoordinatorService.Instance.Data;

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
        public virtual void Setup(UnitSingleController unit, UnitSingleController targetUnit)
        {
            targetPos = targetUnit.Data.TransformData[PartType.Torso].position;
            this.unit = unit;
            // this.targetUnit = targetUnit;
        }

        // Applies damage to the target unit
        protected void Damage()
        {
            BaseActionData baseActionData = ActionShootService.Instance.Data;
            WeaponSO weaponData = (WeaponSO)unit.Data.PartsData[SetWeaponType(baseActionData.ActionType)];
            Vector2Int gridPosition = GridUtilityService.Instance.GetGridPosition(unit.Data.UnitBodyTransform.position);
            GridObject gridObject = gridData.Objects[gridPosition.x, gridPosition.y];
            float damage = weaponData.Damage;
            BaseSO baseData = (BaseSO)unit.Data.PartsData[PartType.Base];
            bool isGridSquareHigh = gridObject.gridSquareType == GridSquareType.High;
            bool isUitGroundOrHover = baseData.UnitType == UnitType.Ground ||
                baseData.UnitType == UnitType.Hover;
            if (isGridSquareHigh && isUitGroundOrHover) damage *= 1.5f;
            targetUnit.Data.UnitMindTransform.GetComponent<UnitHealthController>().TakeDamage(damage, unit);
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
