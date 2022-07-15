using Aili.Audio;
using Aili.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aili.Core
{
    [AddComponentMenu("Aili/Managers/Level Manager")]
    public class LevelManager : MonoSingleton<LevelManager>
    {
        LevelManagerMaster m_LevelManager;
        AudioMaster m_AudioMaster;
        AudioManager m_AudioManager;

        public string CurrentSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public bool IsSceneMatch()
        {
            bool isMatch = false;

            for (int i = 0; i < m_LevelManager.SceneCount(); i++)
            {
                if (CurrentSceneName() == m_LevelManager.m_Scenes[i].m_Name)
                {
                    isMatch = true;
                }
            }

            return isMatch;
        }

        void OverrideSceneState()
        {
            if (IsSceneMatch())
            {
                for (int i = 0; i < m_LevelManager.SceneCount(); i++)
                {
                    #region Override Audio
                    if (m_LevelManager.m_Scenes[i].m_AudioState != null)
                    {
                        if (m_LevelManager.m_Scenes[i].m_AudioState.m_BackgroundMusicClip != null)
                        {
                            m_AudioMaster.m_BackgroundMusic.m_AudioClip = m_LevelManager.m_Scenes[i].m_AudioState.m_BackgroundMusicClip;
                        }

                        if ((m_AudioMaster.m_BackgroundMusic.m_AudioClip != null) && (!m_AudioManager.m_BackgroundMusicSource.isPlaying))
                        {
                            switch (m_LevelManager.m_Scenes[i].m_AudioState.m_State)
                            {
                                case OverrideAudioState.AudioState.Play:
                                    m_AudioManager.m_BackgroundMusicSource.Play();
                                    break;
                                case OverrideAudioState.AudioState.Pause:
                                    m_AudioManager.m_BackgroundMusicSource.Pause();
                                    break;
                                case OverrideAudioState.AudioState.Stop:
                                    m_AudioManager.m_BackgroundMusicSource.Stop();
                                    break;
                            }
                        }

                        m_AudioMaster.m_BackgroundMusic.m_Loop = m_LevelManager.m_Scenes[i].m_AudioState.m_Loop;
                        m_AudioMaster.m_BackgroundMusic.m_Mute = m_LevelManager.m_Scenes[i].m_AudioState.m_Mute;
                    }
                    #endregion
                }
            }
        }

        void Start()
        {
            m_LevelManager = GameManagerMaster.LevelManager;
            m_AudioMaster = GameManagerMaster.AudioManager;
            m_AudioManager = AudioManager.Instance;
        }

        void Update()
        {
            OverrideSceneState();
        }
    }
}
