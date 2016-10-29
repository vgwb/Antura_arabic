using UnityEngine;
using System.Collections;

namespace Battlehub.WiresLegacy
{
	public class VertexHelper  
	{
		private Knot m_segment;
		private Knot m_nextSegment;
		private int m_sliceCount;
		
		private float m_t;
		private Matrix4x4 m_matrix;
		private Vector3 m_arcPositionSegLocal;
		private Vector3 m_vertexPositionArcLocal;
		private Vector3 m_vertexPosition;
		private int m_slice;
		private float m_radius;
		
		
		public void Init(Knot segment, Knot nextSegment, int sliceCount)
		{
			m_segment = segment;
			m_nextSegment = nextSegment;
			m_sliceCount = sliceCount;
			
			SetRing(0);
			SetSlice(0);
		}
		
		public void SetRing(int ringNumber)
		{
			float t;
			if(m_segment.Rings > 1)
			{
				t = ((float)ringNumber) / (m_segment.Rings - 1);
			}
			else
			{
				t = ((float)ringNumber);
			}
			
			
			t = Mathf.Max(Mathf.Min(t, 1.0f), 0.0f);
			SetT(t);
			SetSlice(0);
		}

		
		public void SetSlice(int number)
		{
			m_slice = number;
			
			float angle = m_slice * 2.0f * Mathf.PI  / m_sliceCount;
			angle = NormalizeAngle(angle) + Mathf.PI / 2.0f;
			
			float x = m_radius * Mathf.Cos(angle);
			float y = m_radius * Mathf.Sin(angle);
			float z = 0;
			
			m_vertexPositionArcLocal = new Vector3(x, y, z);
			
			m_vertexPosition = m_segment.Position +  m_matrix.MultiplyPoint(m_vertexPositionArcLocal);
		}
		
		public Vector3 GetCenter()
		{
			return m_segment.Position + m_arcPositionSegLocal;
		}
		
		public Vector3 GetVertexLocal()
		{
			return m_vertexPositionArcLocal;
		}
		
		public Vector3 GetVertex()
		{
			return m_vertexPosition;
		}
		
		public void SetT(float t)
		{
			Vector3 position = m_segment.Position;
			Vector3 p1 = m_segment.P1;
			Vector3 p2 = m_segment.P2;
			Vector3 nextPosition = m_nextSegment.Position;
			
			Vector3 lookVector;
			BezierCubicFirstDerivative(t,
			                           position,
			                           p1,
			                           p2,
			                           nextPosition,
			                           out lookVector);
			lookVector.Normalize();
			
			Vector3 intermediatePosition;
			BezierCubic(t, position,
			            p1,
			            p2,
			            nextPosition,
			            out intermediatePosition);
			
			
			Quaternion orientation = Quaternion.LookRotation(lookVector);
			m_arcPositionSegLocal = intermediatePosition - position;
			
			m_matrix = Matrix4x4.TRS(m_arcPositionSegLocal, orientation, Vector3.one);
			
			m_radius = m_segment.Radius * ( 1 - t) + m_nextSegment.Radius * t;
		}
		
		private float NormalizeAngle(float angle)
		{
			return (2.0f * Mathf.PI + angle % (2.0f * Mathf.PI)) % (2.0f * Mathf.PI);
		}
		
		private void BezierCubic(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, out Vector3 result)
		{
			result = Mathf.Pow(1 - t, 3) * p0 +
				3 * Mathf.Pow(1 - t, 2) *  t * p1 +
					3 * (1 - t) * t * t * p2 +
					t * t * t * p3;
		}
		
		private void BezierCubicFirstDerivative(float t, Vector3 p0, Vector3 p1, Vector3 p2,Vector3 p3, out Vector3 result)
		{
			result = 3 * Mathf.Pow(1 - t, 2) * (p1 - p0) +
				6 * (1 - t) * t * (p2 - p1) +
					3 * t * t * (p3 - p2);
		}
		
	}

}
