using UnityEngine;

// This script will orthographically zoom the main camera based on finger gestures
public class SimpleOrthographicZoom : MonoBehaviour
{
	// The minimum orthographic size value we want to zoom to
	public float MinSize = 10.0f;
	
	// The maximum orthographic size value we want to zoom to
	public float MaxSize = 60.0f;
	
	protected virtual void LateUpdate()
	{
		// Does the main camera exist?
		if (Camera.main != null)
		{
			// Make sure the pinch scale is valid
			if (Lean.LeanTouch.PinchScale > 0.0f)
			{
				// Scale the FOV based on the pinch scale
				Camera.main.orthographicSize /= Lean.LeanTouch.PinchScale;
				
				// Make sure the new FOV is within our min/max
				Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, MinSize, MaxSize);
			}
		}
	}
}