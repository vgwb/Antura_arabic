using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Battlehub.WiresLegacy
{
	public class WireRuntime : MonoBehaviour 
	{
		private const int MaxRings = 100;
		private const int MinRings = 2;
		
		protected Wire m_wire;
		private Knot[] m_knots;
		private VertexHelper m_helper;


		
		private void Start () 
		{
			CreateWire ();	
		}

		public virtual void CreateWire ()
		{
			m_helper = new VertexHelper ();
			m_wire = GetComponent<Wire> ();
			MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
			if (filter == null) 
			{
				filter = gameObject.AddComponent<MeshFilter> ();
				if (filter.sharedMesh != null)
				{
					filter.sharedMesh.name = "Generated Wire";
				}
			}
			MeshRenderer renderer = GetComponent<MeshRenderer> ();
			if (GetComponent<MeshRenderer> () == null)
			{
				renderer = gameObject.AddComponent<MeshRenderer> ();
			}
			renderer.material = m_wire.WireMaterial;
			if (filter.sharedMesh == null || string.IsNullOrEmpty (filter.sharedMesh.name) || filter.sharedMesh.name.Contains ("Generated Wire")) 
			{
				CreateKnots ();
				if (m_knots.Length < 3)
				{
					Debug.LogError ("At least 3 knots required to create wire");
				}
				else
				{
					ComputeControlPoints ();
					ComputeRingsCount ();
					GenerateWire ();
				}
			}
		}

		
		private void CreateKnots()
		{
			m_knots = GetComponentsInChildren<Knot>().ToArray();

		}
		
		private void ComputeRingsCount()
		{
			for(int i = 0; i < m_knots.Length - 1; ++i)
			{
				Knot knot = m_knots[i];
				Knot nextKnot = m_knots[i + 1];
				float ringsCount = (nextKnot.Position - knot.Position).magnitude * m_wire.LOD;
				float actualRingsCount = Mathf.Min(Mathf.Max(MinRings, ringsCount), MaxRings) * knot.LOD;
				knot.Rings = (int)actualRingsCount;
			}	
			
			m_knots[m_knots.Length - 1].Rings = 0;
		}
		
		private void GenerateWire()
		{
			Mesh mesh  = new Mesh();

			mesh.name = "Generated Wire";
			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();
			
			for(int i = 0; i < m_knots.Length - 2; ++i)
			{
				CreateSegment (i, m_knots[i].Rings - 1, vertices, uv);
			}
			CreateSegment (m_knots.Length - 2, m_knots[m_knots.Length - 2].Rings, vertices, uv);
			
			int capVerticesOffset = vertices.Count;
			CreateCaps(vertices, uv);
			
			mesh.vertices = vertices.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = CreateTriangles (vertices, capVerticesOffset);
			mesh.RecalculateNormals();

			MeshFilter filter = gameObject.GetComponent<MeshFilter>();
			filter.mesh = mesh;
		}
		
		private void ComputeControlPoints()
		{
			int n = m_knots.Length - 1;

			float[] a = new float[n];
			float[] b = new float[n];
			float[] c = new float[n];
			Vector3[] r = new Vector3[n];

			a[0] = 0.0f;
			b[0] = 2.0f;
			c[0] = 1.0f;
			r[0] = m_knots[0].Position + 2 * m_knots[1].Position;

			for (int i = 1; i < n - 1; i++)
			{
				a[i] = 1.0f;
				b[i] = 4.0f;
				c[i] = 1.0f;
				r[i] = 4 * m_knots[i].Position + 2 * m_knots[i + 1].Position;
			}

			a[n - 1] = 2.0f;
			b[n - 1] = 7.0f;
			c[n - 1] = 0.0f;
			r[n - 1] = 8 * m_knots[n - 1].Position + m_knots[n].Position;

			for (int i = 1; i < n; i++)
			{
				float m = a[i] / b[i - 1];
				b[i] = b[i] - m * c[i - 1];
				r[i] = r[i] - m * r[i - 1];
			}
			
			m_knots[n - 1].P1 = r[n - 1] / b[n - 1];
			for (int i = n - 2; i >= 0; --i)
			{
				m_knots[i].P1 = (r[i] - c[i] * m_knots[i + 1].P1) / b[i];
			}
				

			for (int i = 0; i < n - 1; i++)
			{
				m_knots[i].P2 = 2.0f * m_knots[i + 1].Position - m_knots[i + 1].P1;
			}
			
			m_knots[n - 1].P2 = 0.5f * (m_knots[n].Position + m_knots[n - 1].P1);
		}
		
		private void CreateCaps(List<Vector3> vertices, List<Vector2> uv)
		{
			m_helper.Init(m_knots[0], m_knots[1], m_wire.Slices);
			m_helper.SetRing (0);
			m_helper.SetSlice(0);
			for (int sliceNumber = 0; sliceNumber < m_wire.Slices; ++sliceNumber) 
			{
				m_helper.SetSlice (sliceNumber);
				vertices.Add( m_helper.GetVertex());
				
				Vector3 vertexLocal = m_helper.GetVertexLocal();
				uv.Add(new Vector2(0.5f + 0.5f * vertexLocal.x / m_knots[0].Radius, 0.5f + 0.5f * vertexLocal.y / m_knots[0].Radius));
			}
			
			m_helper.Init(m_knots[m_knots.Length - 2], m_knots[m_knots.Length - 1], m_wire.Slices);
			m_helper.SetRing (m_knots[m_knots.Length - 2].Rings - 1);
			m_helper.SetSlice(0);
			for (int sliceNumber = 0; sliceNumber < m_wire.Slices; ++sliceNumber) 
			{
				m_helper.SetSlice (sliceNumber);
				vertices.Add(m_helper.GetVertex());
				
				Vector3 vertexLocal = m_helper.GetVertexLocal();
				uv.Add(new Vector2(0.5f + 0.5f * vertexLocal.x / m_knots[m_knots.Length - 1].Radius, 0.5f + 0.5f * vertexLocal.y / m_knots[m_knots.Length - 1].Radius));
			}
			
			m_helper.Init(m_knots[0], m_knots[1], m_wire.Slices);
			m_helper.SetRing (0);
			m_helper.SetSlice(0);
			vertices.Add( m_helper.GetCenter());
			uv.Add(new Vector2(0.5f, 0.5f));
			
			m_helper.Init(m_knots[m_knots.Length - 2], m_knots[m_knots.Length - 1], m_wire.Slices);
			m_helper.SetRing (m_knots[m_knots.Length - 2].Rings - 1);
			m_helper.SetSlice(0);
			vertices.Add(m_helper.GetCenter());
			uv.Add(new Vector2(0.5f, 0.5f));
		}
		
		private int[] CreateTriangles (List<Vector3> vertices, int capVerticesOffset)
		{
			int totalRings = m_knots.Sum (s => s.Rings - 1) + 1;
			int[] triangles = new int[totalRings * m_wire.Slices * 6 + m_wire.Slices * 6];
			int index = 0;
			for (int i = 0; i < totalRings; ++i)
			{
				int offset = i * m_wire.Slices;
				for (int j = 0; j < m_wire.Slices; j++) {
					triangles [index] = (offset + ((j + 1) % m_wire.Slices));
					index++;
					triangles [index] = (offset + j + m_wire.Slices);
					index++;
					triangles [index] = (offset + j);
					index++;
					triangles [index] = (offset + j + m_wire.Slices);
					index++;
					triangles [index] = (offset + ((j + 1) % m_wire.Slices));
					index++;
					triangles [index] = (offset + ((j + 1) % m_wire.Slices) + m_wire.Slices);
					index++;
				}
			}
			for (int i = 0; i < m_wire.Slices; ++i) 
			{
				triangles [index] = vertices.Count - 2;
				index++;
				if (i == m_wire.Slices - 1)
				{
					triangles [index] = capVerticesOffset;
				}
				else 
				{
					triangles [index] = capVerticesOffset + i + 1;
				}
				index++;
				triangles [index] = capVerticesOffset + i;
				index++;
			}
			capVerticesOffset += m_wire.Slices;
			for (int i = 0; i < m_wire.Slices; ++i) 
			{
				triangles [index] = vertices.Count - 1;
				index++;
				triangles [index] = capVerticesOffset + i;
				index++;
				if (i == m_wire.Slices - 1) 
				{
					triangles [index] = capVerticesOffset;
				}
				else 
				{
					triangles [index] = capVerticesOffset + i + 1;
				}
				index++;
			}
			return triangles;
		}
		
		private void CreateSegment (int knotIndex, int rings, List<Vector3> vertices, List<Vector2> uv)
		{
			Knot knot = m_knots [knotIndex];
			Knot nextKnot = m_knots [knotIndex + 1];
			m_helper.Init (knot, nextKnot, m_wire.Slices);
			for (int ringNumber = 0; ringNumber < rings; ++ringNumber) 
			{
				m_helper.SetRing (ringNumber);
				for (int sliceNumber = 0; sliceNumber < m_wire.Slices; ++sliceNumber) 
				{
					m_helper.SetSlice (sliceNumber);
					
					vertices.Add(m_helper.GetVertex());
					uv.Add(new Vector3(knotIndex  + (((float)ringNumber) / (knot.Rings - 1)), ((float)sliceNumber) / (m_wire.Slices - 1)));
				}
			}
		}   
	}
}

