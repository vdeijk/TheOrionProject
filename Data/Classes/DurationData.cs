using UnityEngine;

namespace TurnBasedStrategy.Data
{
    public class DurationData
    {
        public static readonly DurationData Instance = new DurationData();

        public float UnitSpawnDuration { get; } = 1.8f;
        public float CameraBlendDuration { get; } = 0.9f;
        public float MaterialFadeDuration { get; } = 0.3f;
        public float UiTransitionuration { get; } = 0.3f;
        public float CircuitryFlickerDuration { get; } = 1.2f;
        public float TurnUIVisibility { get; } = 1.8f;
        public float VictoryDefeatDelay { get; } = 1.8f;
        public float ActionDuration { get; } = 1.8f;
        public float ExplosionDuration { get; } = 1f;
    }
}