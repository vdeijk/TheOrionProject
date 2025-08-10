using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Manages heat for a unit
    public class HeatSystemController : MonoBehaviour
    {
        public class OnUnitDeadEventArgs : EventArgs
        {
            public UnitSingleController unit;
        }

        [SerializeField] UnitSingleController unit;
        [SerializeField] UnitExplosionController unitExplosion;

        public float heat { get; private set; }
        public float MaxHeat => torsoData.MaxHeat;
        private TorsoSO torsoData => (TorsoSO)unit.Data.PartsData[PartType.Torso];

        public static event EventHandler OnGenerated;
        public static event EventHandler OnDissipated;
        public static event EventHandler<OnUnitDeadEventArgs> OnunitDead;

        private void Awake()
        {
            heat = 0;
        }

        private void OnEnable()
        {
            ActionShootController.OnStoppedShooting += ShootAction_OnStoppedShooting;
        }

        private void OnDisable()
        {
            ActionShootController.OnStoppedShooting -= ShootAction_OnStoppedShooting;
        }

        public void SetMin()
        {
            heat = 0;
        }

        public void GenerateHeat(float plusValue)
        {
            heat = Mathf.Min(heat + plusValue, torsoData.MaxHeat);
            OnGenerated?.Invoke(this, EventArgs.Empty);
        }

        public void DissipateHeat(float minusValue)
        {
            heat = Mathf.Max(heat - minusValue, 0);
            OnDissipated?.Invoke(this, EventArgs.Empty);
        }

        public float GetHeatNormalized()
        {
            return (float)heat / torsoData.MaxHeat;
        }

        // Overheating check
        private void ShootAction_OnStoppedShooting(object sender, EventArgs e)
        {
            if (heat >= torsoData.MaxHeat)
            {
                Die();
            }
        }

        private void Die()
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
    }
}
