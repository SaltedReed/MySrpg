using System;
using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;

namespace MySrpg.UI
{

    public class PlayerPage : BaseUIWidget
    {
        public Button closeBtn;

        public CharacterList characterList { get; private set; }
        public CharacterDisplay characterDisplay { get; private set; }

        public override void OnCreate(UIManager uiMngr, object args = null)
        {
            base.OnCreate(uiMngr, args);

            characterList = uiMngr.Create<CharacterList>("CharacterList", "MainPageUI", "CharacterList");
            characterList.transform.SetParent(transform);
            characterDisplay = uiMngr.Create<CharacterDisplay>("CharacterDisplay", "MainPageUI", "CharacterDisplay");
            characterDisplay.transform.SetParent(transform);

            closeBtn.onClick.AddListener(() =>
            {
                uiManager.Pop();
            });
        }

        public override void OnDrop()
        {
            characterDisplay.OnDrop();
            characterList.OnDrop();
            base.OnDrop();
        }

        /// <param name="args">CharacterConfig[]</param>
        public override void OnOpen(object args = null)
        {
            if (args is null)
                throw new ArgumentNullException("PlayerPage.Open requires CharacterConfig[] as args");

            CharacterConfig[] characters = args as CharacterConfig[];
            if (characters is null)
                throw new InvalidCastException("PlayerPage.Open requires CharacterConfig[] as args");

            base.OnOpen(args);

            // NOTE: open characterDisplay first
            uiManager.Open(characterDisplay, characterList);
            uiManager.Open(characterList, args);
        }

        public override void OnClose()
        {
            uiManager.Close(characterDisplay);
            uiManager.Close(characterList);

            base.OnClose();
        }

    }

}