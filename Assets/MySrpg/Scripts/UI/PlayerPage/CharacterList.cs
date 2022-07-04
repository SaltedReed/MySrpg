using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;

namespace MySrpg.UI
{

    public delegate void OnSelectCharacterHandler(CharacterConfig character);

    public class CharacterList : BaseUIWidget
    {
        public GameObject slotPrefab;
        public Transform slotGroup;

        public event OnSelectCharacterHandler onSelectCharacterHandler;

        private CharacterConfig[] m_characters;
        private List<Button> m_buttons = new List<Button>();


        /// <param name="args">CharacterConfig[]</param>
        public override void OnOpen(object args = null)
        {
            if (args is null)
                throw new ArgumentNullException("CharacterList.Open requires CharacterConfig[] as args");

            m_characters = args as CharacterConfig[];
            if (m_characters is null)
                throw new InvalidCastException("CharacterList.Open requires CharacterConfig[] as args");

            base.OnOpen(args);

            m_buttons.Clear();
            for (int i = 0; i < m_characters.Length; ++i)
            {
                GameObject slot = Instantiate(slotPrefab);
                slot.transform.SetParent(slotGroup);

                Image img = slot.GetComponent<Image>();
                img.sprite = m_characters[i].icon;

                Button btn = slot.GetComponent<Button>(); Debug.Assert(btn != null);
                btn.onClick.AddListener(() =>
                {
                    OnButtonClick(btn);
                });
                m_buttons.Add(btn);
            }

            // select the first character on open
            if (m_characters.Length > 0)
            {
                CharacterConfig character = m_characters[0];
                onSelectCharacterHandler?.Invoke(character);
            }
        }

        public override void OnClose()
        {
            m_buttons.Clear();
            for (int i = slotGroup.childCount - 1; i >= 0; --i)
            {
                Destroy(slotGroup.GetChild(i).gameObject);
            }

            base.OnClose();
        }

        public void OnButtonClick(Button btn)
        {
            int index = m_buttons.FindIndex((Button b) => { return b == btn; });
            CharacterConfig character = m_characters[index];
            onSelectCharacterHandler?.Invoke(character);
        }

    }

}