using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Displays comparison info for weapon parts in the assembly UI
    public class UIAssemblyWeapon : UIAssemblyPart
    {
        [SerializeField] TextMeshProUGUI partNames;
        [SerializeField] TextMeshProUGUI weaponTypeText;
        [SerializeField] TextMeshProUGUI maxAmmoText;
        [SerializeField] TextMeshProUGUI heatText;
        [SerializeField] TextMeshProUGUI rangeText;
        [SerializeField] TextMeshProUGUI damageText;

        // Populates UI fields with old vs new weapon data for comparison
        protected override void SetTexts(PartData partData)
        {
            if (!(partData is WeaponData newWeaponData)) return;

            Unit unit = UnitSelectService.Instance.selectedUnit;
            PartType curPartType = UnitAssemblyService.Instance.curPartType;
            WeaponData oldWeaponData = (WeaponData)unit.partsData[curPartType];

            partNames.text = oldWeaponData.Name + symbol + "<br>" + newWeaponData.Name;
            weaponTypeText.text = "<b>Weapon Type:</b> " + oldWeaponData.weaponType.ToString() + symbol + newWeaponData.weaponType.ToString();
            maxAmmoText.text = "<b>Max Ammo:</b> " + SetMaxAmmo(oldWeaponData) + symbol + SetMaxAmmo(newWeaponData);
            heatText.text = "<b>Heat Cost:</b> " + SetHeatCost(oldWeaponData) + symbol + SetHeatCost(newWeaponData);
            rangeText.text = "<b>Shooting Range:</b> " + oldWeaponData.Range + symbol + newWeaponData.Range;
            damageText.text = "<b>Base Damage:</b> " + oldWeaponData.Damage + symbol + newWeaponData.Damage;
        }

        // Returns heat cost as string, or 'None' if zero
        private string SetHeatCost(WeaponData weaponData)
        {
            return weaponData.HeatCost == 0 ? "None" : weaponData.HeatCost.ToString();
        }

        // Returns max ammo as string, or 'Unlimited' if zero
        private string SetMaxAmmo(WeaponData weaponData)
        {
            return weaponData.MaxAmmo == 0 ? "Unlimited" : weaponData.MaxAmmo.ToString();
        }
    }
}
