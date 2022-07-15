using UnityEngine;

namespace Aili.Audio
{
    [CreateAssetMenu(fileName = "AudioState", menuName = "Aili/Utility/Override Audio State")]
    public class OverrideAudioState : ScriptableObject
    {
        public AudioClip m_BackgroundMusicClip;
        public AudioState m_State;
        public bool m_Loop;
        public bool m_Mute;

        public enum AudioState
        {
            Play,
            Pause,
            Stop
        }
    }
}
