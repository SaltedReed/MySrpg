using System;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;

namespace MyFramework.UI
{

    public sealed class UIManager : SingletonMono<UIManager>
    {
        public Transform uiRoot;

        private Dictionary<string, BaseUIWidget> m_widgets = new Dictionary<string, BaseUIWidget>();
        private Stack<BaseUIWidget> m_uiStack = new Stack<BaseUIWidget>();
        private Stack<object> m_uiOpenArgStack = new Stack<object>();

        public void Push(string wid, object args = null, bool closeTop = true)
        {
            if (closeTop && m_uiStack.Count > 0)
            {
                Close(m_uiStack.Peek());
            }

            BaseUIWidget widget = Find(wid);
            if (widget != null && !widget.isOpen)
            {
                m_uiStack.Push(widget);
                m_uiOpenArgStack.Push(args);
                Open(wid, args);
            }
        }

        public void Push(BaseUIWidget widget, object args = null, bool closeTop = true)
        {
            if (widget is null)
                throw new ArgumentNullException();

            if (closeTop && m_uiStack.Count > 0)
            {
                if (m_uiStack.Peek() is null)
                {
                    m_uiStack.Pop();
                    m_uiOpenArgStack.Pop();
                }
                else
                {
                    Close(m_uiStack.Peek());
                }
            }

            if (widget != null && !widget.isOpen)
            {
                m_uiStack.Push(widget);
                m_uiOpenArgStack.Push(args);
                Open(widget, args);
            }
        }

        public void Pop(bool openTop = true)
        {
            if (m_uiStack.Count == 0)
                throw new IndexOutOfRangeException();

            BaseUIWidget widget = m_uiStack.Pop();
            m_uiOpenArgStack.Pop();
            Close(widget);

            if (openTop && m_uiStack.Count > 0)
            {
                Open(m_uiStack.Peek(), m_uiOpenArgStack.Peek());
            }
        }

        public void DestroyStack()
        {
            while (m_uiStack.Count > 0)
            {
                /*if (m_uiStack.Peek() != null)
                    Destroy(m_uiStack.Peek().wid);*/
                m_uiStack.Pop();
                m_uiOpenArgStack.Pop();
            }
        }

        public T Create<T>(string wid, string path, bool asChild = true, object args = null) where T : BaseUIWidget
        {
            BaseUIWidget widget;
            if (m_widgets.TryGetValue(wid, out widget))
            {
                if (widget != null)
                    return widget as T;
                else
                    m_widgets.Remove(wid);
            }

            GameObject prefab = ResourceManager.Load<GameObject>(path);
            if (prefab is null)
                throw new NullReferenceException($"failed to load from {path} for {wid}");

            GameObject go = Instantiate(prefab);
            if (asChild)
                go.transform.SetParent(uiRoot, false);
            go.SetActive(false);

            T cmp = go.GetComponent<T>();
            cmp.wid = wid;
            m_widgets.Add(wid, cmp);
            cmp.OnCreate(this, args);

            return cmp;
        }

        public void Destroy(string wid, float delay = -1)
        {
            BaseUIWidget widget;
            if (m_widgets.TryGetValue(wid, out widget))
            {
                m_widgets.Remove(wid);

                // destroy from m_uiStack?

                widget.OnDrop();
                /*if (widget != null)
                {
                    if (delay > 0.0f)
                        Destroy(widget.gameObject, delay);
                    else
                        Destroy(widget.gameObject);
                }*/

                //Debug.Log($"destroy {wid}");
            }
        }

        public void Open(string wid, object args = null)
        {
            BaseUIWidget widget = Find(wid);
            if (widget != null && !widget.isOpen)
            {
                Open(widget, args);
            }
        }

        public void Open(BaseUIWidget widget, object args = null)
        {
            widget.gameObject.SetActive(true);
            widget.OnOpen(args);
            //Debug.Log($"opened {widget.gameObject.name}");
        }

        public void Close(string wid)
        {
            BaseUIWidget widget = Find(wid);
            if (widget != null && widget.isOpen)
            {
                Close(widget);
            }
        }

        public void Close(BaseUIWidget widget)
        {
            widget.gameObject.SetActive(false);
            widget.OnClose();
            //Debug.Log($"closed {widget.gameObject.name}");
        }

        public void CloseAll()
        {
            while (m_uiStack.Count > 0)
            {
                Pop(false);
            }

            foreach (BaseUIWidget widget in m_widgets.Values)
            {
                if (widget != null && widget.isOpen)
                {
                    Close(widget);
                }
            }
        }

        public bool IsOpen(string wid)
        {
            BaseUIWidget widget = Find(wid);
            if (widget != null && widget.isOpen)
                return true;
            return false;
        }

        public BaseUIWidget Find(string wid)
        {
            BaseUIWidget widget;
            if (m_widgets.TryGetValue(wid, out widget))
            {
                return widget;
            }
            return null;
        }

        public bool Contains(string wid)
        {
            return m_widgets.ContainsKey(wid);
        }

    }

}