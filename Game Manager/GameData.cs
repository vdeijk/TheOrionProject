using System.Collections.Generic;
using System;

namespace TurnBasedStrategy
{
    [Serializable]
    public class PlayerProgress
    {
        public int level; // Tracks player's current level
    }

    [Serializable]
    public class Mech
    {
        public Dictionary<PartType, PartData> mechData; // Stores part data for a single mech
    }

    [Serializable]
    public class Economy
    {
        public float scrap; // Player's currency/resource
    }

    [Serializable]
    public class Inventory
    {
        public Dictionary<PartType, List<PartData>> playerParts; // All parts owned by the player
    }

    // Main game data container for saving/loading player progress and state
    [Serializable]
    public class GameData
    {
        public bool haveTutorialsBeenViwed;
        public PlayerProgress playerProgress;
        public List<Mech> mechs;
        public Economy economy;
        public Inventory inventory;
    }
}