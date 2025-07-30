using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class SaveDataManager : Singleton<SaveDataManager>
    {
        public GameData gameData { get; private set; }

        private string filePath;

        protected override void Awake()
        {
            Instance = SetSingleton();
            // Save file path is set to persistent data location for cross-platform compatibility
            filePath = Path.Combine(Application.persistentDataPath, "GameData.json");
        }

        // Initializes a new campaign and saves initial game data
        public void StartNewCampaign()
        {
            gameData = new GameData();
            gameData.playerProgress = LevelManager.Instance.InitPlayerProgress();
            gameData.mechs = UnitSpawnService.Instance.InitPlayerMechs();
            gameData.economy = ScrapManager.Instance.InitEconomy();
            gameData.inventory = PartsManager.Instance.InitInventory();
            SaveGameData(gameData);
        }

        // Saves current game state to disk
        public void SaveGameData()
        {
            gameData = new GameData();

            gameData.playerProgress = LevelManager.Instance.GetPlayerProgress();
            gameData.mechs = UnitCategoryService.Instance.GetPlayerMechs();
            gameData.economy = ScrapManager.Instance.GetEconomy();
            gameData.inventory = PartsManager.Instance.GetInventory();

            SaveGameData(gameData);
        }

        // Loads game data from disk and updates managers; returns null if not found or error
        public GameData LoadGameData()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    GameData gameData = JsonUtility.FromJson<GameData>(json);

                    // Update managers with loaded data
                    ScrapManager.Instance.SetEconomy(gameData.economy);
                    PartsManager.Instance.SetInventory(gameData.inventory);
                    LevelManager.Instance.SetPlayerProgress(gameData.playerProgress);
                    UnitSpawnService.Instance.SetPlayerMechs(gameData.mechs);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load game data: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning("No saved data found. Creating new game.");
            }

            return null;
        }

        // Serializes and writes game data to disk
        private void SaveGameData(GameData gameData)
        {
            try
            {
                string json = JsonUtility.ToJson(gameData, true);
                File.WriteAllText(filePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game data: {e.Message}");
            }
        }
    }
}