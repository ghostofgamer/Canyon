﻿using UnityEngine;
using System.Collections;

namespace ClawbearGames
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Audio Source References")]
        [SerializeField] private AudioSource soundSource = null;
        [SerializeField] private AudioSource musicSource = null;


        [Header("Audio Clips References")]
        [SerializeField] private AudioClip button = null;
        public AudioClip Button => button;

        [SerializeField] private AudioClip collectCoinItem = null;
        public AudioClip CollectCoinItem => collectCoinItem;
        [SerializeField] private AudioClip builtBridge = null;
        public AudioClip BuiltBridge => builtBridge;
        [SerializeField] private AudioClip placedCenter = null;
        public AudioClip PlacedCenter => placedCenter;

        [SerializeField] private AudioClip placedBridge = null;
        public AudioClip PlacedBridge => placedBridge;



        [SerializeField] private AudioClip playerExploded = null;
        public AudioClip PlayerExploded => playerExploded;
        [SerializeField] private AudioClip playerFalldown = null;
        public AudioClip PlayerFalldown => playerFalldown;
        [SerializeField] private AudioClip levelCompleted = null;
        public AudioClip LevelCompleted => levelCompleted;
        [SerializeField] private AudioClip levelFailed = null;
        public AudioClip LevelFailed => levelFailed;




        [SerializeField] private AudioClip rewarded = null;
        public AudioClip Rewarded => rewarded;

        [SerializeField] private AudioClip tick = null;
        public AudioClip Tick => tick;

        [SerializeField] private AudioClip unlock = null;
        public AudioClip Unlock => unlock;


        private void Start()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.PPK_SOUND))
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SOUND, 1);
            if (!PlayerPrefs.HasKey(PlayerPrefsKeys.PPK_MUSIC))
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_MUSIC, 1);

            soundSource.mute = IsSoundOff();
            musicSource.mute = IsMusicOff();
        }


        /// <summary>
        /// Determine whether the sound is off.
        /// </summary>
        /// <returns></returns>
        public bool IsSoundOff()
        {
            return (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SOUND, 1) == 0);
        }

        /// <summary>
        /// Determine whether the music is off.
        /// </summary>
        /// <returns></returns>
        public bool IsMusicOff()
        {
            return (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_MUSIC, 1) == 0);
        }


        /// <summary>
        /// Play one audio clip as sound.
        /// </summary>
        /// <param name="audioClip"></param>
        public void PlaySound(AudioClip audioClip)
        {
            soundSource.PlayOneShot(audioClip);
        }


        /// <summary>
        /// Plays the given sound clip as music clip (automatically loop).
        /// The music will volume up from 0 to 0.3 with given increase volume time.
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="increaseVolumeTime"></param>
        public void PlayMusic(AudioClip audioClip, float increaseVolumeTime)
        {
            if (!IsMusicOff()) //Music is on
            {
                musicSource.clip = audioClip;
                musicSource.loop = true;
                musicSource.Play();
                StartCoroutine(CRIncreaseVolume(increaseVolumeTime));
            }
        }


        /// <summary>
        /// Coroutine increase volume from 0 to 0.3 with increasing time.
        /// </summary>
        /// <param name="increasingTime"></param>
        /// <returns></returns>
        private IEnumerator CRIncreaseVolume(float increasingTime)
        {
            musicSource.volume = 0f;
            float t = 0;
            while (t < increasingTime)
            {
                t += Time.deltaTime;
                float factor = t / increasingTime;
                musicSource.volume = Mathf.Lerp(0f, 0.3f, factor);
                yield return null;
            }
        }


        /// <summary>
        /// Stop the current music with given decrease volume time.
        /// </summary>
        /// <param name="decreaseVolumeTime"></param>
        public void StopMusic(float decreaseVolumeTime)
        {
            StartCoroutine(CRDecreaseVolume(decreaseVolumeTime));
        }


        /// <summary>
        /// Coroutine decrease volume from 0.3 to 0 with given decreasing time.
        /// </summary>
        /// <param name="decreasingTime"></param>
        /// <returns></returns>
        private IEnumerator CRDecreaseVolume(float decreasingTime)
        {
            float t = 0;
            float currentVolume = musicSource.volume;
            while (t < decreasingTime)
            {
                t += Time.deltaTime;
                float factor = t / decreasingTime;
                musicSource.volume = Mathf.Lerp(currentVolume, 0, factor);
                yield return null;
            }
            musicSource.Stop();
        }




        /// <summary>
        /// Pauses the music with given decrease volume time.
        /// </summary>
        /// <param name="decreaseVolumeTime"></param>
        public void PauseMusic(float decreaseVolumeTime)
        {
            StartCoroutine(CRPauseMusic(decreaseVolumeTime));
        }


        /// <summary>
        /// Coroutine pause the music.
        /// </summary>
        /// <param name="decreasingTime"></param>
        /// <returns></returns>
        private IEnumerator CRPauseMusic(float decreasingTime)
        {
            float t = 0;
            float currentVolume = musicSource.volume;
            while (t < decreasingTime)
            {
                t += Time.deltaTime;
                float factor = t / decreasingTime;
                musicSource.volume = Mathf.Lerp(currentVolume, 0, factor);
                yield return null;
            }

            musicSource.Pause();
            musicSource.volume = currentVolume;
        }



        /// <summary>
        /// Resumes the music with increase volume time.
        /// </summary>
        /// <param name="increaseVolumeTime"></param>
        public void ResumeMusic(float increaseVolumeTime)
        {
            StartCoroutine(CRResumeMusic(increaseVolumeTime));
        }


        /// <summary>
        /// Coroutine resume the music.
        /// </summary>
        /// <param name="increasingTime"></param>
        /// <returns></returns>
        private IEnumerator CRResumeMusic(float increasingTime)
        {
            musicSource.UnPause();
            musicSource.volume = 0f;
            float t = 0;
            while (t < increasingTime)
            {
                t += Time.deltaTime;
                float factor = t / increasingTime;
                musicSource.volume = Mathf.Lerp(0f, 0.3f, factor);
                yield return null;
            }
        }




        /// <summary>
        /// Toggles the sound mute status.
        /// </summary>
        public void ToggleSound()
        {
            if (IsSoundOff())
            {
                //Turn the sound on
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SOUND, 1);
                soundSource.mute = false;
            }
            else
            {
                //Turn the sound off
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SOUND, 0);
                soundSource.mute = true;
            }
        }

        /// <summary>
        /// Toggles the music mute status.
        /// </summary>
        public void ToggleMusic()
        {
            if (IsMusicOff())
            {
                //Turn the music on
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_MUSIC, 1);
                musicSource.mute = false;
            }
            else
            {
                //Turn the music off
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_MUSIC, 0);
                musicSource.mute = true;
            }
        }
    }
}
