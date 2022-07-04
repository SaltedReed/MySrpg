using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtility
{

    public class EventDispatcher
    {
        private static Dictionary<int, Delegate> m_listeners = new Dictionary<int, Delegate>();

        private static void Add(int evId, Delegate listener)
        {
            Delegate func;
            if (!m_listeners.TryGetValue(evId, out func))
            {
                m_listeners.Add(evId, listener);
            }
            else
            {
                func = (func != null) ? Delegate.Combine(func, listener) : listener;
                m_listeners[evId] = func;
            }
        }

        private static void Remove(int evId, Delegate listener)
        {
            Delegate func;
            if (m_listeners.TryGetValue(evId, out func))
            {
                if (func != null)
                {
                    func = Delegate.Remove(func, listener);
                    m_listeners[evId] = func;
                }
            }
        }


        public static void Register(int evId, Action listener)
        {
            if (listener == null)
            {
                return;
            }

            Add(evId, listener);
        }

        public static void Unregister(int evId, Action listener)
        {
            if (listener == null)
            {
                return;
            }

            Remove(evId, listener);
        }

        public static void Register<T>(int evId, Action<T> listener)
        {
            if (listener == null)
            {
                return;
            }

            Add(evId, listener);
        }

        public static void Unregister<T>(int evId, Action<T> listener)
        {
            if (listener == null)
            {
                return;
            }

            Remove(evId, listener);
        }

        public static void Register<T1, T2>(int evId, Action<T1, T2> listener)
        {
            if (listener == null)
            {
                return;
            }

            Add(evId, listener);
        }

        public static void Unregister<T1, T2>(int evId, Action<T1, T2> listener)
        {
            if (listener == null)
            {
                return;
            }

            Remove(evId, listener);
        }

        public static void Register<T1, T2, T3>(int evId, Action<T1, T2, T3> listener)
        {
            if (listener == null)
            {
                return;
            }

            Add(evId, listener);
        }

        public static void Unregister<T1, T2, T3>(int evId, Action<T1, T2, T3> listener)
        {
            if (listener == null)
            {
                return;
            }

            Remove(evId, listener);
        }

        public static void Register<T1, T2, T3, T4>(int evId, Action<T1, T2, T3, T4> listener)
        {
            if (listener == null)
            {
                return;
            }

            Add(evId, listener);
        }

        public static void Unregister<T1, T2, T3, T4>(int evId, Action<T1, T2, T3, T4> listener)
        {
            if (listener == null)
            {
                return;
            }

            Remove(evId, listener);
        }


        public static void DispatchEvent(int evId)
        {
            Delegate func;

            Debug.Log("dispatched event " + evId);

            if (m_listeners.TryGetValue(evId, out func) && func != null)
            {
                var act = (Action)func;
                act();
            }
            else
            {
                // Debug.LogWarning("event " + evId + " has not been registered");
            }
        }

        public static void DispatchEvent<T1>(int evId, T1 args)
        {
            Delegate func;

            Debug.Log("dispatched event " + evId);
            if (m_listeners.TryGetValue(evId, out func) && func != null)
            {
                var act = (Action<T1>)func;
                act(args);
            }
            else
            {
                // Debug.LogWarning("event " + evId + " has not been registered");
            }
        }

        public static void DispatchEvent<T1, T2>(int evId, T1 arg1, T2 arg2)
        {
            Delegate func;
            Debug.Log("dispatched event " + evId);

            if (m_listeners.TryGetValue(evId, out func) && func != null)
            {
                var tmp = (Action<T1, T2>)func;
                tmp(arg1, arg2);
            }
            else
            {
                // Debug.LogWarning("event " + evId + " has not been registered");
            }
        }

        public static void DispatchEvent<T1, T2, T3>(int evId, T1 arg1, T2 arg2, T3 arg3)
        {
            Delegate func;

            Debug.Log("dispatched event " + evId);

            if (m_listeners.TryGetValue(evId, out func) && func != null)
            {
                var tmp = (Action<T1, T2, T3>)func;
                tmp(arg1, arg2, arg3);
            }
            else
            {
                // Debug.LogWarning("event " + evId + " has not been registered");
            }
        }

        public static void DispatchEvent<T1, T2, T3, T4>(int evId, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Delegate func;

            Debug.Log("dispatched event " + evId);

            if (m_listeners.TryGetValue(evId, out func) && func != null)
            {
                var tmp = (Action<T1, T2, T3, T4>)func;
                tmp(arg1, arg2, arg3, arg4);
            }
            else
            {
                // Debug.LogWarning("event " + evId + " has not been registered");
            }
        }


    }

}