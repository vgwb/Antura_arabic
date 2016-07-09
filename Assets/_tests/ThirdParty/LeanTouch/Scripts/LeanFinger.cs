using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Lean
{
	// This class stores information about a single touch (or simulated touch)
	public class LeanFinger
	{
		// This class stores the ScreenPosition and a timestamp
		public class Snapshot
		{
			public float Age;
			
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
		}
		
		// This is the hardware ID of the finger (or 0 & 1 for simulated fingers)
		public int Index;
		
		// This tells you how long this finger has been active, or inactive
		public float Age;
		
		// This tells you if the finger is currently set (mouse click or touched on screen)
		public bool Set;
		
		// This tells you if the finger is currently being held for a long time
		public bool HeldSet;
		
		// This tells if you if the finger has just been tapped
		public bool Tap;
		
		// This tells you how many times this finger has been tapped
		public int TapCount;
		
		// This tells you the screen position of the touch on the frame it was first set
		public Vector2 StartScreenPosition;
		
		// This tells you the last screen position of the finger
		public Vector2 LastScreenPosition;
		
		// This tells you the total of all the ScreenPositionDelta.magnitude values
		public float TotalDeltaMagnitude;
		
		// This tells you the 'Set' value of the last frame
		public bool LastSet;
		
		// This tells you the 'Held' value of the last frame
		public bool LastHeldSet;
		
		// This tells you the current screen position of the finger
		public Vector2 ScreenPosition;
		
		// This tells you if the current finger had 'IsOverGui' set to true when it began touching the screen
		public bool StartedOverGui;
		
		// Used to store position snapshots, enable RecordFingers in LeanTouch to use this
		public List<Snapshot> Snapshots = new List<Snapshot>();
		
		// Used to find if the GUI is in use
		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>();
		
		// Used to store unused snapshots
		private static List<Snapshot> tempSnapshots = new List<Snapshot>();
		
		// This will return true if the current finger is currently touching the screen
		public bool IsActive
		{
			get
			{
				return LeanTouch.Fingers.Contains(this);
			}
		}
		
		// This will return the amount of seconds of snapshot footage is stored for this finger
		public float SnapshotDuration
		{
			get
			{
				if (Snapshots.Count > 0)
				{
					return Age - Snapshots[0].Age;
				}
				
				return 0.0f;
			}
		}
		
		// This will return true if the current finger is over any Unity GUI elements
		public bool IsOverGui
		{
			get
			{
				var currentEventSystem = EventSystem.current;
				
				if (currentEventSystem != null)
				{
					var eventDataCurrentPosition = new PointerEventData(currentEventSystem);
					
					eventDataCurrentPosition.position = new Vector2(ScreenPosition.x, ScreenPosition.y);
					
					tempRaycastResults.Clear();
					
					currentEventSystem.RaycastAll(eventDataCurrentPosition, tempRaycastResults);
					
					return tempRaycastResults.Count > 0;
				}
				
				return false;
			}
		}
		
		// This tells you if the finger has just begun touching the screen for a long time
		public bool HeldDown
		{
			get
			{
				return HeldSet == true && LastHeldSet == false;
			}
		}
		
		// This tells you if the finger has just stopped touching the screen for a long time
		public bool HeldUp
		{
			get
			{
				return HeldSet == false && LastHeldSet == true;
			}
		}
		
		// This tells you if the finger has just touched the screen
		public bool Down
		{
			get
			{
				return Set == true && LastSet == false;
			}
		}
		
		// This tells you if the finger has just been released from the screen
		public bool Up
		{
			get
			{
				return Set == false && LastSet == true;
			}
		}
		
		// This will return how far in pixels the finger has moved since the last recorded snapshot
		public Vector2 LastSnapshotDelta
		{
			get
			{
				var snapshotCount = Snapshots.Count;
				
				if (snapshotCount > 0)
				{
					var snapshot = Snapshots[snapshotCount - 1];
					
					if (snapshot != null)
					{
						return ScreenPosition - snapshot.ScreenPosition;
					}
				}
				
				return Vector2.zero;
			}
		}
		
		// This will return how far in pixels the finger has moved since the last frame
		public Vector2 DeltaScreenPosition
		{
			get
			{
				return ScreenPosition - LastScreenPosition;
			}
		}
		
		// This will return how far in pixels the finger has moved since the last frame, relative to the device DPI
		public Vector2 ScaledDeltaScreenPosition
		{
			get
			{
				return DeltaScreenPosition * LeanTouch.ScalingFactor;
			}
		}
		
		// This will return how far in pixels the finger has moved relative to its start position
		public Vector2 TotalDeltaScreenPosition
		{
			get
			{
				return ScreenPosition - StartScreenPosition;
			}
		}
		
		// This will return how far in pixels the finger has moved relative to its start position, relative to the device DPI
		public Vector2 ScaledTotalDeltaScreenPosition
		{
			get
			{
				return TotalDeltaScreenPosition * LeanTouch.ScalingFactor;
			}
		}
		
		// This tells you how far this finger has moved recently
		public Vector2 SwipeDelta
		{
			get
			{
				if (LeanTouch.Instance != null)
				{
					return GetSnapshotDelta(LeanTouch.Instance.TapThreshold);
				}
				
				return Vector2.zero;
			}
		}
		
		// This tells you how far this finger has moved recently, relative to the device DPI
		public Vector2 ScaledSwipeDelta
		{
			get
			{
				return SwipeDelta * LeanTouch.ScalingFactor;
			}
		}
		
		// This tells you the total of all the DeltaScreenPosition.magnitude values, relative to the device DPI
		public float ScaledTotalDeltaMagnitude
		{
			get
			{
				return TotalDeltaMagnitude * LeanTouch.ScalingFactor;
			}
		}
		
		// This will return the ray of the finger's current position
		public Ray GetRay(Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			
			if (camera != null)
			{
				return camera.ScreenPointToRay(ScreenPosition);
			}
			
			return default(Ray);
		}
		
		// This will return the ray of the finger's start position
		public Ray GetStartRay(Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			
			if (camera != null)
			{
				return camera.ScreenPointToRay(StartScreenPosition);
			}
			
			return default(Ray);
		}
		
		// This will tell you how far the finger has moved in the past 'deltaTime' seconds
		public Vector2 GetSnapshotDelta(float deltaTime)
		{
			return ScreenPosition - GetSnapshotScreenPosition(Age - deltaTime);
		}
		
		// This will tell you how far the finger has moved in the past 'deltaTime' seconds, relative to the device DPI
		public Vector2 GetScaledSnapshotDelta(float deltaTime)
		{
			return GetSnapshotDelta(deltaTime) * LeanTouch.ScalingFactor;
		}
		
		// This will return the recorded position of the current finger when it was at 'targetAge'
		public Vector2 GetSnapshotScreenPosition(float targetAge)
		{
			var lowerIndex          = GetLowerSnapshotIndex(targetAge);
			var lowerAge            = default(float);
			var lowerScreenPosition = default(Vector2);
			
			GetSnapshot(lowerIndex, out lowerAge, out lowerScreenPosition);
			
			if (targetAge <= lowerAge)
			{
				return lowerScreenPosition;
			}
			
			var upperIndex          = lowerIndex + 1;
			var upperAge            = default(float);
			var upperScreenPosition = default(Vector2);
			
			GetSnapshot(upperIndex, out upperAge, out upperScreenPosition);
			
			if (targetAge >= upperAge)
			{
				return upperScreenPosition;
			}
			
			return Vector2.Lerp(lowerScreenPosition, upperScreenPosition, Mathf.InverseLerp(lowerAge, upperAge, targetAge));
		}
		
		public void GetSnapshot(int index, out float age, out Vector2 screenPosition)
		{
			if (index >= 0 && index < Snapshots.Count)
			{
				var snapshot = Snapshots[index];
				
				age            = snapshot.Age;
				screenPosition = snapshot.ScreenPosition;
			}
			else
			{
				age            = Age;
				screenPosition = ScreenPosition;
			}
		}
		
		// This will return the angle between the finger and the reference point, relative to the screen
		public float GetRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(ScreenPosition.x - referencePoint.x, ScreenPosition.y - referencePoint.y);
		}
		
		public float GetDegrees(Vector2 referencePoint)
		{
			return GetRadians(referencePoint) * Mathf.Rad2Deg;
		}
		
		// This will return the angle between the last finger position and the reference point, relative to the screen
		public float GetLastRadians(Vector2 referencePoint)
		{
			return Mathf.Atan2(LastScreenPosition.x - referencePoint.x, LastScreenPosition.y - referencePoint.y);
		}
		
		public float GetLastDegrees(Vector2 referencePoint)
		{
			return GetLastRadians(referencePoint) * Mathf.Rad2Deg;
		}
		
		// This will return the delta between GetAngle and GetLastAngle
		public float GetDeltaRadians(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint);
		}
		
		public float GetDeltaRadians(Vector2 lastReferencePoint, Vector2 referencePoint)
		{
			var a = GetLastRadians(lastReferencePoint);
			var b = GetRadians(referencePoint);
			var d = Mathf.Repeat(a - b, Mathf.PI * 2.0f);
			
			if (d > Mathf.PI)
			{
				d -= Mathf.PI * 2.0f;
			}
			
			return d;
		}
		
		public float GetDeltaDegrees(Vector2 referencePoint)
		{
			return GetDeltaRadians(referencePoint, referencePoint) * Mathf.Rad2Deg;
		}
		
		public float GetDeltaDegrees(Vector2 lastReferencePoint, Vector2 referencePoint)
		{
			return GetDeltaRadians(lastReferencePoint, referencePoint) * Mathf.Rad2Deg;
		}
		
		// This will return the distance between the last finger and the reference point
		public float GetLastDistance(Vector2 referencePoint)
		{
			return Vector2.Distance(LastScreenPosition, referencePoint);
		}
		
		// This will return the distance between the finger and the reference point
		public float GetDistance(Vector2 referencePoint)
		{
			return Vector2.Distance(ScreenPosition, referencePoint);
		}
		
		// This will return the world position of this finger based on the distance from the camera
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
		
		// This will return the last world position of this finger based on the distance from the camera
		public Vector3 GetLastWorldPosition(float distance, Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			
			if (camera != null)
			{
				var point = new Vector3(ScreenPosition.x, ScreenPosition.y, distance);
				
				return camera.ScreenToWorldPoint(point);
			}
			
			return default(Vector3);
		}
		
		// This will return the change in world position of this finger based on the distance from the camera
		public Vector3 GetDeltaWorldPosition(float distance, Camera camera = null)
		{
			return GetDeltaWorldPosition(distance, distance, camera);
		}
		
		public Vector3 GetDeltaWorldPosition(float lastDistance, float distance, Camera camera = null)
		{
			if (camera == null) camera = Camera.main;
			
			if (camera != null)
			{
				return GetWorldPosition(distance, camera) - GetLastWorldPosition(lastDistance, camera);
			}
			
			return default(Vector3);
		}
		
		public void Show(Texture texture)
		{
			if (texture != null && Set == true)
			{
				var rect = new Rect(0, 0, texture.width, texture.height);
				
				rect.center = new Vector2(ScreenPosition.x, Screen.height - ScreenPosition.y);
				
				GUI.DrawTexture(rect, texture);
			}
		}
		
		// Clear snapshots and pool them, count = -1 for all
		public void ClearSnapshots(int count = -1)
		{
			// Clear old ones only?
			if (count > 0 && count <= Snapshots.Count)
			{
				for (var i = 0; i < count; i++)
				{
					tempSnapshots.Add(Snapshots[i]);
				}
				
				Snapshots.RemoveRange(0, count);
			}
			// Clear all?
			else if (count < 0)
			{
				tempSnapshots.AddRange(Snapshots);
				
				Snapshots.Clear();
			}
		}
		
		// Records a snapshot of the current finger
		public void RecordSnapshot()
		{
			var snapshot          = default(Snapshot);
			var tempSnapshotCount = tempSnapshots.Count;
			
			// Get snapshot from pool?
			if (tempSnapshotCount > 0)
			{
				snapshot = tempSnapshots[tempSnapshotCount - 1];
				
				tempSnapshots.RemoveAt(tempSnapshotCount - 1);
			}
			
			// Make new snapshot?
			if (snapshot == null)
			{
				snapshot = new Snapshot();
			}
			
			snapshot.Age            = Age;
			snapshot.ScreenPosition = ScreenPosition;
			
			// Add to list
			Snapshots.Add(snapshot);
		}
		
		// This will get the index of the closest snapshot whose age is under targetAge
		public int GetLowerSnapshotIndex(float targetAge)
		{
			var snapshotCount = Snapshots.Count;
			
			if (snapshotCount > 0)
			{
				var firstAge = Snapshots[0].Age;
				
				if (targetAge > firstAge)
				{
					for (var i = 1; i < snapshotCount; i++)
					{
						if (Snapshots[i].Age > targetAge)
						{
							return i - 1;
						}
					}
					
					return snapshotCount - 1;
				}
			}
			
			return 0;
		}
	}
}