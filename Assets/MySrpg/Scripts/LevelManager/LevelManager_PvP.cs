

namespace MySrpg
{

    public class LevelManager_PvP : BattleLevelManager
    {
        public CharacterStart[] starts;
        public int globalAffiliation { get; set; }

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
            if (globalAffiliation == roundPlayer)
                m_battleSystem.player1.FinishPlayerRound();
            else
                m_battleSystem.player0.FinishPlayerRound();

            m_battleSystem.onChangeRoundPlayerHandler?.Invoke(roundPlayer);
            Invoke(nameof(StartGlobalPlayer0), 2.1f); // wait for ui, temp
        }

        protected virtual void StartGlobalPlayer0()
        {
            if (globalAffiliation == 0)
                m_battleSystem.player0.StartPlayerRound();
            else
                m_battleSystem.player1.StartPlayerRound();
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
            if (globalAffiliation == roundPlayer)
                m_battleSystem.player1.FinishPlayerRound();
            else
                m_battleSystem.player0.FinishPlayerRound();

            m_battleSystem.onChangeRoundPlayerHandler?.Invoke(roundPlayer);
            Invoke(nameof(StartGlobalPlayer1), 1.5f); // wait for ui, temp
        }

        protected virtual void StartGlobalPlayer1()
        {
            if (globalAffiliation == 1)
                m_battleSystem.player0.StartPlayerRound();
            else
                m_battleSystem.player1.StartPlayerRound();
        }

        protected virtual void Start()
        {
            SrpgNetManager netManager = FindObjectOfType<SrpgNetManager>();
            globalAffiliation = netManager.globalAffiliation;

            InitUI();

            m_battleSystem.CreatePlayer<HumanPlayer>(0);
            m_battleSystem.player0.lvlManager = this;
            m_battleSystem.CreatePlayer<NetPlayer>(1);
            m_battleSystem.player1.lvlManager = this;

            SpawnCharacters();
            m_battleSystem.onOneSideAllDeadHandler += OnOneSideAllDead;

            round = 0;
            StartNextRound();
        }


        protected virtual void SpawnCharacters()
        {
            foreach (CharacterStart s in starts)
            {
                m_battleSystem.SpawnCharacter(s.prefab, s.transform, s.affiliation==globalAffiliation?0:1);
                Destroy(s.gameObject);
            }
        }
    }

}