using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;
using MyUtility;

namespace MySrpg.UI
{

    public class BaseGuide : MonoBehaviour
    {
        public GameObject root;
        public GuideStep[] steps;

        protected int m_nextIndex = 0;
        protected bool m_isCompleted;

        protected virtual void Awake()
        {
            if (!ShouldAwakeAndDisactiveIfShouldnt())
                return;

            EventDispatcher.Register((int)steps[0].eventId, ExecuteNext);
        }

        protected bool ShouldAwakeAndDisactiveIfShouldnt()
        {
            if (!Game.Instance.isNovice || steps is null || steps.Length == 0)
            {
                gameObject.SetActive(false);
                root?.SetActive(false);
                return false;
            }

            return true;
        }

        public void ExecuteNext()
        {
            if (m_isCompleted)
                return;
            if (steps is null)
                return;

            if (!m_isCompleted && m_nextIndex == steps.Length)
            {
                m_isCompleted = true;
                OnComplete();
                return;
            }

            if (m_nextIndex > 0)
                Hide(m_nextIndex - 1);
            Show(m_nextIndex);

            steps[m_nextIndex].Execute();
            m_nextIndex++;

            EventDispatcher.Unregister((int)steps[m_nextIndex - 1].eventId, ExecuteNext);
            if (m_nextIndex < steps.Length)
                EventDispatcher.Register((int)steps[m_nextIndex].eventId, ExecuteNext);
        }

        protected virtual void OnComplete()
        {
            Hide(m_nextIndex - 1);
            gameObject.SetActive(false);
            root.gameObject.SetActive(false);
        }

        protected void Show(int index)
        {
            if (index < 0 || index >= steps.Length)
                return;
            steps[index].gameObject.SetActive(true);
        }

        protected void Hide(int index)
        {
            if (index < 0 || index >= steps.Length)
                return;
            steps[index].gameObject.SetActive(false);
        }
    }

}