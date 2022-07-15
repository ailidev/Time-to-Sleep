using UnityEngine;

namespace Aili.Utility
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    T[] results = Resources.FindObjectsOfTypeAll<T>();
                    if (results.Length == 0)
                    {
                        Debug.LogError($"ScriptableSingleton: results is 0 of {typeof(T).ToString()}");
                        return null;
                    }
                    else if (results.Length > 1)
                    {
                        Debug.LogError($"ScriptableSingleton: results is greater than 1 of {typeof(T).ToString()}");
                        return null;
                    }
                    _instance = results[0];
                    _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
                }
                return _instance;
            }
        }
    }
}
