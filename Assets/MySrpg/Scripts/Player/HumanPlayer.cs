using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Mirror;
using MyFramework;
using MyFramework.UI;
using MySrpg;
using MyUtility;
using Pathfinding;

namespace MySrpg
{

    public delegate void OnCancelAbilityTargetSelectHandler();


    public class HumanPlayer : Player
    {
        public bool isSelectingTarget { get; set; }
        public int pendingAbilityIndex { get; set; } = -1;

        public Character selection { get; private set; }
        public Character playerSelectedTarget { get; private set; }

        private readonly string m_abilityPanelId = "AbilityPanel";
        private readonly string m_characterPanelId = "CharacterPanel";

        private Camera m_mainCam;
        private UIManager m_uiManager;
        private BattleSystem m_battleSys;
        private IndicatorSystem m_indicatorSys;

        private int m_curSelectIndex = 0;
        private bool m_canInput = true;        

        public event OnCancelAbilityTargetSelectHandler onCancelAbilityTargetSelectHandler;

        public override void StartPlayerRound()
        {
            m_canInput = true;
            m_curSelectIndex = -1;
            SelectNextCharacter();
        }

        public void SelectNextCharacter()
        {
            HideFriendInfo();
            if (m_curSelectIndex < m_battleSys.count0 - 1)
            {
                selection = m_battleSys.characters0[++m_curSelectIndex];
                ShowFriendInfo(selection.mapNode, selection);
            }
            else
            {
                if (Game.Instance.isNetGame)
                {
                    if (Game.Instance.isServer)
                    {
                        NetworkServer.SendToAll(new Msg_FinishHalfRound());
                    }
                    else
                    {
                        NetworkClient.Send(new Msg_FinishHalfRound());
                    }
                }

                lvlManager.StartNextHalfRound();
            }
        }

        private void OnCharacterFinishAbility(Character c, Ability a)
        {
            if (c.affiliation == playerAffiliation)
            {
                SelectNextCharacter();
            }
        }

        public override void FinishPlayerRound()
        {
            m_canInput = false;
        }

        private void Awake()
        {
            SrpgGame game = Game.Instance as SrpgGame;

            m_uiManager = UIManager.Instance;

            m_battleSys = game.battleSystem;
            m_battleSys.onCharacterFinishAbilityHandler += OnCharacterFinishAbility;

            m_indicatorSys = game.indicatorSystem;
        }

        private void Start()
        {
            m_mainCam = Camera.main;
        }

        private void OnSelectionIsNull(GraphNode node, Character c)
        {
            // 点击地图外：
            if (node is null && c is null)
                return;

            // 点击地图节点：
            if (c is null)
                return;

            selection = c;

            // 点击我方角色：
            if (IsFriend(c.affiliation))
            {
                ShowFriendInfo(node, c);
            }
            // 点击敌方角色：
            else
            {
                ShowEnemyInfo(node, c);
            }
        }

        private void OnSelectionIsFriend(GraphNode node, Character c)
        {
            // 点击地图外： 
            if (node is null && c is null)
            {
                HideFriendInfo();
                selection = null;
                return;
            }

            if (c is null)
            {
                HideAbilityPanel();
                List<Int3> movPoints = selection.movablePoints;

                // 点击在移动范围内的地图节点：  
                if (movPoints.Contains(node.position))
                {
                    ShowSelectionPoint(node);
                    selection.StartFindPathToNode((Vector3)node.position, (PathP2P p) =>
                    {
                        selection.StartFollowPath(false, () =>
                        {
                            ShowAbilityPanel(selection);
                        });
                    });
                }
                // 点击不在移动范围内的地图节点：
                else
                {
                    HideFriendInfo();
                    selection = null;
                }

                return;
            }

            // 点击我方角色：   
            if (IsFriend(c.affiliation))
            {
                HideFriendInfo();
                selection = c;
                ShowFriendInfo(node, c);
            }
            else
            {
                HideFriendInfo();
                List<Int3> atkPoints = selection.totalAttackablePoints;

                // 点击在攻击范围内的敌方角色： 
                if (atkPoints.Contains(c.mapNode.position))
                {
                    ShowSelectionPoint(c.mapNode);
                    selection.StartFindPathToEnemy(c.mapNode.position, (PathP2P p) =>
                    {
                        if (p.points.Count - 1 <= selection.attackmentRangeCell)
                        {
                            selection.StartLookAtPos(c.transform.position, () =>
                            {
                                selection.playerSelectedTarget = c;
                                selection.NormalAttack();
                            });
                        }
                        else
                        {
                            selection.StartFollowPath(true, () =>
                            {
                                selection.playerSelectedTarget = c;
                                selection.NormalAttack();
                            });
                        }
                    });
                }
                // 点击不在攻击范围内的敌方角色：
                else
                {
                    ShowEnemyInfo(node, c);
                }
            }
        }

