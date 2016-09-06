using UnityEngine;

// This script allows you to transform the GameObject selected by SimpleSelect relative to the gesture center
public class SimpleSelectTransformRelative : SimpleSelect
{
	public bool AllowTranslate = true;

	public bool AllowRotate = true;

	public bool AllowScale = true;

	protected virtual void Update()
	{
		// Make sure we have something selected
		if (SelectedGameObject != null)
		{
			// Make sure the main camera exists
			if (Camera.main != null)
			{
				if (AllowTranslate == true)
				{
					Translate(SelectedGameObject.transform, Lean.LeanTouch.DragDelta);
				}

				if (AllowRotate == true)
				{
					RotateRelative(SelectedGameObject.transform, Lean.LeanTouch.TwistDegrees, Lean.LeanTouch.CenterOfFingers);
				}

				if (AllowScale == true)
				{
					ScaleRelative(SelectedGameObject.transform, Lean.LeanTouch.PinchScale, Lean.LeanTouch.CenterOfFingers);
				}
			}
		}
	}

	public void Translate(Transform transform, Vector2 screenPositionDelta)
	{
		// Screen position of the transform
		var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
			
		// Add the deltaPosition
		screenPosition += (Vector3)screenPositionDelta;
			
		// Convert back to world space
		transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
	}

	public void RotateRelative(Transform transform, float angleDelta, Vector2 referencePoint)
	{
		// World position of the reference point
		var worldReferencePoint = Camera.main.ScreenToWorldPoint(referencePoint);
		
		// Rotate the transform around the world reference point
		transform.RotateAround(worldReferencePoint, Camera.main.transform.forward, angleDelta);
	}

	public void ScaleRelative(Transform transform, float scale, Vector2 referencePoint)
	{
		// Make sure the scale is valid
		if (scale > 0.0f)
		{
			// Screen position of the transform
			var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
			
			// Push the screen position away from the reference point based on the scale
			screenPosition.x = referencePoint.x + (screenPosition.x - referencePoint.x) * scale;
			screenPosition.y = referencePoint.y + (screenPosition.y - referencePoint.y) * scale;
			
			// Convert back to world space
			transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
			
			// Grow the local scale by scale
			transform.localScale *= scale;
		}
	}
}