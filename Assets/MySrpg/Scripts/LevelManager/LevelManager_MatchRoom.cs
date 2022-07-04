using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;

namespace MySrpg
{

    public class LevelManager_MatchRoom : LevelManager
    {
        public float offsetX = 10;
        public float offsetY = 150;

        private void OnGUI()
        {
            if (GUI.Button(new Rect(offsetX, offsetY, 100, 19), "enter"))
            {
                (Game.Instance as SrpgGame).LoadScene("PvP");
            }
        }
    }

}