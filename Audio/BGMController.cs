using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace TurnBasedStrategy
{
    public class BGMController : MonoBehaviour
    {
        public delegate void PlayMusicFunction();
        PlayMusicFunction curPlayMusicFunction;

        [field: SerializeField] public List<AudioClip> bgmTracksBattle { get; private set; } = new List<AudioClip>();

        [SerializeField] float TARGET_VOLUME;
        [SerializeField] float FADE_DURATION;
        [SerializeField] AudioSource ASBattleMusic;
        [SerializeField] AudioSource ASMenuMusic;
        [SerializeField] AudioSource ASVictoryMusic;
        [SerializeField] AudioSource ASPreparationMusic;

        private enum TrackType { Battle, Menu, Victory, Preparation, None }
        private TrackType curTrackType = TrackType.None;
        private AudioSource curAS;
        private AudioClip curTrack;

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
            UnitCategoryService.OnUnitRemoved += UnitCategoryService_OnUnitRemoved;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
            UnitCategoryService.OnUnitRemoved -= UnitCategoryService_OnUnitRemoved;
        }

        private void UnitCategoryService_OnUnitRemoved(object sender, EventArgs e)
        {
            SelectTrackType();
        }

        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            SelectTrackType();
        }

        // Decides which music track to play based on game state
        private void SelectTrackType()
        {
            var unitCategoryService = UnitCategoryService.Instance;
            var curMenu = MenuChangeService.Instance.curMenu;
            bool isInMission = ControlModeManager.Instance.gameControlType == GameControlType.Mission;
            int enemyCount = unitCategoryService.enemies.Count;
            int allyCount = unitCategoryService.allies.Count;
            if (isInMission && enemyCount > 0 && allyCount > 0)
            {
                PlayMusic(TrackType.Battle);
            }
            else if (curMenu == MenuType.Preparation || curMenu == MenuType.Assemble)
            {
                PlayMusic(TrackType.Preparation);
            }
            else
            {
                PlayMusic(TrackType.Menu);
            }
        }

        private IEnumerator ChangeMusic(float durationInSeconds)
        {
            yield return new WaitForSecondsRealtime(durationInSeconds);
            curPlayMusicFunction();
        }

        private void PlayMusic(TrackType trackType)
        {
            if (curTrackType == trackType) return;

            StopCurrentTrack();

            switch (trackType)
            {
                case TrackType.Battle:
                    PlayBattleMusic();
                    break;
                case TrackType.Victory:
                    PlayVictoryMusic();
                    break;
                case TrackType.Preparation:
                    PlayPreparationMusic();
                    break;
                case TrackType.Menu:
                    PlayMenuMusic();
                    break;
                default:
                    curTrackType = TrackType.None;
                    return;
            }

            curTrackType = trackType;
        }

        private void StopCurrentTrack()
        {
            if (curAS == null) return;
            StopAllCoroutines();
            StartCoroutine(AudioFading.Instance.Fade(curAS.volume, 0, curAS, FADE_DURATION));
            curAS = null;
        }

        private void PlayVictoryMusic()
        {
            PlayAudio(ASVictoryMusic, ASVictoryMusic.clip);
        }

        private void PlayMenuMusic()
        {
            PlayAudio(ASMenuMusic, ASMenuMusic.clip);
        }

        private void PlayPreparationMusic()
        {
            PlayAudio(ASPreparationMusic, ASPreparationMusic.clip);
        }

        private void PlayBattleMusic()
        {
            curTrack = bgmTracksBattle[UnityEngine.Random.Range(0, bgmTracksBattle.Count)];
            PlayAudio(ASBattleMusic, curTrack);
            curPlayMusicFunction = PlayBattleMusic;
            StartCoroutine(ChangeMusic(curTrack.length));
            StartCoroutine(AudioFading.Instance.Fade(0, TARGET_VOLUME, ASBattleMusic, FADE_DURATION));
        }

        // Plays the given audio clip on the given source, fading in
        private void PlayAudio(AudioSource audioSource, AudioClip audioClip)
        {
            curAS = audioSource;
            audioSource.clip = audioClip;
            audioSource.volume = 0;
            audioSource.Play();
            StartCoroutine(AudioFading.Instance.Fade(0, TARGET_VOLUME, audioSource, FADE_DURATION));
        }
    }
}

