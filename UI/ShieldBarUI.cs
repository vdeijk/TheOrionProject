using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    public class ShieldBarUI : StatBarUI
    {
        public override void UpdateBar()
        {
            UnitHealthController healthSystem = UnitSelectService.Instance.Data.SelectedUnit.Data.UnitMindTransform.GetComponent<UnitHealthController>();
            barImage.fillAmount = healthSystem.GetHealthNormalized();
            valueText.text = healthSystem.health.ToString();
        }
    }
}