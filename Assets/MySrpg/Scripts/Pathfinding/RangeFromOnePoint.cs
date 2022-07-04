using System.Collections.Generic;
using UnityEngine;
using MyUtility;
using MyFramework;
using Pathfinding;

namespace MySrpg
{

    public delegate void OnFindRangeFromOnePointHandler(RangeFromOnePoint result);


    public class RangeFromOnePoint
    {
        public Vector3 startPos;
        public int rangeCell;
        public ITraversalProvider traversalProvider;
        public OnFindRangeFromOnePointHandler handler;

        public List<Int3> points { get; protected set; }

        protected AllNodePath m_path;


        public bool IsDone()
        {
            return m_path is null || m_path.IsDone();
        }

        public void StartFind()
        {
            int actualRange = Mathf.RoundToInt(rangeCell * (AstarPath.active.graphs[0] as GridGraph).nodeSize * Int3.Precision);

            m_path = AllNodePath.Construct(startPos, actualRange, (Path p) =>
            {
                AllNodePath anpath = p as AllNodePath;
                points = anpath.allNodes.ToInt3();
                handler?.Invoke(this);
            });

            m_path.traversalProvider = traversalProvider;

            AstarPath.StartPath(m_path, true);
        }
    }

}
