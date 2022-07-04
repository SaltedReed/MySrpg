using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using MyUtility;

namespace MySrpg
{

    public delegate void OnFindPathP2PHandler(PathP2P result);


    public class PathP2P
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public ITraversalProvider traversalProvider;
        public OnFindPathP2PHandler handler;

        public List<Int3> points { get; protected set; }

        protected ABPath m_path;

        public bool IsDone()
        {
            return m_path is null || m_path.IsDone();
        }

        public void StartFind()
        {
            m_path = ABPath.Construct(startPos, endPos, (Path p) =>
            {
                points = p.path.ToInt3();
                handler?.Invoke(this);
            });

            m_path.traversalProvider = traversalProvider;

            AstarPath.StartPath(m_path, true);
        }
    }

}