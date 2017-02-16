using UnityEngine;
using System.Collections;

public class AN_GoogleAnalyticsProxy : MonoBehaviour {

	private const string CLASS_NAME = "com.androidnative.gms.analytics.AN_AnalyticsProxy";
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	
	// --------------------------------------
	// Social
	// --------------------------------------
	
	
	public static void StartAnalyticsTracking() {
		CallActivityFunction("StartAnalyticsTracking");
	}


	
	public static void SetTrackerID(string trackingID) {
		CallActivityFunction("SetTrackerID", trackingID);
	}
	
	public static void SendView() {
		CallActivityFunction("SendView");
	}
	
	public static void SendView(string appScreen) {
		CallActivityFunction("SendView", appScreen);
	}
	
	public static void SendEvent(string category, string action, string label, string value)  {
		CallActivityFunction("SendEvent", category, action, label, value);
	}
	
	public static void SendEvent(string category, string action, string label, string value, string key, string val)  {
		CallActivityFunction("SendEvent", category, action, label, value, key, val);
	}
	
	public static void SendTiming(string category, string intervalInMilliseconds, string name, string label) {
		CallActivityFunction("SendTiming", category, intervalInMilliseconds, name, label);
	}
	
	public static void CreateTransaction(string transactionId, string affiliation, string revenue, string tax, string shipping, string currencyCode) {
		CallActivityFunction("CreateTransaction", transactionId, affiliation, revenue, tax, shipping, currencyCode);
	}
	
	public static void CreateItem(string transactionId, string name, string sku, string category, string price, string quantity, string currencyCode) {
		CallActivityFunction("CreateItem", transactionId, name, sku, category, price, quantity, currencyCode);
	}
	
	
	
	public static void SetKey(string key, string value) {
		CallActivityFunction("SetKey", key, value);
	}
	
	public static void ClearKey(string key) {
		CallActivityFunction("ClearKey", key);
	}
	
	public static void SetLogLevel(int lvl) {
		CallActivityFunction("SetLogLevel", lvl.ToString());
	}
	
	
	public static void SetDryRun(string mode) {
		CallActivityFunction("SetDryRun", mode);
	}
	
	public static void EnableAdvertisingIdCollection(string mode) {
		CallActivityFunction("EnableAdvertisingIdCollection", mode);
	}




}
