using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    public class SaveDataManager
    {
        private static SaveDataManager _instance;
        private string filePath;

        public GameSaveState gameSaveData { get; private set; }

        public static SaveDataManager Instance => _instance ??= new SaveDataManager();

        public void Init()
        {
            // Save file path is set to persistent data location for cross-platform compatibility
            filePath = Path.Combine(Application.persistentDataPath, "GameData.json");
        }

        // Initializes a new campaign and saves initial game data
        public void StartNewCampaign()
        {
            gameSaveData = new GameSaveState();
            gameSaveData.playerProgress = LevelManager.Instance.InitPlayerProgress();
            gameSaveData.mechs = SpawnAllyService.Instance.InitMechs();
            gameSaveData.economy = ScrapManager.Instance.InitEconomy();
            gameSaveData.inventory = PartsManager.Instance.InitInventory();
            SaveGameData(gameSaveData);
        }

        // Saves current game state to disk
        public void SaveGameData()
        {
            gameSaveData = new GameSaveState();

            gameSaveData.playerProgress = LevelManager.Instance.GetPlayerProgress();
            gameSaveData.mechs = UnitCategoryService.Instance.GetPlayerUnitsData();
            gameSaveData.economy = ScrapManager.Instance.GetEconomy();
            gameSaveData.inventory = PartsManager.Instance.GetInventory();

            SaveGameData(gameSaveData);
        }

        // Loads game data from disk and updates managers; returns null if not found or error
        public GameData LoadGameData()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    GameSaveState gameSaveData = JsonUtility.FromJson<GameSaveState>(json);

                    // Update managers with loaded data
                    ScrapManager.Instance.SetEconomy(gameSaveData.economy);
                    PartsManager.Instance.SetInventory(gameSaveData.inventory);
                    LevelManager.Instance.SetPlayerProgress(gameSaveData.playerProgress);
                    SpawnAllyService.Instance.SetMechs(gameSaveData.mechs);
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
        private void SaveGameData(GameSaveState gameSaveData)
        {
            try
            {
                string json = JsonUtility.ToJson(gameSaveData, true);
                File.WriteAllText(filePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game data: {e.Message}");
            }
        }
    }
}