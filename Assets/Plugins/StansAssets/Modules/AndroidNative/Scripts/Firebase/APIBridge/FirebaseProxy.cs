using UnityEngine;
using System.Collections;

public static class FirebaseProxy {
	private const string CLASS_NAME = "com.androidnative.firebase.analytics.Bridge";

	public static void Init() {
		CallStaticFunction ("Init");
	}

	public static void SetEnabled(bool enabled) {
		CallStaticFunction ("SetEnabled", enabled);
	}

	public static void SetMinimumSessionDuration(long milliseconds) {
		CallStaticFunction ("SetMinimumSessionDuration", milliseconds);
	}

	public static void SetSessionTimeoutDuration(long milliseconds) {
		CallStaticFunction ("SetSessionTimeoutDuration", milliseconds);
	}

	public static void SetUserId(string userId) {
		CallStaticFunction ("SetUserId", userId);
	}

	public static void SetUserProperty(string name, string value) {
		CallStaticFunction ("SetUserProperty", name, value);
	}

	public static void LogEvent(string name, string data = null) {
		CallStaticFunction ("LogEvent", name, data);
	}

	private static void CallStaticFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
}
