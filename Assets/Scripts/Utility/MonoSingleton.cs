using UnityEngine;

namespace Aili.Utility
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // First pass
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        // Second pass
                        _instance = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
