using UnityEngine;
using System.Collections;

public class AN_GooglePlayUtilsProxy  {

	private const string CLASS_NAME = "com.androidnative.gms.utils.PS_Utility";
	
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	
	
	public static void GetAdvertisingId() {
		CallActivityFunction("GetAdvertisingId");
	}
}
