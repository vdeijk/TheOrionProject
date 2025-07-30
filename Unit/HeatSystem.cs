using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Manages heat for a unit
    public class HeatSystem : MonoBehaviour
    {
        public class OnUnitDeadEventArgs : EventArgs
        {
            public Unit unit;
        }

        [SerializeField] Unit unit;
        [SerializeField] UnitExplosion unitExplosion;

        public float heat { get; private set; }
        public float MaxHeat => torsoData.MaxHeat;
        private TorsoData torsoData => (TorsoData)unit.partsData[PartType.Torso];

        public static event EventHandler OnGenerated;
        public static event EventHandler OnDissipated;
        public static event EventHandler<OnUnitDeadEventArgs> OnunitDead;

        private void Awake()
        {
            heat = 0;
        }

        private void OnEnable()
        {
            ShootAction.OnStoppedShooting += ShootAction_OnStoppedShooting;
        }

        private void OnDisable()
        {
            ShootAction.OnStoppedShooting -= ShootAction_OnStoppedShooting;
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
            GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(unit.unitBodyTransform.position);
            LevelGrid.Instance.RemoveUnitAtGridPosition(unitGridPosition, unit);
            unitExplosion.ExplodeUnit();
            OnunitDead?.Invoke(this, new OnUnitDeadEventArgs
            {
                unit = unit,
            });
            unit.gameObject.SetActive(false);
        }
    }
}
