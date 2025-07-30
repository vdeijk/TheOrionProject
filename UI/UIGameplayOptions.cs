using System.Linq;
using UnityEngine;
using TMPro;

namespace TurnBasedStrategy
{
    public class UIGameplayOptions : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown camInvertDropdown;

        private void Start()
        {
            SetupCamInvertDropdown();
        }

        private void SetupCamInvertDropdown()
        {
            camInvertDropdown.ClearOptions();
            var options = new[] { "Normal", "Inverted" };
            camInvertDropdown.AddOptions(options.ToList());

            var settings = SaveSettingsManager.Instance.LoadSettings();
            bool isInverted = false;
            if (settings != null && settings.GameplaySettings != null)
            {
                isInverted = settings.GameplaySettings.CameraInvert;
            }

            camInvertDropdown.value = isInverted ? 1 : 0;
            camInvertDropdown.RefreshShownValue();

            camInvertDropdown.onValueChanged.RemoveAllListeners();
            camInvertDropdown.onValueChanged.AddListener(OnCamInvertDropdownChanged);

            SetCamInvert(isInverted);
        }

        public void SetCamInvert(bool invert)
        {
            UIOptionsService.Instance.SetCamInvert(invert);
        }

        private void OnCamInvertDropdownChanged(int index)
        {
            SetCamInvert(index == 1);
        }
    }
}