using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using MyFramework;

namespace MySrpg
{

    public class BattleTrophy
    {
        public CharacterConfig character;
        public float exp;
    }


    public delegate void OnCharacterSpawnHandler(Character c);
    public delegate void OnCharacterFinishAbilityHandler(Character c, Ability a);
    public delegate void OnOneSideAllDeadHandler(int affiliation);
    public delegate void OnPreChangeRoundNumHandler(int round);
    public delegate void OnChangeRoundNumHandler(int round);
    public delegate void OnChangeRoundPlayerHandler(int affiliation);


    public class BattleSystem : BaseGameSystem
    {
        public Player player0 { get; private set; }
        public List<Character> characters0 => m_characters0;
        private List<Character> m_characters0 = new List<Character>();
        public List<Character> aliveCharacters0 => m_characters0.FindAll(IsAlive);
        //public int count0 => m_characters0.FindAll(IsAlive).Count;

        public Player player1 { get; private set; }
        public List<Character> characters1 => m_characters1;
        private List<Character> m_characters1 = new List<Character>();
        public List<Character> aliveCharacters1 => m_characters1.FindAll(IsAlive);
        //public int count1 => m_characters1.FindAll(IsAlive).Count;

        public bool isAnySideAllDead => aliveCharacters0.Count == 0 || aliveCharacters1.Count == 0;

        public event OnCharacterSpawnHandler onCharacterSpawnHandler;
        public event OnOneSideAllDeadHandler onOneSideAllDeadHandler;
        public OnPreChangeRoundNumHandler onPreChangeRoundNumHandler;
        public OnChangeRoundNumHandler onChangeRoundNumHandler;
        public OnChangeRoundPlayerHandler onChangeRoundPlayerHandler;
        public OnCharacterFinishAbilityHandler onCharacterFinishAbilityHandler;

        protected override void Awake()
        {
            base.Awake();

            onPreChangeRoundNumHandler += OnPreChangeRoundNum;
        }

        private void OnPreChangeRoundNum(int round)
        {
            Debug.Log("OnPreChangeRoundNum");
            if (characters0 != null)
            {
                foreach (Character c in characters0)
                {
                    c.OnTick();
                }
            }
            if (characters1 != null)
            {
                foreach (Character c in characters1)
                {
                    c.OnTick();
                }
            }
        }

        public void CreatePlayer<T>(int affiliation) where T : Player
        {
            GameObject go = new GameObject($"{typeof(T).Name}_{affiliation}");
            T p = go.AddComponent<T>();
            p.playerAffiliation = affiliation;

            if (affiliation == 0)
                player0 = p;
            else
                player1 = p;
        }

        /*public void CreateHumanPlayer(int affiliation)
        {
            GameObject go = new GameObject($"HumanPlayer_{affiliation}");
            HumanPlayer p = go.AddComponent<HumanPlayer>();
            p.playerAffiliation = affiliation;

            if (affiliation == 0)
                player0 = p;
            else
                player1 = p;
        }

        public void CreateAIPlayer(int affiliation)
        {
            GameObject go = new GameObject($"AIPlayer_{affiliation}");
            AIPlayer p = go.AddComponent<AIPlayer>();
            p.playerAffiliation = affiliation;

            if (affiliation == 0)
                player0 = p;
            else
                player1 = p;
        }*/

        public void SpawnCharacter(GameObject prefab, Transform start, int affiliation)
        {
            GameObject go = Instantiate(prefab);
            go.transform.position = start.position;
            go.transform.rotation = start.rotation;

            Character c = go.GetComponent<Character>();
            c.affiliation = affiliation;
            c.OnSpawn();
            c.onHpChangeHandler += (float _, float newVal) =>
            {
                if (newVal <= 0.0f)
                {
                    if (aliveCharacters0.Count == 0)
                        onOneSideAllDeadHandler?.Invoke(0);
                    else if (aliveCharacters1.Count == 0)
                        onOneSideAllDeadHandler?.Invoke(1);
                }
            };

            if (affiliation == 0)
            {
                c.index = characters0.Count;
                characters0.Add(c);
            }
            else
            {
                c.index = characters1.Count;
                characters1.Add(c);
            }

            onCharacterSpawnHandler?.Invoke(c);
        }


        public List<Character> GetEnemies(int affiliation)
        {
            if (affiliation == 0)
                return m_characters1.FindAll(IsAlive);
            else
                return m_characters0.FindAll(IsAlive);
        }

        public List<Character> GetFriends(int affiliation)
        {
            if (affiliation == 1)
                return m_characters1.FindAll(IsAlive);
            else
                return m_characters0.FindAll(IsAlive);
        }

        public List<Character> GetCharacters()
        {
            List<Character> result = m_characters0.FindAll(IsAlive);
            result.AddRange(m_characters1.FindAll(IsAlive));
            return result;
        }

        public Character GetCharacterOn(Int3 mapPos)
        {
            List<Character> characters = GetCharacters();
            foreach (Character c in characters)
            {
                if (c.mapNode.position == mapPos)
                {
                    return c;
                }
            }
            return null;
        }

        private bool IsAlive(Character c)
        {
            return c != null && c.hp > 0;
        }

        public List<GraphNode> GetCharacterNodes()
        {
            List<Character> cs = GetCharacters();
            List<GraphNode> nodes = new List<GraphNode>(cs.Count);
            for (int i = 0; i < cs.Count; ++i)
            {
                nodes.Add(cs[i].mapNode);
            }

            return nodes;
        }

        public List<GraphNode> GetEnemyNodes(int affiliation)
        {
            List<GraphNode> nodes;
            if (affiliation == 0)
            {
                List<Character> cs1 = m_characters1.FindAll(IsAlive);
                nodes = new List<GraphNode>(cs1.Count);
                for (int i = 0; i<cs1.Count; ++i)
                {
                    nodes.Add(cs1[i].mapNode);
                }
            }
            else
            {
                List<Character> cs0 = m_characters0.FindAll(IsAlive);
                nodes = new List<GraphNode>(cs0.Count);
                for (int i = 0; i < cs0.Count; ++i)
                {
                    nodes.Add(cs0[i].mapNode);
                }
            }
            return nodes;
        }

        public List<GraphNode> GetFriendNodes(int affiliation)
        {
            List<GraphNode> nodes;
            if (affiliation == 1)
            {
                List<Character> cs1 = m_characters1.FindAll(IsAlive);
                nodes = new List<GraphNode>(cs1.Count);
                for (int i = 0; i < cs1.Count; ++i)
                {
                    nodes.Add(cs1[i].mapNode);
                }
            }
            else
            {
                List<Character> cs0 = m_characters0.FindAll(IsAlive);
                nodes = new List<GraphNode>(cs0.Count);
                for (int i = 0; i < cs0.Count; ++i)
                {
                    nodes.Add(cs0[i].mapNode);
                }
            }
            return nodes;
        }

        public List<BattleTrophy> CalculateTrophies(int affiliation)
        {
            List<BattleTrophy> result;
            if (affiliation == 0)
            {
                result = new List<BattleTrophy>(characters0.Count);
                for (int i=0; i<characters0.Count; ++i)
                {
                    result.Add(new BattleTrophy { character = characters0[i].config, exp = Random.Range(8.0f, 15.0f) });
                }
            }
            else
            {
                result = new List<BattleTrophy>(characters1.Count);
                for (int i = 0; i < characters1.Count; ++i)
                {
                    result.Add(new BattleTrophy { character = characters1[i].config, exp = Random.Range(8.0f, 15.0f) });
                }
            }
            return result;
        }
    }

}