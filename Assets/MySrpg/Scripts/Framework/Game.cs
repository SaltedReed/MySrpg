using System.Collections.Generic;
using UnityEngine;
using MyUtility;

namespace MyFramework
{

    public class Game : SingletonMono<Game>
    {
        public bool enteredGame { get; set; } = false;
        public bool isNovice { get; set; } = false;

        public bool isNetGame { get; set; }
        public bool isServer { get; set; }
        public bool isClient { get; set; }

        protected List<BaseGameSystem> m_systems = new List<BaseGameSystem>();
        
        /*public virtual void PreInit()
        {
            foreach (BaseGameSystem gs in m_systems)
            {
                gs.PreInit();
            }
        }

        public virtual void Init()
        {
            foreach (BaseGameSystem gs in m_systems)
            {
                gs.Init();
            }
        }

        public virtual void Tick()
        {
            foreach (BaseGameSystem gs in m_systems)
            {
                gs.Tick();
            }
        }*/

        public T AddSystem<T>() where T : BaseGameSystem
        {
            string nm = typeof(T).Name;
            GameObject go = new GameObject(nm);
            go.transform.parent = transform;

            T cmp = go.AddComponent<T>();
            m_systems.Add(cmp);

            OnAddSystem(cmp);

            Debug.Log($"added {nm}");

            return cmp;
        }

        protected virtual void OnAddSystem<T>(T sys) where T : BaseGameSystem
        {

        }

        public void RemoveSystem<T>() where T : BaseGameSystem
        {
            for (int i=0; i<m_systems.Count; ++i)
            {
                if (m_systems[i].GetType() == typeof(T))
                {
                    BaseGameSystem sys = m_systems[i];
                    OnRemoveSystem(m_systems[i]);
                    m_systems.RemoveAt(i);
                    if (sys != null)
                        Destroy(sys.gameObject);

                    Debug.Log($"removed {typeof(T).Name}");
                }
            }
        }

        protected virtual void OnRemoveSystem<T>(T sys) where T : BaseGameSystem
        {

        }

    }

}