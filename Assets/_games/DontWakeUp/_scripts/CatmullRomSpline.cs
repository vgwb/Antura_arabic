using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Knot
{
	public float distanceFromStart = -1f;
	public CatmullRomSpline.SubKnot[] subKnots = new CatmullRomSpline.SubKnot[CatmullRomSpline.NbSubSegmentPerSegment+1]; //[0, 1]
	public Vector3 position;

	public Knot(Vector3 position)
	{
		this.position = position;
	}

	public void Invalidate()
	{
		distanceFromStart = -1f;
	}
}

public class CatmullRomSpline
{
	public struct SubKnot
	{
		public float distanceFromStart;
		public Vector3 position;
		public Vector3 tangent;
	}

	public class Marker
	{
		public int segmentIndex;
		public int subKnotAIndex;
		public int subKnotBIndex;
		public float lerpRatio;
	}

	public List<Knot> knots = new List<Knot>();
	
	public const int NbSubSegmentPerSegment = 10;

	private const int MinimumKnotNb = 4;
	private const int FirstSegmentKnotIndex = 2;
	
	public int NbSegments { get { return System.Math.Max(0, knots.Count - 3); } }

	public Vector3 FindPositionFromDistance(float distance)
    {
        Vector3 tangent = Vector3.zero;
        
        Marker result = new Marker();
        bool foundSegment = PlaceMarker(result, distance);

        if(foundSegment)
        {
			tangent = GetPosition(result);
        }
		
        return tangent;
    }

	public Vector3 FindTangentFromDistance(float distance)
    {
        Vector3 tangent = Vector3.zero;
        
        Marker result = new Marker();
        bool foundSegment = PlaceMarker(result, distance);

        if(foundSegment)
        {
			tangent = GetTangent(result);
        }
		
        return tangent;
    }

	public static Vector3 ComputeBinormal(Vector3 tangent, Vector3 normal)
	{
		return Vector3.Cross(tangent, normal).normalized;
	}

	public float Length()
	{
		if(NbSegments == 0) return 0f;

		//Parametrize();

		return System.Math.Max(0, GetSegmentDistanceFromStart(NbSegments-1));
	}

	public void Clear()
	{
		knots.Clear();
	}

	public void MoveMarker(Marker marker, float distance) //in Unity units
	{
		PlaceMarker(marker, distance, marker);
	}

	public Vector3 GetPosition(Marker marker)
    {
        Vector3 pos = Vector3.zero;

		if(NbSegments == 0) return pos;

		SubKnot[] subKnots = GetSegmentSubKnots(marker.segmentIndex);
        
		pos = Vector3.Lerp(subKnots[marker.subKnotAIndex].position, 
			subKnots[marker.subKnotBIndex].position, marker.lerpRatio);
		
        return pos;
    }

	public Vector3 GetTangent(Marker marker)
    {
		Vector3 tangent = Vector3.zero;

		if(NbSegments == 0) return tangent;

		SubKnot[] subKnots = GetSegmentSubKnots(marker.segmentIndex);
        
		tangent = Vector3.Lerp(subKnots[marker.subKnotAIndex].tangent, 
			subKnots[marker.subKnotBIndex].tangent, marker.lerpRatio);
		
        return tangent;
    }

	private float Epsilon { get { return 1f / NbSubSegmentPerSegment; } }

	private SubKnot[] GetSegmentSubKnots(int i) 
	{
		return knots[FirstSegmentKnotIndex+i].subKnots;
	}

	public float GetSegmentDistanceFromStart(int i) 
	{
		return knots[FirstSegmentKnotIndex+i].distanceFromStart;
	}

	private bool IsSegmentValid(int i)
	{
		return knots[i].distanceFromStart != -1f && knots[i+1].distanceFromStart != -1f &&
			knots[i+2].distanceFromStart != -1f && knots[i+3].distanceFromStart != -1f;
	}

	private bool OutOfBoundSegmentIndex(int i)
	{
		return i < 0 || i >= NbSegments;
	}

	public void Parametrize()
	{
		Parametrize(0, NbSegments-1);
	}
	
