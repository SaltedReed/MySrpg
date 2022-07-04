using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;

namespace MySrpg.UI
{

    public enum TranslateType
    {
        Direct,
        Slow
    }


    [RequireComponent(typeof(Image))]
    public class BaseGuideUI : MonoBehaviour, ICanvasRaycastFilter
    {
        public Vector3 center => m_material is null ? Vector3.zero : (Vector3)m_material.GetVector("_Center");

        protected Material m_material;
        protected Vector3 m_center; 
        protected RectTransform m_target;
        protected Vector3[] m_targetCorners = new Vector3[4];

        protected virtual void Awake()
        {
            m_material = transform.GetComponent<Image>().material;
        }

        public void Guide(Canvas canvas, RectTransform target, TranslateType translateType = TranslateType.Direct, float translateTime = 1)
        {
            m_target = target;

            if (target != null)
            {
                target.GetWorldCorners(m_targetCorners);

                for (int i = 0; i < m_targetCorners.Length; i++)
                {
                    m_targetCorners[i] = WorldToScreenPoint(canvas, m_targetCorners[i]);
                }
                m_center.x = m_targetCorners[0].x + (m_targetCorners[3].x - m_targetCorners[0].x) / 2;
                m_center.y = m_targetCorners[0].y + (m_targetCorners[1].y - m_targetCorners[0].y) / 2;
            }
            else
            {
                m_center = Vector3.zero;
                m_targetCorners[0] = new Vector3(-2000, -2000, 0);
                m_targetCorners[1] = new Vector3(-2000, 2000, 0);
                m_targetCorners[2] = new Vector3(2000, 2000, 0);
                m_targetCorners[3] = new Vector3(2000, -2000, 0);
            }

            switch (translateType)
            {
                case TranslateType.Direct:
                    TranslateDirect();
                    break;
                case TranslateType.Slow:
                    TranslateSlow(translateTime);
                    break;
            }
        }

        protected virtual void TranslateDirect()
        {
            m_material.SetVector("_Center", m_center);
        }

        protected virtual void TranslateSlow(float translateTime)
        {
            StartCoroutine(MoveCenterCoroutine(m_material.GetVector("_Center"), m_center, translateTime));
        }

        protected IEnumerator MoveCenterCoroutine(Vector3 start, Vector3 end, float time)
        {
            float progress = 0.0f;            

            while (progress < 1.0)
            {
                progress += Time.deltaTime / time;
                m_material.SetVector("_Center", Vector3.Lerp(start, end, progress));

                yield return null;
            }
        }


        protected Vector2 WorldToScreenPoint(Canvas canvas, Vector3 world)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, world);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, canvas.worldCamera, out localPoint);
            return localPoint;
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (m_target is null)
                return false;

            return !RectTransformUtility.RectangleContainsScreenPoint(m_target, sp);
        }
    }

}