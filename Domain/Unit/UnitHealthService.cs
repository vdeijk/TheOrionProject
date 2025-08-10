using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    // Provides health-related calculations and utilities for units
    public class UnitHealthService
    {
        private UnitHealthData _data = new(1.5f, 0.75f, 30f);
        private static UnitHealthService _instance;

        public static UnitHealthService Instance => _instance ??= new UnitHealthService();

        // Heals all allied units to max health and resets heat
        public void HealAllAlies()
        {
            foreach (var unit in UnitCategoryService.Instance.Data.Allies)
            {
                unit.Data.UnitMindTransform.GetComponent<UnitHealthController>().SetMax();
                unit.Data.UnitMindTransform.GetComponent<HeatSystemController>().SetMin();
            }
        }

        // Returns damage multiplier based on unit type effectiveness
        public float CalculateUnitTypeMultiplier(UnitType shooterType, UnitType targetType)
        {
            UnitEffectivenessSettings shooterData = _data.EffectivenessChart
                .FirstOrDefault(e => e.Self == shooterType);

            if (targetType == shooterData.StrongAgainst)
            {
                return _data.DamageMultiplierPlus;
            }
            else if (targetType == shooterData.WeakAgainst)
            {
                return _data.DamageMultiplierMinus;
            }

            return 1;
        }

        // Returns damage multiplier based on terrain type
        public float CalculateTileTypeMultiplier(GridSquareType gridSquareType, UnitSingleController unit)
        {
            BaseSO baseData = (BaseSO)unit.Data.PartsData[PartType.Base];
            if (baseData.UnitType == UnitType.Air) return 1;

            switch (gridSquareType)
            {
                case GridSquareType.Forest:
                    int randomNumber = Random.Range(0, 100);
                    if (randomNumber < _data.ForestEvasionChance)
                    {
                        return 0;
                    }
                    return 1;

                case GridSquareType.Rough:
                    return .7f;

                default:
                    return 1;
            }
        }

        // Returns damage multiplier based on weapon and damage type
        public float CalculateWeaponTypeMultiplier(DamageType damageType, WeaponType weaponType)
        {
            if (damageType == DamageType.Shield && weaponType == WeaponType.Laser)
            {
                return 2;
            }
            if (damageType == DamageType.Armor && weaponType == WeaponType.Missile)
            {
                return 2;
            }
            if (damageType == DamageType.Shield && weaponType == WeaponType.Missile)
            {
                return .25f;
            }
            if (damageType == DamageType.Armor && weaponType == WeaponType.Laser)
            {
                return .25f;
            }

            return 1;
        }
    }
}
