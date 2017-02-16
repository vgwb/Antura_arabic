using UnityEngine;
using System.Collections;

public class AsyncTask : MonoBehaviour {


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

}

