using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework.UI;
using MyUtility;

namespace MySrpg.UI
{

    public class OverheadUI : BaseUIWidget
    {
        public HpBar hpBar;
        public BuffList buffList;

        [HideInInspector]
        public Transform targetPoint;

        public Character owner { get; private set; }

        private Camera m_mainCam;


        public void OnSetOwner(Character c)
        {
            m_mainCam = Camera.main;
            targetPoint = c.hpBarPoint;
            c.onHpChangeHandler += OnOwnerChangeHealth;

            hpBar.OnSetOwner(c);
            buffList.OnSetOwner(c);
        }

        public void OnOwnerChangeHealth(float _, float newVal)
        {
            if (newVal <= 0.0f)
            {
                if (owner != null && owner.gameObject.activeInHierarchy)
                    owner.onHpChangeHandler -= OnOwnerChangeHealth;
                Destroy(gameObject, 0.1f);
            }
        }

        private void Update()
        {
            transform.position = m_mainCam.WorldToScreenPoint(targetPoint.position);
        }

    }

}