using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    public class PartsManager
    {
        private static PartsManager _instance;
        
        public GameController _gameController { get; private set; }
        public Dictionary<PartType, List<PartSO>> allParts { get; private set; }
        public Dictionary<PartType, List<PartSO>> playerParts { get; private set; }
        public Dictionary<PartType, List<PartSO>> enemyParts { get; private set; }

        public static PartsManager Instance => _instance ??= new PartsManager();

        public static event EventHandler OnPartAdded;
        public static event EventHandler OnPartRemoved;

        public void Init(GameController gameController)
        {
            _gameController = gameController;

            // Load all parts and initialize enemy parts
            enemyParts = allParts = LoadParts();

            // Initialize player parts as empty lists for each type
            playerParts = new Dictionary<PartType, List<PartSO>>();

            // Give player two copies of each part at start
            foreach (var kvp in allParts)
            {
                playerParts[kvp.Key] = new List<PartSO>();
                foreach (var part in kvp.Value)
                {
                    playerParts[kvp.Key].Add(part);
                    playerParts[kvp.Key].Add(part);
                }
            }
        }

        // Adds a part to the player's inventory, handling weapon types specially
        public void AddPlayerPart(PartSO partData, PartType partType)
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
        public void RemovePlayerPart(PartSO partData, PartType partType)
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
        public InventoryState InitInventory()
        {
            InventoryState inventory = new InventoryState();
            inventory.playerParts = playerParts;
            playerParts = inventory.playerParts = new Dictionary<PartType, List<PartSO>>
                     {
                         { PartType.Base, new List<PartSO>() },
                         { PartType.Torso, new List<PartSO>() },
                         { PartType.WeaponPrimary, new List<PartSO>() },
                         { PartType.WeaponSecondary, new List<PartSO>() }
                     };

            playerParts[PartType.Base].Add(_gameController.starterBase);
            playerParts[PartType.Torso].Add(_gameController.starterTorso);
            playerParts[PartType.WeaponPrimary].Add(_gameController.starterWeapon);
            playerParts[PartType.WeaponSecondary].Add(_gameController.starterWeapon);

            return inventory;
        }

        public void SetInventory(InventoryState inventory)
        {
            playerParts = inventory.playerParts;
        }

        public InventoryState GetInventory()
        {
            InventoryState inventory = new InventoryState();
            inventory.playerParts = playerParts;
            return inventory;
        }

        // Returns a random enemy part of the given type
        public PartSO GetEnemyPart(PartType partType)
        {
            if (!enemyParts.ContainsKey(partType) || enemyParts[partType].Count == 0) return null;

            return enemyParts[partType][UnityEngine.Random.Range(0, enemyParts[partType].Count)];
        }

        // Returns a random part of the given type from all available parts
        public PartSO GetRandomPart(PartType partType)
        {
            if (!allParts.ContainsKey(partType) || allParts[partType].Count == 0) return null;

            return allParts[partType][UnityEngine.Random.Range(0, allParts[partType].Count)];
        }

        // Loads all part assets from Resources folders
        private Dictionary<PartType, List<PartSO>> LoadParts()
        {
            return new Dictionary<PartType, List<PartSO>>
            {
                { PartType.Base, new List<PartSO>(Resources.LoadAll<PartSO>("Part Data/Bases")) },
                { PartType.Torso, new List<PartSO>(Resources.LoadAll<PartSO>("Part Data/Torsos")) },
                { PartType.WeaponPrimary, new List<PartSO>(Resources.LoadAll<PartSO>("Part Data/Weapons")) },
                { PartType.WeaponSecondary, new List<PartSO>(Resources.LoadAll<PartSO>("Part Data/Weapons")) }
            };
        }
    }
}

