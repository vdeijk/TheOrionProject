using UnityEngine;

namespace TurnBasedStrategy
{
    [System.Serializable]
    public class UnitMovementData
    {
        [field: SerializeField] public float HoverHeightOffset { get; private set; } = 2f;
        [field: SerializeField] public float AirHeightOffset = 4f;
        [field: SerializeField] public Transform Faction0;
    }
}
