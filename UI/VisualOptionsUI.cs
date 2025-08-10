using System.Linq;
using UnityEngine;
using TMPro;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class VisualOptionsUI : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown displayModeDropdown;
        [SerializeField] private TMP_Dropdown graphicsQualityDropdown;

        private Resolution[] availableResolutions;

        private void Start()
        {
            var settings = SaveSettingsManager.Instance.LoadSettings();
            SetupResolutionDropdown(settings);
            SetupDisplayModeDropdown(settings);
            SetupGraphicsQualityDropdown(settings);
        }

        public void SetScreenRes(int width, int height)
        {
            OptionsUIMonobService.Instance.SetScreenRes(width, height);
        }

        public void SetDisplayMode(int mode)
        {
            OptionsUIMonobService.Instance.SetDisplayMode(mode);
        }

        public void SetGraphicsQual(int qualityIndex)
        {
            OptionsUIMonobService.Instance.SetGraphicsQual(qualityIndex);
        }

        private void SetupResolutionDropdown(SettingsData settings = null)
        {
            availableResolutions = Screen.resolutions.Distinct().ToArray();
            resolutionDropdown.ClearOptions();

            var options = availableResolutions
                .Select(res => $"{res.width} x {res.height} @{res.refreshRate}Hz")
                .ToList();

            resolutionDropdown.AddOptions(options);

            int currentIndex = 0;
            if (settings != null && settings.VisualSettings != null)
            {
                var savedRes = settings.VisualSettings.ScreenResolution;
                currentIndex = System.Array.FindIndex(availableResolutions, r =>
                    r.width == savedRes.width && r.height == savedRes.height);
            }
            else
            {
                currentIndex = System.Array.FindIndex(availableResolutions, r =>
                    r.width == Screen.currentResolution.width &&
                    r.height == Screen.currentResolution.height);
            }

            resolutionDropdown.value = currentIndex >= 0 ? currentIndex : 0;
            resolutionDropdown.RefreshShownValue();

            resolutionDropdown.onValueChanged.RemoveAllListeners();
            resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);

            if (currentIndex >= 0)
            {
                var res = availableResolutions[currentIndex];
                SetScreenRes(res.width, res.height);
            }
        }

        private void SetupDisplayModeDropdown(SettingsData settings = null)
        {
            displayModeDropdown.ClearOptions();
            var options = new[] { "Windowed", "Exclusive Fullscreen", "Fullscreen Window" };
            displayModeDropdown.AddOptions(options.ToList());

            int currentMode = 0;
            if (settings != null && settings.VisualSettings != null)
            {
                currentMode = settings.VisualSettings.Fullscreen
                    ? (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen ? 1 : 2)
                    : 0;
            }
            else
            {
                currentMode = Screen.fullScreenMode switch
                {
                    FullScreenMode.Windowed => 0,
                    FullScreenMode.ExclusiveFullScreen => 1,
                    FullScreenMode.FullScreenWindow => 2,
                    _ => 0
                };
            }

            displayModeDropdown.value = currentMode;
            displayModeDropdown.RefreshShownValue();

            displayModeDropdown.onValueChanged.RemoveAllListeners();
            displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);

            SetDisplayMode(currentMode);
        }

        private void SetupGraphicsQualityDropdown(SettingsData settings = null)
        {
            graphicsQualityDropdown.ClearOptions();
            var options = QualitySettings.names.ToList();
            graphicsQualityDropdown.AddOptions(options);

            int qualityLevel = settings != null && settings.VisualSettings != null
                ? settings.VisualSettings.QualityLevel
                : QualitySettings.GetQualityLevel();

            graphicsQualityDropdown.value = qualityLevel;
            graphicsQualityDropdown.RefreshShownValue();

            graphicsQualityDropdown.onValueChanged.RemoveAllListeners();
            graphicsQualityDropdown.onValueChanged.AddListener(SetGraphicsQual);

            SetGraphicsQual(qualityLevel);
        }

        private void OnResolutionDropdownChanged(int index)
        {
            var res = availableResolutions[index];
            SetScreenRes(res.width, res.height);
        }
    }
}