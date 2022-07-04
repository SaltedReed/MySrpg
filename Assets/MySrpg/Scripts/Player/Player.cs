using System.Collections.Generic;
using UnityEngine;

namespace MySrpg
{

    public abstract class Player : MonoBehaviour
    {
        public int playerAffiliation;
        public BattleLevelManager lvlManager { get; set; }

        public virtual void StartPlayerRound() { }

        public virtual void FinishPlayerRound() { }
    }

}