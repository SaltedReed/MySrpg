using UnityEngine;
using MyUtility;

namespace MySrpg
{

    public abstract class AbilityEvent
    {
        public float timePoint;

        public abstract void Execute(Ability ability);
    }


    public class AbilityEvent_PlayAnim : AbilityEvent
    {
        public string animKey;

        public override void Execute(Ability ability)
        {
            //Debug.Log($"executing AbilityEvent_PlayAnim");

            ability.owner.animator.SetTrigger(animKey);
        }
    }


    public class AbilityEvent_SpawnVfx : AbilityEvent
    {
        public string path;
        public bool onTarget;

        public override void Execute(Ability ability)
        {
            //Debug.Log($"executing AbilityEvent_SpawnVfx");

            GameObject prefab = ResourceManager.Load<GameObject>("Vfx", path); Debug.Assert(prefab!=null);

            Transform trans;
            if (onTarget)
            {
                foreach (Character c in ability.targets)
                {
                    trans = c.transform;
                    VfxManager.SpawnParticle(prefab, prefab.transform.position, trans.localToWorldMatrix, Quaternion.identity);
                }
            }
            else
            {
                trans = ability.owner.transform;
                VfxManager.SpawnParticle(prefab, prefab.transform.position, trans.localToWorldMatrix, Quaternion.identity);
            }
        }
    }


    public class AbilityEvent_StealDamage : AbilityEvent
    {
        public bool useOwnerAttackment;
        // will not be used if useOwnerAttackment is true
        public float baseDamage;
        // [0, 1]
        public float critChance;
        public float critDamage;
        public bool applyToSelf;

        public override void Execute(Ability ability)
        {
            //Debug.Log($"executing AbilityEvent_StealDamage");

            float absDmg;
            if (applyToSelf)
            {
                absDmg = CalculateAbsDamage(ability, ability.owner);
                ability.owner.TakeDamage(absDmg, ability.owner);
            }
            else
            {
                if (ability.targets is null)
                {
                    return;
                }

                foreach (Character target in ability.targets)
                {
                    absDmg = CalculateAbsDamage(ability, target);
                    target.TakeDamage(absDmg, ability.owner);
                }
            }
        }

        private float CalculateAbsDamage(Ability ability, Character target)
        {
            float absDmg = (useOwnerAttackment ? ability.owner.attackment : baseDamage) - target.defense;
            if (Random.Range(0.0f, 1.0f) >= critChance)
            {
                absDmg += critDamage;
            }
            absDmg = Mathf.Max(0, absDmg);

            return absDmg;
        }
    }


    public class AbilityEvent_AddBuff : AbilityEvent
    {
        public BaseBuff prototype;
        public bool applyToSelf;

        public override void Execute(Ability ability)
        {
            //Debug.Log($"executing AbilityEvent_AddBuff");

            BaseBuff buff = prototype.Copy();
            buff.ability = ability;

            if (applyToSelf)
            {
                ability.owner.AttachBuff(buff);
            }
            else
            {
                if (ability.targets is null)
                    return;

                foreach (Character target in ability.targets)
                {
                    target.AttachBuff(buff);
                }
            }
        }
    }


    public class AbilityEvent_SpawnProjectile : AbilityEvent
    {
        public string path;
        public float speed;
        //public Vector3 startOffset;
        public AbilityEvent[] onHit;

        public override void Execute(Ability ability)
        {
            //Debug.Log($"executing AbilityEvent_SpawnProjectile");

            GameObject prefab = ResourceManager.Load<GameObject>("Projectiles", path);

            GameObject go = GameObject.Instantiate(prefab);

            Vector3 pos = ability.owner.transform.position;
            pos.y += 0.8f;
            go.transform.position = pos;

            Vector3 targetPos = ability.targets[0].transform.position;
            targetPos.y += 0.8f;
            go.transform.LookAt(targetPos);

            Projectile proj = go.GetComponent<Projectile>();
            proj.ability = ability;
            proj.speed = speed;
            proj.onHit = onHit;
            proj.target = ability.targets[0];
        }
    }


    public class AbilityEvent_ShakeCamera : AbilityEvent
    {
        // temp

        public override void Execute(Ability ability)
        {
            //Debug.Log($"executing AbilityEvent_ShakeCamera");

            Camera cam = Camera.main;
            BaseCameraShake cs = cam.GetComponent<BaseCameraShake>();
            if (cs is null)
            {
                cs = cam.gameObject.AddComponent<CameraShake_Count>();
                cs.camera = cam;
            }
            cs.Begin();
        }
    }

}