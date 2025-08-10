using UnityEngine;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy
{
    [System.Serializable]
    public class ProjectileHitData
    {
        [field: SerializeField] public Transform BulletHitPrefab { get; private set; }
        [field: SerializeField] public Transform MissiletHitPrefab { get; private set; }
        [field: SerializeField] public Transform BeamHitPrefab { get; private set; }
        [field: SerializeField] public ProjectileController Controller { get; private set; }
    }
}