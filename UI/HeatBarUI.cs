using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class HeatBarUI : StatBarUI
    {
        public override void UpdateBar()
        {
            UnitHealthController healthSystem = UnitSelectService.Instance.Data.SelectedUnit.Data.UnitMindTransform.GetComponent<UnitHealthController>();
            barImage.fillAmount = healthSystem.GetHealthNormalized();
            valueText.text = healthSystem.health.ToString();
        }
    }
}