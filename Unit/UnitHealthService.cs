using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Provides health-related calculations and utilities for units
    public class UnitHealthService : Singleton<UnitHealthService>
    {
        private static readonly List<UnitTypeEffectiveness> effectivenessChart = new()
        {
            new UnitTypeEffectiveness(UnitType.Ground, UnitType.Air, UnitType.Hover),
            new UnitTypeEffectiveness(UnitType.Air, UnitType.Hover, UnitType.Ground),
            new UnitTypeEffectiveness(UnitType.Hover, UnitType.Ground, UnitType.Air)
        };

        [SerializeField] float DamageMultiplierPlus = 1.5f;
        [SerializeField] float DamageMultiplierMinus = .75f;
        [SerializeField] float forestEvasionChance = 30;

        // Heals all allied units to max health and resets heat
        public void HealAllAlies()
        {
            foreach (var unit in UnitCategoryService.Instance.allies)
            {
                unit.unitMindTransform.GetComponent<HealthSystem>().SetMax();
                unit.unitMindTransform.GetComponent<HeatSystem>().SetMin();
            }
        }

        // Returns damage multiplier based on unit type effectiveness
        public float CalculateUnitTypeMultiplier(UnitType shooterType, UnitType targetType)
        {
            UnitTypeEffectiveness shooterData = effectivenessChart
                .FirstOrDefault(e => e.Self == shooterType);

            if (targetType == shooterData.StrongAgainst)
            {
                return DamageMultiplierPlus;
            }
            else if (targetType == shooterData.WeakAgainst)
            {
                return DamageMultiplierMinus;
            }

            return 1;
        }

        // Returns damage multiplier based on terrain type
        public float CalculateTileTypeMultiplier(GridSquareType gridSquareType, Unit unit)
        {
            BaseData baseData = (BaseData)unit.partsData[PartType.Base];
            if (baseData.unitType == UnitType.Air) return 1;

            switch (gridSquareType)
            {
                case GridSquareType.Forest:
                    int randomNumber = Random.Range(0, 100);
                    if (randomNumber < forestEvasionChance)
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
