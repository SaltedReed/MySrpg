using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using MyFramework;
using Pathfinding;
using Mirror;


namespace MySrpg
{

    public delegate void OnHpChangeHandler(float oldVal, float newVal);

    public delegate void OnAttachBuffHandler(BaseBuff buff);
    public delegate void OnDetachBuffHandler(BaseBuff buff);

    public delegate void OnUseAbilityHandler(int index);
    public delegate void OnAbilityCdStartHandler(int index, int cd);
    public delegate void OnAbilityCdUpdateHandler(int index, int cd);
    public delegate void OnAbilityCdEndHandler(int index);

    public delegate void OnFinishPathFollowHandler();


    [RequireComponent(typeof(AbilityExecutor))]
    public sealed class Character : MonoBehaviour
    {
        public CharacterConfig config;
        public Transform hpBarPoint;

        public int index { get; set; }

        public Sprite icon => config.icon;
        public string characterName => config.characterName;

        public int maxLevel => config.maxLevel;
        public int level 
        {
            get => m_level;
            set => m_level = Mathf.Clamp(value, 0, maxLevel);
        }
        private int m_level;

        public float maxExp => config.maxExpFormula.Get(level);
        public float exp 
        {
            get => m_exp;
            set
            {
                if (value >= maxExp)
                {
                    if (level == maxLevel)
                    {
                        m_exp = maxExp;
                    }
                    else
                    {
                        m_exp -= maxExp;
                        ++level;
                    }
                }
                else
                {
                    m_exp = Mathf.Max(0, value);
                }
            }
        }
        private float m_exp;

        public int movementRangeCell => config.movementRangeCell;

        public float maxHp => config.maxHpFormula.Get(level);
        public float hp
        {
            get => m_hp;
            set
            {
                float old = m_hp;
                m_hp = Mathf.Clamp(value, 0, maxHp);

                onHpChangeHandler?.Invoke(old, m_hp);
            }
        }
        private float m_hp;

        public float maxEnergy => config.maxEnergyFormula.Get(level);
        public float energy
        {
            get => m_energy;
            set => m_energy = Mathf.Clamp(value, 0, maxEnergy);
        }
        private float m_energy;

        public float defaultDefense => config.defenseFormula.Get(level);
        public float defense
        {
            get => m_defense;
            set => m_defense = Mathf.Max(0, value);
        }
        private float m_defense;

        public float defaultAttackment => config.attackmentFormula.Get(level);
        public float attackment
        {
            get => m_attackment;
            set => m_attackment = Mathf.Max(0, value);
        }
        private float m_attackment;


        public enum PropertyIndex : int
        {
            Hp = 0,
            Energy,
            Defense,
            Attackment,
            OutOfRange
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return hp;
                    case 1:
                        return energy;
                    case 2:
                        return defense;
                    case 3:
                        return attackment;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        hp = value;
                        break;
                    case 1:
                        energy = value;
                        break;
                    case 2:
                        defense = value;
                        break;
                    case 3:
                        attackment = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        // temp
        public int affiliation;

        public CharacterType characterType => config.characterType;

        public int attackmentRangeCell => config.attackmentRangeCell;

        /// <summary>
        /// The first ability is the normal attackment
        /// </summary>
        public Ability[] abilities => m_abilities;
        private Ability[] m_abilities = new Ability[2];

        public Ability normalAttackment => m_abilities is null || m_abilities.Length == 0 ? null : m_abilities[0];
        
        public Ability currentAbility { get; private set; }

        public Character playerSelectedTarget { get; set; }

        public List<BaseBuff> buffs => m_buffs;
        private List<BaseBuff> m_buffs = new List<BaseBuff>();

        public Animator animator { get; private set; }

        public GraphNode mapNode => AstarPath.active.graphs.Length == 0 ? null :
            AstarPath.active.graphs[0].GetNearest(transform.position).node;

        public MapNodeTag traversal { get; set; }

        public List<Int3> movablePoints { get; private set; } = new List<Int3>();
        public List<Int3> attackablePoints { get; private set; } = new List<Int3>();
        public List<Int3> totalAttackablePoints { get; private set; } = new List<Int3>();
        /// <summary>
        /// the first element is self's position, the last is the enemy's position
        /// </summary>
        public List<Int3> pathToFollow { get; set; } = new List<Int3>();

        public bool isMoving
        {
            get => m_isMoving;
            set
            {
                m_isMoving = value;
                animator.SetBool(m_animKey_moving, value);
            }
        }
        private bool m_isMoving;

        private int m_animKey_hit = Animator.StringToHash("hit");
        private int m_animKey_moving = Animator.StringToHash("moving");

