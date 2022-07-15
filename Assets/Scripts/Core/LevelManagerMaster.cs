using System;
using System.Collections.Generic;
using System.IO;
using Aili.Audio;
#if UNITY_EDITOR
using Aili.ReadOnlyField;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aili.Core
{
    [Serializable]
    public class Level
    {
        [SerializeField]
#if UNITY_EDITOR
        [ReadOnly]
#endif
        public string m_Name;
        [SerializeField]
        public OverrideAudioState m_AudioState;
    }

    [CreateAssetMenu(fileName = "LevelManager", menuName = "Aili/Managers/Level Manager")]
    public class LevelManagerMaster : ScriptableObject
    {
        [SerializeField]
#if UNITY_EDITOR
        [ReadOnly]
#endif
        public LoadSceneState m_LoadSceneState;
        public enum LoadSceneState
        {
            Idle,
            Loading
        }

        [SerializeField]
        public List<Level> m_Scenes;

        public int SceneCount()
        {
            return SceneManager.sceneCountInBuildSettings;
        }

        void UpdateListName()
        {
            // Update nama elemen menjadi nama Scene
            for (int i = 0; i < SceneCount(); i++)
            {
                if (m_Scenes[i] != null)
                {
                    m_Scenes[i].m_Name = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                }
            }
        }

        void OnEnable()
        {
            // Buat daftar Level sebanyak Scene di Build Settings
            if (m_Scenes == null)
            {
                m_Scenes = new List<Level>(new Level[SceneCount()]);
            }
        }

        void OnValidate()
        {
            // Cegah menambah/menghapus daftar
            if (m_Scenes.Count > SceneCount())
            {
                m_Scenes.RemoveAt(m_Scenes.Count - 1);
            }
            if (m_Scenes.Count < SceneCount())
            {
                m_Scenes.Add(new Level());
            }

            UpdateListName();
        }
    }
}
