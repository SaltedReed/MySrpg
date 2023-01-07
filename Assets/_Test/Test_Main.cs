using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;
using MyFramework.UI;
using MySrpg;
using MySrpg.UI;
using MyUtility;

namespace MyTest
{

    public class Test_Main : MonoBehaviour
    {
        public Transform uiRoot;
        public CharacterConfig player;
        public CharacterDisplay charDisplay;


        private void OnGUI()
        {
            ///////////////////////////////////////////////////////////////////
            if (GUILayout.Button("open character list"))
            {
                UIManager uiSys = UIManager.Instance;
                if (uiSys.uiRoot is null)
                    uiSys.uiRoot = uiRoot;

                string wid = "character list";
                if (!uiSys.Contains(wid))
                {
                    CharacterList panel = uiSys.Create<CharacterList>(wid, "MainPageUI", "CharacterList");
                }
                uiSys.Open(wid, new CharacterConfig[] { player });
            }
            if (GUILayout.Button("close character list"))
            {
                UIManager uiSys = UIManager.Instance;
                string wid = "character list";
                uiSys.Close(wid);
            }

            ///////////////////////////////////////////////////////////////////
            if (GUILayout.Button("select character"))
            {
                charDisplay.OnSelectCharacter(player);
            }

            ///////////////////////////////////////////////////////////////////
            if (GUILayout.Button("open player page"))
            {
                UIManager uiSys = UIManager.Instance;
                if (uiSys.uiRoot is null)
                    uiSys.uiRoot = uiRoot;

                string wid = "PlayerPage";
                if (!uiSys.Contains(wid))
                {
                    PlayerPage panel = uiSys.Create<PlayerPage>(wid, "MainPageUI", "PlayerPage");
                }
                uiSys.Open(wid, new CharacterConfig[] { player });
            }
            if (GUILayout.Button("close player page"))
            {
                UIManager uiSys = UIManager.Instance;
                string wid = "PlayerPage";
                uiSys.Close(wid);
            }

            ///////////////////////////////////////////////////////////////////
            if (GUILayout.Button("open main page"))
            {
                UIManager uiSys = UIManager.Instance;
                if (uiSys.uiRoot is null)
                    uiSys.uiRoot = uiRoot;

                string wid = "MainPage";
                if (!uiSys.Contains(wid))
                {
                    MainPage panel = uiSys.Create<MainPage>(wid, "MainPageUI", "MainPage");
                }
                uiSys.Push(wid, new CharacterConfig[] { player });
            }
        }
    }

}