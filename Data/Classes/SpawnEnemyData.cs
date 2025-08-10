using UnityEngine;

namespace TurnBasedStrategy
{
    [System.Serializable]
    public class SpawnEnemyData : SpawnBaseData
    {
        [field: SerializeField] public int AverageOpposition { get; private set; } = 6;
        [field: SerializeField] public float LevelScalingFactor { get; private set; } = 0.2f;
    }
}
