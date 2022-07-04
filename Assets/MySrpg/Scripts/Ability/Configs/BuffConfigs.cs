using System;
using UnityEngine;
using LitJson;

namespace MySrpg
{


    [Serializable, PolymorphismJson]
    public class BuffConfig
    {
        public string name;
        public string iconPath;
        public int maxDuration;

        public virtual string DebugStr()
        {
            return $"name: {name}\nmaxDuration: {maxDuration}\n";
        }
    }

    [Serializable]
    public class BuffConfig_ModifyProperty : BuffConfig
    {
        public Character.PropertyIndex property;
        public double valueToAdd;

        public override string DebugStr()
        {
            return base.DebugStr() + $"property: {property}\nvalueToAdd: {valueToAdd}\n";
        }
    }


}