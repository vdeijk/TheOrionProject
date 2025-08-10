using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    // Displays comparison info for weapon parts in the assembly UI
    public class AssemblyWeaponUI : AssemblyPartUI
    {
        [SerializeField] TextMeshProUGUI partNames;
        [SerializeField] TextMeshProUGUI weaponTypeText;
        [SerializeField] TextMeshProUGUI maxAmmoText;
        [SerializeField] TextMeshProUGUI heatText;
        [SerializeField] TextMeshProUGUI rangeText;
        [SerializeField] TextMeshProUGUI damageText;

        // Populates UI fields with old vs new weapon data for comparison
        protected override void SetTexts(PartSO partData)
        {
            if (!(partData is WeaponSO newWeaponData)) return;

            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            PartType curPartType = AssemblyService.Instance.Data.CurPartType;
            WeaponSO oldWeaponData = (WeaponSO)unit.Data.PartsData[curPartType];

            partNames.text = oldWeaponData.Name + symbol + "<br>" + newWeaponData.Name;
            weaponTypeText.text = "<b>Weapon Type:</b> " + oldWeaponData.weaponType.ToString() + symbol + newWeaponData.weaponType.ToString();
            maxAmmoText.text = "<b>Max Ammo:</b> " + SetMaxAmmo(oldWeaponData) + symbol + SetMaxAmmo(newWeaponData);
            heatText.text = "<b>Heat Cost:</b> " + SetHeatCost(oldWeaponData) + symbol + SetHeatCost(newWeaponData);
            rangeText.text = "<b>Shooting Range:</b> " + oldWeaponData.Range + symbol + newWeaponData.Range;
            damageText.text = "<b>Base Damage:</b> " + oldWeaponData.Damage + symbol + newWeaponData.Damage;
        }

        // Returns heat cost as string, or 'None' if zero
        private string SetHeatCost(WeaponSO weaponData)
        {
            return weaponData.HeatCost == 0 ? "None" : weaponData.HeatCost.ToString();
        }

        // Returns max ammo as string, or 'Unlimited' if zero
        private string SetMaxAmmo(WeaponSO weaponData)
        {
            return weaponData.MaxAmmo == 0 ? "Unlimited" : weaponData.MaxAmmo.ToString();
        }
    }
}
