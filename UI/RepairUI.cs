using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class RepairUI : MonoBehaviour
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
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        public void RepairHealth()
        {
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            UnitHealthController healthSystem = unit.Data.UnitMindTransform.GetComponent<UnitHealthController>();
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
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            UnitHealthController healthSystem = unit.Data.UnitMindTransform.GetComponent<UnitHealthController>();
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
            if (MenuChangeMonobService.Instance.curMenu == MenuType.Repair)
            {
                Setup();
            }
        }

        private void Setup()
        {
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            UnitHealthController healthSystem = unit.Data.UnitMindTransform.GetComponent<UnitHealthController>();
            HeatSystemController heatSystem = unit.Data.UnitMindTransform.GetComponent<HeatSystemController>();

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