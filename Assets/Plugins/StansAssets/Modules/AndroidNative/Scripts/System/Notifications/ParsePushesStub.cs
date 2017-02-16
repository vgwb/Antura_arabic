//#define PARSE_PUSH_ENABLED

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if PARSE_PUSH_ENABLED
using Parse;
#endif

public static class ParsePushesStub {

	public static event Action<string, Dictionary<string, object>> OnPushReceived = delegate {};

	public static void InitParse() {
#if PARSE_PUSH_ENABLED
		GameObject parse = new GameObject("AN_ParsePushesStub");
		ParseInitializeBehaviour parseInit = parse.AddComponent<ParseInitializeBehaviour>();
		parseInit.applicationID = AndroidNativeSettings.Instance.ParseAppId;
		parseInit.dotnetKey = AndroidNativeSettings.Instance.DotNetKey;
		//parseInit.Awake();
		
		ParsePush.ParsePushNotificationReceived += (sender, args) => {
			#if UNITY_ANDROID
			AndroidJavaClass parseUnityHelper = new AndroidJavaClass("com.parse.ParsePushUnityHelper");
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			// Call default behavior.
			parseUnityHelper.CallStatic("handleParsePushNotificationReceived", currentActivity, args.StringPayload);

			// Fire notification received callback
			OnPushReceived(args.StringPayload, args.Payload as Dictionary<string, object>);
			#endif
		};
#endif
	}
}
