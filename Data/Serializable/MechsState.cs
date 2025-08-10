using System;
using System.Collections.Generic;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    [Serializable]
    public class MechsState
    {
        public Dictionary<PartType, PartSO> mechData; // Stores part data for a single mech
    }
}
