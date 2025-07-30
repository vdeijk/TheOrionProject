
namespace TurnBasedStrategy
{
    public readonly struct UnitTypeEffectiveness
    {
        public UnitType Self { get; }
        public UnitType StrongAgainst { get; }
        public UnitType WeakAgainst { get; }

        public UnitTypeEffectiveness(UnitType self, UnitType strongAgainst, UnitType weakAgainst)
        {
            Self = self;
            StrongAgainst = strongAgainst;
            WeakAgainst = weakAgainst;
        }
    }
}