using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;
using MyUtility;

namespace MySrpg.UI
{

    public class BattleUI : BaseUIWidget
    {
        private GameObject m_overheadUIPrefab;

        private AbilityPanel m_abilityPanel;
        private CharacterPanel m_characterPanel;
        private RoundNum m_roundNum;
        private RoundPlayer m_roundPlayer;

        private BattleSystem m_battleSystem;


        public override void OnCreate(UIManager uiMngr, object args = null)
        {
            base.OnCreate(uiMngr, args);

            m_abilityPanel = uiManager.Create<AbilityPanel>("AbilityPanel", "BattleUI", "AbilityPanel");
            m_characterPanel = uiManager.Create<CharacterPanel>("CharacterPanel", "BattleUI", "CharacterPanel");
            m_roundNum = uiManager.Create<RoundNum>("RoundNum", "BattleUI", "RoundNum", false);
            m_roundPlayer = uiManager.Create<RoundPlayer>("RoundPlayer", "BattleUI", "RoundPlayer", false);

            m_overheadUIPrefab = ResourceManager.Load<GameObject>("BattleUI", "Overhead");
            Debug.Assert(m_overheadUIPrefab != null);
        }

        /// <param name="args">BattleSystem</param>
        public override void OnOpen(object args = null)
        {
            if (args is null)
                throw new ArgumentNullException("BattleUI.Open requires BattleSystem as args");

            m_battleSystem = args as BattleSystem;
            if (m_battleSystem is null)
                throw new InvalidCastException("BattleUI.Open requires BattleSystem as args");

            base.OnOpen(args);

            m_battleSystem.onCharacterSpawnHandler += OnCharacterSpawn;
            m_battleSystem.onChangeRoundNumHandler += OnChangeRoundNum;
            m_battleSystem.onChangeRoundPlayerHandler += OnChangeRoundPlayer;
        }

        public override void OnClose()
        {
            uiManager.Close(m_abilityPanel);
            uiManager.Close(m_characterPanel);

            if (m_battleSystem != null)
            {
                m_battleSystem.onCharacterSpawnHandler -= OnCharacterSpawn;
                m_battleSystem.onChangeRoundNumHandler -= OnChangeRoundNum;
                m_battleSystem.onChangeRoundPlayerHandler -= OnChangeRoundPlayer;
            }

            base.OnClose();
        }

        public void OnCharacterSpawn(Character c)
        {
            GameObject go = Instantiate(m_overheadUIPrefab);
            go.name = $"Overhead_{c.gameObject.name}";
            go.transform.SetParent(uiManager.uiRoot);

            OverheadUI overhead = go.GetComponent<OverheadUI>();
            overhead.OnSetOwner(c);
        }

        public void OnChangeRoundNum(int round)
        {
            uiManager.Open(m_roundNum, round);
            Invoke(nameof(CloseRoundNum), 1.0f);
        }

        private void CloseRoundNum()
        {
            uiManager.Close(m_roundNum);
        }

        public void OnChangeRoundPlayer(int affiliation)
        {
            if (affiliation == 0)
            {
                Invoke(nameof(OpenRoundHumanPlayer), 1.0f);
                Invoke(nameof(CloseRoundPlayer), 2.0f);
            }
            else
            {
                uiManager.Open(m_roundPlayer, 1);
                Invoke(nameof(CloseRoundPlayer), 1.0f);
            }
        }

        private void OpenRoundHumanPlayer()
        {
            uiManager.Open(m_roundPlayer, 0);
        }

        private void CloseRoundPlayer()
        {
            uiManager.Close(m_roundPlayer);
        }

    }

}