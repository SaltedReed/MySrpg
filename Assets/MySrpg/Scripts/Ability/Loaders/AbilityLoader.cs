using System.IO;
using UnityEngine;
using MyUtility;
using LitJson;

namespace MySrpg
{

    public static class AbilityLoader
    {

        public static Ability Load(string path, AbilityExecutor executor, Character character)
        {
            Debug.Assert(executor != null);
            Debug.Assert(character != null);

            /*if (!path.StartsWith("/"))
                path = "/" + path;
            path = Application.dataPath + path;*/

            if (path.EndsWith("json"))
            {
                return LoadAbilityFromJson(path, executor, character);
            }
            else
            {
                Debug.LogError($"failed to load {path}: unsupported format");
                return null;
            }
        }

        private static Ability LoadAbilityFromJson(string path, AbilityExecutor executor, Character character)
        {
            AbilityConfig c = null;
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                c = JsonMapper.ToObject<AbilityConfig>(json);
            }

            if (c is null)
                return null;

            Ability a = LoadAbilityConfig(c, executor, character);
            return a;
        }

        private static Ability LoadAbilityConfig(AbilityConfig c, AbilityExecutor executor, Character character)
        {
            Ability a = new Ability(c.abilityName, character, executor);
            a.icon = ResourceManager.Load<Sprite>("Textures", c.iconPath);
            a.timeLength = (float)c.timeLength;
            a.maxCooldown = c.cooldown;
            a.useOwnerRange = c.useOwnerRange;
            a.rangeCell = c.rangeCell;
            a.rangeType = c.rangeType;
            foreach (AbilityEventConfig aeConfig in c.abilityEvents)
            {
                AbilityEvent ae = LoadAbilityEventConfig(aeConfig);
                if (ae is null)
                    return null;

                a.AddEvent(ae);
            }

            return a;
        }


        private static AbilityEvent LoadAbilityEventConfig(AbilityEventConfig c)
        {
            AbilityEvent ae = null;

            if (c is AbilityEventConfig_PlayAnim)
            {
                AbilityEventConfig_PlayAnim realc = c as AbilityEventConfig_PlayAnim;

                AbilityEvent_PlayAnim realae = new AbilityEvent_PlayAnim();
                realae.timePoint = (float)realc.timePoint;
                realae.animKey = realc.animKey;

                ae = realae;
            }
            else if (c is AbilityEventConfig_StealDamage)
            {
                AbilityEventConfig_StealDamage realc = c as AbilityEventConfig_StealDamage;

                AbilityEvent_StealDamage realae = new AbilityEvent_StealDamage();
                realae.timePoint = (float)realc.timePoint;
                realae.useOwnerAttackment = realc.useOwnerAttackment;
                realae.baseDamage = (float)realc.baseDamage;
                realae.critChance = (float)realc.critChance;
                realae.critDamage = (float)realc.critDamage;
                realae.applyToSelf = realc.applyToSelf;

                ae = realae;
            }
            else if (c is AbilityEventConfig_SpawnVfx)
            {
                AbilityEventConfig_SpawnVfx realc = c as AbilityEventConfig_SpawnVfx;

                AbilityEvent_SpawnVfx realae = new AbilityEvent_SpawnVfx();
                realae.timePoint = (float)realc.timePoint;
                realae.path = realc.path;
                realae.onTarget = realc.onTarget;

                ae = realae;
            }
            else if (c is AbilityEventConfig_AddBuff)
            {
                AbilityEventConfig_AddBuff realc = c as AbilityEventConfig_AddBuff;

                AbilityEvent_AddBuff realae = new AbilityEvent_AddBuff();
                realae.timePoint = (float)realc.timePoint;
                realae.prototype = LoadBuffConfig(realc.prototype);
                realae.applyToSelf = realc.applyToSelf;

                ae = realae;
            }
            else if (c is AbilityEventConfig_SpawnProjectile)
            {
                AbilityEventConfig_SpawnProjectile realc = c as AbilityEventConfig_SpawnProjectile;

                AbilityEvent_SpawnProjectile realae = new AbilityEvent_SpawnProjectile();
                realae.timePoint = (float)realc.timePoint;
                realae.path = realc.path;
                realae.speed = (float)realc.speed;
                //realae.startOffset = new Vector3((float)realc.offsetX, (float)realc.offsetY, (float)realc.offsetZ);
                if (realc.onHit != null)
                {
                    realae.onHit = new AbilityEvent[realc.onHit.Length];
                    for (int i=0; i<realc.onHit.Length; ++i)
                    {
                        realae.onHit[i] = LoadAbilityEventConfig(realc.onHit[i]);
                    }
                }

                ae = realae;
            }
            else if (c is AbilityEventConfig_ShakeCamera)
            {
                AbilityEventConfig_ShakeCamera realc = c as AbilityEventConfig_ShakeCamera;

                AbilityEvent_ShakeCamera realae = new AbilityEvent_ShakeCamera();
                realae.timePoint = (float)realc.timePoint;

                ae = realae;
            }

            return ae;
        }


        private static BaseBuff LoadBuffConfig(BuffConfig c)
        {
            BaseBuff b = null;

            if (c is BuffConfig_ModifyProperty)
            {
                BuffConfig_ModifyProperty realc = c as BuffConfig_ModifyProperty;

                Buff_ModifyProperty realb = new Buff_ModifyProperty();
                realb.name = realc.name;
                if (!string.IsNullOrEmpty(realc.iconPath))
                    realb.icon = ResourceManager.Load<Sprite>("Textures", realc.iconPath);
                realb.maxDuration = realc.maxDuration;
                realb.property = realc.property;
                realb.valueToAdd = (float)realc.valueToAdd;

                b = realb;
            }

            return b;
        }
    }

}