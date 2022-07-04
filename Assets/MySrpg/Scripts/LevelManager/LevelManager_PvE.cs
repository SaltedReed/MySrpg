using UnityEngine;
using MyFramework;
using MyFramework.UI;
using MySrpg.UI;
using MyUtility;

namespace MySrpg
{

    public class LevelManager_PvE : BattleLevelManager
    {
        public override void StartNextRound()
        {
            // todo: battleSystem.isOver
            if (m_battleSystem.isAnySideAllDead)
                return;

            if (round < 1)
                DoStartNextRound();
            else
                Invoke(nameof(DoStartNextRound), 0.5f);
        }

        protected virtual void DoStartNextRound()
        {
            m_battleSystem.onPreChangeRoundNumHandler?.Invoke(round + 1);

            round++;
            m_battleSystem.onChangeRoundNumHandler?.Invoke(round);

            roundPlayer = 0;
            m_battleSystem.player1.FinishPlayerRound();
            m_battleSystem.onChangeRoundPlayerHandler?.Invoke(roundPlayer);

            Invoke(nameof(StartFriendRound), 2.1f); // wait for ui, temp
        }

        protected void StartFriendRound()
        {
            m_battleSystem.player0.StartPlayerRound();

            if (round == 1)
            {
                EventDispatcher.DispatchEvent((int)EventId.StartBattle);
            }
        }

        public override void StartNextHalfRound()
        {
            if (m_battleSystem.isAnySideAllDead)
                return;

            if (roundPlayer == 1)
            {
                StartNextRound();
                return;
            }

            Invoke(nameof(DoStartNextHalfRound), 0.5f);
        }

        protected virtual void DoStartNextHalfRound()
        {
            roundPlayer = 1;
            m_battleSystem.player0.FinishPlayerRound();
            m_battleSystem.onChangeRoundPlayerHandler?.Invoke(roundPlayer);

            Invoke(nameof(StartEnemyRound), 1.5f); // wait for ui, temp
        }

        protected void StartEnemyRound()
        {
            m_battleSystem.player1.StartPlayerRound();
        }


        protected virtual void Start()
        {
            InitUI();

            m_battleSystem.CreatePlayer<HumanPlayer>(0);
            m_battleSystem.player0.lvlManager = this;
            m_battleSystem.CreatePlayer<AIPlayer>(1);
            m_battleSystem.player1.lvlManager = this;

            SpawnCharacters();
            m_battleSystem.onOneSideAllDeadHandler += OnOneSideAllDead;

            round = 0;
            StartNextRound();
        }

        protected virtual void SpawnCharacters()
        {
            CharacterStart[] starts = FindObjectsOfType<CharacterStart>();
            foreach (CharacterStart s in starts)
            {
                m_battleSystem.SpawnCharacter(s.prefab, s.transform, s.affiliation);
                Destroy(s.gameObject);
            }
        }

    }

}