        private void OnSelectionIsEnemy(GraphNode node, Character c)
        {
            // 点击地图外： 
            if (node is null && c is null)
            {
                HideFriendInfo();
                selection = null;
                return;
            }

            // 点击地图节点： 
            if (c is null)
            {
                HideFriendInfo();
                selection = null;
                return;
            }

            HideEnemyInfo();
            selection = c;

            // 点击我方角色：
            if (IsFriend(c.affiliation))
            {
                ShowFriendInfo(node, c);
            }
            // 点击敌方角色：
            else
            {
                ShowEnemyInfo(node, c);
            }
        }

        private bool IsFriend(int affiliation)
        {
            return affiliation == playerAffiliation;
        }

        private void ShowFriendInfo(GraphNode node, Character c)
        {
            ShowMovableAndTotalAttackablePoints(c);
            ShowSelectionPoint(node);
            if (c.index != m_curSelectIndex)
                ShowCharacterPanel(c);
            else
                ShowAbilityPanel(c);
        }

        private void HideFriendInfo()
        {
            HideMovableAndTotalAttackablePoints();
            HideSelectionPoint();
            HideAbilityPanel();
        }

        private void ShowEnemyInfo(GraphNode node, Character c)
        {
            ShowMovableAndTotalAttackablePoints(c);
            ShowSelectionPoint(node);
            ShowCharacterPanel(c);
        }

        private void HideEnemyInfo()
        {
            HideMovableAndTotalAttackablePoints();
            HideSelectionPoint();
            HideCharacterPanel();
        }

        private void ShowMovableAndTotalAttackablePoints(Character c)
        {
            c.StartFindMovablePoints((RangeFromOnePoint p) =>
            {
                m_indicatorSys.ShowMovableCells(p.points.ToVector3().ToArray());

                c.StartFindTotalAttackablePoints((RangeFromPoints p) =>
                {
                    m_indicatorSys.ShowAttackableCells(p.points, p.startPoints);
                });
            });
        }

        private void HideMovableAndTotalAttackablePoints()
        {
            m_indicatorSys.HideAttackableCells();
            m_indicatorSys.HideMovableCells();
        }

        private void ShowSelectionPoint(GraphNode node)
        {
            m_indicatorSys.ShowSelectionCell((Vector3)node.position);
        }

        private void HideSelectionPoint()
        {
            m_indicatorSys.HideSelectionCell();
        }

        private void ShowAbilityPanel(Character c)
        {
            if (m_uiManager.IsOpen(m_characterPanelId))
                HideCharacterPanel();
            m_uiManager.Open(m_abilityPanelId, c.abilities);
        }

        private void HideAbilityPanel()
        {
            m_uiManager.Close(m_abilityPanelId);
        }

        private void ShowCharacterPanel(Character c)
        {
            if (m_uiManager.IsOpen(m_abilityPanelId))
                HideAbilityPanel();
            m_uiManager.Open(m_characterPanelId, c);
        }

        private void HideCharacterPanel()
        {
            m_uiManager.Close(m_characterPanelId);
        }


