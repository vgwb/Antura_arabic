using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Called when the tutorial animations are complete
    public void OnTutorialComplete()
    {
        Debug.Log("Tutorial complete");
    }
}
