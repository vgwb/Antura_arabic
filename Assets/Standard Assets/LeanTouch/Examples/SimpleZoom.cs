using UnityEngine;

// This script will zoom the main camera based on finger gestures
public class SimpleZoom : MonoBehaviour
{
	// The minimum field of view value we want to zoom to
	public float MinFov = 10.0f;
	
	// The maximum field of view value we want to zoom to
	public float MaxFov = 60.0f;
	
	protected virtual void LateUpdate()
	{
		// Does the main camera exist?
		if (Camera.main != null)
		{
			// Make sure the pinch scale is valid
			if (Lean.LeanTouch.PinchScale > 0.0f)
			{
				// Scale the FOV based on the pinch scale
				Camera.main.fieldOfView /= Lean.LeanTouch.PinchScale;
				
				// Make sure the new FOV is within our min/max
				Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, MinFov, MaxFov);
			}
		}
	}
}