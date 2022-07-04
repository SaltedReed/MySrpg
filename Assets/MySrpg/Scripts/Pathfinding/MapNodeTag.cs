using System;

namespace MySrpg
{

    // NOTE: must correspond with AstarPathfinding tag
    // when used as character's traversal ability, it is bit masks
    [Flags]
    public enum MapNodeTag : int
    {
        BasicGround = 1,
        Water       = 1<<1
    }

}
