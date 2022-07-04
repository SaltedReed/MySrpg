using UnityEngine;

namespace MyFramework.UI
{

    public enum UIState
    {
        Close = 0,
        Open
    }


    public delegate void UIWidgetEventHandler(BaseUIWidget widget);


    public abstract class BaseUIWidget : MonoBehaviour
    {
        [HideInInspector]
        public string wid;

        public event UIWidgetEventHandler onOpenHandler;
        public event UIWidgetEventHandler onCloseHandler;

        public UIManager uiManager { get; protected set; }

        public UIState state { get; protected set; }

        public bool isOpen => m_initialized && state == UIState.Open;

        protected bool m_initialized;

        public virtual void OnCreate(UIManager uiMngr, object args = null)
        {
            uiManager = uiMngr;
            m_initialized = true;
        }

        public virtual void OnDrop()
        {
            m_initialized = false;
        }

        public virtual void OnOpen(object args = null)
        {
            Debug.Assert(m_initialized);

            state = UIState.Open;
            onOpenHandler?.Invoke(this);
        }

        public virtual void OnClose()
        {
            Debug.Assert(m_initialized);

            state = UIState.Close;
            onCloseHandler?.Invoke(this);
        }

    }

}