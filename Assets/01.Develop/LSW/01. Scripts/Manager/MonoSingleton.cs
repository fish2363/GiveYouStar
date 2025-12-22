using UnityEngine;

namespace _01.Develop.LSW._01._Scripts.Manager
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
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
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected MonoSingleton() { }

        protected MonoSingleton(bool shouldCreateNewInstance)
        {
            if (shouldCreateNewInstance)
            {
                _instance = null;
            }
        }
    }
}