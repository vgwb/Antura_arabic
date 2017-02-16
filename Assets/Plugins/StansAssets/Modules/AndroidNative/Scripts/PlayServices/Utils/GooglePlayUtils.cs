using System;
using UnityEngine;
using System.Collections;

public class GooglePlayUtils : SA.Common.Pattern.Singleton<GooglePlayUtils> {


	//Actions
	public static Action<GP_AdvertisingIdLoadResult> ActionAdvertisingIdLoaded							= delegate {};

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}



	public  void GetAdvertisingId() {
		AN_GooglePlayUtilsProxy.GetAdvertisingId();
	}

	private void OnAdvertisingIdLoaded(string data) {

		string[] info;
		info = data.Split(AndroidNative.DATA_SPLITTER [0]);


		string id = info[0];
		bool isLimitAdTrackingEnabled = System.Convert.ToBoolean(info[1]);


		GP_AdvertisingIdLoadResult res;
		if(id != null && id.Length > 0) {
			res =  new GP_AdvertisingIdLoadResult();
			res.id = id;
			res.isLimitAdTrackingEnabled = isLimitAdTrackingEnabled;
		} else {
			res =  new GP_AdvertisingIdLoadResult(new SA.Common.Models.Error());
		}

		ActionAdvertisingIdLoaded(res);
	}
}
