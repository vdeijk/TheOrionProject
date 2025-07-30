using System.IO;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class SaveSettingsManager : Singleton<SaveSettingsManager>
    {
        public SettingsData settingsData { get; private set; }

        private string filePath;
        protected override void Awake()
        {
            Instance = SetSingleton();
            // Store settings in persistent data path for cross-platform compatibility
            filePath = Path.Combine(Application.persistentDataPath, "PlayerSettings.json");
            LoadSettings(); // Load settings at startup
        }

        // Saves the provided settings data to disk
        public void SaveSettings(SettingsData settings)
        {
            settingsData = settings;
            try
            {
                string json = JsonUtility.ToJson(settingsData, true);
                File.WriteAllText(filePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save player settings: {e.Message}");
            }
        }

        // Loads settings from disk, or creates new defaults if not found or error
        public SettingsData LoadSettings()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    settingsData = JsonUtility.FromJson<SettingsData>(json);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load player settings: {e.Message}");
                    settingsData = new SettingsData();
                }
            }
            else
            {
                settingsData = new SettingsData();
            }
            return settingsData;
        }
    }
}