	public void Parametrize(int fromSegmentIndex, int toSegmentIndex)
	{
		if(knots.Count < MinimumKnotNb) return;
		
		int nbSegments = System.Math.Min(toSegmentIndex+1, NbSegments);
		fromSegmentIndex = System.Math.Max(0, fromSegmentIndex);
		float totalDistance = 0;

		if(fromSegmentIndex > 0)
		{
			totalDistance = GetSegmentDistanceFromStart(fromSegmentIndex-1);
		}

		for(int i=fromSegmentIndex; i<nbSegments; i++)
		{
			/*if(IsSegmentValid(i) && !force)
			{
				totalDistance = GetSegmentDistanceFromStart(i);
				continue;
			}*/
			
			SubKnot[] subKnots = GetSegmentSubKnots(i);
			
			for(int j=0; j<subKnots.Length; j++)
			{
				SubKnot sk = new SubKnot();
				
				sk.distanceFromStart = totalDistance += ComputeLengthOfSegment(i, (j-1)*Epsilon, j*Epsilon);
				sk.position = GetPositionOnSegment(i, j*Epsilon);
				sk.tangent = GetTangentOnSegment(i, j*Epsilon);

				subKnots[j] = sk;
			}
			
			knots[FirstSegmentKnotIndex+i].distanceFromStart = totalDistance;
		}
	}

	public bool PlaceMarker(Marker result, float distance, Marker from = null)
    {
		//result = new Marker();
		SubKnot[] subKnots;
		int nbSegments = NbSegments;

		if(nbSegments == 0) return false;

		//Parametrize();

		if(distance <= 0)
		{
			result.segmentIndex = 0;
			result.subKnotAIndex = 0;
			result.subKnotBIndex = 1;
			result.lerpRatio = 0f;
			return true;
		}
		else if(distance >= Length())
		{
			subKnots = GetSegmentSubKnots(nbSegments-1);
			result.segmentIndex = nbSegments-1;
			result.subKnotAIndex = subKnots.Length-2;
			result.subKnotBIndex = subKnots.Length-1;
			result.lerpRatio = 1f;
			return true;
		}

		int fromSegmentIndex = 0;
		int fromSubKnotIndex = 1;
		if(from != null)
		{
			fromSegmentIndex = from.segmentIndex;
			//fromSubKnotIndex = from.subKnotAIndex;
		}

		for(int i=fromSegmentIndex; i<nbSegments; i++)
		{
			if(distance > GetSegmentDistanceFromStart(i)) continue;

			subKnots = GetSegmentSubKnots(i);
			
			for(int j=fromSubKnotIndex; j<subKnots.Length; j++)
			{
				SubKnot sk = subKnots[j];

				if(distance > sk.distanceFromStart) continue;

				result.segmentIndex = i;
				result.subKnotAIndex = j-1;
				result.subKnotBIndex = j;
				result.lerpRatio = 1f - ((sk.distanceFromStart - distance) / 
					(sk.distanceFromStart - subKnots[j-1].distanceFromStart));

				break;
			}

			break;
		}

		return true;
	}
	
	private float ComputeLength()
    {
	    if(knots.Count < 4) return 0;
     
        float length = 0;
  
        int nbSegments = NbSegments;
		for(int i=0; i<nbSegments; i++)
		{
			length += ComputeLengthOfSegment(i, 0f, 1f);
		}

        return length;
    }
	
	private float ComputeLengthOfSegment(int segmentIndex, float from, float to)
    {
		float length = 0;
		from = Mathf.Clamp01(from);
		to = Mathf.Clamp01(to);

        Vector3 lastPoint = GetPositionOnSegment(segmentIndex, from);

        for(float j=from+Epsilon; j<to+Epsilon/2f; j+=Epsilon)
        {
            Vector3 point = GetPositionOnSegment(segmentIndex, j);
			length += Vector3.Distance(point, lastPoint);
			lastPoint = point;
        }

        return length;
    }

	public void DebugDrawEquallySpacedDots()
	{
		Gizmos.color = Color.red;
		int nbPoints = NbSubSegmentPerSegment*NbSegments;
		float length = Length();

		Marker marker = new Marker();
		PlaceMarker(marker, 0f); 

		for(int i=0; i<=nbPoints; i++)
		{
			MoveMarker(marker, i*(length/nbPoints));

			Vector3 position = GetPosition(marker);
			//Vector3 tangent = GetTangent(marker);

			//Vector3 position = FindPositionFromDistance(i*(length/nbPoints));
			//Vector3 tangent = FindTangentFromDistance(i*(length/nbPoints));

			//Vector3 binormal = ComputeBinormal(tangent, new Vector3(0, 0, 1));

			Gizmos.DrawWireSphere(position, 0.025f);
			//Debug.DrawRay(position, binormal * 0.2f, Color.green);
		}
	}

