using System;
using System.Collections.Generic;

namespace TurnBasedStrategy.Data
{
    [Serializable]
    public class InventoryState
    {
        public Dictionary<PartType, List<PartSO>> playerParts; // All parts owned by the player
    }
}