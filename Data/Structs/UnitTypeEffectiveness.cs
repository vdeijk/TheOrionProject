
namespace TurnBasedStrategy
{
    public readonly struct UnitEffectivenessSettings
    {
        public UnitType Self { get; }
        public UnitType StrongAgainst { get; }
        public UnitType WeakAgainst { get; }

        public UnitEffectivenessSettings(UnitType self, UnitType strongAgainst, UnitType weakAgainst)
        {
            Self = self;
            StrongAgainst = strongAgainst;
            WeakAgainst = weakAgainst;
        }
    }
}