	public void DebugDrawSubKnots()
	{
		Gizmos.color = Color.yellow;
		int nbSegments = NbSegments;

		for(int i=0; i<nbSegments; i++)
		{
			SubKnot[] subKnots = GetSegmentSubKnots(i);

			for(int j=0; j<subKnots.Length; j++)
			{
				Gizmos.DrawWireSphere(subKnots[j].position, 0.025f);
				//Gizmos.DrawWireSphere(new Vector3(segments[i].subSegments[j].length, 0, 0), 0.025f);
			}
		}
	}

	public void DebugDrawSpline()
    {
        if(knots.Count >= 4)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(knots[0].position, 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(knots[knots.Count-1].position, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(knots[knots.Count-2].position, 0.2f);

			int nbSegments = NbSegments;
            for(int i=0; i<nbSegments; i++)
            {
                Vector3 lastPoint = GetPositionOnSegment(i, 0f);

				Gizmos.DrawWireSphere(lastPoint, 0.2f);

                for(float j=Epsilon; j<1f+Epsilon/2f; j+=Epsilon)
                {
                    Vector3 point = GetPositionOnSegment(i, j);
					Debug.DrawLine(lastPoint, point, Color.white); 
					lastPoint = point;
                }
            }
        }
    }
	
	private Vector3 GetPositionOnSegment(int segmentIndex, float t)
	{
		return FindSplinePoint(knots[segmentIndex].position, knots[segmentIndex+1].position, 
            knots[segmentIndex+2].position, knots[segmentIndex+3].position, t);
	}
	
	private Vector3 GetTangentOnSegment(int segmentIndex, float t)
	{
		return FindSplineTangent(knots[segmentIndex].position, knots[segmentIndex+1].position, 
            knots[segmentIndex+2].position, knots[segmentIndex+3].position, t).normalized;
	}
	
	private static Vector3 FindSplinePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 ret = new Vector3();

		float t2 = t * t;
		float t3 = t2 * t;

		ret.x = 0.5f * ((2.0f * p1.x) +
			(-p0.x + p2.x) * t +
			(2.0f * p0.x - 5.0f * p1.x + 4 * p2.x - p3.x) * t2 +
			(-p0.x + 3.0f * p1.x - 3.0f * p2.x + p3.x) * t3);

		ret.y = 0.5f * ((2.0f * p1.y) +
			(-p0.y + p2.y) * t +
			(2.0f * p0.y - 5.0f * p1.y + 4 * p2.y - p3.y) * t2 +
			(-p0.y + 3.0f * p1.y - 3.0f * p2.y + p3.y) * t3);

		ret.z = 0.5f * ((2.0f * p1.z) +
			(-p0.z + p2.z) * t +
			(2.0f * p0.z - 5.0f * p1.z + 4 * p2.z - p3.z) * t2 +
			(-p0.z + 3.0f * p1.z - 3.0f * p2.z + p3.z) * t3);

		return ret;
	}

    private static Vector3 FindSplineTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		Vector3 ret = new Vector3();

		float t2 = t * t;

		ret.x = 0.5f * (-p0.x + p2.x) + 
			(2.0f * p0.x - 5.0f * p1.x + 4 * p2.x - p3.x) * t + 
			(-p0.x + 3.0f * p1.x - 3.0f * p2.x + p3.x) * t2 * 1.5f;

		ret.y = 0.5f * (-p0.y + p2.y) + 
			(2.0f * p0.y - 5.0f * p1.y + 4 * p2.y - p3.y) * t + 
			(-p0.y + 3.0f * p1.y - 3.0f * p2.y + p3.y) * t2 * 1.5f;

		ret.z = 0.5f * (-p0.z + p2.z) + 
			(2.0f * p0.z - 5.0f * p1.z + 4 * p2.z - p3.z) * t + 
			(-p0.z + 3.0f * p1.z - 3.0f * p2.z + p3.z) * t2 * 1.5f;

		return ret;
	}
}