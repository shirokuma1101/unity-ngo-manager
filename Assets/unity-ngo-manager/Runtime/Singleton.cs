using Unity.Netcode;
using UnityEngine;

namespace NGOManager.Utility.Singleton
{
    /// <summary>
    /// Singleton class for MonoBehaviour.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }


        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Singleton class for MonoBehaviour that is persistent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonPersistent<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }


        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Used when Singleton (or similar classes) cannot be inherited (e.g. inherited).
    /// </summary>
    /// <example>
    /// <code>
    /// class BaseClass : MonoBehaviour { }
    /// class DerivedClass : BaseClass
    /// {
    ///     public static DerivedClass Instance => SingletonAttacher<DerivedClass>.Instance;
    /// }
    /// </code>
    /// </example>
    /// <typeparam name="T"></typeparam>
    public static class SingletonAttacher<T> where T : Component
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindAnyObjectByType<T>();
                }
                return instance;
            }
        }

        private static T instance;
    }

    /// <summary>
    /// Singleton class for NetworkBehaviour.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonNetwork<T> : NetworkBehaviour where T : Component
    {
        public static T Instance { get; private set; }


        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Singleton class for NetworkBehaviour that is persistent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonNetworkPersistent<T> : NetworkBehaviour where T : Component
    {
        public static T Instance { get; private set; }


        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