        private int m_targetPathNodeIndex;
        private Vector3 m_targetPathNodePos;
        private OnFinishPathFollowHandler m_onFinishPathFollowHandler;
        private Vector3 m_lookDir;
        private Quaternion m_lookAtQuat;
        private bool m_lookAtLastPathNode;

        private BattleSystem m_battleSys;

        public event OnHpChangeHandler onHpChangeHandler;

        public event OnAttachBuffHandler onAttachBuffHandler;
        public event OnDetachBuffHandler onDetachBuffHandler;

        public event OnUseAbilityHandler onUseAbilityHandler;
        public OnAbilityCdStartHandler onAbilityCdStartHandler;
        public OnAbilityCdUpdateHandler onAbilityCdUpdateHandler;
        public OnAbilityCdEndHandler onAbilityCdEndHandler;


        #region Pathfinding

        public void StartFindMovablePoints(OnFindRangeFromOnePointHandler handler)
        {
            BattleSystem battleSystem = (Game.Instance as SrpgGame).battleSystem;
            List<Int3> occupiedPoints = battleSystem.GetCharacterNodes().ToInt3();

            MovablePointsTravProvider provider = new MovablePointsTravProvider();
            provider.traversal = traversal;
            provider.occupiedPoints = occupiedPoints;

            RangeFromOnePoint range = new RangeFromOnePoint();
            range.startPos = transform.position;
            range.rangeCell = movementRangeCell;
            range.traversalProvider = provider;
            range.handler = (RangeFromOnePoint result) =>
            {
                movablePoints = result.points;
                handler?.Invoke(result);
            };

            range.StartFind();
        }

        public void StartFindAttackablePoints( OnFindRangeFromOnePointHandler handler)
        {
            AttackablePointsTravProvider provider = new AttackablePointsTravProvider();
            provider.friends = (Game.Instance as SrpgGame).battleSystem.GetFriendNodes(affiliation).ToInt3();

            RangeFromOnePoint range = new RangeFromOnePoint();
            range.startPos = transform.position;
            range.rangeCell = normalAttackment.rangeCell;
            range.traversalProvider = provider;
            range.handler = (RangeFromOnePoint result) =>
            {
                attackablePoints = result.points;
                handler?.Invoke(result);
            };

            range.StartFind();
        }

        public void StartFindTotalAttackablePoints(OnFindRangeFromPointsHandler handler)
        {
            if (movablePoints is null || movablePoints.Count == 0)
            {
                Debug.LogError($"{gameObject.name}cannot find total attackable points, because its movablePoints is empty");
                return;
            }

            AttackablePointsTravProvider provider = new AttackablePointsTravProvider();
            provider.friends = (Game.Instance as SrpgGame).battleSystem.GetFriendNodes(affiliation).ToInt3();

            RangeFromPoints range = new RangeFromPoints();
            range.startPoints = movablePoints;
            range.rangeCell = normalAttackment.rangeCell;
            range.traversalProvider = provider;
            range.handler = (RangeFromPoints result) =>
            {
                totalAttackablePoints = result.points;
                handler?.Invoke(result);
            };

            range.StartFind();
        }

        public void StartFindPathToEnemy(Int3 enemyPos, OnFindPathP2PHandler handler)
        {
            PathP2PTravProvider provider = new PathP2PTravProvider();
            provider.traversal = traversal;
            // the node where the enemy is at can be outside movement range
            provider.rangeCell = movementRangeCell + 1;
            provider.startPos = transform.position;
            List<Int3> occu = (Game.Instance as SrpgGame).battleSystem.GetCharacterNodes().ToInt3();
            occu.Remove(mapNode.position);
            occu.Remove(enemyPos);
            provider.occupiedPoints = occu;

            PathP2P path = new PathP2P();
            path.startPos = transform.position;
            path.endPos = (Vector3)enemyPos;
            path.traversalProvider = provider;
            path.handler = (PathP2P result) =>
            {
                pathToFollow = result.points;
                handler?.Invoke(result);
            };

            path.StartFind();
        }

        public void StartFindPathToNode(Vector3 pos, OnFindPathP2PHandler handler)
        {
            PathP2PTravProvider provider = new PathP2PTravProvider();
            provider.traversal = traversal;
            provider.rangeCell = movementRangeCell;
            provider.startPos = transform.position;
            List<Int3> occu = (Game.Instance as SrpgGame).battleSystem.GetCharacterNodes().ToInt3();
            occu.Remove(mapNode.position);
            provider.occupiedPoints = occu;

            PathP2P path = new PathP2P();
            path.startPos = transform.position;
            path.endPos = pos;
            path.traversalProvider = provider;
            path.handler = (PathP2P result) =>
            {
                pathToFollow = result.points;
                // to unify the code of following path-to-enemy and following path-to-node
                if (result.points.Count > 0)
                    pathToFollow.Add(result.points[result.points.Count - 1]);

                handler?.Invoke(result);
            };

            path.StartFind();
        }

