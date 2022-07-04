using System.Collections;
using UnityEngine;

namespace MySrpg
{

    public sealed class AbilityExecutor : MonoBehaviour
    {
        private IEnumerator m_running;

        public void Execute(IEnumerator exe)
        {
            if (m_running != null)
            {
                StopCoroutine(m_running);
            }

            m_running = exe;
            StartCoroutine(exe);
        }

        public void OnComplete()
        {
            m_running = null;
        }
    }

}