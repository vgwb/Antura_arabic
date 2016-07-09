using UnityEngine;

// This script will spam the console with finger info
public class SimpleInfo : MonoBehaviour
{
	protected virtual void OnEnable()
	{
		// Hook events
		Lean.LeanTouch.OnFingerDown     += OnFingerDown;
		Lean.LeanTouch.OnFingerSet      += OnFingerSet;
		Lean.LeanTouch.OnFingerUp       += OnFingerUp;
		Lean.LeanTouch.OnFingerDrag     += OnFingerDrag;
		Lean.LeanTouch.OnFingerTap      += OnFingerTap;
		Lean.LeanTouch.OnFingerSwipe    += OnFingerSwipe;
		Lean.LeanTouch.OnFingerHeldDown += OnFingerHeldDown;
		Lean.LeanTouch.OnFingerHeldSet  += OnFingerHeld;
		Lean.LeanTouch.OnFingerHeldUp   += OnFingerHeldUp;
		Lean.LeanTouch.OnMultiTap       += OnMultiTap;
		Lean.LeanTouch.OnDrag           += OnDrag;
		Lean.LeanTouch.OnSoloDrag       += OnSoloDrag;
		Lean.LeanTouch.OnMultiDrag      += OnMultiDrag;
		Lean.LeanTouch.OnPinch          += OnPinch;
		Lean.LeanTouch.OnTwistDegrees   += OnTwistDegrees;
		Lean.LeanTouch.OnTwistRadians   += OnTwistRadians;
	}
	
	protected virtual void OnDisable()
	{
		// Unhook events
		Lean.LeanTouch.OnFingerDown     -= OnFingerDown;
		Lean.LeanTouch.OnFingerSet      -= OnFingerSet;
		Lean.LeanTouch.OnFingerUp       -= OnFingerUp;
		Lean.LeanTouch.OnFingerDrag     -= OnFingerDrag;
		Lean.LeanTouch.OnFingerTap      -= OnFingerTap;
		Lean.LeanTouch.OnFingerSwipe    -= OnFingerSwipe;
		Lean.LeanTouch.OnFingerHeldDown -= OnFingerHeldDown;
		Lean.LeanTouch.OnFingerHeldSet  -= OnFingerHeld;
		Lean.LeanTouch.OnFingerHeldUp   -= OnFingerHeldUp;
		Lean.LeanTouch.OnMultiTap       -= OnMultiTap;
		Lean.LeanTouch.OnDrag           -= OnDrag;
		Lean.LeanTouch.OnSoloDrag       -= OnSoloDrag;
		Lean.LeanTouch.OnMultiDrag      -= OnMultiDrag;
		Lean.LeanTouch.OnPinch          -= OnPinch;
		Lean.LeanTouch.OnTwistDegrees   -= OnTwistDegrees;
		Lean.LeanTouch.OnTwistRadians   -= OnTwistRadians;
	}
	
	public void OnFingerDown(Lean.LeanFinger finger)
	{
		Debug.Log("Finger " + finger.Index + " began touching the screen");
	}
	
	public void OnFingerSet(Lean.LeanFinger finger)
	{
		Debug.Log("Finger " + finger.Index + " is still touching the screen");
	}
	
	public void OnFingerUp(Lean.LeanFinger finger)
	{
		Debug.Log("Finger " + finger.Index + " finished touching the screen");
	}
	
	public void OnFingerDrag(Lean.LeanFinger finger)
	{
		Debug.Log("Finger " + finger.Index + " moved " + finger.DeltaScreenPosition + " pixels across the screen");
	}
	
	public void OnFingerTap(Lean.LeanFinger finger)
	{
		Debug.Log("Finger " + finger.Index + " tapped the screen");
	}
	
	public void OnFingerSwipe(Lean.LeanFinger finger)
	{
		Debug.Log("Finger " + finger.Index + " swiped the screen");
	}
	
	public void OnFingerHeldDown(Lean.LeanFinger finger)
	{
		Debug.Log("Finger " + finger.Index + " began touching the screen for a long time");
	}
	
	public void OnFingerHeld(Lean.LeanFinger finger)
	{
		Debug.Log("Finger " + finger.Index + " is still touching the screen for a long time");
	}
	
	public void OnFingerHeldUp(Lean.LeanFinger finger)
	{
		Debug.Log("Finger " + finger.Index + " stopped touching the screen for a long time");
	}
	
	public void OnMultiTap(int fingerCount)
	{
		Debug.Log("The screen was just tapped by " + fingerCount + " finger(s)");
	}
	
	public void OnDrag(Vector2 pixels)
	{
		Debug.Log("One or many fingers moved " + pixels + " across the screen");
	}
	
	public void OnSoloDrag(Vector2 pixels)
	{
		Debug.Log("One finger moved " + pixels + " across the screen");
	}
	
	public void OnMultiDrag(Vector2 pixels)
	{
		Debug.Log("Many fingers moved " + pixels + " across the screen");
	}
	
	public void OnPinch(float scale)
	{
		Debug.Log("Many fingers pinched " + scale + " percent");
	}
	
	public void OnTwistDegrees(float angle)
	{
		Debug.Log("Many fingers twisted " + angle + " degrees");
	}
	
	public void OnTwistRadians(float angle)
	{
		Debug.Log("Many fingers twisted " + angle + " radians");
	}
}