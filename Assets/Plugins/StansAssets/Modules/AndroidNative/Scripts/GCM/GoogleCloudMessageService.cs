//#define ONE_SIGNAL_ENABLED

////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;

public class GoogleCloudMessageService : SA.Common.Pattern.Singleton<GoogleCloudMessageService> {

	//Actions
	public static event Action<string> ActionCouldMessageLoaded 						 						= delegate {};
	public static event Action<GP_GCM_RegistrationResult> ActionCMDRegistrationResult  							= delegate {};

	public static event Action<string, Dictionary<string, object>> ActionGCMPushLaunched						= delegate {};
 	public static event Action<string, Dictionary<string, object>> ActionGCMPushReceived						= delegate {};
	public static event Action<string, Dictionary<string, object>> ActionParsePushReceived 						= delegate {};

	#if ONE_SIGNAL_ENABLED
	// NotificationReceived - Event will be fired when a push notification is received when the user is in your game.
	// notification = The Notification dictionary filled from a serialized native OSNotification object
	public event Action<OSNotification> OnOneSignalNotificationReceived = delegate {};

	// NotificationOpened - Event will be fired when a push notification is opened.
	// result = The Notification open result describing : 1. The notification opened 2. The action taken by the user
	public event Action<OSNotificationOpenedResult> OnOneSignalNotificationOpened = delegate {};
	#endif

	private string _lastMessage = string.Empty;
	private string _registrationId = string.Empty;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public void Init() {
		switch(AndroidNativeSettings.Instance.PushService) {
		case AN_PushNotificationService.Google:
			InitPushNotifications();
			break;
		case AN_PushNotificationService.OneSignal:
			InitOneSignalNotifications();
			break;
		case AN_PushNotificationService.Parse:
			InitParsePushNotifications();
			break;
		}
	}

	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	public void InitOneSignalNotifications() {
		#if ONE_SIGNAL_ENABLED
		OneSignal.StartInit(AndroidNativeSettings.Instance.OneSignalAppID, AndroidNativeSettings.Instance.GCM_SenderId)
				 .HandleNotificationReceived(HandleNotificationReceived)
				 .HandleNotificationOpened(HandleNotificationOpened)
				 .EndInit();
		#endif
	}

	#if ONE_SIGNAL_ENABLED
	// Called when your app is in focus and a notificaiton is recieved.
	// The name of the method can be anything as long as the signature matches.
	// Method must be static or this object should be marked as DontDestroyOnLoad
	private void HandleNotificationReceived(OSNotification notification) {
		OnOneSignalNotificationReceived (notification);
	}

	// Called when a notification is opened.
	// The name of the method can be anything as long as the signature matches.
	// Method must be static or this object should be marked as DontDestroyOnLoad
	public void HandleNotificationOpened(OSNotificationOpenedResult result) {
		OnOneSignalNotificationOpened (result);
	}
	#endif

	public void InitPushNotifications() {
		AN_NotificationProxy.InitPushNotifications (
			AndroidNativeSettings.Instance.PushNotificationSmallIcon == null ? string.Empty : AndroidNativeSettings.Instance.PushNotificationSmallIcon.name.ToLower(),
			AndroidNativeSettings.Instance.PushNotificationLargeIcon == null ? string.Empty : AndroidNativeSettings.Instance.PushNotificationLargeIcon.name.ToLower(),
		    AndroidNativeSettings.Instance.PushNotificationSound == null ? string.Empty : AndroidNativeSettings.Instance.PushNotificationSound.name,
		    AndroidNativeSettings.Instance.EnableVibrationPush, AndroidNativeSettings.Instance.ShowPushWhenAppIsForeground,
			AndroidNativeSettings.Instance.ReplaceOldNotificationWithNew,
			string.Format("{0}|{1}|{2}|{3}", 255 * AndroidNativeSettings.Instance.PushNotificationColor.a,
		              						 255 * AndroidNativeSettings.Instance.PushNotificationColor.r,
		              						 255 * AndroidNativeSettings.Instance.PushNotificationColor.g,
		              						 255 * AndroidNativeSettings.Instance.PushNotificationColor.b));
	}

	public void InitPushNotifications(string smallIcon, string largeIcon, string sound, bool enableVibrationPush, bool showWhenAppForeground, bool replaceOldNotificationWithNew, string color) {
		AN_NotificationProxy.InitPushNotifications (smallIcon, largeIcon, sound,enableVibrationPush, showWhenAppForeground, replaceOldNotificationWithNew, color);
	}

	public void InitParsePushNotifications() {
		ParsePushesStub.InitParse();
		ParsePushesStub.OnPushReceived += HandleOnPushReceived;
	}

	public void RgisterDevice() {
		AN_NotificationProxy.GCMRgisterDevice(AndroidNativeSettings.Instance.GCM_SenderId);
	}

	public void LoadLastMessage() {
		AN_NotificationProxy.GCMLoadLastMessage();
	}

	public void RemoveLastMessageInfo() {
		AN_NotificationProxy.GCMRemoveLastMessageInfo();
	}

	//--------------------------------------
	// HANDLER
	//--------------------------------------

	private void HandleOnPushReceived (string stringPayload, Dictionary<string, object> payload)
	{
		ActionParsePushReceived(stringPayload, payload);
	}

	private void GCMNotificationCallback(string data) {
		Debug.Log("[GCMNotificationCallback] JSON Data: " + data);

		string[] bundle = data.Split (new string[] { "|" }, StringSplitOptions.None);
		string msg = bundle[0];
		Dictionary<string, object> json = ANMiniJSON.Json.Deserialize(bundle[1]) as Dictionary<string, object>;

		ActionGCMPushReceived(msg, json);
	}

	private void GCMNotificationLaunchedCallback(string data) {
		Debug.Log("[GCMNotificationLaunchedCallback] JSON Data: " + data);

		string[] bundle = data.Split (new string[] { "|" }, StringSplitOptions.None);
		string msg = bundle[0];
		Dictionary<string, object> json = ANMiniJSON.Json.Deserialize(bundle[1]) as Dictionary<string, object>;
		
		ActionGCMPushLaunched(msg, json);
	}
	
	//--------------------------------------
	// GET / SET
	//--------------------------------------
	
	public string registrationId {
		get {
			return _registrationId;
		}
	}

	public string lastMessage {
		get {
			return _lastMessage;
		}
	}
	
	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void OnLastMessageLoaded(string data) {
		_lastMessage = data;
		ActionCouldMessageLoaded(lastMessage);

	}

	
	private void OnRegistrationReviced(string regId) {
		_registrationId = regId;

		ActionCMDRegistrationResult(new GP_GCM_RegistrationResult(_registrationId));
	}
	
	private void OnRegistrationFailed() {
		ActionCMDRegistrationResult(new GP_GCM_RegistrationResult());
	}
	
	
	
}
