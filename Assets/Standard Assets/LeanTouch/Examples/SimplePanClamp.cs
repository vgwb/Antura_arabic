using UnityEngine;

// This script will pan the main camera based on finger gestures and clamp its position
public class SimplePanClamp : MonoBehaviour
{
	[Tooltip("The distance from the main camera the world positions will be sampled from")]
	public float Distance = 10.0f;

	[Tooltip("The minimum X position")]
	public float MinX = -5.0f;

	[Tooltip("The maximum X position")]
	public float MaxX = 5.0f;

	[Tooltip("The minimum Y position")]
	public float MinY = -5.0f;

	[Tooltip("The maximum Y position")]
	public float MaxY = 5.0f;

	[Tooltip("The minimum Z position")]
	public float MinZ = -5.0f;

	[Tooltip("The maximum Z position")]
	public float MaxZ = 5.0f;

	protected virtual void LateUpdate()
	{
		// Does the main camera exist?
		if (Camera.main != null)
		{
			// Store the current camera position in a temp variable
			var worldPosition = Camera.main.transform.position;

			// Modify the world position by the delta world position of all fingers
			worldPosition -= Lean.LeanTouch.GetDeltaWorldPosition(10.0f);
			
			// Clamp on all axes
			worldPosition.x = Mathf.Clamp(worldPosition.x, MinX, MaxX);
			worldPosition.y = Mathf.Clamp(worldPosition.y, MinY, MaxY);
			worldPosition.z = Mathf.Clamp(worldPosition.z, MinZ, MaxZ);

			// Set the new world position
			Camera.main.transform.position = worldPosition;
		}
	}
}