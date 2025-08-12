using UnityEngine;

namespace Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T Instance { get; private set; }

        // Use ValidAwake() instead of Awake() when inheriting from this class
        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            
            Instance = this as T;
            ValidAwake();
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        protected virtual void ValidAwake() { }
    }
}