using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MySrpg.UI
{

    public class RectGuideUI : BaseGuideUI
    {
        private float m_width; 
        private float m_height;


        protected override void TranslateDirect()
        {
            base.TranslateDirect();

            m_width = (m_targetCorners[3].x - m_targetCorners[0].x) / 2;
            m_height = (m_targetCorners[1].y - m_targetCorners[0].y) / 2;

            m_material.SetFloat("_SliderX", m_width);
            m_material.SetFloat("_SliderY", m_height);
        }

        protected override void TranslateSlow(float translateTime)
        {
            base.TranslateSlow(translateTime);
            StartCoroutine(ScaleCoroutine(m_material.GetFloat("_SliderX"), m_width,
                m_material.GetFloat("_SliderY"), m_height, translateTime));
        }

        protected IEnumerator ScaleCoroutine(float startW, float endW, float startH, float endH, float time)
        {
            float progress = 0.0f;

            while (progress < 1.0)
            {
                progress += Time.deltaTime / time;

                m_material.SetFloat("_SliderX", Mathf.Lerp(startW, endW, progress));
                m_material.SetFloat("_SliderY", Mathf.Lerp(startH, endH, progress));

                yield return null;
            }
        }
    }

}