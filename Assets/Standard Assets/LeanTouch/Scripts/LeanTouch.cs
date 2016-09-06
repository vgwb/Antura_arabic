using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Lean
{
	// If you add this component to your scene, then it will convert all mouse and touch data into easy to use data
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("Lean/Touch")]
	public partial class LeanTouch : MonoBehaviour
	{
		// This contains all the active and enabled LeanTouch instances
		public static List<LeanTouch> Instances = new List<LeanTouch>();
		
		// This list contains all currently active fingers (including simulated ones)
		public static List<LeanFinger> Fingers = new List<LeanFinger>(10);
		
		// This list contains all currently inactive fingers (e.g. this allows for pooling and tapping)
		public static List<LeanFinger> InactiveFingers = new List<LeanFinger>(10);
		
		// This contains the combined screen position delta of all fingers
		public static Vector2 DragDelta;
		
		// This contains the screen position delta of a single finger (if more than one finger is pressed down, then this value will be reset to 0,0)
		public static Vector2 SoloDragDelta;
		
		// This contains the combined screen position delta of multiple fingers (if only one finger is pressed down, then this value will be reset to 0,0)
		public static Vector2 MultiDragDelta;
		
		// This contains the average screen twist delta of multiple fingers in degrees (this requires at least two fingers to be pressed down)
		public static float TwistDegrees;
		
		// This contains the average screen twist delta of multiple fingers in radians (this requires at least two fingers to be pressed down)
		public static float TwistRadians;
		
		// This contains the average screen pinch scale of multiple fingers (this requires at least two fingers to be pressed down)
		public static float PinchScale = 1.0f;
		
		// This gets fired when a finger begins touching the screen (LeanFinger = The current finger)
		public static System.Action<LeanFinger> OnFingerDown;
		
		// This gets fired every frame a finger is touching the screen (LeanFinger = The current finger)
		public static System.Action<LeanFinger> OnFingerSet;
		
		// This gets fired when a finger stops touching the screen (LeanFinger = The current finger)
		public static System.Action<LeanFinger> OnFingerUp;
		
		// This gets fired when a finger moves across the screen (LeanFinger = The current finger)
		public static System.Action<LeanFinger> OnFingerDrag;
		
		// This gets fired when a finger taps the screen (this is when a finger begins and stops touching the screen within the 'TapThreshold' time) (LeanFinger = The current finger)
		public static System.Action<LeanFinger> OnFingerTap;
		
		// This gets fired when a finger swipes the screen (this is when a finger begins and stops touching the screen within the 'TapThreshold' time, and also moves more than the 'SwipeThreshold' distance) (LeanFinger = The current finger)
		public static System.Action<LeanFinger> OnFingerSwipe;
		
		// This gets fired when a finger begins being held on the screen (this is when a finger has been set for longer than the 'HeldThreshold' time) (LeanFinger = The current finger)
		public static System.Action<LeanFinger> OnFingerHeldDown;
		
		// This gets fired when a finger is held on the screen (this is when a finger has been set for longer than the 'HeldThreshold' time) (LeanFinger = The current finger)
		public static System.Action<LeanFinger> OnFingerHeldSet;
		
		// This gets fired when a finger stops being held on the screen (this is when a finger has been set for longer than the 'HeldThreshold' time) (LeanFinger = The current finger)
		public static System.Action<LeanFinger> OnFingerHeldUp;
		
		// This gets fired when at least one finger taps the screen (int = Highest Finger Count)
		public static System.Action<int> OnMultiTap;
		
		// This gets fired when a finger moves across the screen (Vector2 = DragDelta)
		public static System.Action<Vector2> OnDrag;
		
		// This gets fired when a single finger moves across the screen (if more than one finger is pressed down, then this value will not get fired) (Vector2 = SoloDragDelta)
		public static System.Action<Vector2> OnSoloDrag;
		
		// This gets fired when a multiple fingers moves across the screen (if only one finger is pressed down, then this value will not get fired) (Vector2 = MultiDragDelta)
		public static System.Action<Vector2> OnMultiDrag;
		
		// This gets fired when a pinch gesture occurs (float = PinchScale)
		public static System.Action<float> OnPinch;
		
		// This gets fired when a twist gesture occurs (float = TwistDegrees)
		public static System.Action<float> OnTwistDegrees;
		
		// This gets fired when a twist gesture occurs (float = TwistRadians)
		public static System.Action<float> OnTwistRadians;
		
		[Tooltip("This allows you to set how many seconds are required between a finger down/up for a tap to be registered")]
		public float TapThreshold = 0.5f;
		
		[Tooltip("This allows you to set how many pixels of movement (relative to the ReferenceDpi) are required within the TapThreshold for a swipe to be triggered")]
		public float SwipeThreshold = 50.0f;
		
		[Tooltip("This allows you to set how many seconds a finger must be held down for it to be regarded as being held down")]
		public float HeldThreshold = 1.0f;
		
		[Tooltip("This allows you to set the default DPI you want the input scaling to be based on")]
		public int ReferenceDpi = 200;
		
		[Tooltip("This allows you to enable recording of finger movements")]
		public bool RecordFingers = true;
		
		[Tooltip("This allows you to set the amount of pixels a finger must move for another snapshot to be stored")]
		public float RecordThreshold = 5.0f;
		
		[Tooltip("This allows you to set the maximum amount of seconds that can be recorded, 0 = unlimited")]
		public float RecordLimit = 10.0f;
		
		[Tooltip("This allows you to simulate multi touch inputs on devices that don't support them (e.g. desktop)")]
		public bool SimulateMultiFingers = true;
		
		[Tooltip("This allows you to set which key is required to simulate multi key twisting")]
		public KeyCode PinchTwistKey = KeyCode.LeftControl;
		
		[Tooltip("This allows you to set which key is required to simulate multi key dragging")]
		public KeyCode MultiDragKey = KeyCode.LeftAlt;
		
		[Tooltip("This allows you to set which texture will be used to show the simulated fingers")]
		public Texture2D FingerTexture;
		
		// This stores the highest mouse button index
		private static int highestMouseButton = 7;
		
		// Used to find if the GUI is in use
		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		// This stores how many fingers were touching the screen last frame
		private int lastFingerCount;
		
		// This stores how many seconds at least one finger has been touching the screen
		private float multiFingerTime;
		
		// This stores how many fingers at most were held during the multiFingerTime
		private int multiFingerCount;
		
		// Returns the main instance
		public static LeanTouch Instance
		{
			get
			{
				return Instances.Count > 0 ? Instances[0] : null;
			}
		}

		// If you multiply this value with any other pixel delta (e.g. DragDelta), then it will become device resolution independant
		public static float ScalingFactor
		{
			get
			{
				var scalingFactor = 1.0f;
				var referenceDpi  = 200;
				
				// Grab the current reference DPI, if it exists
				if (Instances.Count > 0)
				{
					referenceDpi = Instances[0].ReferenceDpi;
				}
				
				// If this screen has a known DPI, scale the value based on it
				if (Screen.dpi > 0 && referenceDpi > 0)
				{
					scalingFactor = Mathf.Sqrt(referenceDpi) / Mathf.Sqrt(Screen.dpi);
				}
				
				return scalingFactor;
			}
		}
		
		// This will return the DragDelta after it's been scaled to current DPI
		public static Vector2 ScaledDragDelta
		{
			get
			{
				return DragDelta * ScalingFactor;
			}
		}
		
		// This will return the SoloDragDelta after it's been scaled to current DPI
		public static Vector2 ScaledSoloDragDelta
		{
			get
			{
				return SoloDragDelta * ScalingFactor;
			}
		}
		
		// This will return the MultiDragDelta after it's been scaled to current DPI
		public static Vector2 ScaledMultiDragDelta
		{
			get
			{
				return MultiDragDelta * ScalingFactor;
			}
		}
		
		// Returns true if any mouse button is pressed
		public static bool AnyMouseButtonSet
		{
			get
			{
				for (var i = 0; i < highestMouseButton; i++)
				{
					if (Input.GetMouseButton(i) == true)
					{
						return true;
					}
				}
				
				return false;
			}
		}
		
		// This will return true if the mouse or any finger is currently using the GUI
		public static bool GuiInUse
		{
			get
			{
				// Legacy GUI in use?
				if (GUIUtility.hotControl > 0)
				{
					return true;
				}
				
				// New GUI in use?
				for (var i = Fingers.Count - 1; i >= 0; i--)
				{
					if (Fingers[i].IsOverGui == true)
					{
						return true;
					}
				}
				
				return false;
			}
		}

		// The average ScreenPosition of all fingers
		public static Vector2 CenterOfFingers
		{
			get
			{
				return GetCenterOfFingers(Fingers);
			}
		}

		// The average ScreenPosition of all fingers during the last frame
		public static Vector2 LastCenterOfFingers
		{
			get
			{
				return GetLastCenterOfFingers(Fingers);
			}
		}

		private static PointerEventData tempPointerEventData;

		private static EventSystem tempEventSystem;

		// This will return true if the input screenPosition is over any GUI elements
		public static bool PointOverGui(Vector2 screenPosition)
		{
			var currentEventSystem = EventSystem.current;
			
			if (currentEventSystem != null)
			{
				if (currentEventSystem != tempEventSystem)
				{
					tempEventSystem = currentEventSystem;

					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}

				tempPointerEventData.position = screenPosition;
				
				tempRaycastResults.Clear();
				
				currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);
				
				return tempRaycastResults.Count > 0;
			}
			
			return false;
		}

		// This wraps GetDeltaWorldPosition to be easier to use
		public static Vector3 GetDeltaWorldPosition(float distance, Camera camera = null)
		{
			return GetDeltaWorldPosition(Fingers, distance, camera);
		}

		// This gets the delta world position of the fingers at distance from the camera
		public static Vector3 GetDeltaWorldPosition(List<LeanFinger> fingers, float distance, Camera camera = null)
		{
			var total = Vector3.zero;
			
			if (fingers != null)
			{
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = Fingers[i];

					if (finger != null)
					{
						total += finger.GetDeltaWorldPosition(distance, camera);
						count += 1;
					}
				}

				if (count > 0)
				{
					total /= count;
				}
			}

			return total;
		}
		
		// This wraps GetCenterOfFingers to be easier to use
		public static Vector2 GetCenterOfFingers(List<LeanFinger> fingers)
		{
			var center = default(Vector2); GetCenterOfFingers(fingers, ref center); return center;
		}

		// If fingers contains more than one instance then this will return true and fill center with the average ScreenPosition
		public static bool GetCenterOfFingers(List<LeanFinger> fingers, ref Vector2 center)
		{
			if (fingers != null)
			{
				var total = Vector2.zero;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = Fingers[i];

					if (finger != null)
					{
						total += finger.ScreenPosition;
						count += 1;
					}
				}

				if (count > 0)
				{
					center = total / count;

					return true;
				}
			}

			return false;
		}
		
		// This wraps GetLastCenterOfFingers to be easier to use
		public static Vector2 GetLastCenterOfFingers(List<LeanFinger> fingers)
		{
			var center = default(Vector2); GetLastCenterOfFingers(fingers, ref center); return center;
		}

		// If fingers contains more than one instance then this will return true and fill center with the average LastScreenPosition
		public static bool GetLastCenterOfFingers(List<LeanFinger> fingers, ref Vector2 center)
		{
			if (fingers != null)
			{
				var total = Vector2.zero;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = Fingers[i];

					if (finger != null)
					{
						total += finger.LastScreenPosition;
						count += 1;
					}
				}

				if (count > 0)
				{
					center = total / count;

					return true;
				}
			}

			return false;
		}
		
		// This wraps GetAverageFingerDistance to be easier to use
		public static float GetAverageFingerDistance(Vector2 referencePoint)
		{
			return GetAverageFingerDistance(Fingers, referencePoint);
		}

		// This wraps GetAverageFingerDistance to be easier to use
		public static float GetAverageFingerDistance(List<LeanFinger> fingers, Vector2 referencePoint)
		{
			var distance = default(float); GetAverageFingerDistance(fingers, referencePoint, ref distance); return distance;
		}

		// This will return the average distance between all fingers and the reference point
		public static bool GetAverageFingerDistance(List<LeanFinger> fingers, Vector2 referencePoint, ref float distance)
		{
			if (fingers != null)
			{
				var total = 0.0f;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = Fingers[i];

					if (finger != null)
					{
						total += finger.GetDistance(referencePoint);
						count += 1;
					}
				}

				if (count > 0)
				{
					distance = total / count;

					return true;
				}
			}

			return false;
		}

		// This wraps GetLastAverageFingerDistance to be easier to use
		public static float GetLastAverageFingerDistance(Vector2 referencePoint)
		{
			return GetLastAverageFingerDistance(Fingers, referencePoint);
		}

		// This wraps GetLastAverageFingerDistance to be easier to use
		public static float GetLastAverageFingerDistance(List<LeanFinger> fingers, Vector2 referencePoint)
		{
			var distance = default(float); GetLastAverageFingerDistance(fingers, referencePoint, ref distance); return distance;
		}

		// This will return the average distance between all fingers and the reference point
		public static bool GetLastAverageFingerDistance(List<LeanFinger> fingers, Vector2 referencePoint, ref float distance)
		{
			if (fingers != null)
			{
				var total = 0.0f;
				var count = 0;

				for (var i = fingers.Count - 1; i >= 0; i--)
				{
					var finger = Fingers[i];

					if (finger != null)
					{
						total += finger.GetLastDistance(referencePoint);
						count += 1;
					}
				}

				if (count > 0)
				{
					distance = total / count;

					return true;
				}
			}

			return false;
		}
		
		protected virtual void OnEnable()
		{
			Instances.Add(this);

#if UNITY_EDITOR
			// Set the finger texture?
			if (FingerTexture == null)
			{
				var guids = UnityEditor.AssetDatabase.FindAssets("FingerVisualization t:texture2d");
				
				if (guids.Length > 0)
				{
					var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
					
					FingerTexture = UnityEditor.AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;
				}
			}
#endif
		}
		
		protected virtual void Update()
		{
			// Only update if this is the first instance
			if (Instances[0] != this)
			{
				return;
			}

			UpdateAllInputs();
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(this);
		}
		
		protected virtual void OnGUI()
		{
			// Show simulated multi fingers?
			if (FingerTexture != null && Input.touchCount == 0 && Fingers.Count > 1)
			{
				for (var i = Fingers.Count - 1; i >= 0; i--)
				{
					Fingers[i].Show(FingerTexture);
				}
			}
		}
		
		private void UpdateAllInputs()
		{
			UpdateFingers();
			UpdateMultiTap();
			UpdateGestures();
			UpdateEvents();
		}
		
		private void UpdateFingers()
		{
			// Age inactive fingers
			for (var i = InactiveFingers.Count - 1; i >= 0; i--)
			{
				InactiveFingers[i].Age += Time.unscaledDeltaTime;
			}
			
			// Reset finger data
			for (var i = Fingers.Count - 1; i >= 0; i--)
			{
				var finger = Fingers[i];
				
				// Was this set to up last time?
				if (finger.Up == true)
				{
					// Make finger inactive
					Fingers.RemoveAt(i); InactiveFingers.Add(finger);
					
					// Reset age so we can time how long it's been inactive
					finger.Age = 0.0f;
					
					// Pool old snapshots
					finger.ClearSnapshots();
				}
				else
				{
					finger.LastSet            = finger.Set;
					finger.LastHeldSet        = finger.HeldSet;
					finger.LastScreenPosition = finger.ScreenPosition;
					
					finger.Set     = false;
					finger.HeldSet = false;
					finger.Tap     = false;
				}
			}
			
			// Update real fingers
			if (Input.touchCount > 0)
			{
				for (var i = 0; i < Input.touchCount; i++)
				{
					var touch = Input.GetTouch(i);
					
					AddFinger(touch.fingerId, touch.position);
				}
			}
			// If there are no real touches, simulate some from the mouse?
			else if (AnyMouseButtonSet == true)
			{
				var screen        = new Rect(0, 0, Screen.width, Screen.height);
				var mousePosition = (Vector2)Input.mousePosition;
				
				// Is the mouse within the screen?
				if (screen.Contains(mousePosition) == true)
				{
					AddFinger(0, mousePosition);
					
					// Simulate pinch & twist?
					if (SimulateMultiFingers == true)
					{
						if (Input.GetKey(PinchTwistKey) == true)
						{
							var center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
							
							AddFinger(1, center - (mousePosition - center));
						}
						// Simulate multi drag?
						else if (Input.GetKey(MultiDragKey) == true)
						{
							AddFinger(1, mousePosition);
						}
					}
				}
			}
			
			// Go through all fingers
			for (var i = Fingers.Count - 1; i >= 0; i--)
			{
				var finger = Fingers[i];
				
				// Up?
				if (finger.Up == true)
				{
					// Tap?
					if (finger.Age <= TapThreshold && finger.ScaledTotalDeltaMagnitude < SwipeThreshold)
					{
						finger.Tap       = true;
						finger.TapCount += 1;
					}
					else
					{
						finger.TapCount = 0;
					}
				}
				// Down?
				else if (finger.Down == false)
				{
					// Age it
					finger.Age += Time.unscaledDeltaTime;
					
					// Held?
					if (finger.Age >= HeldThreshold)
					{
						finger.HeldSet = true;
					}
				}
				
				finger.TotalDeltaMagnitude += finger.DeltaScreenPosition.magnitude;
			}
		}
		
		// This handles the OnMultiTap event
		private void UpdateMultiTap()
		{
			var fingerCount = Fingers.Count;
			
			// At least one finger set?
			if (fingerCount >= 1)
			{
				multiFingerTime += Time.unscaledDeltaTime;
				multiFingerCount = Mathf.Max(multiFingerCount, fingerCount);
				
				// Did this just begin?
				if (lastFingerCount == 0)
				{
					multiFingerTime  = 0.0f;
					multiFingerCount = 0;
				}
			}
			
			// All fingers released?
			if (fingerCount == 0 && lastFingerCount > 0)
			{
				// Was at least one finger tapped?
				if (multiFingerTime <= TapThreshold)
				{
					if (OnMultiTap != null) OnMultiTap(multiFingerCount);
				}
			}
			
			lastFingerCount = fingerCount;
		}
		
		// Read finger data to find any gestures
		private void UpdateGestures()
		{
			var fingerCount = Fingers.Count;
			
			// Reset values
			DragDelta      = Vector3.zero;
			SoloDragDelta  = Vector2.zero;
			MultiDragDelta = Vector2.zero;
			PinchScale     = 1.0f;
			TwistRadians   = 0.0f;
			TwistDegrees   = 0.0f;
			
			// Fingers?
			if (fingerCount > 0)
			{
				// Drag delta
				for (var i = 0; i < fingerCount; i++)
				{
					DragDelta += Fingers[i].DeltaScreenPosition;
				}
				
				// Solo?
				if (fingerCount == 1)
				{
					// Drag
					SoloDragDelta = Fingers[0].DeltaScreenPosition;
				}
				// Multi?
				else
				{
					var lastCenter   = LastCenterOfFingers;
					var center       = CenterOfFingers;
					var lastDistance = GetLastAverageFingerDistance(lastCenter);
					var distance     = GetAverageFingerDistance(center);
					
					// Pinch?
					if (lastDistance > 0.0f && distance > 0.0f)
					{
						PinchScale = distance / lastDistance;
					}
					
					// Twist?
					for (var i = 0; i < fingerCount; i++)
					{
						TwistRadians += Fingers[i].GetDeltaRadians(lastCenter, center);
						TwistDegrees += Fingers[i].GetDeltaDegrees(lastCenter, center);
					}
					
					// Drag
					for (var i = 0; i < fingerCount; i++)
					{
						MultiDragDelta += Fingers[i].DeltaScreenPosition;
					}
				}
				
				// Scale values
				TwistRadians   /= fingerCount;
				TwistDegrees   /= fingerCount;
				DragDelta      /= fingerCount;
				MultiDragDelta /= fingerCount;
			}
		}
		
		// Fire events if there are any listeners
		private void UpdateEvents()
		{
			for (var i = 0; i < Fingers.Count; i++)
			{
				var finger = Fingers[i];
				
				if (finger.Down     == true && OnFingerDown     != null) OnFingerDown(finger);
				if (finger.Set      == true && OnFingerSet      != null) OnFingerSet(finger);
				if (finger.Up       == true && OnFingerUp       != null) OnFingerUp(finger);
				if (finger.Tap      == true && OnFingerTap      != null) OnFingerTap(finger);
				if (finger.HeldDown == true && OnFingerHeldDown != null) OnFingerHeldDown(finger);
				if (finger.HeldSet  == true && OnFingerHeldSet  != null) OnFingerHeldSet(finger);
				if (finger.HeldUp   == true && OnFingerHeldUp   != null) OnFingerHeldUp(finger);
				
				// Dragged?
				if (finger.DeltaScreenPosition != Vector2.zero)
				{
					if (OnFingerDrag != null) OnFingerDrag(finger);
				}
				
				// Swiped?
				if (finger.Up == true && finger.GetScaledSnapshotDelta(TapThreshold).magnitude >= SwipeThreshold)
				{
					if (OnFingerSwipe != null) OnFingerSwipe(finger);
				}
			}
			
			if (DragDelta      != Vector2.zero && OnDrag      != null) OnDrag(DragDelta);
			if (SoloDragDelta  != Vector2.zero && OnSoloDrag  != null) OnSoloDrag(SoloDragDelta);
			if (MultiDragDelta != Vector2.zero && OnMultiDrag != null) OnMultiDrag(MultiDragDelta);
			if (PinchScale     != 1.0f         && OnPinch     != null) OnPinch(PinchScale);
			
			if (TwistDegrees != 0.0f)
			{
				if (OnTwistDegrees != null) OnTwistDegrees(TwistDegrees);
				if (OnTwistRadians != null) OnTwistRadians(TwistRadians);
			}
		}
		
		// Add a finger based on index, or return the existing one
		private void AddFinger(int index, Vector2 screenPosition)
		{
			var finger = FindFinger(index);
			
			// No finger found?
			if (finger == null)
			{
				var inactiveIndex = FindInactiveFingerIndex(index);
				
				// Use inactive finger?
				if (inactiveIndex >= 0)
				{
					finger = InactiveFingers[inactiveIndex]; InactiveFingers.RemoveAt(inactiveIndex);
					
					// Inactive for too long?
					if (finger.Age > TapThreshold)
					{
						finger.TapCount = 0;
					}
					
					// Reset values
					finger.Age         = 0.0f;
					finger.LastSet     = false;
					finger.Set         = false;
					finger.LastHeldSet = false;
					finger.HeldSet     = false;
					finger.Tap         = false;
				}
				// Create new finger?
				else
				{
					finger = new LeanFinger();
					
					finger.Index = index;
				}
				
				finger.StartScreenPosition = screenPosition;
				finger.LastScreenPosition  = screenPosition;
				finger.ScreenPosition      = screenPosition;
				finger.StartedOverGui      = finger.IsOverGui;
				finger.TotalDeltaMagnitude = 0.0f;
				
				Fingers.Add(finger);
			}
			
			finger.Set            = true;
			finger.ScreenPosition = screenPosition;
			
			// Record?
			if (RecordFingers == true)
			{
				// Too many snapshots?
				if (RecordLimit > 0.0f)
				{
					if (finger.SnapshotDuration > RecordLimit)
					{
						var removeCount = finger.GetLowerSnapshotIndex(finger.Age - RecordLimit);
						
						finger.ClearSnapshots(removeCount);
					}
				}
				
				// Record snapshot?
				if (RecordThreshold > 0.0f)
				{
					if (finger.Snapshots.Count == 0 || finger.LastSnapshotDelta.magnitude >= RecordThreshold)
					{
						finger.RecordSnapshot();
					}
				}
				else
				{
					finger.RecordSnapshot();
				}
			}
		}

		// Find the finger with the specified index, or return null
		private LeanFinger FindFinger(int index)
		{
			for (var i = Fingers.Count - 1; i>= 0; i--)
			{
				var finger = Fingers[i];

				if (finger.Index == index)
				{
					return finger;
				}
			}

			return null;
		}

		// Find the index of the inactive finger with the specified index, or return -1
		private int FindInactiveFingerIndex(int index)
		{
			for (var i = InactiveFingers.Count - 1; i>= 0; i--)
			{
				if (InactiveFingers[i].Index == index)
				{
					return i;
				}
			}

			return -1;
		}
	}
}