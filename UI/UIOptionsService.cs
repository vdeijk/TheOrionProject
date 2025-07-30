using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
    public class UIOptionsService : Singleton<UIOptionsService>
    {
        public bool cameraInvert { get; private set; } = false;

        [SerializeField] private CanvasScaler[] canvasScalers;
        [SerializeField] private AudioMixer audioMixer;

        protected override void Awake()
        {
            Instance = SetSingleton();

            canvasScalers = (CanvasScaler[])FindObjectsByType(typeof(CanvasScaler), FindObjectsSortMode.None);
        }

        public void Save()
        {
            var settingsData = BuildSettingsDataFromUI();
            SaveSettingsManager.Instance.SaveSettings(settingsData);
        }

        public void SetScreenRes(int width, int height)
        {
            Screen.SetResolution(width, height, true);
        }

        public void SetDisplayMode(int mode)
        {
            switch (mode)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
            }
        }

        public void SetGraphicsQual(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex, true);
        }


        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat("Music", value);
        }

        public void SetSFXVolume(float value)
        {
            audioMixer.SetFloat("SFX", value);
        }

        public void SetUIVolume(float value)
        {
            audioMixer.SetFloat("UI", value);
        }

        public void SetCamInvert(bool invert)
        {
            cameraInvert = invert;
        }

        private SettingsData BuildSettingsDataFromUI()
        {
            var settingsData = new SettingsData
            {
                AudioSettings = new AudioSettings
                {
                    MusicVolume = GetMusicVolumeFromMixer(),
                    SFXVolume = GetSFXVolumeFromMixer(),
                    UIVolume = GetUIVolumeFromMixer()
                },
                VisualSettings = new VisualSettings
                {
                    ScreenResolution = Screen.currentResolution,
                    Fullscreen = Screen.fullScreen,
                    QualityLevel = QualitySettings.GetQualityLevel()
                },
                UISettings = new UISettings
                {
                    UIScale = canvasScalers != null ? canvasScalers[0].scaleFactor : 1.0f
                },
                GameplaySettings = new GameplaySettings
                {
                    CameraInvert = cameraInvert
                }
            };
            return settingsData;
        }

        private float GetMusicVolumeFromMixer()
        {
            audioMixer.GetFloat("Music", out float dB);
            return Mathf.InverseLerp(-80f, 0f, dB);
        }

        private float GetSFXVolumeFromMixer()
        {
            audioMixer.GetFloat("SFX", out float dB);
            return Mathf.InverseLerp(-80f, 0f, dB);
        }

        private float GetUIVolumeFromMixer()
        {
            audioMixer.GetFloat("UI", out float dB);
            return Mathf.InverseLerp(-80f, 0f, dB);
        }
    }
}