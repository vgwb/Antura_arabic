using UnityEngine;
using System;
using System.Collections;

public class AN_LicenseManager : SA.Common.Pattern.Singleton<AN_LicenseManager> {
	
	public static Action<AN_LicenseRequestResult> 	OnLicenseRequestResult = 	delegate {};

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
		
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	//--------------------------------------
	// PUBLIC API CALL METHODS
	//--------------------------------------


	public void StartLicenseRequest() {
		StartLicenseRequest(AndroidNativeSettings.Instance.base64EncodedPublicKey);
	}

	public void StartLicenseRequest(string base64PublicKey) {
		AN_LicenseManagerProxy.StartLicenseRequest (base64PublicKey);
	}

	private void OnLicenseRequestRes(string data) {
		Debug.Log("[OnLicenseRequestResult] Data: " + data);
		string[] rawData = data.Split(new string[] {"|"}, StringSplitOptions.None);
		AN_LicenseRequestResult result = new AN_LicenseRequestResult((AN_LicenseStatusCode)Enum.Parse (typeof(AN_LicenseStatusCode), rawData[0]),
		                                                             (AN_LicenseErrorCode)Enum.Parse (typeof(AN_LicenseErrorCode), rawData[1]));
		OnLicenseRequestResult (result);
	}
}
