using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;
using MyFramework.UI;
using MySrpg;

namespace MyTest
{

    public class Test_Game : Game
    {
        public BattleSystem battleSystem { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            battleSystem = AddSystem<BattleSystem>();
        }
    }

}