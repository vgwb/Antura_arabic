using UnityEngine;

namespace Battlehub.SplineEditor
{
    [ExecuteInEditMode]
    public class Spline : SplineBase
    {
        private const float Mag = 5.0f;

        protected override void OnCurveChanged()
        {
#if UNITY_EDITOR
            PersistentVersions[0]++;
            OnVersionChanged();
#endif
        }
#if UNITY_EDITOR
        protected override void AwakeOverride()
        {
            PersistentVersions[0]++;
            OnVersionChanged();
        }
#endif

        protected override float GetMag()
        {
            return Mag;
        }
        private void AppendCurve(float mag, bool enforceNeighbour)
        {
            Vector3 dir = transform.InverseTransformDirection(GetDirection(1.0f));
            Vector3 point = GetPoint(1.0f);
            point = transform.InverseTransformPoint(point);

            int pointsCount = 3;
            float deltaT = 1.0f / pointsCount;
            float t = deltaT;
            Vector3[] points = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; ++i)
            {
                points[i] = point + dir * mag * t;
                t += deltaT;
            }

            AppendCurve(points, enforceNeighbour);
        }

        private void PrependCurve(float mag, int curveIndex, bool enforceNeighbour)
        {
            Vector3 dir = GetDirection(0.0f, curveIndex);
            Vector3 point = GetPoint(0.0f, curveIndex);

            dir = transform.InverseTransformDirection(dir);
            point = transform.InverseTransformPoint(point);

            int pointsCount =  3;
            float deltaT = 1.0f / pointsCount;
            float t = 1.0f;
            Vector3[] points = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; ++i)
            {
                points[i] = point - dir * mag * t;
                t -= deltaT;
            }

            PrependCurve(points, curveIndex, mag, enforceNeighbour);
        }

        public override void Load(SplineSnapshot snapshot)
        {
            LoadSpline(snapshot);
#if UNITY_EDITOR
            PersistentVersions[0]++;
            OnVersionChanged();
#endif
        }

    
        public void Extend(bool prepend = false)
        {
            if (prepend && !Loop)
            {
                const int curveIndex = 0;
                PrependCurve(Mag, curveIndex, false);
            }
            else
            {
                AppendCurve(Mag, false);
            }

           
#if UNITY_EDITOR
            PersistentVersions[0]++;
            OnVersionChanged();
#endif
        }

        public bool Remove(int curveIndex)
        {
            bool result = RemoveCurve(curveIndex);
#if UNITY_EDITOR
            PersistentVersions[0]++;
            OnVersionChanged();
#endif

            return result;
        }
    }
}

