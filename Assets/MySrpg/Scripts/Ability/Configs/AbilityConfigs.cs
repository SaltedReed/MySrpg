using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace MySrpg
{


    #region AbilityEventConfigs

    [Serializable, PolymorphismJson]
    public class AbilityEventConfig
    {
        public double timePoint;

        public virtual string DebugStr()
        {
            return $"timePoint: {timePoint}\n";
        }
    }


    [Serializable]
    public class AbilityEventConfig_PlayAnim : AbilityEventConfig
    {
        public string animKey;

        public override string DebugStr()
        {
            return base.DebugStr() + $"animKey: {animKey}\n";
        }
    }

    [Serializable]
    public class AbilityEventConfig_SpawnVfx : AbilityEventConfig
    {
        public string path;
        public bool onTarget;

        public override string DebugStr()
        {
            return base.DebugStr() + $"path: {path}\nonTarget: {onTarget}\n";
        }
    }

    [Serializable]
    public class AbilityEventConfig_StealDamage : AbilityEventConfig
    {
        public bool useOwnerAttackment;
        public double baseDamage;
        public double critChance;
        public double critDamage;
        public bool applyToSelf;

        public override string DebugStr()
        {
            return base.DebugStr() +
                $"useOwnerAttackment: {useOwnerAttackment}\n" +
                $"baseDamage: {baseDamage}\n" +
                $"critChance: {critChance}\n" +
                $"critDamage: {critDamage}\n" +
                $"applyToSelf: {applyToSelf}\n";
        }

    }


    [Serializable]
    public class AbilityEventConfig_AddBuff : AbilityEventConfig
    {
        public BuffConfig prototype;
        public bool applyToSelf;

        public override string DebugStr()
        {
            return base.DebugStr() + $"prototype:\n{prototype.DebugStr()}\napplyToSelf: {applyToSelf}\n";
        }
    }


    [Serializable]
    public class AbilityEventConfig_SpawnProjectile : AbilityEventConfig
    {
        public string path;
        public double speed;
        //public double offsetX;
        //public double offsetY;
        //public double offsetZ;
        public AbilityEventConfig[] onHit;

        public override string DebugStr()
        {
            string str = base.DebugStr() + 
                $"path: {path}\n" + 
                $"speed: {speed}" /* + 
                $"offset: ({offsetX}, {offsetY}, {offsetZ})"*/;
            if (onHit != null)
            {
                str += "onHit:\n";
                foreach (AbilityEventConfig c in onHit)
                {
                    str += c.DebugStr();
                }
            }

            return str;
        }
    }


    [Serializable]
    public class AbilityEventConfig_ShakeCamera : AbilityEventConfig
    {
    }

    #endregion


    [Serializable]
    public class AbilityConfig
    {
        public string abilityName;
        public string iconPath;
        public double timeLength;
        public int cooldown;
        public bool useOwnerRange;
        public int rangeCell;
        public RangeType rangeType;
        public List<AbilityEventConfig> abilityEvents;

        public string DebugStr()
        {
            string str = $"abilityName: {abilityName}\n" +
                $"iconPath: {iconPath}\n" +
                $"timeLength: {timeLength}\n" +
                $"cooldown: {cooldown}" +
                $"useOwnerRange: {useOwnerRange}\n" +
                $"rangeCell: {rangeCell}\n" +
                $"rangeType: {rangeType.ToString()}\n" +
                $"abilityEvents:\n";
            foreach (AbilityEventConfig c in abilityEvents)
                str += c.DebugStr() + "\n";
            return str;
        }
    }

}
