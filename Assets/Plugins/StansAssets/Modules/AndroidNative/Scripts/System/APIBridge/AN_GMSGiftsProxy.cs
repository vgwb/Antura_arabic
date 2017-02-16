using UnityEngine;
using System.Collections;

public class AN_GMSGiftsProxy : MonoBehaviour {

	private const string CLASS_NAME = "com.androidnative.gms.core.GameClientBridge";
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}

	//--------------------------------------
	// GIFTS
	//--------------------------------------
	
	public static void sendGiftRequest(int type, string playload, int requestLifetimeDays, string icon, string description) {
		CallActivityFunction("sendGiftRequest", type.ToString(), playload, requestLifetimeDays.ToString(), icon, description);
	}
	
	public static void showRequestAccepDialog() {
		CallActivityFunction("showRequestAccepDialog");
	}
	
	
	public static void acceptRequests(string ids) {
		CallActivityFunction("acceptRequests", ids);
	}
	
	public static void dismissRequest(string ids) {
		CallActivityFunction("dismissRequest", ids);
	}
	
	public static void leaveRoom() {
		CallActivityFunction("leaveRoom");
	}

	
	public static void showInvitationBox()  {
		CallActivityFunction("showInvitationBox");
	}
}
