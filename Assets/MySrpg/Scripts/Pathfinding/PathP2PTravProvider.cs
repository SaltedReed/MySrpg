using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace MySrpg
{

    public class PathP2PTravProvider : ITraversalProvider
    {
        public MapNodeTag traversal;

        public int rangeCell
        {
            get => m_rangeCell;
            set
            {
                m_rangeCell = value;

                float nodeSize = (AstarPath.active.graphs[0] as GridGraph).nodeSize;
                float maxDistance = rangeCell * nodeSize;
                m_maxDistanceSqr = maxDistance * maxDistance + 0.1f;
            }
        }
        private int m_rangeCell;

        public Vector3 startPos;

        public List<Int3> occupiedPoints;

        private float m_maxDistanceSqr;


        public bool CanTraverse(Path path, GraphNode node)
        {
            // terrain traversal
            MapNodeTag nodeTag = (MapNodeTag)node.Tag;
            if ((nodeTag & traversal) != nodeTag)
                return false;

            // movement range
            if (((Vector3)node.position - startPos).sqrMagnitude > m_maxDistanceSqr)
                return false;

            // occupation
            if (occupiedPoints != null && occupiedPoints.Contains(node.position))
                return false;

            return true;
        }

        public uint GetTraversalCost(Path path, GraphNode node)
        {
            return 1;
        }
    }

}