using UnityEngine;

namespace EA4S
{
    public static class MathUtils
    {
        public static float AngleCounterClockwise(Vector2 a, Vector2 b)
        {
            float dot = Vector2.Dot(a.normalized, b.normalized);
            dot = Mathf.Clamp(dot, -1.0f, 1.0f);

            if (Cross(a, b) >= 0)
                return Mathf.Acos(dot);
            return Mathf.PI * 2 - Mathf.Acos(dot);
        }

        /// <summary>
        /// Lerp the current transform rotation to point towards world position "position" using t.
        /// The look-at is planar, meaning that the transform.up will be Vector3.up.
        /// </summary>
        public static void LerpLookAtPlanar(Transform transform, Vector3 position, float t)
        {
            Vector3 targetDir3D = (transform.position - position);
            if (targetDir3D.sqrMagnitude < 0.001f)
                return;

            Vector2 targetDir = new Vector2(targetDir3D.x, targetDir3D.z);
            Vector2 currentDir = new Vector2(transform.forward.x, transform.forward.z);

            targetDir.Normalize();
            currentDir.Normalize();

            var desiredAngle = AngleCounterClockwise(targetDir, Vector2.down);
            var currentAngle = AngleCounterClockwise(currentDir, Vector2.up);

            currentAngle = Mathf.LerpAngle(currentAngle, desiredAngle, t);

            transform.rotation = Quaternion.AngleAxis(currentAngle * Mathf.Rad2Deg, Vector3.up);
        }


        static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
    }
}