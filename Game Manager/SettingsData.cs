using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    [Serializable]
    public class AudioSettings
    {
        public float MusicVolume = 1.0f;
        public float SFXVolume = 1.0f;
        public float UIVolume = 1.0f;
    }

    [Serializable]
    public class VisualSettings
    {
        public Resolution ScreenResolution = new Resolution();
        public bool Fullscreen = true;
        public int QualityLevel = 2;
    }

    [Serializable]
    public class UISettings
    {
        public float UIScale = 1.0f;
    }

    [Serializable]
    public class GameplaySettings
    {
        public bool CameraInvert = false;
    }

    // Container for all player settings, used for saving/loading preferences
    [Serializable]
    public class SettingsData
    {
        public AudioSettings AudioSettings;
        public VisualSettings VisualSettings;
        public UISettings UISettings;
        public GameplaySettings GameplaySettings;
    }
}