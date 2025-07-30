using UnityEngine;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class WeaponData : PartData
    {
        public int MaxAmmo;
        public int Range;
        public int Damage;
        public int HeatCost;
        public WeaponType weaponType;

        public Transform projectilePrefab;
        public Transform hitPrefab;
        public AudioClip shootClip;
        public AudioClip hitClip;
    }
}
