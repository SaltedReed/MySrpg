using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MyFramework;

namespace MySrpg
{

    public class NetPlayer : Player
    {
        private BattleSystem m_battleSys;
                

        private void Awake()
        {
            Debug.Log("NetPlayer Awake");

            SrpgGame game = Game.Instance as SrpgGame;
            m_battleSys = game.battleSystem;

            if (game.isServer)
            {
                Debug.Log("NetPlayer register server callbacks");
                NetworkServer.RegisterHandler<Msg_UseAbility>(Server_OnMsgUseAbility);
                NetworkServer.RegisterHandler<Msg_FollowPath>(Server_OnMsgFollowPath);
                NetworkServer.RegisterHandler<Msg_FinishHalfRound>(Server_OnMsgFinishHalfRound);
            }
            else
            {
                Debug.Log("NetPlayer register client callbacks");
                NetworkClient.RegisterHandler<Msg_UseAbility>(Client_OnMsgUseAbility);
                NetworkClient.RegisterHandler<Msg_FollowPath>(Client_OnMsgFollowPath);
                NetworkClient.RegisterHandler<Msg_FinishHalfRound>(Client_OnMsgFinishHalfRound);
            }
        }

        private void OnDestroy()
        {
            if (Game.Instance.isServer)
            {
                NetworkServer.UnregisterHandler<Msg_UseAbility>();
                NetworkServer.UnregisterHandler<Msg_FollowPath>();
                NetworkServer.UnregisterHandler<Msg_FinishHalfRound>();
            }
            else
            {
                NetworkClient.UnregisterHandler<Msg_UseAbility>();
                NetworkClient.UnregisterHandler<Msg_FollowPath>();
                NetworkClient.UnregisterHandler<Msg_FinishHalfRound>();
            }
        }

        private void Server_OnMsgUseAbility(NetworkConnection conn, Msg_UseAbility msg)
        {
            OnMsgUseAbility(msg);
        }

        private void Client_OnMsgUseAbility(Msg_UseAbility msg)
        {
            if (!Game.Instance.isServer)
                OnMsgUseAbility(msg);
        }

        private void OnMsgUseAbility(Msg_UseAbility msg)
        {
            Character selection = m_battleSys.characters1[msg.characterIndex];
            if (msg.targetIndex >= 0)
                selection.playerSelectedTarget = m_battleSys.characters0[msg.targetIndex];
            selection.UseAbility(msg.abilityIndex);
        }

        private void Server_OnMsgFollowPath(NetworkConnection conn, Msg_FollowPath msg)
        {
            OnMsgFollowPath(msg);
        }

        private void Client_OnMsgFollowPath(Msg_FollowPath msg)
        {
            if (!Game.Instance.isServer)
                OnMsgFollowPath(msg);
        }

        private void OnMsgFollowPath(Msg_FollowPath msg)
        {
            Character selection = m_battleSys.characters1[msg.characterIndex];
            selection.pathToFollow = msg.path;
            selection.StartFollowPath(msg.lookAtLastNode, null); // todo: deal with delay for atk after path
        }

        private void Server_OnMsgFinishHalfRound(NetworkConnection conn, Msg_FinishHalfRound msg)
        {
            OnMsgFinishHalfRound(msg);
        }

        private void Client_OnMsgFinishHalfRound(Msg_FinishHalfRound msg)
        {
            if (!Game.Instance.isServer)
                OnMsgFinishHalfRound(msg);
        }

        private void OnMsgFinishHalfRound(Msg_FinishHalfRound msg)
        {
            lvlManager.StartNextHalfRound();
        }

    }

}