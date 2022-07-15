using Aili.Utility;
using UnityEngine;

namespace Aili.Core
{
    [CreateAssetMenu(fileName = "SettingsManager", menuName = "Aili/Managers/Settings Manager")]
    public class SettingsManagerMaster : ScriptableSingleton<SettingsManagerMaster>
    {
        [Header("Global Game Settings")]
        [Range(30, 60)]
        public int m_FrameRate = 30;
    }
}