        #endregion


        #region Movement

        public void StartFollowPath(bool lookAtLastPathNode, OnFinishPathFollowHandler handler)
        {
            m_lookAtLastPathNode = lookAtLastPathNode;

            // the last path node will not be reached
            if (pathToFollow is null || pathToFollow.Count < 2)
            {
                return;
            }

            if (Game.Instance.isNetGame && affiliation == m_battleSys.player0.playerAffiliation)
            {
                Msg_FollowPath msg = new Msg_FollowPath
                {
                    characterIndex = index,
                    path = pathToFollow,
                    lookAtLastNode = lookAtLastPathNode
                };

                if (Game.Instance.isServer)
                {
                    NetworkServer.SendToAll(msg);
                }
                else
                {
                    NetworkClient.Send(msg);
                }
            }

            m_targetPathNodeIndex = 1;
            m_targetPathNodePos = (Vector3)pathToFollow[1];
            m_onFinishPathFollowHandler = handler;

            if (pathToFollow.Count == 2)
            {
                isMoving = false;
                if (m_lookAtLastPathNode)
                {
                    StartLookAtPos(m_targetPathNodePos, OnFinishPathFollow);
                }
                else
                {
                    OnFinishPathFollow();
                }
            }
            else
            {
                isMoving = true;
                StartLookAtPos(m_targetPathNodePos);
            }
        }

        private void UpdateMovement()
        {
            if (!isMoving)
                return;

            transform.position = Vector3.MoveTowards(transform.position, m_targetPathNodePos, Time.deltaTime * 2.0f);
            if ((transform.position-m_targetPathNodePos).sqrMagnitude <= 0.001f)
            {
                ++m_targetPathNodeIndex; Debug.Assert(m_targetPathNodeIndex < pathToFollow.Count);
                m_targetPathNodePos = (Vector3)pathToFollow[m_targetPathNodeIndex];

                // finished path following and no need to face the last node (when moving to an map node)
                if (m_targetPathNodeIndex == pathToFollow.Count - 1 && !m_lookAtLastPathNode)
                {
                    OnFinishPathFollow();
                }
                // finished path following and need to face the last node (when moving to an enemy)
                else if (m_targetPathNodeIndex == pathToFollow.Count - 1)
                {
                    isMoving = false;
                    StartLookAtPos(m_targetPathNodePos, OnFinishPathFollow);
                }
                // haven't finish path following
                else
                {
                    StartLookAtPos(m_targetPathNodePos);
                }
            }
        }

        public void StartLookAtPos(Vector3 pos, Action onComplete = null)
        {
            Vector3 x0zPos = new Vector3(pos.x, 0, pos.z);
            Vector3 x0zSelfPos = new Vector3(transform.position.x, 0, transform.position.z);
            m_lookDir = (x0zPos - x0zSelfPos).normalized;
            m_lookAtQuat = Quaternion.LookRotation(m_lookDir, transform.up);

            StartCoroutine(LookAtPosCoroutine(onComplete));
        }

        private IEnumerator LookAtPosCoroutine(Action onComplete)
        {
            while (Vector3.Dot(m_lookDir, transform.forward) < 0.99f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, m_lookAtQuat, Time.deltaTime * 200.0f);
                yield return null;
            }

            /*if (isMoving && m_targetPathNodeIndex == pathToFollow.Count - 1)
            {
                OnFinishPathFollow();
            }*/
            onComplete?.Invoke();
        }

        private void OnFinishPathFollow()
        {
            isMoving = false;
            m_onFinishPathFollowHandler?.Invoke();

            EventDispatcher.DispatchEvent((int)EventId.CharFinishMove);
        }

        #endregion


        #region Normal Attackment

        public void NormalAttack()
        {
            UseAbility(0);
        }

        #endregion


        #region Ability

