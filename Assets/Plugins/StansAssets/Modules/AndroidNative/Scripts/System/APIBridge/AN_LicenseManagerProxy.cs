using UnityEngine;
using System.Collections;

public class AN_LicenseManagerProxy {

	private const string CLASS_NAME = "com.androidnative.licensing.LicenseManager";
	
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	
	
	public static void StartLicenseRequest(string base64PublicKey) {
		CallActivityFunction("StartLicenseRequest", base64PublicKey);
	}
}
