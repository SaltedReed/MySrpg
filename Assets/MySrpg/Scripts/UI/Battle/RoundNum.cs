using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyFramework;
using MyFramework.UI;
using MySrpg;

namespace MySrpg.UI
{

    public class RoundNum : BaseUIWidget
    {
        public Text numText;

        public int round
        {
            get => int.Parse(numText.text);
            set => numText.text = value.ToString();
        }

        /// <param name="args">int</param>
        public override void OnOpen(object args = null)
        {
            if (args is null)
                throw new ArgumentNullException("RoundNum.Open requires int as args");

            base.OnOpen(args);
            round = (int)args;
        }
    }

}