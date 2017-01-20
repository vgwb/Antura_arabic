using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Utility script that automatically activates a list of game objects when awoken.
/// </summary>
// refactor: add a namespace
public class AutoActivate : MonoBehaviour
{
    public List<GameObject> toAwake = new List<GameObject>();

	void Awake()
    {
        foreach (var g in toAwake)
        {
            if(g != null)
            {
                g.SetActive(true);
            }
        }
    }
}
