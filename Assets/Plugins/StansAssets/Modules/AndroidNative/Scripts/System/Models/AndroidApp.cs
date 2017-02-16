using UnityEngine;
using System;
using System.Collections;


public class AndroidApp : SA.Common.Pattern.Singleton<AndroidApp> {


	public Action<AndroidActivityResult> OnActivityResult =  delegate{};



	//--------------------------------------
	// LISTNERS
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
		Debug.Log("GooglePlayTBM Created");
	}

	public void ActivateListner() {

	}
	

	private void onActivityResult(string data) {
		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		AndroidActivityResult result =  new AndroidActivityResult(storeData[0], storeData[1]);


		OnActivityResult(result);
	}
}
