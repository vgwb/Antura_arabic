using UnityEngine;
using UnityEngine.UI;

// This script will tell you which direction you swiped in
public class SimpleSwipeDirection8 : MonoBehaviour
{
	[Tooltip("The text element we will display the swipe information in")]
	public Text InfoText;
	
	protected virtual void OnEnable()
	{
		// Hook into the OnSwipe event
		Lean.LeanTouch.OnFingerSwipe += OnFingerSwipe;
	}
	
	protected virtual void OnDisable()
	{
		// Unhook from the OnSwipe event
		Lean.LeanTouch.OnFingerSwipe -= OnFingerSwipe;
	}
	
	public void OnFingerSwipe(Lean.LeanFinger finger)
	{
		// Make sure the info text exists
		if (InfoText != null)
		{
			// Store the swipe delta in a temp variable
			var swipe = finger.SwipeDelta;
			var left  = new Vector2(-1.0f,  0.0f);
			var right = new Vector2( 1.0f,  0.0f);
			var down  = new Vector2( 0.0f, -1.0f);
			var up    = new Vector2( 0.0f,  1.0f);
			
			if (SwipedInThisDirection(swipe, left) == true)
			{
				InfoText.text = "You swiped left!";
			}
			
			if (SwipedInThisDirection(swipe, right) == true)
			{
				InfoText.text = "You swiped right!";
			}
			
			if (SwipedInThisDirection(swipe, down) == true)
			{
				InfoText.text = "You swiped down!";
			}
			
			if (SwipedInThisDirection(swipe, up) == true)
			{
				InfoText.text = "You swiped up!";
			}

			if (SwipedInThisDirection(swipe, left + up) == true)
			{
				InfoText.text = "You swiped left and up!";
			}

			if (SwipedInThisDirection(swipe, left + down) == true)
			{
				InfoText.text = "You swiped left and down!";
			}

			if (SwipedInThisDirection(swipe, right + up) == true)
			{
				InfoText.text = "You swiped right and up!";
			}

			if (SwipedInThisDirection(swipe, right + down) == true)
			{
				InfoText.text = "You swiped right and down!";
			}
		}
	}

	private bool SwipedInThisDirection(Vector2 swipe, Vector2 direction)
	{
		// Find the normalized dot product between the swipe and our desired angle (this will return the acos between the vectors)
		var dot = Vector2.Dot(swipe.normalized, direction.normalized);

		// With 8 directions, each direction takes up 45 degrees (360/8), but we're comparing against dot product, so we need to halve it
		var limit = Mathf.Cos(22.5f * Mathf.Deg2Rad);

		// Return true if this swipe is within the limit of this direction
		return dot >= limit;
	}
}