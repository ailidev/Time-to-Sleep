using Aili.Utility;
using UnityEngine;

namespace Aili.Core
{
    [AddComponentMenu("Aili/Managers/Game Manager")]
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        GameManagerMaster m_GameManagerMaster;
    }
}
