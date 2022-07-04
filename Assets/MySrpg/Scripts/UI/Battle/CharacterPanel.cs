using System;
using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;

namespace MySrpg.UI
{

    public class CharacterPanel : BaseUIWidget
    {
        public Text atkNumText;
        public Text dfnsNumText;
        public Image[] iconSlots;

        /// <param name="args">Character</param>
        public override void OnOpen(object args = null)
        {
            if (args is null)
                throw new ArgumentNullException("CharacterPanel.Open requires Character as args");

            Character character = args as Character;
            if (character is null)
                throw new InvalidCastException("CharacterPanel.Open requires Character as args");

            base.OnOpen(args);

            atkNumText.text = character.attackment.ToString();
            dfnsNumText.text = character.defense.ToString();

            for (int i=1; i<character.abilities.Length; ++i)
            {
                iconSlots[i-1].sprite = character.abilities[i].icon;
            }
        }

        public override void OnClose()
        {
            base.OnClose();

            atkNumText.text = null;
            dfnsNumText.text = null;

            for (int i = 0; i < iconSlots.Length; ++i)
            {
                iconSlots[i].sprite = null;
            }
        }
    }

}