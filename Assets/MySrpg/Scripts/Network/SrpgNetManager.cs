using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MyFramework;

namespace MySrpg
{

    public sealed class SrpgNetManager : NetworkManager
    {
        public int globalAffiliation { get; private set; }

        public override void OnStartClient()
        {
            Debug.Log("SrpgNetManager OnStartClient");
            base.OnStartClient();

            Game game = Game.Instance;
            game.isNetGame = true;
            game.isClient = true;
        }

        public override void OnStartServer()
        {
            Debug.Log("SrpgNetManager OnStartServer");
            base.OnStartServer();

            Game game = Game.Instance;
            game.isNetGame = true;
            game.isServer = true;

            globalAffiliation = 1; // temp
        }


        public override void OnDestroy()
        {
            base.OnDestroy();

            Game game = Game.Instance;
            game.isNetGame = false;
            game.isServer = false;
            game.isClient = false;
        }
    }

}