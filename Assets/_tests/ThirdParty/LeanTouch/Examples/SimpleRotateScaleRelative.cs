using UnityEngine;

// This script will rotate and scale the GameObject based on finger gestures, relative to the gesture location
public class SimpleRotateScaleRelative : MonoBehaviour
{
	protected virtual void LateUpdate()
	{
		// Get the center point of all touches
		var center = Lean.LeanTouch.GetCenterOfFingers();
		
		// This will rotate the current transform based on a multi finger twist gesture
		Lean.LeanTouch.RotateObjectRelative(transform, Lean.LeanTouch.TwistDegrees, center);
		
		// This will scale the current transform based on a multi finger pinch gesture
		Lean.LeanTouch.ScaleObjectRelative(transform, Lean.LeanTouch.PinchScale, center);
	}
}