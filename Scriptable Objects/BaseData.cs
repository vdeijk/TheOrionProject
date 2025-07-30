using UnityEngine;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class BaseData : PartData
    {
        public int MaxHealth;
        public int Range;
        public UnitType unitType = UnitType.Ground;
        public int MaxArmor;

        public float MoveSpeed;
        public float RotateSpeed;
    }
}
