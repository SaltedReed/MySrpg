using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;
using MyUtility;

namespace MySrpg.UI
{

    public class BuffList : BaseUIWidget
    {
        public Character owner { get; private set; }

        private GameObject m_iconSlotPrefab;
        private Dictionary<BaseBuff, GameObject> m_icons = new Dictionary<BaseBuff, GameObject>();

        public void OnSetOwner(Character c)
        {
            owner = c;

            if (c!=null)
            {
                c.onAttachBuffHandler += Add;
                c.onDetachBuffHandler += Remove;
            }
        }

        public void Add(BaseBuff buff)
        {
            if (buff is null || buff.icon is null)
                return;

            GameObject go = Instantiate(m_iconSlotPrefab);
            go.GetComponent<Image>().sprite = buff.icon;
            go.transform.SetParent(transform);

            m_icons.Add(buff, go);
        }

        public void Remove(BaseBuff buff)
        {
            if (buff is null || buff.icon is null)
                return;

            GameObject go;
            if (m_icons.TryGetValue(buff, out go))
            {
                m_icons.Remove(buff);
                Destroy(go);
            }
        }

        private void Start()
        {
            m_iconSlotPrefab = ResourceManager.Load<GameObject>("GeneralUI", "IconSlot");
        }

        private void OnDisable()
        {
            if (owner != null && owner.gameObject.activeInHierarchy)
            {
                owner.onAttachBuffHandler -= Add;
                owner.onDetachBuffHandler -= Remove;
            }
        }

    }

}