using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
    public class UIRepair : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI armorText;
        [SerializeField] TextMeshProUGUI shieldText;
        [SerializeField] TextMeshProUGUI heatText;
        [SerializeField] Image healthBar;
        [SerializeField] Image armorBar;
        [SerializeField] Image shieldBar;
        [SerializeField] Image heatBar;
        [SerializeField] TextMeshProUGUI scrapAmountHealth;
        [SerializeField] TextMeshProUGUI scrapAmountArmor;
        [SerializeField] float baseRepairCost = 100f;

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        public void RepairHealth()
        {
            Unit unit = UnitSelectService.Instance.selectedUnit;
            HealthSystem healthSystem = unit.unitMindTransform.GetComponent<HealthSystem>();
            float missingHealth = healthSystem.MaxHealth - healthSystem.health;
            float cost = CalculateCost(missingHealth);
            float scrapAmount = ScrapManager.Instance.curScrap;
            if (missingHealth > 0f && cost < scrapAmount)
            {
                ScrapManager.Instance.RemoveScrap(cost);
                healthSystem.Repair(DamageType.Health, missingHealth);
                Setup();
            }
        }

        public void RepairArmor()
        {
            Unit unit = UnitSelectService.Instance.selectedUnit;
            HealthSystem healthSystem = unit.unitMindTransform.GetComponent<HealthSystem>();
            float missingArmor = healthSystem.MaxArmor - healthSystem.armor;
            float cost = CalculateCost(missingArmor);
            float scrapAmount = ScrapManager.Instance.curScrap;
            if (missingArmor > 0f && cost < scrapAmount)
            {
                ScrapManager.Instance.RemoveScrap(cost);
                healthSystem.Repair(DamageType.Armor, missingArmor);
                Setup();
            }
        }

        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            if (MenuChangeService.Instance.curMenu == MenuType.Repair)
            {
                Setup();
            }
        }

        private void Setup()
        {
            Unit unit = UnitSelectService.Instance.selectedUnit;
            HealthSystem healthSystem = unit.unitMindTransform.GetComponent<HealthSystem>();
            HeatSystem heatSystem = unit.unitMindTransform.GetComponent<HeatSystem>();

            healthText.text = healthSystem.health.ToString();
            armorText.text = healthSystem.armor.ToString();
            shieldText.text = healthSystem.shield.ToString();
            heatText.text = heatSystem.heat.ToString();

            healthBar.fillAmount = healthSystem.GetHealthNormalized();
            armorBar.fillAmount = healthSystem.GetArmorNormalized();
            shieldBar.fillAmount = healthSystem.GetShieldNormalized();
            heatBar.fillAmount = heatSystem.GetHeatNormalized();

            float missingHealth = healthSystem.MaxHealth - healthSystem.health;
            float missingArmor = healthSystem.MaxArmor - healthSystem.armor;

            scrapAmountHealth.text = CalculateCost(missingHealth).ToString() + " Scr";
            scrapAmountArmor.text = CalculateCost(missingArmor).ToString() + " Scr";
        }
        private int CalculateCost(float missingPoints)
        {
            if (missingPoints <= 0f) return 0;

            return Mathf.CeilToInt(missingPoints * baseRepairCost);
        }
    }
}