using UnityEngine;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class TorsoData : PartData
    {
        public int MaxShield;
        public int MaxHeat;
        public int SalvageBoost;
        public WeaponType Synergy;
        public int HeatVent;
    }
}
