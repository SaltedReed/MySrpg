using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySrpg;
using MyFramework;

namespace MyTest
{

    public class Test_ChangeRound : MonoBehaviour
    {
        public LevelManager_PvE pveManager;

        private void OnGUI()
        {
            if (GUILayout.Button("next"))
            {
                pveManager.StartNextRound();
            }
        }
    }

}