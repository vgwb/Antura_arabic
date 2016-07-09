using UnityEngine;
using UnityEngine.UI;

// This script will tell you which direction you swiped in
public class SimpleSwipeDirection : MonoBehaviour
{
	public Text InfoText;
	
	protected virtual void OnEnable()
	{
		// Hook into the OnSwipe event
		Lean.LeanTouch.OnFingerSwipe += OnFingerSwipe;
	}
	
	protected virtual void OnDisable()
	{
		// Unhook into the OnSwipe event
		Lean.LeanTouch.OnFingerSwipe -= OnFingerSwipe;
	}
	
	public void OnFingerSwipe(Lean.LeanFinger finger)
	{
		// Make sure the info text exists
		if (InfoText != null)
		{
			// Store the swipe delta in a temp variable
			var swipe = finger.SwipeDelta;
			
			if (swipe.x < -Mathf.Abs(swipe.y))
			{
				InfoText.text = "You swiped left!";
			}
			
			if (swipe.x > Mathf.Abs(swipe.y))
			{
				InfoText.text = "You swiped right!";
			}
			
			if (swipe.y < -Mathf.Abs(swipe.x))
			{
				InfoText.text = "You swiped down!";
			}
			
			if (swipe.y > Mathf.Abs(swipe.x))
			{
				InfoText.text = "You swiped up!";
			}
		}
	}
}