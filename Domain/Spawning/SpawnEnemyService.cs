using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    public class SpawnEnemyService : SpawnBaseService
    {
        public SpawnEnemyData Data { get; private set; }

        private static SpawnEnemyService _instance;

        public static SpawnEnemyService Instance => _instance ??= new SpawnEnemyService();

        public void Init(SpawnEnemyData data)
        {
            Data = data;
        }

        // Spawns all enemy units
        public void SpawnEnemies()
        {
            InitMechs();
            SpawnUnits(Data);
        }

        // Initializes enemy mechs with enemy parts
        public List<MechsState> InitMechs()
        {
            Data.Mechs = new List<MechsState>();

            for (int i = 0; i < GenerateCount(); i++)
            {
                var mechData = new Dictionary<PartType, PartSO>
            {
                { PartType.Base, PartsManager.Instance.GetEnemyPart(PartType.Base) },
                { PartType.Torso, PartsManager.Instance.GetEnemyPart(PartType.Torso) },
                { PartType.WeaponPrimary, PartsManager.Instance.GetEnemyPart(PartType.WeaponPrimary) },
                { PartType.WeaponSecondary, PartsManager.Instance.GetEnemyPart(PartType.WeaponSecondary) }
            };

                Data.Mechs.Add(new MechsState { mechData = mechData });
            }

            return Data.Mechs;
        }

        // Calculates enemy count based on level and random factor
        private int GenerateCount()
        {
            float levelModifier = LevelManager.Instance.level * Data.LevelScalingFactor;
            float scaledAverage = Data.AverageOpposition * levelModifier;

            float randomFactor = Random.Range(0.7f, 1.3f);
            int result = Mathf.RoundToInt(scaledAverage * randomFactor);

            return Mathf.Clamp(result, 1, Data.Max);
        }
    }
}