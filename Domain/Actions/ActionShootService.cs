using System;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Game;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    /// <summary>
    /// Handles shooting actions for a unit, including targeting, animation, and projectile creation.
    /// </summary>
    public class ActionShootService : BaseActionService<ShootActionData>
    {
        private static ActionShootService _instance;
        private bool hasShotBeenFired = false;
        private float timer;

        // Determines which weapon part is used for this action
        public static ActionShootService Instance => _instance ??= new ActionShootService();
        private UnitSingleController selectedTarget => Data.selectedTarget;

        public static event EventHandler OnStartedShooting;
        public static event EventHandler OnStoppedShooting;

        public override ShootActionData Data
        {
            get
            {
                var selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
                if (selectedUnit == null) return null;
                var action = selectedUnit.Data.UnitEntityTransform.GetComponent<ActionShootController>();
                return action?.TypedData;
            }
        }

        public void ExecuteUpdate()
        {
            // Calculate direction to target
            targetDirection = (selectedTarget.transform.position - rb.transform.position).normalized;
            curForward = new Vector3(rb.transform.forward.x, targetDirection.y, rb.transform.forward.z);

            // Determine shooting state and act accordingly
            switch (SetShootState())
            {
                case ShootStateType.Rotate:
                    unitMovement.UpdateRotation(curForward, targetDirection);
                    break;

                case ShootStateType.Shoot:
                    Shoot();
                    break;

                case ShootStateType.Stop:
                    CompleteAction();
                    ControlModeManager.Instance.EnterMissionMode();
                    OnStoppedShooting?.Invoke(this, EventArgs.Empty);
                    break;

                case ShootStateType.Wait:
                    unitMovement.StopRotation();
                    break;
            }

            timer -= Time.fixedDeltaTime;
        }

        /// <summary>
        /// Initiates the shooting action for the selected target.
        /// </summary>
        public override void TakeAction(Vector2Int gridPosition)
        {
            UnitSingleController targetUnit = GridUnitService.Instance.GetUnit(gridPosition);
            UnitSelectService.Instance.SelectTarget(targetUnit);

            timer = durationData.ActionDuration * 2;
            hasShotBeenFired = false;

            Data.UnitMovement.StartMovement(false);

            ControlModeManager.Instance.EnterActionMode();

            OnStartedShooting?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns an AI action for shooting, prioritizing targets with lower health.
        /// </summary>
        protected override UnitAIData GetEnemyAction(Vector2Int gridPosition)
        {
            UnitSingleController targetUnit = GridUnitService.Instance.GetUnit(gridPosition);
            float healthNormalized = targetUnit.Data.UnitMindTransform.GetComponent<UnitHealthController>().GetHealthNormalized();

            return new UnitAIData
            {
                GridPosition = gridPosition,
                ActionValue = 100 + Mathf.RoundToInt((1 - healthNormalized) * 100),
                BaseActionData = Data,
            };
        }

        /// <summary>
        /// Executes the shoot logic, including animation, projectile, audio, and ammo/heat management.
        /// </summary>
        public void Shoot()
        {
            hasShotBeenFired = true;

            Data.UnitAnimator.AnimateShootAction();
            ProjectileService.Instance.Create(Data);
            ProjectileAudioService.Instance.CreateShootClip(Data);

            float heatCost = Data.WeaponSO.HeatCost;
            HeatSystemController heatSystem = Data.Unit.Data.UnitMindTransform.GetComponent<HeatSystemController>();
            UnitAmmoController ammoSystem = UnitAmmoService.Instance.GetAmmoSystem(Data.WeaponPartType);
            switch (Data.WeaponType)
            {
                case WeaponType.Laser:
                    heatSystem.GenerateHeat(heatCost);
                    break;
                case WeaponType.Bullet:
                    heatSystem.GenerateHeat(heatCost);
                    ammoSystem.SpendAmmo();
                    break;
                case WeaponType.Missile:
                    ammoSystem.SpendAmmo();
                    break;
            }
        }

        /// <summary>
        /// Determines the current shooting state based on timer and orientation.
        /// </summary>
        public ShootStateType SetShootState()
        {
            if (timer <= 0)
            {
                return ShootStateType.Stop;
            }
            if (timer < durationData.ActionDuration && !hasShotBeenFired)
            {
                return ShootStateType.Shoot;
            }
            else if (Vector3.Dot(curForward, targetDirection) <= .99f)
            {
                return ShootStateType.Rotate;
            }

            return ShootStateType.Wait;
        }
    }
}

/*
        /// <summary>
        /// Determines which weapon part type to use based on the action type.
        /// </summary>
        public PartType SetWeaponType()
        {
            switch (Data.actionType)
            {
                case ActionType.ShootPrimary:
                    return PartType.WeaponPrimary;
                case ActionType.ShootSecondary:
                    return PartType.WeaponSecondary;
                default:
                    return PartType.WeaponPrimary;
            }
        }*/