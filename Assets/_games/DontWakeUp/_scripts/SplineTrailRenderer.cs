using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EA4S {
public class SplineTrailRenderer : MonoBehaviour 
{
	public enum MeshDisposition { Continuous, Fragmented };
	public enum FadeType { None, MeshShrinking, Alpha, Both }

	public class AdvancedParameters
	{
		public int baseNbQuad = 500;
		public int nbQuadIncrement = 200;
		//public bool releaseMemoryOnClear = true;
		public int nbSegmentToParametrize = 3; //0 means all segments
		public float lengthToRedraw = 0; //0 means fadeDistance and should suffice, but you can
										 //redraw a smaller section when there is no fade as an 
										 //optimization
		public bool shiftMeshData = true; //optimization to prevent memory shortage on very long
										   //trails. May induce lag spikes when shifting.
										   //todo: fix the exception induced by modifying the max length (mesh sliding)
	}
	
	public bool emit = true;
	public float emissionDistance = 1f;
	public float height = 1f;
	public float width = 0.2f;
	public Color vertexColor = Color.white;
	public Vector3 normal = new Vector3(0, 0, 1);
	public bool dynamicNormalUpdate = false;
	public MeshDisposition meshDisposition = MeshDisposition.Continuous;
	public FadeType fadeType = FadeType.None;
	public float fadeLengthBegin = 5f;
	public float fadeLengthEnd = 5f;
	//public float fadeSpeed = 0f; 
	public float maxLength = 50f;
	public bool debugDrawSpline = false;

	private AdvancedParameters advancedParameters = new AdvancedParameters(); 

	[HideInInspector]
	public CatmullRomSpline spline;
	
	private const int NbVertexPerQuad = 4;
	private const int NbTriIndexPerQuad = 6;

	Vector3[] vertices;
	int[] triangles;
	Vector2[] uv;
	Vector2[] uv2;
    Color[] colors;
	Vector3[] normals;

	private Vector3 origin;
	private int maxInstanciedTriCount = 0;
	private Mesh mesh;
	private int allocatedNbQuad;
	private int lastStartingQuad;
	private int quadOffset;

	public void Clear()
	{
		Init();
	}

	public void ImitateTrail(SplineTrailRenderer trail)
	{
		emit = trail.emit;
		emissionDistance = trail.emissionDistance;
		height = trail.height;
		width = trail.width;
		vertexColor = trail.vertexColor;
		normal = trail.normal;
		meshDisposition = trail.meshDisposition;
		fadeType = trail.fadeType;
		fadeLengthBegin = trail.fadeLengthBegin;
		fadeLengthEnd = trail.fadeLengthEnd;
		maxLength = trail.maxLength;
		debugDrawSpline = trail.debugDrawSpline;
		GetComponent<Renderer>().material = trail.GetComponent<Renderer>().material;
	}

	private void Start () 
	{
		if(spline == null)
		{
			Init();
		}
	}

	private void LateUpdate()
	{
		if(emit)
		{
			List<Knot> knots = spline.knots;
			Vector3 point = transform.position;

			knots[knots.Count-1].position = point;
			knots[knots.Count-2].position = point;

			if(Vector3.Distance(knots[knots.Count-3].position, point) > emissionDistance &&
				Vector3.Distance(knots[knots.Count-4].position, point) > emissionDistance)
			{
				knots.Add(new Knot(point));
			}
		}

		RenderMesh();
	}
	
	private void RenderMesh() 
	{
		if(advancedParameters.nbSegmentToParametrize == 0)
			spline.Parametrize();
		else
			spline.Parametrize(spline.NbSegments-advancedParameters.nbSegmentToParametrize, spline.NbSegments);
		
		float length = Mathf.Max(spline.Length() - 0.1f, 0);
		
		int nbQuad = ((int)(1f/width * length)) + 1 - quadOffset;
		
		if(allocatedNbQuad < nbQuad) //allocate more memory for the mesh
		{
			Reallocate(nbQuad);
			length = Mathf.Max(spline.Length() - 0.1f, 0);
			nbQuad = ((int)(1f/width * length)) + 1 - quadOffset;
		}

		int startingQuad = lastStartingQuad;
		float lastDistance = startingQuad * width + quadOffset * width;
		maxInstanciedTriCount = System.Math.Max(maxInstanciedTriCount, (nbQuad-1) * NbTriIndexPerQuad);
		
		Vector3 n = normal;
		if(dynamicNormalUpdate)
		{
			if(n == Vector3.zero)
			{
				n = (transform.position - Camera.main.transform.position).normalized;
			}
	
			for(int i=0; i<normals.Length; i++)
			{
				normals[i] = n;
			}
		}

		CatmullRomSpline.Marker marker = new CatmullRomSpline.Marker();
		spline.PlaceMarker(marker, lastDistance); 

		Vector3 lastPosition = spline.GetPosition(marker);
		Vector3 lastTangent = spline.GetTangent(marker);
		Vector3 lastBinormal = CatmullRomSpline.ComputeBinormal(lastTangent, n);

		int drawingEnd = meshDisposition == MeshDisposition.Fragmented ? nbQuad-1 : nbQuad-1;
		
		
		
		float startingDist = lastDistance;
		for(int i=startingQuad; i<drawingEnd; i++)
		{
			float distance = lastDistance+width;
			int firstVertexIndex = i * NbVertexPerQuad;
			int firstTriIndex = i * NbTriIndexPerQuad;

			spline.MoveMarker(marker, distance);

			Vector3 position = spline.GetPosition(marker);
			Vector3 tangent = spline.GetTangent(marker);
			Vector3 binormal = CatmullRomSpline.ComputeBinormal(tangent, n);

			float h = FadeMultiplier(lastDistance, length);
			float h2 = FadeMultiplier(distance, length);
			float rh = h * height, rh2 = h2 * height; 

			if(fadeType == FadeType.Alpha || fadeType == FadeType.None)
			{
				rh = h > 0 ? height : 0;
				rh2 = h2 > 0 ? height : 0;
			}

			if(meshDisposition == MeshDisposition.Continuous)
			{
				vertices[firstVertexIndex] = transform.InverseTransformPoint(lastPosition - origin + (lastBinormal * (rh * 0.5f)));
				vertices[firstVertexIndex + 1] = transform.InverseTransformPoint(lastPosition - origin + (-lastBinormal * (rh * 0.5f)));
        		vertices[firstVertexIndex + 2] = transform.InverseTransformPoint(position - origin + (binormal * (rh2 * 0.5f)));
				vertices[firstVertexIndex + 3] = transform.InverseTransformPoint(position - origin + (-binormal * (rh2 * 0.5f)));
            
				uv[firstVertexIndex] =  new Vector2(lastDistance/height, 1);  
				uv[firstVertexIndex + 1] = new Vector2(lastDistance/height, 0);
        		uv[firstVertexIndex + 2] = new Vector2(distance/height, 1); 
				uv[firstVertexIndex + 3] = new Vector2(distance/height, 0);
			}
			else
			{
				Vector3 pos = lastPosition + (lastTangent * width * -0.5f) - origin;

				vertices[firstVertexIndex] = transform.InverseTransformPoint(pos + (lastBinormal * (rh * 0.5f)));
			    vertices[firstVertexIndex + 1] = transform.InverseTransformPoint(pos + (-lastBinormal * (rh * 0.5f)));
        	    vertices[firstVertexIndex + 2] = transform.InverseTransformPoint(pos + (lastTangent * width) + (lastBinormal * (rh * 0.5f)));
			    vertices[firstVertexIndex + 3] = transform.InverseTransformPoint(pos + (lastTangent * width) + (-lastBinormal * (rh * 0.5f)));
				
				uv[firstVertexIndex] =  new Vector2(0, 1);  
			    uv[firstVertexIndex + 1] = new Vector2(0, 0);
        	    uv[firstVertexIndex + 2] = new Vector2(1, 1); 
			    uv[firstVertexIndex + 3] = new Vector2(1, 0);
			}
			
			float relLength = length-startingDist;
			uv2[firstVertexIndex] =  new Vector2((lastDistance-startingDist)/relLength, 1);  
			uv2[firstVertexIndex + 1] = new Vector2((lastDistance-startingDist)/relLength, 0);
    		uv2[firstVertexIndex + 2] = new Vector2((distance-startingDist)/relLength, 1); 
			uv2[firstVertexIndex + 3] = new Vector2((distance-startingDist)/relLength, 0);

			triangles[firstTriIndex] = firstVertexIndex;
			triangles[firstTriIndex + 1] = firstVertexIndex + 1;
			triangles[firstTriIndex + 2] = firstVertexIndex + 2;
			triangles[firstTriIndex + 3] = firstVertexIndex + 2; 
			triangles[firstTriIndex + 4] = firstVertexIndex + 1;
			triangles[firstTriIndex + 5] = firstVertexIndex + 3; 

			colors[firstVertexIndex] = vertexColor;
			colors[firstVertexIndex + 1] = vertexColor;
        	colors[firstVertexIndex + 2] = vertexColor;
			colors[firstVertexIndex + 3] = vertexColor;

			if(fadeType == FadeType.Alpha || fadeType == FadeType.Both)
			{
				colors[firstVertexIndex].a *= h;
				colors[firstVertexIndex + 1].a *= h;
        		colors[firstVertexIndex + 2].a *= h2;
				colors[firstVertexIndex + 3].a *= h2;
			}

			lastPosition = position;
			lastTangent = tangent;
			lastBinormal = binormal;
			lastDistance = distance;
		}
		
		for(int i=(nbQuad-1)*NbTriIndexPerQuad; i<maxInstanciedTriCount; i++) //clear a few tri ahead
			triangles[i] = 0;

		lastStartingQuad = advancedParameters.lengthToRedraw == 0 ? 
			System.Math.Max(0, nbQuad - ((int)(maxLength / width) + 5)) :
			System.Math.Max(0, nbQuad - ((int)(advancedParameters.lengthToRedraw / width) + 5));

        mesh.Clear();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.uv2 = uv2;
		mesh.triangles = triangles;
        mesh.colors = colors;
		mesh.normals = normals;
	}

	private void OnDrawGizmos()
    {
		//DebugDrawEquallySpacedDots();
		if(advancedParameters != null && spline != null && debugDrawSpline)
		{
			spline.DebugDrawSpline();
		}
		//DebugDrawSubKnots();
    }

	private void Init()
	{
		if(spline == null)
		{
			spline = new CatmullRomSpline();
		}
		
		origin = Vector3.zero;//transform.position;

		mesh = GetComponent<MeshFilter>().mesh;

#if UNITY_4_0
		mesh.MarkDynamic();
#endif

		allocatedNbQuad = advancedParameters.baseNbQuad;
		maxInstanciedTriCount = 0;
		lastStartingQuad = 0;
		quadOffset = 0;


		vertices = new Vector3[advancedParameters.baseNbQuad * NbVertexPerQuad];
		triangles = new int[advancedParameters.baseNbQuad * NbTriIndexPerQuad];
		uv = new Vector2[advancedParameters.baseNbQuad * NbVertexPerQuad];
		uv2 = new Vector2[advancedParameters.baseNbQuad * NbVertexPerQuad];
		colors = new Color[advancedParameters.baseNbQuad * NbVertexPerQuad];
		normals = new Vector3[advancedParameters.baseNbQuad * NbVertexPerQuad];
		
		Vector3 n = normal;
		if(n == Vector3.zero)
		{
			n = (transform.position - Camera.main.transform.position).normalized;
		}

		for(int i=0; i<normals.Length; i++)
		{
			normals[i] = n;
		}

		//spline.knots.Clear();
		spline.Clear();

		List<Knot> knots = spline.knots;
		Vector3 point = transform.position;

		knots.Add(new Knot(point));
		knots.Add(new Knot(point));
		knots.Add(new Knot(point));
		knots.Add(new Knot(point));
		knots.Add(new Knot(point));
	}

	private void Reallocate(int nbQuad)
	{
		if(advancedParameters.shiftMeshData && lastStartingQuad > 0/*advancedParameters.nbQuadIncrement / 4*/) //slide
		{
			int newIndex = 0;
			for(int i=lastStartingQuad; i<nbQuad; i++)
			{
				vertices[newIndex] = vertices[i];
				triangles[newIndex] = triangles[i];
				uv[newIndex] = uv[i];
				colors[newIndex] = colors[i];
				normals[newIndex] =  normals[i];

				newIndex++;
			}

			quadOffset += lastStartingQuad;
			lastStartingQuad = 0;
			
		}
		
		float length = Mathf.Max(spline.Length() - 0.1f, 0);
		nbQuad = ((int)(1f/width * length)) + 1 - quadOffset;
		
		if(allocatedNbQuad < nbQuad)
		{
			if((allocatedNbQuad + advancedParameters.nbQuadIncrement) * NbVertexPerQuad > 65000)
			{
				Clear();
				return;
			}

			allocatedNbQuad += advancedParameters.nbQuadIncrement;

			Vector3[] vertices_2 = new Vector3[allocatedNbQuad * NbVertexPerQuad];
			int[] triangles_2 = new int[allocatedNbQuad * NbTriIndexPerQuad];
			Vector2[] uv_2 = new Vector2[allocatedNbQuad * NbVertexPerQuad];
			Vector2[] uv2_2 = new Vector2[allocatedNbQuad * NbVertexPerQuad];
			Color[] colors_2 = new Color[allocatedNbQuad * NbVertexPerQuad];
			Vector3[] normals_2 = new Vector3[allocatedNbQuad * NbVertexPerQuad];

			vertices.CopyTo(vertices_2, 0);
			triangles.CopyTo(triangles_2, 0);
			uv.CopyTo(uv_2, 0);
			uv2.CopyTo(uv2_2, 0);
			
			colors.CopyTo(colors_2, 0);
			normals.CopyTo(normals_2, 0);

			vertices = vertices_2;
			triangles = triangles_2;
			uv = uv_2;
			uv2 = uv2_2;
			colors = colors_2;
			normals = normals_2;
			
			
			
		}
	}

	float FadeMultiplier(float distance, float length)
	{
		float ha = Mathf.Clamp01((distance - Mathf.Max(length-maxLength, 0)) / fadeLengthBegin);
		float hb = Mathf.Clamp01((length-distance) / fadeLengthEnd);

		return Mathf.Min(ha, hb);
	}
}
}