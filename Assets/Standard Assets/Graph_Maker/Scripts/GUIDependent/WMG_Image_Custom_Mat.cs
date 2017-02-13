using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Class used in place of Image component for series area shading rectangles to ensure that it works with Unity UI Masking such as within a ScrollView.
/// </summary>
public class WMG_Image_Custom_Mat : Image
{
	#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
	public override Material GetModifiedMaterial(Material baseMaterial)
	{
		Material cModifiedMat = base.GetModifiedMaterial(baseMaterial);
		return cModifiedMat;
	}
	#else
	public Material GetModifiedMaterial(Material baseMaterial)
	{
		return material;
	}
	#endif
}