using System;
using Aili.Utility;
using UnityEngine;
using UnityEngine.Audio;

namespace Aili.Audio
{
    [Serializable]
    public class AudioMasterInfo
    {
        public AudioMixer m_AudioMixer;
        public bool m_Mute;
        [Range(0.0001f, 1f)]
        public float m_Volume;
    }

    [Serializable]
    public class AudioInfo
    {
        public AudioMixerGroup m_AudioMixerGroup;
        public AudioClip m_AudioClip;
        public bool m_Autoplay = false;
        public bool m_Loop = false;
        public bool m_Mute = false;
        [Range(0, 1f)]
        public float m_Volume = 1f;
    }

    [CreateAssetMenu(fileName = "AudioManager", menuName = "Aili/Managers/Audio Manager")]
    public class AudioMaster : ScriptableObject
    {
        [Header("Audio Manager")]
        public AudioMasterInfo m_AudioMaster;
        public AudioInfo m_BackgroundMusic;
        public AudioInfo m_SoundEffect;
    }
}
