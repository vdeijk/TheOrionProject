using System;
using UnityEngine;
using System.Collections;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Manages ammo for a unit's weapon
    public class UnitAmmoController : MonoBehaviour
    {
        public UnitSingleController unit;
        public int ammo { get; protected set; }
        [field: SerializeField] public PartType partType { get; private set; }
        public static event EventHandler OnValueUpdated;

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        private void OnEnable()
        {
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void Start()
        {
            SetMax();
        }

        public void SetMax()
        {
            WeaponSO weaponData = (WeaponSO)unit.Data.PartsData[partType];
            ammo = weaponData.MaxAmmo;
        }

        // Spends one ammo if weapon type uses ammo
        public void SpendAmmo()
        {
            WeaponSO weaponData = (WeaponSO)unit.Data.PartsData[partType];
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
        public bool HasAmmo(BaseActionData baseActionData)
        {
            if (!(baseActionData is ShootActionData)) return true;

            WeaponSO weaponData = (WeaponSO)unit.Data.PartsData[partType];
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
            if (MenuChangeMonobService.Instance.curMenu == MenuType.Main)
            {
                SetMax();
            }
        }
    }
}
