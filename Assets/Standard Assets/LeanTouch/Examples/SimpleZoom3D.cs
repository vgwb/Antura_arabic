using UnityEngine;

// This script will zoom the main camera based on finger gestures
public class SimpleZoom3D : MonoBehaviour
{
	[Tooltip("The minimum field of view angle we want to zoom to")]
	public float Minimum = 10.0f;
	
	[Tooltip("The maximum field of view angle we want to zoom to")]
	public float Maximum = 60.0f;
	
	protected virtual void LateUpdate()
	{
		// Does the main camera exist?
		if (Camera.main != null)
		{
			// Make sure the pinch scale is valid
			if (Lean.LeanTouch.PinchScale > 0.0f)
			{
				// Store the old FOV in a temp variable
				var fieldOfView = Camera.main.fieldOfView;

				// Scale the FOV based on the pinch scale
				fieldOfView /= Lean.LeanTouch.PinchScale;
				
				// Clamp the FOV to out min/max values
				fieldOfView = Mathf.Clamp(fieldOfView, Minimum, Maximum);

				// Set the new FOV
				Camera.main.fieldOfView = fieldOfView;
			}
		}
	}
}