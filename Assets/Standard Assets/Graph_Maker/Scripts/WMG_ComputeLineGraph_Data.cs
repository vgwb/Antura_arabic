using UnityEngine;
using System.Collections;

/// <summary>
/// Helper class used to hold data for area shading when WMG_Series::areaShadingUsesComputeShader = true.
/// </summary>
public class WMG_ComputeLineGraph_Data : MonoBehaviour {
	public float[] pointVals = new float[4000];
}
