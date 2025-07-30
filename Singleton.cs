using UnityEngine;

namespace TurnBasedStrategy
{
    // Generic MonoBehaviour singleton base class
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        // Ensures only one instance exists
        protected virtual void Awake()
        {
            Instance = SetSingleton();
        }

        // Sets the singleton instance, destroys duplicates
        protected T SetSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"Duplicate singleton of type {typeof(T).Name} detected on '{gameObject.name}'. Destroying this component.");
                Destroy(this);
                return null;
            }
            return this as T;
        }
    }
}