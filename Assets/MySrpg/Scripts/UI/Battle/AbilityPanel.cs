using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyFramework;
using MyFramework.UI;
using MyUtility;
using Pathfinding;


namespace MySrpg.UI
{

    public class AbilityPanel : BaseUIWidget
    {
        public AbilityButton[] buttons;
        public Button cancelBtn;
        public Button passBtn;

        private HumanPlayer m_player;


        /// <param name="args">Ability[]</param>
        public override void OnOpen(object args = null)
        {
            if (args is null)
                throw new ArgumentNullException("AbilityPanel.Open requires Ability[] as args");

            Ability[] abilities = args as Ability[];
            if (abilities is null)
                throw new InvalidCastException("AbilityPanel.Open requires Ability[] as args");

            base.OnOpen(args);

            cancelBtn.interactable = false;
            cancelBtn.onClick.AddListener(OnClickCancelBtn);

            passBtn.interactable = true;
            passBtn.onClick.AddListener(OnClickPassBtn);

            m_player = (Game.Instance as SrpgGame).battleSystem.player0 as HumanPlayer;
            m_player.selection.onUseAbilityHandler += OnUseAbility;
            m_player.selection.onAbilityCdStartHandler += OnAbilityCdStart;
            m_player.selection.onAbilityCdUpdateHandler += OnAbilityCdUpdate;
            m_player.selection.onAbilityCdEndHandler += OnAbilityCdEnd;
            m_player.onCancelAbilityTargetSelectHandler += OnCancel;

            for (int i=0; i<abilities.Length; ++i)
            {
                buttons[i].OnBindAbility(abilities[i], i, this);
            }
        }

        public override void OnClose()
        {
            base.OnClose();

            m_player.selection.onUseAbilityHandler -= OnUseAbility;
            m_player.selection.onAbilityCdStartHandler -= OnAbilityCdStart;
            m_player.selection.onAbilityCdUpdateHandler -= OnAbilityCdUpdate;
            m_player.selection.onAbilityCdEndHandler -= OnAbilityCdEnd;
            m_player.onCancelAbilityTargetSelectHandler -= OnCancel;

            cancelBtn.onClick.RemoveListener(OnClickCancelBtn);
            passBtn.onClick.RemoveListener(OnClickPassBtn);

            for (int i = 0; i < buttons.Length; ++i)
            {
                buttons[i].OnUnbindAbility();
            }
        }

        public void OnClickAbilityBtn(int index)
        {
            Ability a = m_player.selection.abilities[index];
            if (a.autoFindTargets)
            {
                m_player.selection.UseAbility(index);
            }
            else
            {
                m_player.isSelectingTarget = true;
                m_player.pendingAbilityIndex = index;

                cancelBtn.interactable = true;
                passBtn.interactable = false;

                IndicatorSystem indicatorSys = (Game.Instance as SrpgGame).indicatorSystem;
                indicatorSys.HideMovableCells();
                indicatorSys.HideAttackableCells();
                indicatorSys.HideSelectionCell();
                a.StartFindPotentialPoints((List<Int3> points) =>
                {
                    Debug.Log($"found {points.Count} potential targets");
                    indicatorSys.ShowAbilityRangeCells(points.ToVector3().ToArray());
                });
            }
        }

        private void OnClickCancelBtn()
        {
            m_player.CancelAbilityTargetSelection();
        }

        private void OnClickPassBtn()
        {
            m_player.SelectNextCharacter();
        }

        private void OnCancel()
        {
            cancelBtn.interactable = false;
            passBtn.interactable = true;
        }

        private void OnUseAbility(int index)
        {
            cancelBtn.interactable = false;
            passBtn.interactable = false;
        }

        private void OnAbilityCdStart(int index, int cd)
        {
            buttons[index].OnCdStart(cd);
        }

        private void OnAbilityCdUpdate(int index, int cd)
        {
            buttons[index].OnCdUpdate(cd);
        }

        private void OnAbilityCdEnd(int index)
        {
            buttons[index].OnCdEnd();
        }

    }

}