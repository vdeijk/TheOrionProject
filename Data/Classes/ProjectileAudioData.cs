using UnityEngine;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy
{
    [System.Serializable]
    public class ProjectileAudioData
    {
        [field: SerializeField] public Transform AudioPrefab { get; private set; }
        [field: SerializeField] public ProjectileController Controller { get; private set; }
    }
}
