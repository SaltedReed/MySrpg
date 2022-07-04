using UnityEngine;
using MyFramework;
using MyFramework.UI;
using MySrpg.UI;
using MyUtility;

namespace MySrpg
{

    public class LevelManager_Main : LevelManager
    {
        public GameObject startScreen;
        public float startScreenDuration = 2.0f;

        protected UIManager m_uiManager;

        protected virtual void Start()
        {
            m_uiManager = UIManager.Instance;
            m_uiManager.uiRoot = uiRoot;

            if (!Game.Instance.enteredGame)
            {
                Game.Instance.enteredGame = true;

                startScreen.SetActive(true);
                Invoke(nameof(StartScreen2MainPage), startScreenDuration);
            }
            else
            {
                OpenMainPage();
            }
        }

        protected void StartScreen2MainPage()
        {
            startScreen.SetActive(false);            
            OpenMainPage();
        }

        protected void OpenMainPage()
        {
            MainPage mainPage = m_uiManager.Create<MainPage>("MainPage", "Prefabs/UI/MainPage");
            m_uiManager.Push(mainPage);
        }
                
    }

}