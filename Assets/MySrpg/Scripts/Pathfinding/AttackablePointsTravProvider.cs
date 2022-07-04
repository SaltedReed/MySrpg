using System.Collections.Generic;
using Pathfinding;

namespace MySrpg
{

    public class AttackablePointsTravProvider : ITraversalProvider
    {
        public List<Int3> friends;

        public bool CanTraverse(Path path, GraphNode node)
        {
            if (friends != null && friends.Contains(node.position))
                return false;
            return true;
        }

        public uint GetTraversalCost(Path path, GraphNode node)
        {
            return 0;
        }
    }

}