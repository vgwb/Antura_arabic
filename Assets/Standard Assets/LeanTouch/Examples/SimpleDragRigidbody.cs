using UnityEngine;

// This script allows you to drag this GameObject using any finger, as long it has a collider
[RequireComponent(typeof(Rigidbody))]
public class SimpleDragRigidbody : MonoBehaviour
{
	[Tooltip("This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)")]
	public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
	
	// This stores the finger that's currently dragging this GameObject
	private Lean.LeanFinger draggingFinger;

	// Cached rigidbody attached to this gameObject
	private Rigidbody body;
	
	protected virtual void OnEnable()
	{
		// Hook into the OnFingerDown event
		Lean.LeanTouch.OnFingerDown += OnFingerDown;
		
		// Hook into the OnFingerUp event
		Lean.LeanTouch.OnFingerUp += OnFingerUp;

		// Get attached Rigidbody?
		if (body == null)
		{
			body = GetComponent<Rigidbody>();
		}
	}
	
	protected virtual void OnDisable()
	{
		// Unhook the OnFingerDown event
		Lean.LeanTouch.OnFingerDown -= OnFingerDown;
		
		// Unhook the OnFingerUp event
		Lean.LeanTouch.OnFingerUp -= OnFingerUp;
	}
	
	protected virtual void LateUpdate()
	{
		// If there is an active finger, move this GameObject based on it
		if (draggingFinger != null)
		{
			// Does the main camera exist?
			if (Camera.main != null)
			{
				// Convert this GameObject's world position into screen coordinates and store it in a temp variable
				var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
				
				// Modify screen position by the finger's delta screen position
				screenPosition += (Vector3)draggingFinger.DeltaScreenPosition;
				
				// Convert the screen position into world coordinates and update this GameObject's world position with it
				transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
				
				// Reset velocity
				body.velocity = Vector3.zero;
			}
		}
	}
	
	public void OnFingerDown(Lean.LeanFinger finger)
	{
		// Raycast information
		var ray = finger.GetRay();
		var hit = default(RaycastHit);
		
		// Was this finger pressed down on a collider?
		if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
		{
			// Was that collider this one?
			if (hit.collider.gameObject == gameObject)
			{
				// Set the current finger to this one
				draggingFinger = finger;
			}
		}
	}
	
	public void OnFingerUp(Lean.LeanFinger finger)
	{
		// Was the current finger lifted from the screen?
		if (finger == draggingFinger)
		{
			// Unset the current finger
			draggingFinger = null;
			
			// Convert this GameObject's world position into screen coordinates and store it in a temp variable
			var screenPosition = Camera.main.WorldToScreenPoint(transform.position);
				
			// Modify screen position by the finger's delta screen position over the past 0.1 seconds
			screenPosition += (Vector3)finger.GetSnapshotDelta(0.1f);
				
			// Convert the screen position into world coordinates and subtract it by the old position to find the world delta over the past 0.1 seconds
			var worldDelta = Camera.main.ScreenToWorldPoint(screenPosition) - transform.position;
				
			// Set the velocity and divide it by 0.1, because velocity is applied over 1 second, and our delta is currently only for 0.1 second
			body.velocity = worldDelta / 0.1f;
		}
	}
}