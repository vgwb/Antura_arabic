using UnityEngine;

/// <summary>
/// Class used to indicate this object will respond to rays cast by WMG_Raycaster.
/// </summary>
public class WMG_Raycatcher : MonoBehaviour  {
	public bool IncludeMaterialAlpha = true; // whether to multiply the alpha of the image color tint with the alpha of the image pixel
}
