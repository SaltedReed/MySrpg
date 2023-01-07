using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Pathfinding;

namespace MySrpg
{

    public struct Msg_Join : NetworkMessage
    {

    }

    public struct Msg_UseAbility : NetworkMessage
    {
        public int characterIndex;
        public int abilityIndex;
        public int targetIndex;
    }


    public struct Msg_FollowPath : NetworkMessage
    {
        public int characterIndex;
        public List<Int3> path;
        public bool lookAtLastNode;
    }


    public struct Msg_FinishHalfRound : NetworkMessage
    {

    }

}