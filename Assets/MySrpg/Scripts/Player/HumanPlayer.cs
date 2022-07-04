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
            // �����ͼ�⣺
            if (node is null && c is null)
                return;

            // �����ͼ�ڵ㣺
            if (c is null)
                return;

            selection = c;

            // ����ҷ���ɫ��
            if (IsFriend(c.affiliation))
            {
                ShowFriendInfo(node, c);
            }
            // ����з���ɫ��
            else
            {
                ShowEnemyInfo(node, c);
            }
        }

        private void OnSelectionIsFriend(GraphNode node, Character c)
        {
            // �����ͼ�⣺ 
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

                // ������ƶ���Χ�ڵĵ�ͼ�ڵ㣺  
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
                // ��������ƶ���Χ�ڵĵ�ͼ�ڵ㣺
                else
                {
                    HideFriendInfo();
                    selection = null;
                }

                return;
            }

            // ����ҷ���ɫ��   
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

                // ����ڹ�����Χ�ڵĵз���ɫ�� 
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
                // ������ڹ�����Χ�ڵĵз���ɫ��
                else
                {
                    ShowEnemyInfo(node, c);
                }
            }
        }

        private void OnSelectionIsEnemy(GraphNode node, Character c)
        {
            // �����ͼ�⣺ 
            if (node is null && c is null)
            {
                HideFriendInfo();
                selection = null;
                return;
            }

            // �����ͼ�ڵ㣺 
            if (c is null)
            {
                HideFriendInfo();
                selection = null;
                return;
            }

            HideEnemyInfo();
            selection = c;

            // ����ҷ���ɫ��
            if (IsFriend(c.affiliation))
            {
                ShowFriendInfo(node, c);
            }
            // ����з���ɫ��
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
            #region ���봦���ע��
            // ��ǰû��ѡ�еĽ�ɫ��
            // �����ͼ�⣺  ��
            // ����ҷ���ɫ������ѡ�н�ɫ�����ҷ���ɫ-��ʾ��
            // ����з���ɫ������ѡ�н�ɫ�����з���ɫ-��ʾ��
            // �����ͼ�ڵ㣺��

            // ��ǰѡ�н�ɫ���ҷ��ģ�
            // �����ͼ�⣺                ���ҷ���ɫ-���ء���ѡ�н�ɫ��Ϊnull
            // ����ҷ���ɫ��              ���ҷ���ɫ-���ء�������ѡ�н�ɫ�����ҷ���ɫ-��ʾ��
            // ����ڹ�����Χ�ڵĵз���ɫ��  ���ҷ���ɫ-���ء�����ʾѡ�нڵ㣻ѡ�н�ɫ�ƶ������˽ڵ㸽����Ȼ���չ���������
            // ������ڹ�����Χ�ڵĵз���ɫ�����ҷ���ɫ-���ء�������ѡ�н�ɫ�����з���ɫ-��ʾ��
            // ������ƶ���Χ�ڵĵ�ͼ�ڵ㣺  ���ҷ���ɫ-���ء�����ʾѡ�нڵ㣻ѡ�н�ɫ�ƶ����ýڵ㣬Ȼ���ҷ���ɫ-��ʾ��
            // ��������ƶ���Χ�ڵĵ�ͼ�ڵ㣺���ҷ���ɫ-���ء���ѡ�н�ɫ��Ϊnull

            // ��ǰѡ�н�ɫ�ǵз��ģ�
            // �����ͼ�⣺  ���з���ɫ-���ء���ѡ�н�ɫ��Ϊnull
            // ����ҷ���ɫ�����з���ɫ-���ء�������ѡ�н�ɫ�����ҷ���ɫ-��ʾ��
            // ����з���ɫ�����з���ɫ-���ء�������ѡ�н�ɫ�����з���ɫ-��ʾ��
            // �����ͼ�ڵ㣺���з���ɫ-���ء���ѡ�н�ɫ��Ϊnull

            // �ӹ��̣�
            // + �ҷ���ɫ-��ʾ������ʾ�����ߺ��ܵĿɹ������򡷣���ʾѡ�нڵ㣻��ʾ�������
            // + �ҷ���ɫ-���أ������ؿ����ߺ��ܵĿɹ������򡷣�����ѡ�нڵ㣻���ؼ������
            // + �з���ɫ-��ʾ������ʾ�����ߺ��ܵĿɹ������򡷣���ʾѡ�нڵ㣻��ʾ��ɫ���
            // + �з���ɫ-���أ������ؿ����ߺ��ܵĿɹ������򡷣�����ѡ�нڵ㣻���ؽ�ɫ���
            // + ��ʾ�����ߺ��ܵĿɹ���������ʾ������������ʾ�ܵĿɹ�������
            // + ���ؿ����ߺ��ܵĿɹ����������ؿ��������������ܵĿɹ�������
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