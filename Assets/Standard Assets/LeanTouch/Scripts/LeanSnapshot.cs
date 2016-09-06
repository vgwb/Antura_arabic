using UnityEngine;
using System.Collections.Generic;

namespace Lean
{
	// This class stores a snapshot of where a finger was at a previous point in time
	public class LeanSnapshot
	{
		// The age of the finger when this snapshot was created
		public float Age;
		
		// The screen position of the finger when this snapshot was created
		public Vector2 ScreenPosition;
		
		// This will return the world position of this snapshot based on the distance from the camera
		public Vector3 GetWorldPosition(float distance, Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			
			if (camera != null)
			{
				var point = new Vector3(ScreenPosition.x, ScreenPosition.y, distance);
				
				return camera.ScreenToWorldPoint(point);
			}
			
			return default(Vector3);
		}

		public static List<LeanSnapshot> InactiveSnapshots = new List<LeanSnapshot>(1000);

		// Return the last inactive snapshot, or allocate a new one
		public static LeanSnapshot Pop()
		{
			if (InactiveSnapshots.Count > 0)
			{
				var index    = InactiveSnapshots.Count - 1;
				var snapshot = InactiveSnapshots[index];

				InactiveSnapshots.RemoveAt(index);

				return snapshot;
			}

			return new LeanSnapshot();
		}
	}
}