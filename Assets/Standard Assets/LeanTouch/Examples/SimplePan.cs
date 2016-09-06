using UnityEngine;

// This script will pan the main camera based on finger gestures
public class SimplePan : MonoBehaviour
{
	[Tooltip("The distance from the main camera the world positions will be sampled from")]
	public float Distance = 10.0f;

	protected virtual void LateUpdate()
	{
		// Does the main camera exist?
		if (Camera.main != null)
		{
			// Get the world delta of all the fingers
			var worldDelta = Lean.LeanTouch.GetDeltaWorldPosition(10.0f);
			
			// Subtract the delta to the position
			Camera.main.transform.position -= worldDelta;
		}
	}
}