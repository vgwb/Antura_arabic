using UnityEngine;

namespace EA4S.Assessment
{
    public static class Vector3Extension
    {
        public static bool DistanceIsLessThan( this Vector3 me, Vector3 other, float distance)
        {
            float dx = me.x - other.x;
            float dy = me.y - other.y;
            float dz = me.z - other.z;
            float squaredDistance = dx * dx + dy * dy + dz * dz;
            return squaredDistance < distance * distance;
        }
    }
}
