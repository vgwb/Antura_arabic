using UnityEngine;
using System.Collections;

public class AN_FBProxy : MonoBehaviour {

	private const string CLASS_NAME = "com.androidnative.features.social.fb.FBExtended";
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	
	// --------------------------------------
	// Social
	// --------------------------------------
	
	
	public static void SendTrunRequest(string title, string messgae, string data, string to) {
		CallActivityFunction("SendRequest", title, messgae, data, to);
	}

}
