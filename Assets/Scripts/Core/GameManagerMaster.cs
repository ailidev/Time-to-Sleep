using System;
using Aili.Audio;
using Aili.Utility;
using UnityEngine;

namespace Aili.Core
{
    /// <summary>
    /// Pusat dari referensi dari semua Manager
    /// </summary>
    [CreateAssetMenu(fileName = "GameManager", menuName = "Aili/Game Manager")]
    public class GameManagerMaster : ScriptableSingleton<GameManagerMaster>
    {
        [SerializeField]
        private AudioMaster m_AudioManager;
        public static AudioMaster AudioManager { get { return Instance.m_AudioManager; } }

        [SerializeField]
        private LevelManagerMaster m_LevelManager;
        public static LevelManagerMaster LevelManager { get { return Instance.m_LevelManager; } }

        [SerializeField]
        private SettingsManagerMaster m_SettingsManager;
        public static SettingsManagerMaster SettingsManager { get { return Instance.m_SettingsManager; } }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            Debug.Log("This message will appear before Awake() method.");
        }
    }
}
