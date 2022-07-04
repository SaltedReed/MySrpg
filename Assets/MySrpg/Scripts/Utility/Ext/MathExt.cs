using UnityEngine;

namespace MyUtility
{

    public static class Vector3Ext
    {
        public static float SqrMagnitudeXZ(this Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x, dz = a.z - b.z;
            return dx * dx + dz * dz;
        }

        public static Vector3 L2W(this Vector3 v, Matrix4x4 l2wMat)
        {
            return l2wMat * new Vector4(v.x, v.y, v.z, 1);
        }
    }

}