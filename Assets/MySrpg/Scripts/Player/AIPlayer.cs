using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;

namespace MySrpg
{

    public class AIPlayer : Player
    {
        private BattleSystem m_battleSys;
        private int m_curSelectIndex;

        public override void StartPlayerRound()
        {
            m_curSelectIndex = -1;
            SelectNextCharacter();
        }

        private void SelectNextCharacter()
        {
            if (m_curSelectIndex == m_battleSys.count1 - 1)
            {
                lvlManager.StartNextRound();
            }
            else
            {
                DecideAbility(m_battleSys.characters1[++m_curSelectIndex]);
            }
        }

        private void OnCharacterFinishAbility(Character c, Ability a)
        {
            if (c.affiliation == playerAffiliation)
            {
                Invoke(nameof(SelectNextCharacter), 0.5f);
            }
        }

        private void DecideAbility(Character c)
        {
            // temp
            if (m_curSelectIndex == 0)
            {
                c.abilities[0].AddTarget(m_battleSys.GetEnemies(playerAffiliation)[0]);
                c.UseAbility(0);
            }
            else
            {
                c.abilities[1].AddTarget(m_battleSys.GetEnemies(playerAffiliation)[1]);
                c.UseAbility(1);
            }
        }

        private void Awake()
        {
            m_battleSys = (Game.Instance as SrpgGame).battleSystem;
            m_battleSys.onCharacterFinishAbilityHandler += OnCharacterFinishAbility;
        }
    }

}