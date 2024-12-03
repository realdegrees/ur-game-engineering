using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    // This class is a singleton that manages the game state

    public abstract class Manager<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; protected set; } = null;

        public Transform GetTransform { get; private set; }
        protected virtual void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
                Debug.Log(typeof(T).Name + " initialized");
            }

        }

        protected virtual void Init() { }

        protected virtual void OnDestroy()
        {
            Debug.LogWarning(typeof(T).Name + " destroyed!");
        }
    }
}