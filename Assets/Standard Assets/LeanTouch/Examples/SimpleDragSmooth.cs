using UnityEngine;

// This script allows you to drag this GameObject using any finger, as long it has a collider
public class SimpleDragSmooth : MonoBehaviour
{
	[Tooltip("This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)")]
	public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
	
	[Tooltip("How quickly smoothly this GameObject moves toward the target position")]
	public float Sharpness = 10.0f;

	// This stores the finger that's currently dragging this GameObject
	private Lean.LeanFinger draggingFinger;

	private Vector3 targetPosition;
	
	protected virtual void OnEnable()
	{
		// Make the target position match the current position at the start
		targetPosition = transform.position;

		// Hook into the OnFingerDown event
		Lean.LeanTouch.OnFingerDown += OnFingerDown;
		
		// Hook into the OnFingerUp event
		Lean.LeanTouch.OnFingerUp += OnFingerUp;
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
				
				// Convert the screen position into world coordinates and add the change to the target position
				targetPosition += Camera.main.ScreenToWorldPoint(screenPosition) - transform.position;
			}
		}

		// The framerate independent damping factor
		var factor = Mathf.Exp(- Sharpness * Time.deltaTime);
		
		// Dampen the current position toward the target
		transform.position = Vector3.Lerp(targetPosition, transform.position, factor);
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
		}
	}
}