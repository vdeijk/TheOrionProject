using UnityEngine;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class GameDurations : ScriptableObject
    {
        [field: SerializeField] public float unitSpawnDuration { get; private set; } = 1.8f;
        [field: SerializeField] public float cameraBlendDuration { get; private set; } = 0.9f;
        [field: SerializeField] public float materialFadeDuration { get; private set; } = 0.3f;
        [field: SerializeField] public float uiTransitionuration { get; private set; } = 0.3f;
        [field: SerializeField] public float circuitryFlickerDuration { get; private set; } = 1.2f;
        [field: SerializeField] public float turnUIVisibility { get; private set; } = 1.8f;
        [field: SerializeField] public float victoryDefeatDelay { get; private set; } = 1.8f;
        [field: SerializeField] public float actionDuration { get; private set; } = 1.8f;
        [field: SerializeField] public float explosionDuration { get; private set; } = 1f;
    }
}