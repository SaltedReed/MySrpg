using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;
using Pathfinding;

namespace MySrpg
{

    public enum RangeType
    {
        AllDir
    }


    public delegate void OnFindPotentialPointsHandler(List<Int3> points);


    public sealed class Ability
    {
        public string abilityName { get; private set; }
        public Sprite icon { get; set; }

        public float timeLength { get; set; }

        public int maxCooldown { get; set; }
        public int cooldown { get; private set; }

        public bool autoFindTargets { get; set; }

        public bool useOwnerRange { get; set; }

        public int rangeCell
        {
            get => useOwnerRange ? owner.attackmentRangeCell : m_rangeCell;
            set
            {
                if (!useOwnerRange)
                    m_rangeCell = value;
                else
                    Debug.LogWarning($"{abilityName} useOwnerRange=true, cannot change rangeCell");
            }
        }
        private int m_rangeCell;

        public RangeType rangeType
        {
            get => useOwnerRange ? RangeType.AllDir : m_rangeType;
            set
            {
                if (!useOwnerRange)
                    m_rangeType = value;
                else
                    Debug.LogWarning($"{abilityName} useOwnerRange=true, cannot change rangeType");
            }
        }
        public RangeType m_rangeType;

        public List<AbilityEvent> abilityEvents => m_sortedAbilityEvents;
        private List<AbilityEvent> m_sortedAbilityEvents = new List<AbilityEvent>();

        public Character owner { get; private set; }
        public int index { get; set; }

        public List<Character> targets => m_targets;
        private List<Character> m_targets = new List<Character>();

        private AbilityExecutor m_executor;


        public Ability(string name, Character ownerCharacter, AbilityExecutor executor)
        {
            abilityName = name;
            owner = ownerCharacter;
            m_executor = executor;
            cooldown = 0;
        }

        public void AddEvent(AbilityEvent ae)
        {
            if (ae is null)
                throw new ArgumentNullException();

            for (int i=0; i<m_sortedAbilityEvents.Count; ++i)
            {
                if (m_sortedAbilityEvents[i].timePoint > ae.timePoint)
                {
                    m_sortedAbilityEvents.Insert(i, ae);
                    return;
                }
            }
            m_sortedAbilityEvents.Add(ae);
        }

        public void StartFindPotentialPoints(OnFindPotentialPointsHandler handler)
        {
            // temp
            RangeFromOnePoint range = new RangeFromOnePoint();
            range.startPos = owner.transform.position;
            range.rangeCell = rangeCell;
            range.handler = (RangeFromOnePoint p) =>
            {
                List<Int3> points = new List<Int3>();
                List<Character> enemies = (Game.Instance as SrpgGame).battleSystem.GetEnemies(owner.affiliation);
                foreach (Character e in enemies)
                {
                    Int3 point = e.mapNode.position;
                    if (p.points.Contains(point))
                    {
                        points.Add(point);
                    }
                }

                handler?.Invoke(points);
            };

            range.StartFind();
        }

        /// <returns>the number of the targets found</returns>
        public int FindTargets(GraphNode node, Character c)
        {
            // temp
            ClearTargets();

            float maxDist = rangeCell * (AstarPath.active.graphs[0] as GridGraph).nodeSize + 0.1f;
            if (c != null && (c.transform.position-owner.transform.position).sqrMagnitude <= maxDist * maxDist)
            {
                AddTarget(c);
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void Use(Action onComplete = null)
        {
            //Debug.Log($"{abilityName} is used");

            cooldown = maxCooldown;
            owner.onAbilityCdStartHandler?.Invoke(index, maxCooldown);

            // otherwise m_targets has been filled
            if (autoFindTargets)
            {
                FindTargets();
            }

            if (m_targets.Count > 0)
            {
                m_executor.Execute(ExecuteEventsCoroutine(onComplete));
            }
            else
            {
                Debug.LogWarning($"{abilityName} is to be used, but no target found");
            }
        }

        private void FindTargets()
        {
            BattleSystem battleSys = (Game.Instance as SrpgGame).battleSystem;
            List<Character> potentials = battleSys.GetEnemies(owner.affiliation);

            float maxDist = rangeCell * (AstarPath.active.graphs[0] as GridGraph).nodeSize + 0.1f;
            Vector2 xzOwner = new Vector2(owner.transform.position.x, owner.transform.position.z);
            foreach (Character c in potentials)
            {
                Vector2 xzPotential = new Vector2(c.transform.position.x, c.transform.position.z);
                // temp
                if ((xzOwner - xzPotential).sqrMagnitude <= maxDist)
                {
                    AddTarget(c);
                }
            }
        }

        private IEnumerator ExecuteEventsCoroutine(Action onComplete)
        {
            float startTime = Time.time;
            float wait;
            foreach (AbilityEvent ae in m_sortedAbilityEvents)
            {
                if (ae is null)
                {
                    Debug.LogError($"{(owner is null ? "null" : owner.characterName)}'s ability {abilityName} has a null event");
                    continue;
                }

                wait = ae.timePoint - (Time.time - startTime);
                if (wait > 0.0f)
                    yield return new WaitForSeconds(wait);

                ae.Execute(this);
            }

            wait = timeLength - (Time.time - startTime);
            if (wait > 0.0f)
                yield return new WaitForSeconds(wait);

            m_executor.OnComplete();
            onComplete?.Invoke();

            ClearTargets();
        }

        public bool HasTarget(Character target)
        {
            return target != null && m_targets.Contains(target);
        }

        public void AddTarget(Character target)
        {
            if (target is null)
                throw new ArgumentNullException();

            m_targets.Add(target);
        }

        public void ClearTargets()
        {
            m_targets.Clear();
        }

        public void OnTick()
        {
            if (index == 0)
                return;

            if (cooldown <= 0)
                return;

            --cooldown;
            owner.onAbilityCdUpdateHandler?.Invoke(index, cooldown);

            if (cooldown == 0)
            {
                owner.onAbilityCdEndHandler?.Invoke(index);
            }
        }
    }

}