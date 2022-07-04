using System.Collections;
using UnityEngine;

namespace MySrpg
{

    public class CameraShake_Count : BaseCameraShake
    {
        public int minCounts = 2;
        public int maxCounts = 3;
        public float extent = 0.3f;
        public float speed = 9.0f;

        private Vector3 m_originPos;
        private int m_counts;

        public override void Begin()
        {
            m_originPos = camera.transform.position;
            m_counts = Random.Range(minCounts, maxCounts+1);
            StartCoroutine(ShakeCoroutine());
        }

        private IEnumerator ShakeCoroutine()
        {
            float lastXSign = 1.0f;
            float lastYSign = 1.0f;

            while (m_counts > 0)
            {
                --m_counts;

                float x = Random.RandomRange(-extent, extent);
                if (Mathf.Sign(x) * lastXSign > 0.0f)
                    x *= -1.0f;
                lastXSign = Mathf.Sign(x);

                float y = Random.RandomRange(-extent, extent);
                if (Mathf.Sign(y) * lastYSign > 0.0f)
                    y *= -1.0f;
                lastYSign = Mathf.Sign(y);

                //Debug.Log($"({x}, {y})");
                Vector3 targetPos = camera.transform.position + new Vector3(x, y, 0.0f);

                yield return MoveToCoroutine(camera.transform.position, targetPos);
            }

            yield return MoveToCoroutine(camera.transform.position, m_originPos);
        }

        private IEnumerator MoveToCoroutine(Vector3 curPos, Vector3 targetPos)
        {
            while ((curPos - targetPos).sqrMagnitude > 0.001f)
            {
                curPos = Vector3.MoveTowards(curPos, targetPos, Time.deltaTime * speed);
                camera.transform.position = curPos;
                yield return null;
            }
        }

    }

}