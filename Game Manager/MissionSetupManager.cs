using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class MissionSetupManager : Singleton<MissionSetupManager>
    {
        [SerializeField] MissionData missionData;
        [SerializeField] int AverageOpposition = 6;
        [SerializeField] int MinEnemies = 1;
        [SerializeField] int MaxEnemies = 20;
        [SerializeField] float LevelScalingFactor = 0.2f;

        // Nullable types allow for missions without a specific unit/weapon/damage type
        public UnitType? unitType { get; private set; }
        public WeaponType? weaponType { get; private set; }
        public DamageType? damageType { get; private set; }
        public string missionTip { get; private set; }
        public int difficultyLevel { get; private set; }
        public int numberOfEnemies { get; private set; }

        // Generates randomized mission settings based on current level and mission data
        public void GenerateSettings()
        {
            missionData.InitializeMissionTips();
            missionData.InitializeTextVariants();

            unitType = GenerateUnitType();
            weaponType = GenerateWeaponType();
            damageType = GenerateDamageType();
            numberOfEnemies = GenerateNumberOfEnemies();
            difficultyLevel = SetDifficulty();
            missionTip = GenerateMissionTip();
        }

        // Difficulty is determined by the number of enemies
        private int SetDifficulty()
        {
            switch (numberOfEnemies)
            {
                case 1:
                case 2:
                    return 1;
                case 3:
                case 4:
                    return 2;
                case 5:
                case 6:
                case 7:
                    return 3;
                case 8:
                case 9:
                case 10:
                    return 4;
                default:
                    return 5;
            }
        }

        // Calculates enemy count based on level and random factor
        private int GenerateNumberOfEnemies()
        {
            float levelModifier = LevelManager.Instance.level * LevelScalingFactor;
            float scaledAverage = AverageOpposition * levelModifier;

            float randomFactor = UnityEngine.Random.Range(0.7f, 1.3f);
            int result = Mathf.RoundToInt(scaledAverage * randomFactor);

            return Mathf.Clamp(result, MinEnemies, MaxEnemies);
        }

        // 25% chance to have no specific unit type
        private UnitType? GenerateUnitType()
        {
            if (UnityEngine.Random.value < 0.25f)
            {
                return null;
            }

            Array values = Enum.GetValues(typeof(UnitType));
            return (UnitType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }

        // 25% chance to have no specific weapon type
        private WeaponType? GenerateWeaponType()
        {
            if (UnityEngine.Random.value < 0.25f)
            {
                return null;
            }

            Array values = Enum.GetValues(typeof(WeaponType));
            return (WeaponType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }

        // 25% chance to have no specific damage type
        private DamageType? GenerateDamageType()
        {
            if (UnityEngine.Random.value < 0.25f)
            {
                return null;
            }

            Array values = Enum.GetValues(typeof(DamageType));
            return (DamageType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }

        // Selects a random mission tip from the available list
        private string GenerateMissionTip()
        {
            if (missionData == null || missionData.missionTips == null) return "";

            int count = missionData.missionTips.Count;

            if (count <= 0) return "";

            int tipNumber = UnityEngine.Random.Range(0, count);
            return missionData.missionTips[tipNumber];
        }
    }
}
