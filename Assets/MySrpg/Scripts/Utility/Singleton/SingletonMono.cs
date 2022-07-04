using UnityEngine;

namespace MyUtility
{

    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance
        {
            get
            {
                if (m_instance is null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    m_instance = go.AddComponent<T>();
                }
                return m_instance;
            }
        }
        private static T m_instance;

        public bool dontDestroyOnLoad = true;


        protected virtual void Awake()
        {
            if (m_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            m_instance = GetComponent<T>();
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

}