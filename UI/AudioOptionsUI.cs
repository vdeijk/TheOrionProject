using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class AudioOptionsUI : MonoBehaviour
    {
        [SerializeField] AudioMixer audioMixer;
        [SerializeField] Slider musicSlider;
        [SerializeField] Slider sfxSlider;
        [SerializeField] Slider uiSlider;
        [SerializeField] float minDb = -60f;
        [SerializeField] float maxDb = 0f;

        private void Start()
        {
            var settings = SaveSettingsManager.Instance.LoadSettings();
            if (settings != null && settings.AudioSettings != null)
            {
                float musicDb = Mathf.Lerp(minDb, maxDb, settings.AudioSettings.MusicVolume);
                float sfxDb = Mathf.Lerp(minDb, maxDb, settings.AudioSettings.SFXVolume);
                float uiDb = Mathf.Lerp(minDb, maxDb, settings.AudioSettings.UIVolume);

                audioMixer.SetFloat("Music", musicDb);
                audioMixer.SetFloat("SFX", sfxDb);
                audioMixer.SetFloat("UI", uiDb);

                musicSlider.value = settings.AudioSettings.MusicVolume;
                sfxSlider.value = settings.AudioSettings.SFXVolume;
                uiSlider.value = settings.AudioSettings.UIVolume;
            }
            else
            {
                audioMixer.SetFloat("Music", Mathf.Lerp(minDb, maxDb, 0.8f));
                audioMixer.SetFloat("SFX", Mathf.Lerp(minDb, maxDb, 0.5f));
                audioMixer.SetFloat("UI", Mathf.Lerp(minDb, maxDb, 1.0f));

                musicSlider.value = 0.8f;
                sfxSlider.value = 0.5f;
                uiSlider.value = 1.0f;
            }

            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            uiSlider.onValueChanged.AddListener(SetUIVolume);
        }

        public void SetMusicVolume(float value)
        {
            float dB = Mathf.Lerp(minDb, maxDb, Mathf.Pow(Mathf.Clamp01(value), 2f));
            OptionsUIMonobService.Instance.SetMusicVolume(dB);
        }

        public void SetSFXVolume(float value)
        {
            float dB = Mathf.Lerp(minDb, maxDb, Mathf.Pow(Mathf.Clamp01(value), 2f));
            OptionsUIMonobService.Instance.SetSFXVolume(dB);
        }

        public void SetUIVolume(float value)
        {
            float dB = Mathf.Lerp(minDb, maxDb, Mathf.Pow(Mathf.Clamp01(value), 2f));
            OptionsUIMonobService.Instance.SetUIVolume(dB);
        }
    }
}