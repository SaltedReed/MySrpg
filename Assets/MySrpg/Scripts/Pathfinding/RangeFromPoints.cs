using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyFramework;
using Pathfinding;

namespace MySrpg
{

    public delegate void OnFindRangeFromPointsHandler(RangeFromPoints result);


    public class RangeFromPoints
    {
        public List<Int3> startPoints;
        public int rangeCell;
        public ITraversalProvider traversalProvider;
        public OnFindRangeFromPointsHandler handler;

        public List<Int3> points => new List<Int3>(m_pointSet);

        protected HashSet<Int3> m_pointSet;
        protected bool m_isDone = true;

        public bool IsDone()
        {
            return m_isDone;
        }

        public void StartFind()
        {
            if (startPoints.Count == 0)
                return;

            m_isDone = false;

            m_pointSet = new HashSet<Int3>();
            int progress = 0;
            int maxProgress = startPoints.Count;

            for (int i = 0; i < startPoints.Count; ++i)
            {
                RangeFromOnePoint range = new RangeFromOnePoint();
                range.startPos = (Vector3)startPoints[i];
                range.rangeCell = rangeCell;
                range.traversalProvider = traversalProvider;
                range.handler = (RangeFromOnePoint result) =>
                {
                    m_pointSet.UnionWith(result.points);
                    progress++;
                    if (progress == maxProgress)
                    {
                        handler?.Invoke(this);
                        m_isDone = true;
                    }
                };

                range.StartFind();
            }
        }


    }

}