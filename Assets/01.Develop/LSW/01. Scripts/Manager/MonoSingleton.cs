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
            if (_instance == null || _instance == this)
            {
                if (_instance == null)
                    _instance = this as T;
                
                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }
                
                DontDestroyOnLoad(gameObject);
                OnSingletonAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// \`MonoSingleton\` 초기화 시 추가 동작이 필요하면 오버라이드
        protected virtual void OnSingletonAwake() { }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}