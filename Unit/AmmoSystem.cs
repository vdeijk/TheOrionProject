using System;
using UnityEngine;
using System.Collections;

namespace TurnBasedStrategy
{
    // Manages ammo for a unit's weapon
    public class AmmoSystem : MonoBehaviour
    {
        public Unit unit;
        public int ammo { get; protected set; }
        [field: SerializeField] public PartType partType { get; private set; }
        public static event EventHandler OnValueUpdated;

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void Start()
        {
            SetMax();
        }

        public void SetMax()
        {
            WeaponData weaponData = (WeaponData)unit.partsData[partType];
            ammo = weaponData.MaxAmmo;
        }

        // Spends one ammo if weapon type uses ammo
        public void SpendAmmo()
        {
            WeaponData weaponData = (WeaponData)unit.partsData[partType];
            switch (weaponData.weaponType)
            {
                case WeaponType.Laser:
                    break;
                default:
                    ammo = Mathf.Max(ammo -= 1, 0);
                    break;
            }
            OnValueUpdated?.Invoke(this, EventArgs.Empty);
        }

        // Checks if the weapon has enough ammo for the given action
        public bool HasAmmo(BaseAction baseAction)
        {
            if (!(baseAction is ShootAction)) return true;
            WeaponData weaponData = (WeaponData)unit.partsData[partType];
            switch (weaponData.weaponType)
            {
                case WeaponType.Bullet:
                case WeaponType.Laser:
                    return true;
                default:
                    return ammo - 1 >= 0;
            }
        }

        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            if (MenuChangeService.Instance.curMenu == MenuType.Main)
            {
                SetMax();
            }
        }
    }
}
