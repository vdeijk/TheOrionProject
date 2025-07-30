using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class HealthBar : StatBar
    {
        private void OnEnable()
        {
            HealthSystem.OnDamaged += HealthSystem_OnDamaged;
        }

        private void OnDisable()
        {
            HealthSystem.OnDamaged -= HealthSystem_OnDamaged;
        }

        private void HealthSystem_OnDamaged(object sender, EventArgs e)
        {
            UpdateBar();
        }

        public override void UpdateBar()
        {
            if (UnitSelectService.Instance.selectedUnit == null) return;

            HealthSystem healthSystem = UnitSelectService.Instance.selectedUnit.unitMindTransform.GetComponent<HealthSystem>();
            barImage.fillAmount = healthSystem.GetHealthNormalized();
            valueText.text = healthSystem.health.ToString();
        }
    }
}