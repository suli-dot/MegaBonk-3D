using UnityEngine;
using System.Collections.Generic;

namespace MegabonkMobile.Utilities
{
    /// <summary>
    /// Менеджер аудио для мобильной игры
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [System.Serializable]
        public class SoundClip
        {
            public SfxType type;
            public AudioClip clip;
            [Range(0f, 1f)]
            public float volume = 1f;
        }

        [System.Serializable]
        public class MusicTrack
        {
            public MusicType type;
            public AudioClip clip;
            [Range(0f, 1f)]
            public float volume = 0.7f;
        }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Sound Clips")]
        [SerializeField] private List<SoundClip> soundClips = new List<SoundClip>();

        [Header("Music Tracks")]
        [SerializeField] private List<MusicTrack> musicTracks = new List<MusicTrack>();

        [Header("Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;

        private Dictionary<SfxType, SoundClip> soundDictionary = new Dictionary<SfxType, SoundClip>();
        private Dictionary<MusicType, MusicTrack> musicDictionary = new Dictionary<MusicType, MusicTrack>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudio();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudio()
        {
            // Создать AudioSource для музыки
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
            }

            // Создать AudioSource для SFX
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SfxSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
            }

            // Заполнить словари
            foreach (var sound in soundClips)
            {
                soundDictionary[sound.type] = sound;
            }

            foreach (var music in musicTracks)
            {
                musicDictionary[music.type] = music;
            }

            // Загрузить настройки
            LoadSettings();
        }

        /// <summary>
        /// Воспроизвести звуковой эффект
        /// </summary>
        public void PlaySfx(SfxType type)
        {
            if (sfxSource == null || !soundDictionary.ContainsKey(type)) return;

            SoundClip sound = soundDictionary[type];
            if (sound.clip != null)
            {
                sfxSource.PlayOneShot(sound.clip, sound.volume * sfxVolume * masterVolume);
            }
        }

        /// <summary>
        /// Воспроизвести музыку
        /// </summary>
        public void PlayMusic(MusicType type)
        {
            if (musicSource == null || !musicDictionary.ContainsKey(type)) return;

            MusicTrack track = musicDictionary[type];
            if (track.clip != null)
            {
                musicSource.clip = track.clip;
                musicSource.volume = track.volume * musicVolume * masterVolume;
                musicSource.Play();
            }
        }

        /// <summary>
        /// Остановить музыку
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// Установить громкость
        /// </summary>
        public void SetVolume(float master, float music, float sfx)
        {
            masterVolume = master;
            musicVolume = music;
            sfxVolume = sfx;

            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
        }

        private void LoadSettings()
        {
            Meta.SettingsData settings = Meta.SaveService.LoadSettings();
            SetVolume(settings.masterVolume, settings.musicVolume, settings.sfxVolume);
        }
    }

    public enum SfxType
    {
        Attack,
        Hit,
        LevelUp,
        Pickup,
        EnemyDeath,
        PlayerDeath,
        ButtonClick
    }

    public enum MusicType
    {
        MainMenu,
        Gameplay,
        Boss
    }
}

