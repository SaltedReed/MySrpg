using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using MyFramework;
using MyUtility;

namespace MySrpg
{

    // temp
    public class IndicatorSystem : BaseGameSystem
    {
        public float normalCellInc = -0.01f;
        public float selectionCellInc = 0.01f;
        public float movableCellInc = 0.0f;
        public float attackableCellInc = 0.0f;
        public float abilityRangeCellInc = 0.0f;

        protected readonly string m_normalCellPrefabPath = "Prefabs/Indicator/EmptyQuad";
        protected readonly string m_selectionCellPrefabPath = "Prefabs/Indicator/GreyQuadProj";
        protected readonly string m_movableCellPrefabPath = "Prefabs/Indicator/BlueQuadProj";
        protected readonly string m_attackableCellPrefabPath = "Prefabs/Indicator/RedQuadProj";
        protected readonly string m_abilityRangeCellPrefabPath = "Prefabs/Indicator/YellowQuadProj";

        protected Transform m_root;

        protected GridGraph m_map;
        protected GameObject[] m_normalCells;
        protected GameObject m_selectionCell;
        protected GameObject[] m_movableCells;
        protected GameObject[] m_attackableCells;
        protected GameObject[] m_abilityRangeCells;


        public void ShowAbilityRangeCells(Vector3[] positions)
        {
            if (m_abilityRangeCells != null && m_abilityRangeCells.Length > 0)
                HideAbilityRangeCells();

            GameObject prefab = ResourceManager.Load<GameObject>(m_abilityRangeCellPrefabPath);
            if (prefab is null)
            {
                Debug.LogError($"failed to load the ability range cell prefab from {m_abilityRangeCellPrefabPath}");
                return;
            }

            m_abilityRangeCells = new GameObject[positions.Length];
            for (int i = 0; i < m_abilityRangeCells.Length; ++i)
            {
                GameObject go = Instantiate(prefab);
                go.transform.position = new Vector3(positions[i].x, abilityRangeCellInc, positions[i].z);
                go.transform.localScale = Vector3.one * 0.9f;
                go.transform.SetParent(m_root);
                m_abilityRangeCells[i] = go;
            }
        }

        public void HideAbilityRangeCells()
        {
            if (m_abilityRangeCells is null)
                return;

            for (int i = 0; i < m_abilityRangeCells.Length; ++i)
            {
                Destroy(m_abilityRangeCells[i]);
            }
            m_abilityRangeCells = null;
        }

        public void ShowAttackableCells(List<Int3> points, List<Int3> except)
        {
            HashSet<Int3> set = new HashSet<Int3>(points);
            set.ExceptWith(except);
            ShowAttackableCells(new List<Int3>(set).ToVector3().ToArray());
        }

        public void ShowAttackableCells(Vector3[] positions)
        {
            if (m_attackableCells != null && m_attackableCells.Length > 0)
                HideAttackableCells();

            GameObject prefab = ResourceManager.Load<GameObject>(m_attackableCellPrefabPath);
            if (prefab is null)
            {
                Debug.LogError($"failed to load the attackable cell prefab from {m_attackableCellPrefabPath}");
                return;
            }

            m_attackableCells = new GameObject[positions.Length];
            for (int i = 0; i < m_attackableCells.Length; ++i)
            {
                GameObject go = Instantiate(prefab);
                go.transform.position = new Vector3(positions[i].x, attackableCellInc, positions[i].z);
                go.transform.localScale = Vector3.one * 0.9f;
                go.transform.SetParent(m_root);
                m_attackableCells[i] = go;
            }
        }

        public void HideAttackableCells()
        {
            if (m_attackableCells is null)
                return;

            for (int i = 0; i < m_attackableCells.Length; ++i)
            {
                Destroy(m_attackableCells[i]);
            }
            m_attackableCells = null;
        }

        public void ShowMovableCells(Vector3[] positions)
        {
            if (m_movableCells != null && m_movableCells.Length > 0)
                HideMovableCells();

            GameObject prefab = ResourceManager.Load<GameObject>(m_movableCellPrefabPath);
            if (prefab is null)
            {
                Debug.LogError($"failed to load the movable cell prefab from {m_movableCellPrefabPath}");
                return;
            }

            m_movableCells = new GameObject[positions.Length];
            for (int i = 0; i < m_movableCells.Length; ++i)
            {
                GameObject go = Instantiate(prefab);
                go.transform.position = new Vector3(positions[i].x, movableCellInc, positions[i].z);
                go.transform.localScale = Vector3.one * 0.9f;
                go.transform.SetParent(m_root);
                m_movableCells[i] = go;
            }
        }

        public void HideMovableCells()
        {
            if (m_movableCells is null)
                return;

            for (int i = 0; i < m_movableCells.Length; ++i)
            {
                Destroy(m_movableCells[i]);
            }
            m_movableCells = null;
        }

        public void ShowSelectionCell(Vector3 pos)
        {
            if (m_selectionCell.gameObject.activeInHierarchy)
                HideSelectionCell();

            m_selectionCell.transform.position = new Vector3(pos.x, selectionCellInc, pos.z);
            m_selectionCell.SetActive(true);
        }

        public void HideSelectionCell()
        {
            m_selectionCell.SetActive(false);
        }

        protected override void Awake()
        {
            base.Awake();

            GameObject rootGo = new GameObject("Indicators");
            m_root = rootGo.transform;

            GameObject selectionCellPrefab = ResourceManager.Load<GameObject>(m_selectionCellPrefabPath);
            if (selectionCellPrefab != null)
            {
                m_selectionCell = Instantiate(selectionCellPrefab);
                m_selectionCell.SetActive(false);
                m_selectionCell.transform.SetParent(m_root);
            }
            else
            {
                Debug.LogError($"failed to load the selection cell prefab from {m_selectionCellPrefabPath}");
            }
        }

        protected override void Start()
        {
            base.Start();

            m_map = AstarPath.active.graphs[0] as GridGraph;

            GameObject normalCellPrefab = ResourceManager.Load<GameObject>(m_normalCellPrefabPath);
            if (normalCellPrefab != null)
            {
                m_normalCells = new GameObject[m_map.CountNodes()];
                GridNode[] nodes = m_map.nodes;
                for (int i = 0; i < m_normalCells.Length; ++i)
                {
                    // temp
                    GameObject go = Instantiate(normalCellPrefab);
                    Vector3 pos = (Vector3)nodes[i].position;
                    go.transform.position = new Vector3(pos.x, normalCellInc, pos.z);
                    go.transform.localScale = Vector3.one * 0.9f;
                    go.transform.SetParent(m_root);
                }
            }
            else
            {
                Debug.LogError($"failed to load the normal cell prefab from {m_normalCellPrefabPath}");
            }
        }
    }

}