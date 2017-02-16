using System;
using UnityEngine;
using System.Collections;

public class TVAppController : SA.Common.Pattern.Singleton<TVAppController> {

	private bool _IsRuningOnTVDevice = false;

	public static event Action DeviceTypeChecked = delegate {};



	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void CheckForATVDevice()  {
		AN_TVControllerProxy.AN_CheckForATVDevice();
	}


	private void OnDeviceStateResponce(string data) {
		if(data.Equals("1")) {
			_IsRuningOnTVDevice = true;
		}

		DeviceTypeChecked();
	}



	public bool IsRuningOnTVDevice {
		get {
			return _IsRuningOnTVDevice;
		}
	}
}
