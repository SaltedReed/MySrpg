using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace MyUtility
{


    public static class GraphNodeListExt
    {
        public static List<Vector3> ToVector3(this List<GraphNode> nodes)
        {
            List<Vector3> vecs = new List<Vector3>();

            foreach (GraphNode n in nodes)
            {
                vecs.Add((Vector3)n.position);
            }

            return vecs;
        }

        public static List<Int3> ToInt3(this List<GraphNode> nodes)
        {
            List<Int3> ints = new List<Int3>();

            foreach (GraphNode n in nodes)
            {
                ints.Add(n.position);
            }

            return ints;
        }
    }


    public static class Int3ListExt
    {
        public static List<Vector3> ToVector3(this List<Int3> ints)
        {
            List<Vector3> vecs = new List<Vector3>();

            foreach (Int3 n in ints)
            {
                vecs.Add((Vector3)n);
            }

            return vecs;
        }
    }


    public static class Vector3ListExt
    {
        public static List<Int3> ToInt3(this List<Vector3> vecs)
        {
            List<Int3> ints = new List<Int3>();

            foreach (Vector3 v in vecs)
            {
                ints.Add((Int3)v);
            }

            return ints;
        }
    }

}