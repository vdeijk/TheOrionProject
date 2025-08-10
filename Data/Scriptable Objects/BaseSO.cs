namespace TurnBasedStrategy.Data
{
    public class BaseSO : PartSO
    {
        public int MaxHealth { get; set; }
        public UnitType UnitType { get; set; } = UnitType.Ground;
        public int MaxArmor { get; set; }

        public float MoveSpeed { get; set; }
        public float RotateSpeed { get; set; }
    }
}
