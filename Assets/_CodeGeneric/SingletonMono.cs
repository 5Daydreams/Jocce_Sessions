using UnityEngine;

namespace _Code
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if(_instance != null) Destroy(this);
            DontDestroyOnLoad(this);
        }
    }
}