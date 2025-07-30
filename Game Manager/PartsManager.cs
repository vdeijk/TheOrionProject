using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class PartsManager : Singleton<PartsManager>
    {
        public Dictionary<PartType, List<PartData>> allParts { get; private set; }
        public Dictionary<PartType, List<PartData>> playerParts { get; private set; }
        public Dictionary<PartType, List<PartData>> enemyParts { get; private set; }
        [field: SerializeField] public PartData starterBase { get; private set; }
        [field: SerializeField] public PartData starterTorso { get; private set; }
        [field: SerializeField] public PartData starterWeapon { get; private set; }

        public static event EventHandler OnPartAdded;
        public static event EventHandler OnPartRemoved;

        protected override void Awake()
        {
            Instance= SetSingleton();

            // Load all parts and initialize enemy parts
            enemyParts = allParts = LoadParts();

            // Initialize player parts as empty lists for each type
            playerParts = new Dictionary<PartType, List<PartData>>();

            // Give player two copies of each part at start
            foreach (var kvp in allParts)
            {
                playerParts[kvp.Key] = new List<PartData>();
                foreach (var part in kvp.Value)
                {
                    playerParts[kvp.Key].Add(part);
                    playerParts[kvp.Key].Add(part);
                }
            }
        }

        // Adds a part to the player's inventory, handling weapon types specially
        public void AddPlayerPart(PartData partData, PartType partType)
        {
            if (partType == PartType.WeaponPrimary || partType == PartType.WeaponSecondary)
            {
                playerParts[PartType.WeaponPrimary].Add(partData);
                playerParts[PartType.WeaponSecondary].Add(partData);
            }
            else
            {
                playerParts[partType].Add(partData);
            }

            OnPartAdded?.Invoke(this, EventArgs.Empty);
        }

        // Removes a part from the player's inventory, handling weapon types specially
        public void RemovePlayerPart(PartData partData, PartType partType)
        {
            if (partType == PartType.WeaponPrimary || partType == PartType.WeaponSecondary)
            {
                playerParts[PartType.WeaponPrimary].Remove(partData);
                playerParts[PartType.WeaponSecondary].Remove(partData);
            }
            else
            {
                playerParts[partType].Remove(partData);
            }

            OnPartRemoved?.Invoke(this, EventArgs.Empty);
        }

        // Initializes inventory with starter parts for a new campaign
        public Inventory InitInventory()
        {
            Inventory inventory = new Inventory();
            inventory.playerParts = playerParts;
            playerParts = inventory.playerParts = new Dictionary<PartType, List<PartData>>
                     {
                         { PartType.Base, new List<PartData>() },
                         { PartType.Torso, new List<PartData>() },
                         { PartType.WeaponPrimary, new List<PartData>() },
                         { PartType.WeaponSecondary, new List<PartData>() }
                     };

            playerParts[PartType.Base].Add(starterBase);
            playerParts[PartType.Torso].Add(starterTorso);
            playerParts[PartType.WeaponPrimary].Add(starterWeapon);
            playerParts[PartType.WeaponSecondary].Add(starterWeapon);

            return inventory;
        }

        public void SetInventory(Inventory inventory)
        {
            playerParts = inventory.playerParts;
        }

        public Inventory GetInventory()
        {
            Inventory inventory = new Inventory();
            inventory.playerParts = playerParts;
            return inventory;
        }

        // Returns a random enemy part of the given type
        public PartData GetEnemyPart(PartType partType)
        {
            if (!enemyParts.ContainsKey(partType) || enemyParts[partType].Count == 0) return null;

            return enemyParts[partType][UnityEngine.Random.Range(0, enemyParts[partType].Count)];
        }

        // Returns a random part of the given type from all available parts
        public PartData GetRandomPart(PartType partType)
        {
            if (!allParts.ContainsKey(partType) || allParts[partType].Count == 0) return null;

            return allParts[partType][UnityEngine.Random.Range(0, allParts[partType].Count)];
        }

        // Loads all part assets from Resources folders
        private Dictionary<PartType, List<PartData>> LoadParts()
        {
            return new Dictionary<PartType, List<PartData>>
            {
                { PartType.Base, new List<PartData>(Resources.LoadAll<PartData>("Part Data/Bases")) },
                { PartType.Torso, new List<PartData>(Resources.LoadAll<PartData>("Part Data/Torsos")) },
                { PartType.WeaponPrimary, new List<PartData>(Resources.LoadAll<PartData>("Part Data/Weapons")) },
                { PartType.WeaponSecondary, new List<PartData>(Resources.LoadAll<PartData>("Part Data/Weapons")) }
            };
        }
    }
}

