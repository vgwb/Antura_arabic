using UnityEngine;

namespace Battlehub.SplineEditor
{
    public static class CurveUtils
    {
        public static float GetT(this SplineBase spline, int curveIndex, Vector3 testPoint, float eps = 0.01f)
        {
            float s1 = 1.0f / spline.CurveCount * curveIndex;
            float s2 = s1 + 1.0f / spline.CurveCount;
            return GetT(spline, s1, s2, testPoint, eps);
        }

        private static float GetT(this SplineBase spline, float tStart, float tEnd, Vector3 testPoint, float eps = 0.01f)
        {
            float sqrEps = eps * eps;
            Vector3 start = spline.GetPoint(tStart);
            Vector3 end = spline.GetPoint(tEnd);

            Vector3 toStart = start - testPoint;
            Vector3 toEnd = end - testPoint;
            if (toStart.sqrMagnitude < toEnd.sqrMagnitude)
            {
                if ((end - start).sqrMagnitude <= sqrEps)
                {
                    return tStart;
                }
                return spline.GetT(tStart, (tStart + tEnd) / 2.0f, testPoint, eps);
            }

            if ((end - start).sqrMagnitude <= sqrEps)
            {
                return tEnd;
            }
            return spline.GetT((tStart + tEnd) / 2.0f, tEnd, testPoint, eps);
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2);
        }


    }
}

