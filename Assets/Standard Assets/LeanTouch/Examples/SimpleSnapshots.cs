using UnityEngine;
using System.Collections.Generic;

// This script will set up LineRenderers based on recorded fingers
public class SimpleSnapshots : MonoBehaviour
{
	[Tooltip("")]
	public LineRenderer Prefab;

	// This stores all the lines that are spawned while a finger is active
	private List<LineRenderer> lines = new List<LineRenderer>();
	
	protected virtual void OnEnable()
	{
		// Hook into the OnFingerTap event
		Lean.LeanTouch.OnFingerSet += OnFingerSet;

		// Hook into the OnFingerUp event
		Lean.LeanTouch.OnFingerUp  += OnFingerUp;
	}

	protected virtual void OnDisable()
	{
		// Unhook from the OnFingerTap event
		Lean.LeanTouch.OnFingerSet -= OnFingerSet;

		// Unhook from the OnFingerUp event
		Lean.LeanTouch.OnFingerUp  -= OnFingerUp;
	}

	private void OnFingerSet(Lean.LeanFinger finger)
	{
		// Make sure the prefab exists
		if (Prefab != null)
		{
			// Get the name of this finger, and find the line that has the same name
			var fingerName = GetFingerName(finger);
			var line       = lines.Find(l => l.name == fingerName);

			// If the line doesn't exist, create it, give it the same name as the finger, and add it to the lines list
			if (line == null)
			{
				line = Instantiate(Prefab);

				line.name = fingerName;

				lines.Add(line);
			}

			// Copy all snapshot data into this line
			line.SetVertexCount(finger.Snapshots.Count);
			
			for (var j = 0; j < finger.Snapshots.Count; j++)
			{
				var snapshot = finger.Snapshots[j];
				
				if (snapshot != null)
				{
					line.SetPosition(j, snapshot.GetWorldPosition(1.0f));
				}
			}
		}
	}

	private void OnFingerUp(Lean.LeanFinger finger)
	{
		// Get the name of this finger, and find the line that has the same name
		var fingerName = GetFingerName(finger);
		var line       = lines.Find(l => l.name == fingerName);

		// If the line exists, remove it from the lines list, and destroy it
		if (line != null)
		{
			lines.Remove(line);

			Destroy(line.gameObject);
		}
	}

	// This will generate a unique name based on the finger index
	private string GetFingerName(Lean.LeanFinger finger)
	{
		return finger.Index.ToString();
	}
}