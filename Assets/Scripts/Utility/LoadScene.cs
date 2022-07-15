using System;
using System.Collections;
using System.Collections.Generic;
using Aili.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Aili.Utility
{
    [AddComponentMenu("Aili/Utility/Load Scene")]
    public class LoadScene : MonoBehaviour
    {
        LevelManagerMaster m_LevelMaster;

        [SerializeField]
        public LoadSceneMethod m_Mode = LoadSceneMethod.Single;
        public enum LoadSceneMethod
        {
            Additive,
            Single
        }

// #if UNITY_EDITOR
//         [Dropdown("SceneList")]
// #endif
        [SerializeField]
        public string m_Scenes;

        [SerializeField]
        List<string> m_SceneList = new List<string>();

        Button m_Button;

        void GetScenes()
        {
            if (m_LevelMaster != null)
            {
                for (int i = 0; i < m_LevelMaster.SceneCount(); i++)
                {
                    string sceneName = m_LevelMaster.m_Scenes[i].m_Name;
                    if (sceneName != null)
                    {
                        m_SceneList.Add(sceneName);
                    }
                    if (m_SceneList[i] != m_LevelMaster.m_Scenes[i].m_Name)
                    {
                        m_SceneList[i] = m_LevelMaster.m_Scenes[i].m_Name;
                    }
                }

                while (m_SceneList.Count > m_LevelMaster.SceneCount())
                {
                    m_SceneList.RemoveAt(m_SceneList.Count - 1);
                }
            }
        }

        IEnumerator LoadAsynchronously(string name, LoadSceneMethod mode)
        {
            if (name != null)
            {
                Time.timeScale = 1;
                AsyncOperation loading = new AsyncOperation();

                switch (mode)
                {
                    case LoadSceneMethod.Additive:
                        loading = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                        break;
                    case LoadSceneMethod.Single:
                        loading = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
                        break;
                }

                loading.allowSceneActivation = false;

                while (!loading.isDone)
                {
                    m_LevelMaster.m_LoadSceneState = LevelManagerMaster.LoadSceneState.Loading;

                    if (loading.progress >= .9f)
                    {
                        loading.allowSceneActivation = true;

                        m_LevelMaster.m_LoadSceneState = LevelManagerMaster.LoadSceneState.Idle;
                    }

                    // yield return new WaitForSeconds(1f);
                    yield return null;
                }
            }
            else
            {
                throw new NullReferenceException("Loading scene failed! Please assign Scene name.");
            }
        }

        public void LoadSceneByName(string scene, LoadSceneMethod mode)
        {
            if (m_Button != null)
            {
                // Cegah Tapjacking
                m_Button.interactable = false;
            }
            // m_LevelMaster.m_PreloadScene = Scenes;
            // StartCoroutine(LoadAsynchronously(m_LevelMaster.m_PreloadScene));
            StartCoroutine(LoadAsynchronously(scene, mode));
        }

        public void RestartScene()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        void OnValidate()
        {
            GetScenes();
        }

        void Start()
        {
            m_LevelMaster = GameManagerMaster.LevelManager;

            m_LevelMaster.m_LoadSceneState = LevelManagerMaster.LoadSceneState.Idle;

            if (TryGetComponent(out m_Button))
            {
                m_Button.onClick.AddListener(delegate { LoadSceneByName(m_Scenes, m_Mode); });
            }
        }
    }
}
