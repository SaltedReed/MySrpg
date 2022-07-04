using System.Collections.Generic;
using Pathfinding;

namespace MySrpg
{

    public class MovablePointsTravProvider : ITraversalProvider
    {
        public MapNodeTag traversal;
        public List<Int3> occupiedPoints;

        public bool CanTraverse(Path path, GraphNode node)
        {
            // terrain traversal
            MapNodeTag nodeTag = (MapNodeTag)node.Tag;
            if ((nodeTag & traversal) != nodeTag)
                return false;

            // enemy traversal
            if (occupiedPoints != null && occupiedPoints.Contains(node.position))
            {
                return false;
            }

            return true;
        }

        public uint GetTraversalCost(Path path, GraphNode node)
        {
            return 0;
        }
    }

}