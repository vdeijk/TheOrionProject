using UnityEngine;

namespace TurnBasedStrategy.Data
{
    public class WeaponSO : PartSO
    {
        public int MaxAmmo;
        public int Damage;
        public int HeatCost;
        public WeaponType weaponType;

        public Transform projectilePrefab;
        public Transform hitPrefab;
        public AudioClip shootClip;
        public AudioClip hitClip;
    }
}
