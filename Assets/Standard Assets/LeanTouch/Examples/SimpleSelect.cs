using UnityEngine;

// This script allows you to select a GameObject using any finger, as long it has a collider
public class SimpleSelect : MonoBehaviour
{
	[Tooltip("This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)")]
	public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
	
	[Tooltip("The previously selected GameObject")]
	public GameObject SelectedGameObject;
	
	protected virtual void OnEnable()
	{
		// Hook into the OnFingerTap event
		Lean.LeanTouch.OnFingerTap += OnFingerTap;
	}
	
	protected virtual void OnDisable()
	{
		// Unhook from the OnFingerTap event
		Lean.LeanTouch.OnFingerTap -= OnFingerTap;
	}
	
	public void OnFingerTap(Lean.LeanFinger finger)
	{
		// Raycast information
		var ray = finger.GetRay();
		var hit = default(RaycastHit);
		
		// Was this finger pressed down on a collider?
		if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
		{
			// Remove the color from the currently selected one?
			if (SelectedGameObject != null)
			{
				ColorGameObject(SelectedGameObject, Color.white);
			}

			SelectedGameObject = hit.collider.gameObject;

			ColorGameObject(SelectedGameObject, Color.green);
		}
	}

	private static void ColorGameObject(GameObject gameObject, Color color)
	{
		// Make sure the GameObject exists
		if (gameObject != null)
		{
			// Get renderer from this GameObject
			var renderer = gameObject.GetComponent<Renderer>();

			// Make sure the Renderer exists
			if (renderer != null)
			{
				// Get material copy from this renderer
				var material = renderer.material;

				// Make sure the material exists
				if (material != null)
				{
					// Set new color
					material.color = color;
				}
			}
		}
	}
}