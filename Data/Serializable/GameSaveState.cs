using System;
using System.Collections.Generic;

namespace TurnBasedStrategy.Data
{
    // Main game data container for saving/loading player progress and state

    [Serializable]
    public class GameSaveState
    {
        public bool haveTutorialsBeenViwed;
        public PlayerProgressState playerProgress;
        public List<MechsState> mechs;
        public EconomyState economy;
        public InventoryState inventory;
    }
}