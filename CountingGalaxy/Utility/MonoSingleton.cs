using UnityEngine;

namespace Utility
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        protected static T Instance
        {
            get
            {
                if (instance)
                {
                    return instance;
                }
            
                instance = FindObjectOfType<T>();
                if (instance)
                {
                    return instance;
                }
                
                GameObject obj = new()
                {
                    name = typeof(T).Name
                };
                instance = obj.AddComponent<T>();

                return instance;
            }

            private set => instance = value;
        }
    
        private void Awake()
        {
            if (instance)
            {
                DestroyImmediate(gameObject);
                return;
            }
        
            Instance = this as T;
            ValidAwake();
        }

        protected virtual void ValidAwake()
        {
            // Override this method in child classes
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