        public void UseAbility(int index)
        {
            if (index < 0 || index >= m_abilities.Length)
                throw new ArgumentOutOfRangeException($"index = {index}");
            // temp
            if (currentAbility != null)
                throw new InvalidOperationException("cannot use an ability when another is in use");

            if (Game.Instance.isNetGame && affiliation == m_battleSys.player0.playerAffiliation)
            {
                Msg_UseAbility
                    msg = new Msg_UseAbility
                {
                    characterIndex = this.index,
                    abilityIndex = index,
                    targetIndex = playerSelectedTarget != null ? playerSelectedTarget.index : -1
                };

                if (Game.Instance.isServer)
                {
                    NetworkServer.SendToAll(msg);
                }
                else
                {
                    NetworkClient.Send(msg);
                }
            }

            currentAbility = m_abilities[index];
            if (playerSelectedTarget != null)
            {
                currentAbility.ClearTargets();
                currentAbility.AddTarget(playerSelectedTarget);
                playerSelectedTarget = null;
            }
            currentAbility.Use(() => 
            {
                (Game.Instance as SrpgGame).battleSystem.onCharacterFinishAbilityHandler?.Invoke(this, currentAbility);
                currentAbility = null; 
            });

            if (index != 0)
                onUseAbilityHandler?.Invoke(index);
            else
                EventDispatcher.DispatchEvent((int)EventId.CharNormalAttack);
        }

        private void UpdateAbilities()
        {
            foreach (Ability a in m_abilities)
            {
                a.OnTick();
            }
        }

        private void InitAbilities()
        {
            AbilityExecutor executor = GetComponent<AbilityExecutor>();

            for (int i=0; i<m_abilities.Length; ++i)
            {
                LoadAbility(i, executor);
            }

            Debug.Log($"{gameObject.name} initialized abilities");
        }

        private void LoadAbility(int index, AbilityExecutor executor)
        {
            Ability a = AbilityLoader.Load(config.abilityConfigPaths[index], executor, this);
            Debug.Assert(a != null);
            a.index = index;
            m_abilities[index] = a;
        }

        public void SpawnVfxForCurrentAbility(string path)
        {
            if (currentAbility is null || currentAbility.targets is null)
                return;

            GameObject prefab = ResourceManager.Load<GameObject>("Vfx", path); Debug.Assert(prefab != null);

            foreach (Character c in currentAbility.targets)
            {
                Transform trans = c.transform;
                VfxManager.SpawnParticle(prefab, prefab.transform.position, trans.localToWorldMatrix, Quaternion.identity);
            }
        }

        #endregion


        #region Buff

        public void AttachBuff(BaseBuff buff)
        {
            if (buff is null)
                throw new ArgumentNullException();

            m_buffs.Add(buff);
            buff.OnAttach(this);

            onAttachBuffHandler?.Invoke(buff);

            Debug.Log($"buff {buff.name} attached to {gameObject.name}, duration={buff.duration}");
        }

        private void DetachDeadBuffs()
        {
            if (m_buffs is null)
                return;

            for (int i=m_buffs.Count-1; i>=0; --i)
            {
                BaseBuff buff = m_buffs[i];
                if (buff.duration <= 0)
                {
                    DetachBuff(buff);
                }
            }
        }

        private void DetachBuff(BaseBuff buff)
        {
            Debug.Log($"buff {buff.name} detached from {gameObject.name}");

            buff.OnDetach();
            onDetachBuffHandler?.Invoke(buff);
            m_buffs.Remove(buff);
        }

        private void UpdateBuffs()
        {
            if (m_buffs is null)
                return;

            foreach (BaseBuff buff in m_buffs)
            {
                if (buff.duration > 0)
                {
                    buff.OnTick();
                }
            }
        }

#endregion


        #region Life

        public void OnSpawn()
        {
            Debug.Log($"{gameObject.name} spawned");

            hp = maxHp;
            energy = maxEnergy;
            defense = defaultDefense;
            attackment = defaultAttackment;
            traversal = config.traversal;

            animator = GetComponentInChildren<Animator>();

            InitAbilities();
        }

        public void OnTick()
        {
            UpdateAbilities();
            UpdateBuffs();
            DetachDeadBuffs();
        }

        public void TakeDamage(float absDmg, Character attacker)
        {
            Debug.Log($"{gameObject.name} took {absDmg} damage from " +
                $"{(attacker is null ? "null" : attacker.gameObject.name)}, " +
                $"hp: {hp} -> {hp-absDmg}");

            hp -= absDmg;

            if (hp <= 0.0f)
            {
                OnDie();
            }

            animator.SetTrigger(m_animKey_hit);
        }

        private void OnDie()
        {
            Debug.Log($"{gameObject.name} died");

            gameObject.SetActive(false);
        }

        #endregion


        #region Unity Functions

        private void Awake()
        {
            m_battleSys = (Game.Instance as SrpgGame).battleSystem;
        }

        private void Update()
        {
            UpdateMovement();
        }

        #endregion

    }

}