        private void Update()
        {
            #region 输入处理的注释
            // 当前没有选中的角色：
            // 点击地图外：  无
            // 点击我方角色：更新选中角色；《我方角色-显示》
            // 点击敌方角色：更新选中角色；《敌方角色-显示》
            // 点击地图节点：无

            // 当前选中角色是我方的：
            // 点击地图外：                《我方角色-隐藏》；选中角色设为null
            // 点击我方角色：              《我方角色-隐藏》；更新选中角色；《我方角色-显示》
            // 点击在攻击范围内的敌方角色：  《我方角色-隐藏》；显示选中节点；选中角色移动到敌人节点附近，然后普攻攻击敌人
            // 点击不在攻击范围内的敌方角色：《我方角色-隐藏》；更新选中角色；《敌方角色-显示》
            // 点击在移动范围内的地图节点：  《我方角色-隐藏》；显示选中节点；选中角色移动到该节点，然后《我方角色-显示》
            // 点击不在移动范围内的地图节点：《我方角色-隐藏》；选中角色设为null

            // 当前选中角色是敌方的：
            // 点击地图外：  《敌方角色-隐藏》；选中角色设为null
            // 点击我方角色：《敌方角色-隐藏》；更新选中角色；《我方角色-显示》
            // 点击敌方角色：《敌方角色-隐藏》；更新选中角色；《敌方角色-显示》
            // 点击地图节点：《敌方角色-隐藏》；选中角色设为null

            // 子过程：
            // + 我方角色-显示：《显示可行走和总的可攻击区域》；显示选中节点；显示技能面板
            // + 我方角色-隐藏：《隐藏可行走和总的可攻击区域》；隐藏选中节点；隐藏技能面板
            // + 敌方角色-显示：《显示可行走和总的可攻击区域》；显示选中节点；显示角色面板
            // + 敌方角色-隐藏：《隐藏可行走和总的可攻击区域》；隐藏选中节点；隐藏角色面板
            // + 显示可行走和总的可攻击区域：显示可行走区域；显示总的可攻击区域
            // + 隐藏可行走和总的可攻击区域：隐藏可行走区域；隐藏总的可攻击区域
            #endregion

            if (!m_canInput)
                return;

            GraphNode node = null;
            Character c = null;
            bool insideMap = false;

            Vector3? hitPoint;
            if (!GetMouseInput(out hitPoint))
                return;

            if (hitPoint != null)
            {
                insideMap = true;
            }

            if (insideMap)
            {
                node = AstarPath.active.GetNearest(hitPoint.Value).node;
                c = m_battleSys.GetCharacterOn(node.position);
            }

            if (isSelectingTarget)
            {
                (Game.Instance as SrpgGame).indicatorSystem.HideAbilityRangeCells();

                if (selection.abilities[pendingAbilityIndex].FindTargets(node, c) > 0)
                {
                    selection.StartLookAtPos((Vector3)node.position, () =>
                    {
                        selection.UseAbility(pendingAbilityIndex);

                        isSelectingTarget = false;
                        pendingAbilityIndex = -1;
                    });
                }
                else
                {
                    CancelAbilityTargetSelection();
                }
                           
                return;
            }

            if (selection is null)
                OnSelectionIsNull(node, c);
            else if (IsFriend(selection.affiliation))
                OnSelectionIsFriend(node, c);
            else
                OnSelectionIsEnemy(node, c);
        }

        public void CancelAbilityTargetSelection()
        {
            IndicatorSystem indicatorSys = (Game.Instance as SrpgGame).indicatorSystem;
            indicatorSys.HideAbilityRangeCells();
            ShowMovableAndTotalAttackablePoints(selection);

            onCancelAbilityTargetSelectHandler?.Invoke();

            isSelectingTarget = false;
            pendingAbilityIndex = -1;
        }

        /// <param name="hitPoint">if detected mouse input and the ray from mouse position hit anything: hit point;
        /// else: null</param>
        /// <returns>if detected mouse input: true; if not: false</returns>
        private bool GetMouseInput(out Vector3? hitPoint)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = m_mainCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    hitPoint = hit.point;
                }
                else
                {
                    hitPoint = null;
                }
                return true;
            }
            hitPoint = null;
            return false;
        }

    }

}