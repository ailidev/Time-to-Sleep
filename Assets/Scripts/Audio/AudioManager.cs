using System;
using Aili.Core;
using Aili.Utility;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Aili.Audio
{
    [AddComponentMenu("Aili/Managers/Audio Manager")]
    public class AudioManager : MonoSingleton<AudioManager>
    {
        AudioMaster m_AudioMaster;
        [HideInInspector]
        public AudioSource m_BackgroundMusicSource, m_SoundEffectSource;
        [Header("Sliders")]
        [SerializeField]
        public Slider m_BackgroundMusicSlider;
        [SerializeField]
        public Slider m_SoundEffectSlider;

        #region Instantiate Audio
        AudioSource InstantiateAudioSource(string name, AudioInfo audio)
        {
            #region Instantiate
            AudioSource source = new GameObject(name, typeof(AudioSource)).GetComponent<AudioSource>();
            source.transform.parent = gameObject.transform;
            #endregion

            #region Setup
            if (audio.m_AudioMixerGroup != null || audio.m_AudioClip != null)
            {
                source.outputAudioMixerGroup = audio.m_AudioMixerGroup;
                source.clip = audio.m_AudioClip;
            }
            else
            {
                Debug.LogWarning($"Audio Mixer Group doesn\'t exist.");
                Debug.LogWarning($"Audio Clip doesn\'t exist.");
            }
            source.playOnAwake = false;
            if (audio.m_Autoplay) source.Play();
            source.loop = audio.m_Loop;
            source.mute = audio.m_Mute;
            source.volume = audio.m_Volume;
            #endregion

            return source;
        }
        #endregion

        #region Update State
        void UpdateState()
        {
            #region Audio Master
            float masterVolume = Mathf.Clamp(Mathf.Log10(m_AudioMaster.m_AudioMaster.m_Volume) * 20f, -80f, 0f);

            if (m_AudioMaster.m_AudioMaster.m_Mute)
            {
                m_AudioMaster.m_AudioMaster.m_AudioMixer.SetFloat("AudioMaster", -80f);
            }
            else
            {
                m_AudioMaster.m_AudioMaster.m_AudioMixer.SetFloat("AudioMaster", masterVolume);
            }
            #endregion

            #region Background Music
            if (m_AudioMaster.m_BackgroundMusic.m_AudioClip != null)
            {
                m_BackgroundMusicSource.clip = m_AudioMaster.m_BackgroundMusic.m_AudioClip;
            }
            m_BackgroundMusicSource.loop = m_AudioMaster.m_BackgroundMusic.m_Loop;
            m_BackgroundMusicSource.mute = m_AudioMaster.m_BackgroundMusic.m_Mute;
            m_BackgroundMusicSource.volume = m_AudioMaster.m_BackgroundMusic.m_Volume;
            #endregion

            #region Sound Effect
            if (m_AudioMaster.m_SoundEffect.m_AudioClip != null)
            {
                m_SoundEffectSource.clip = m_AudioMaster.m_SoundEffect.m_AudioClip;
            }
            m_SoundEffectSource.loop = m_AudioMaster.m_SoundEffect.m_Loop;
            m_SoundEffectSource.mute = m_AudioMaster.m_SoundEffect.m_Mute;
            m_SoundEffectSource.volume = m_AudioMaster.m_SoundEffect.m_Volume;
            #endregion
        }
        #endregion

        #region Interact with Audio Manager Master
        public void SyncSliderWithManager(bool reverse)
        {
            if ((m_BackgroundMusicSlider != null) && (m_SoundEffectSlider != null))
            {
                if (reverse)
                {
                    m_AudioMaster.m_BackgroundMusic.m_Volume = m_BackgroundMusicSlider.value;
                    m_AudioMaster.m_SoundEffect.m_Volume = m_SoundEffectSlider.value;
                }
                else
                {
                    m_BackgroundMusicSlider.value = m_AudioMaster.m_BackgroundMusic.m_Volume;
                    m_SoundEffectSlider.value = m_AudioMaster.m_SoundEffect.m_Volume;
                }
            }
            else
            {
                throw new NullReferenceException("Audio Sliders is missing");
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip != null)
            {
                m_AudioMaster.m_SoundEffect.m_AudioClip = clip;
                m_SoundEffectSource.clip = clip;
                m_SoundEffectSource.PlayOneShot(clip);
            }
        }
        #endregion

        #region Save & Load
        void SaveLoad()
        {

        }
        #endregion

        void Start()
        {
            m_AudioMaster = GameManagerMaster.AudioManager;

            m_BackgroundMusicSource = InstantiateAudioSource("BGM", m_AudioMaster.m_BackgroundMusic);
            m_SoundEffectSource = InstantiateAudioSource("SFX", m_AudioMaster.m_SoundEffect);

            SaveLoad();
        }

        void Update()
        {
            UpdateState();
            SyncSliderWithManager(false);
        }
    }
}
