using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Handles shooting actions for a unit, including targeting, animation, and projectile creation.
    /// </summary>
    public class ShootAction : BaseAction
    {
        public enum ShootState { Rotate, Wait, Stop, Shoot }

        [SerializeField] GameDurations gameDurations;
        [SerializeField] UnitAnimator unitAnimator;
        [SerializeField] UnitMovement unitMovement;
        [SerializeField] WeaponMovement weaponMovement;
        [SerializeField] Rigidbody rb;

        private Vector3 targetDirection;
        private Vector3 curFwd;
        private bool hasShotBeenFired = false;
        private float timer;
        private WeaponData weaponData;

        // Determines which weapon part is used for this action
        public PartType weaponPartType => SetWeaponType();
        private Unit selectedTarget => UnitSelectService.Instance.selectedTarget;
        public WeaponType weaponType => weaponData.weaponType;

        public static event EventHandler OnStartedShooting;
        public static event EventHandler OnStoppedShooting;

        protected override void Awake()
        {
            base.Awake();
            actionName = "Shoot";
            GRID_POSITION_TYPE = GridPositionType.Enemies;
        }

        private void OnEnable()
        {
            UnitCategoryService.OnUnitAdded += UnitManager_OnUnitAdded;
            AssemblyPartInfo.OnPartAssembled += AssemblyPartInfo_OnPartAssembled;
        }

        private void OnDisable()
        {
            UnitCategoryService.OnUnitAdded -= UnitManager_OnUnitAdded;
            AssemblyPartInfo.OnPartAssembled -= AssemblyPartInfo_OnPartAssembled;
        }

        private void Start()
        {
            InitData();
        }

        private void FixedUpdate()
        {
            if (!isActive || !selectedTarget) return;

            // Calculate direction to target
            targetDirection = (selectedTarget.transform.position - rb.transform.position).normalized;
            curFwd = new Vector3(rb.transform.forward.x, targetDirection.y, rb.transform.forward.z);

            // Determine shooting state and act accordingly
            switch (SetShootState())
            {
                case ShootState.Rotate:
                    unitMovement.UpdateRotation(curFwd, targetDirection);
                    break;

                case ShootState.Shoot:
                    Shoot();
                    break;

                case ShootState.Stop:
                    ActionComplete();
                    ControlModeManager.Instance.EnterMissionMode();
                    OnStoppedShooting?.Invoke(this, EventArgs.Empty);
                    break;

                case ShootState.Wait:
                    unitMovement.StopRotation();
                    break;
            }

            timer -= Time.fixedDeltaTime;
        }

        /// <summary>
        /// Initiates the shooting action for the selected target.
        /// </summary>
        public override void TakeAction(GridPosition gridPosition)
        {
            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            UnitSelectService.Instance.SelectTarget(targetUnit);

            timer = gameDurations.actionDuration * 2;
            hasShotBeenFired = false;

            unitMovement.StartMovement(false);

            ActionStart();

            ControlModeManager.Instance.EnterActionMode();

            OnStartedShooting?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns an AI action for shooting, prioritizing targets with lower health.
        /// </summary>
        protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            float healthNormalized = targetUnit.unitMindTransform.GetComponent<HealthSystem>().GetHealthNormalized();

            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 100 + Mathf.RoundToInt((1 - healthNormalized) * 100),
                baseAction = this,
            };
        }

        private void AssemblyPartInfo_OnPartAssembled(object sender, EventArgs e)
        {
            if (UnitSelectService.Instance.selectedUnit == unit)
            {
                InitData();
            }
        }

        private void UnitManager_OnUnitAdded(object sender, UnitCategoryService.OnUnitAddedArgs e)
        {
            if (UnitSelectService.Instance.selectedUnit == unit)
            {
                InitData();
            }
        }

        /// <summary>
        /// Initializes weapon data and range from unit parts.
        /// </summary>
        private void InitData()
        {
            weaponData = (WeaponData)unit.partsData[weaponPartType];
            Range = weaponData.Range;
        }

        /// <summary>
        /// Determines which weapon part type to use based on the action type.
        /// </summary>
        private PartType SetWeaponType()
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

        /// <summary>
        /// Executes the shoot logic, including animation, projectile, audio, and ammo/heat management.
        /// </summary>
        private void Shoot()
        {
            hasShotBeenFired = true;

            unitAnimator.AnimateShootAction();
            ProjectileService.Instance.Create(this);
            ProjectileAudioService.Instance.CreateShootClip(this);

            AmmoSystem ammoSystem = UnitAmmoService.Instance.GetAmmoSystem(weaponPartType);
            switch (weaponType)
            {
                case WeaponType.Laser:
                    unit.unitMindTransform.GetComponent<HeatSystem>().GenerateHeat(weaponData.HeatCost);
                    break;
                case WeaponType.Bullet:
                    unit.unitMindTransform.GetComponent<HeatSystem>().GenerateHeat(weaponData.HeatCost);
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
        private ShootState SetShootState()
        {
            if (timer <= 0)
            {
                return ShootState.Stop;
            }
            if (timer < gameDurations.actionDuration && !hasShotBeenFired)
            {
                return ShootState.Shoot;
            }
            else if (Vector3.Dot(curFwd, targetDirection) <= .99f)
            {
                return ShootState.Rotate;
            }

            return ShootState.Wait;
        }
    }
}

