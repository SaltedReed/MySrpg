using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyFramework.UI;

namespace MySrpg.UI
{

    [RequireComponent(typeof(ValueBarWidget))]
    public sealed class HpBar : MonoBehaviour
    {
        public ValueBarWidget valueBar { get; private set; }
        public Character owner { get; private set; }



        public void OnSetOwner(Character c)
        {
            valueBar = GetComponent<ValueBarWidget>();

            owner = c;
            if (c != null)
            {
                valueBar.MaxAmount = c.maxHp;
                valueBar.Amount = c.hp;
                c.onHpChangeHandler += OnOwnerChangeHealth;
            }
        }

        public void OnOwnerChangeHealth(float _, float newVal)
        {
            valueBar.Amount = newVal < 0.0f ? 0.0f : newVal;            
        }
    }

}