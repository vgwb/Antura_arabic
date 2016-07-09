using UnityEngine;

// This script will move the GameObject based on finger gestures
public class SimpleMove : MonoBehaviour
{
	protected virtual void LateUpdate()
	{
		// This will move the current transform based on a finger drag gesture
		Lean.LeanTouch.MoveObject(transform, Lean.LeanTouch.DragDelta);
	}
}