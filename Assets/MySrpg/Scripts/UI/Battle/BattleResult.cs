using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MyFramework.UI;

namespace MySrpg.UI
{

    public class BattleResult : BaseUIWidget, IPointerClickHandler
    {
        public GameObject winUI;
        public GameObject loseUI;

        /// <param name="args">bool</param>
        public override void OnOpen(object args = null)
        {
            if (args is null)
                throw new ArgumentNullException("BattleResult.Open requires bool as args");

            bool win = (bool)args;

            base.OnOpen(args);

            if (win)
            {
                winUI.SetActive(true);
                loseUI.SetActive(false);
            }
            else
            {
                winUI.SetActive(false);
                loseUI.SetActive(true);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            uiManager.Close(this);

            BattleTrophyResult ui = uiManager.Create<BattleTrophyResult>("BattleTrophyResult", "BattleUI", "BattleTrophyResult", false);
            uiManager.Open(ui);
        }
    }

}