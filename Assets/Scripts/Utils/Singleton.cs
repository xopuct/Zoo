using UnityEngine;

namespace Zoo
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public bool DontDestroy = false;

        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this as T;
                if (DontDestroy)
                    DontDestroyOnLoad(gameObject);
                Initialize();
            }
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void Shutdown()
        {
        }

        protected virtual void OnDestroy()
        {
            if ((Instance as Singleton<T>) == this)
            {
                try
                {
                    Shutdown();
                }
                finally
                {
                    Instance = null;
                }
            }
        }
    }

}
