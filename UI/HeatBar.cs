using TurnBasedStrategy;
using UnityEngine;

public class HeatBar : StatBar
{
    public override void UpdateBar()
    {
        HealthSystem healthSystem = UnitSelectService.Instance.selectedUnit.unitMindTransform.GetComponent<HealthSystem>();
        barImage.fillAmount = healthSystem.GetHealthNormalized();
        valueText.text = healthSystem.health.ToString();
    }
}
