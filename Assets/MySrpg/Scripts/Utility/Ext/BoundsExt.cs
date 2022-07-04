using UnityEngine;

namespace MyUtility
{

    public static class BoundsExt
    {
        public static Bounds GetBoundsInParentSpace(this Bounds b, Vector3 origin, Quaternion rot)
        {
            Vector3 right = rot * Vector3.right; //Debug.DrawLine(origin, origin + right, Color.red, 1.0f);
            Vector3 fwd = rot * Vector3.forward; //Debug.DrawLine(origin, origin + fwd, Color.blue, 1.0f);
            Vector3 up = rot * Vector3.up;

            Vector3 center = right * b.center.x + up * b.center.y + fwd * b.center.z + origin;
            return new Bounds(center, b.size);
        }
    }

}