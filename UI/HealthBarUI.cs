using System;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class HealthBarUI : StatBarUI
    {
        private void OnEnable()
        {
            UnitHealthController.OnDamaged += HealthSystem_OnDamaged;
        }

        private void OnDisable()
        {
            UnitHealthController.OnDamaged -= HealthSystem_OnDamaged;
        }

        private void HealthSystem_OnDamaged(object sender, EventArgs e)
        {
            UpdateBar();
        }

        public override void UpdateBar()
        {
            if (UnitSelectService.Instance.Data.SelectedUnit == null) return;

            UnitHealthController healthSystem = UnitSelectService.Instance.Data.SelectedUnit.Data.UnitMindTransform.GetComponent<UnitHealthController>();
            barImage.fillAmount = healthSystem.GetHealthNormalized();
            valueText.text = healthSystem.health.ToString();
        }
    }
}