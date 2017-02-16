using UnityEngine;
using System.Collections;

public class AN_AppInvitesProxy  {

	private const string CLASS_NAME = "com.androidnative.gms.core.AppInvitesController";


	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	

	public static void CreateBuilder(int id, string title) {
		CallActivityFunction("createBuilder", id, title);
	}
	
	
	public static void SetMessage(int id, string msg) {
		CallActivityFunction("setMessage", id, msg);
	}
	
	public static void SetDeepLink(int id, string url) {
		CallActivityFunction("setDeepLink", id, url);
	}
	
	public static void SetCallToActionText(int id, string actionText) {
		CallActivityFunction("setCallToActionText", id, actionText);
	}
	
	public static void SetGoogleAnalyticsTrackingId(int id, string trackingId) {
		CallActivityFunction("setGoogleAnalyticsTrackingId", id, trackingId);
	}
	
	public static void SetAndroidMinimumVersionCode(int id, int versionCode) {
		CallActivityFunction("setAndroidMinimumVersionCode", id, versionCode);
	}
	
	public static void SetAdditionalReferralParameters(int id, string keys, string values) {
		CallActivityFunction("setAdditionalReferralParameters", id, keys, values);
	}


	public static void StartInvitationDialog(int builderId) {
		CallActivityFunction("startInvitationDialog", builderId);
	}

	public static void GetInvitation(bool autoLaunchDeepLink) {
		CallActivityFunction("getInvitation", autoLaunchDeepLink);
	}

}
