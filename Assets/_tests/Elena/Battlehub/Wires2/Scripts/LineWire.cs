using UnityEngine;
using Battlehub.MeshDeformer2;

namespace Battlehub.Wires
{
#if FUTURE
    [RequireComponent(typeof(LineRenderer))]
    public class LineWire : Spline
    {
        [SerializeField]
        [HideInInspector]
        private int m_sliceCount = 5;

        [SerializeField]
        [HideInInspector]
        private float m_wireRadius = 0.1f;

        private LineRenderer m_lineRenderer;

        public int SliceCount
        {
            get { return m_sliceCount; }
            set
            {
                m_sliceCount = value;
            }
        }

        public float WireRadius
        {
            get { return m_wireRadius; }
            set
            {
                m_wireRadius = value;
            }
        }

        protected override void AwakeOverride()
        {
            m_lineRenderer = GetComponent<LineRenderer>();
            ResetLineRenderer();
        }

        protected override void OnCurveChanged(int pointIndex, int curveIndex)
        {
            base.OnCurveChanged(pointIndex, curveIndex);

            pointIndex = curveIndex * 3;
            for(int i = 0; i <= SliceCount; i++)
            {
                float t = i;
                t /= SliceCount;
                m_lineRenderer.SetPosition(pointIndex + i, GetPoint(t, curveIndex));
            }
        }

        private void ResetLineRenderer()
        {
            m_lineRenderer.SetVertexCount(CurveCount * SliceCount + 1);
            Vector3[] points = new Vector3[CurveCount * SliceCount + 1];
            for(int i = 0; i < points.Length; ++i)
            {
                float t = i;
                t /= (points.Length - 1);
                points[i] = GetPoint(t);
            }

            m_lineRenderer.SetPositions(points);
            m_lineRenderer.SetWidth(WireRadius, WireRadius);
        }
    }
#endif
}
