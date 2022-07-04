using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MySrpg;
using MyFramework;
using MyFramework.UI;

namespace MySrpg.UI
{

    public class BattleTrophyResult : BaseUIWidget, IPointerClickHandler
    {
        public GameObject listItemPrefab;
        public Transform list;

        public override void OnOpen(object args = null)
        {
            base.OnOpen(args);

            BattleSystem battleSys = (Game.Instance as SrpgGame).battleSystem;
            List<BattleTrophy> result = battleSys.CalculateTrophies(battleSys.player0.playerAffiliation);
            foreach (BattleTrophy bt in result)
            {
                GameObject itemGo = Instantiate(listItemPrefab);
                itemGo.transform.SetParent(list);

                TrophyListItem item = itemGo.GetComponent<TrophyListItem>();
                item.icon = bt.character.icon;
                item.exp = bt.exp;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            uiManager.Close(this);
            Invoke(nameof(LoadMain), 0.5f);
        }

        private void LoadMain()
        {
            (Game.Instance as SrpgGame).LoadScene("Main");
        }
    }

}