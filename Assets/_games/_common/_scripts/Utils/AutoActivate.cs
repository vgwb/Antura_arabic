using UnityEngine;
using System.Collections.Generic;

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
