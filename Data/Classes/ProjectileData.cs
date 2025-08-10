using UnityEngine;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy
{
    [System.Serializable]
    public class ProjectileData
    {
        [field: SerializeField] public Transform BulletPrefab { get; private set; }
        [field: SerializeField] public Transform MissileProjectilePrefab { get; private set; }
        [field: SerializeField] public Transform LaserPrefab { get; private set; }
        [field: SerializeField] public ProjectileController Controller { get; private set; }
    }
}