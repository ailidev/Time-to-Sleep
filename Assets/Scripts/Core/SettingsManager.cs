using Aili.Utility;
using UnityEngine;

namespace Aili.Core
{
    [AddComponentMenu("Aili/Managers/Settings Manager")]
    public class SettingsManager : MonoSingleton<SettingsManager>
    {
        SettingsManagerMaster m_SettingsMaster;

        void SetupSettings()
        {
            // Set Frame Rate
            Application.targetFrameRate = m_SettingsMaster.m_FrameRate;
            // Application.targetFrameRate = Screen.currentResolution.refreshRate;
        }

        void Start()
        {
            m_SettingsMaster = GameManagerMaster.SettingsManager;
        }

        void Update()
        {
            SetupSettings();
        }
    }
}
