using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyFramework;
using MyFramework.UI;
using MySrpg;

namespace MySrpg.UI
{


    public class RoundPlayer : BaseUIWidget
    {
        public Text playerText;

        /// <param name="args">int</param>
        public override void OnOpen(object args = null)
        {
            if (args is null)
                throw new ArgumentNullException("RoundPlayer.Open requires int as args");

            int who = (int)args;

            base.OnOpen(args);

            if (who == 0)
                playerText.text = "ио╟К";
            else
                playerText.text = "об╟К";
        }
    }

}