using UnityEngine;
using MyFramework;
using MyFramework.UI;
using MySrpg.UI;
using MyUtility;

namespace MySrpg
{

    public abstract class BattleLevelManager : LevelManager
    {
        protected int round;
        // local in PvE, global in PvP
        protected int roundPlayer;

        protected UIManager m_uiManager;
        protected BattleSystem m_battleSystem;
        protected IndicatorSystem m_indicatorSystem;

        public abstract void StartNextRound();

        public abstract void StartNextHalfRound();

        protected virtual void Awake()
        {
            SrpgGame game = Game.Instance as SrpgGame;

            m_battleSystem = game.AddSystem<BattleSystem>();
            m_indicatorSystem = game.AddSystem<IndicatorSystem>();
        }

        protected virtual void OnDestroy()
        {
            SrpgGame game = Game.Instance as SrpgGame;
            game.RemoveSystem<BattleSystem>();
            game.RemoveSystem<IndicatorSystem>();
        }

        protected virtual void InitUI()
        {
            m_uiManager = UIManager.Instance;
            m_uiManager.uiRoot = uiRoot;

            BattleUI battleUI = m_uiManager.Create<BattleUI>("BattleUI", "Prefabs/UI/Battle/BattleUI", true, this);
            m_uiManager.Open(battleUI, m_battleSystem);
        }

        protected virtual void OnOneSideAllDead(int affiliation)
        {
            bool won = affiliation != m_battleSystem.player0.playerAffiliation;
            (Game.Instance as SrpgGame).onBattleFinishHandler?.Invoke(won, 0);

            m_uiManager.Create<BattleResult>("BattleResult", "Prefabs/UI/Battle/BattleResult", false);

            float delay = 1.0f;
            if (!won)
            {
                Invoke(nameof(OpenLoseUI), delay);
            }
            else
            {
                Invoke(nameof(OpenWinUI), delay);
            }
        }

        protected void OpenWinUI()
        {
            m_uiManager.Open("BattleResult", true);
        }

        protected void OpenLoseUI()
        {
            m_uiManager.Open("BattleResult", false);
        }


    }

}