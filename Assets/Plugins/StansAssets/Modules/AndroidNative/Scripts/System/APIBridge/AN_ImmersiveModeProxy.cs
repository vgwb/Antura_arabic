using UnityEngine;
using System.Collections;

public class AN_ImmersiveModeProxy {

	private const string CLASS_NAME = "com.androidnative.features.ImmersiveMode";


	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}

	public static void enableImmersiveMode() {
		CallActivityFunction("enableImmersiveMode");
	}
}
