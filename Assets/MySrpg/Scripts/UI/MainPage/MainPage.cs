using System;
using UnityEngine;
using UnityEngine.UI;
using MyFramework;
using MyFramework.UI;
using MyUtility;


namespace MySrpg.UI
{

    public class MainPage : BaseUIWidget
    {
        public Button pveBtn;
        public Button pvpBtn;
        public Button playerPageBtn;
        public Button taskPageBtn;
        public GameObject taskPageRedDot;

        private PlayerPage m_playerPage;
        private TaskPage m_taskPage;

        public override void OnCreate(UIManager uiMngr, object args = null)
        {
            base.OnCreate(uiMngr, args);

            m_playerPage = uiManager.Create<PlayerPage>("PlayerPage", "Prefabs/UI/PlayerPage/PlayerPage");
            m_taskPage = uiManager.Create<TaskPage>("TaskPage", "Prefabs/UI/TaskPage/TaskPage");

            pveBtn.onClick.AddListener(OnPvEBtnClick);
            pvpBtn.onClick.AddListener(OnPvPBtnClick);
            playerPageBtn.onClick.AddListener(OnPlayerPageBtnClick);
            taskPageBtn.onClick.AddListener(OnTaskPageBtnClick);

            TaskSystem taskSys = (Game.Instance as SrpgGame).taskSystem;
            taskSys.onTaskUnlockHandler += ShowTaskPageRedDot;
        }

        private void ShowTaskPageRedDot(Task t)
        {
            if (taskPageRedDot != null)
                taskPageRedDot.SetActive(true);
        }

        public override void OnOpen(object args = null)
        {
            base.OnOpen(args);
            EventDispatcher.DispatchEvent((int)EventId.OpenMainPage);
        }

        public override void OnDrop()
        {
            uiManager.Destroy(m_playerPage.wid);
            uiManager.Destroy(m_taskPage.wid);

            TaskSystem taskSys = (Game.Instance as SrpgGame).taskSystem;
            taskSys.onTaskUnlockHandler -= ShowTaskPageRedDot;

            base.OnDrop();
        }

        public void OnPvEBtnClick()
        {
            DestroyUI();
            (Game.Instance as SrpgGame).LoadScene("PvE");
        }

        public void OnPvPBtnClick()
        {
            DestroyUI();
            (Game.Instance as SrpgGame).LoadScene("MatchRoom");
        }

        public void OnPlayerPageBtnClick()
        {
            CharacterConfig[] characterConfigs = (Game.Instance as SrpgGame).characterConfigs.ToArray();
            uiManager.Push(m_playerPage, characterConfigs);
        }

        public void OnTaskPageBtnClick()
        {
            if (taskPageRedDot.activeInHierarchy)
                taskPageRedDot.SetActive(false);
            uiManager.Push(m_taskPage);
        }

        private void DestroyUI()
        {
            uiManager.DestroyStack();
        }
    }

}