using System.Collections.Generic;

namespace TurnBasedStrategy
{
    public class UnitHealthData
    {
        public List<UnitEffectivenessSettings> EffectivenessChart { get; set; }
        public float DamageMultiplierPlus { get; set; }
        public float DamageMultiplierMinus { get; set; }
        public float ForestEvasionChance { get; set; }

        public UnitHealthData(float damageMultiplierPlus, float damageMultiplierMinus, float forestEvasionChance)
        {
            EffectivenessChart = new()
            {
                new UnitEffectivenessSettings(UnitType.Ground, UnitType.Air, UnitType.Hover),
                new UnitEffectivenessSettings(UnitType.Air, UnitType.Hover, UnitType.Ground),
                new UnitEffectivenessSettings(UnitType.Hover, UnitType.Ground, UnitType.Air)
            };
            DamageMultiplierPlus = damageMultiplierPlus;
            DamageMultiplierMinus = damageMultiplierMinus;
            ForestEvasionChance = forestEvasionChance;
        }
    }
}
