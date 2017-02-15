using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AndroidNotificationManager : SA.Common.Pattern.Singleton<AndroidNotificationManager>  {
	public const int LENGTH_SHORT = 0; // 2 seconds 
	public const int LENGTH_LONG  = 1; // 3.5 seconds
	
	
	
	//Actions
	public Action<int> OnNotificationIdLoaded = delegate{};
	

	
	private const string PP_KEY = "AndroidNotificationManagerKey";
	private const string PP_ID_KEY = "AndroidNotificationManagerKey_ID";
	private const string DATA_SPLITTER = "|";
	
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	public void LocadAppLaunchNotificationId() {
		AN_NotificationProxy.requestCurrentAppLaunchNotificationId();
	}
	
	public void HideAllNotifications() {
		AN_NotificationProxy.HideAllNotifications ();
	}
	
	public void ShowToastNotification(string text) {
		ShowToastNotification (text, LENGTH_SHORT);
	}
	
	public void ShowToastNotification(string text, int duration) {
		AN_NotificationProxy.ShowToastNotification (text, duration);
	}
	
	public int ScheduleLocalNotification(string title, string message, int seconds) {
		AndroidNotificationBuilder builder = new AndroidNotificationBuilder (GetNextId, title, message, seconds);
		return ScheduleLocalNotification (builder);
	}
	
	public int ScheduleLocalNotification(AndroidNotificationBuilder builder) {
		AN_NotificationProxy.ScheduleLocalNotification(builder);
		
		LocalNotificationTemplate notification =  new LocalNotificationTemplate(builder.Id, builder.Title, builder.Message, DateTime.Now.AddSeconds(builder.Time));
		List<LocalNotificationTemplate> scheduled = LoadPendingNotifications();
		scheduled.Add(notification);
		
		SaveNotifications(scheduled);
		
		return builder.Id;
	}
	
	public void CancelLocalNotification(int id, bool clearFromPrefs = true) {
		AN_NotificationProxy.CanselLocalNotification(id);
		
		if(clearFromPrefs) {
			List<LocalNotificationTemplate> scheduled = LoadPendingNotifications();
			List<LocalNotificationTemplate> newList =  new List<LocalNotificationTemplate>();
			
			foreach(LocalNotificationTemplate n in scheduled) {
				if(n.id != id) {
					newList.Add(n);
				}
			}
			
			
			SaveNotifications(newList);
			
		}
	}
	
	
	public void CancelAllLocalNotifications() {

		List<LocalNotificationTemplate> scheduled = LoadPendingNotifications();
		
		foreach(LocalNotificationTemplate n in scheduled) {
			CancelLocalNotification(n.id, false);
		}
		
		SaveNotifications(new List<LocalNotificationTemplate>());
	}


	// --------------------------------------
	// Get / Set
	// --------------------------------------

	
	public int GetNextId {
		get {
			int id = 1;
			if(PlayerPrefs.HasKey(PP_ID_KEY)) {
				id = PlayerPrefs.GetInt(PP_ID_KEY);
				id++;
			} 
			
			PlayerPrefs.SetInt(PP_ID_KEY, id);
			return id;
		}
		
	}


	// --------------------------------------
	// Events
	// --------------------------------------
	
	
	private void OnNotificationIdLoadedEvent(string data)  {
		int id = System.Convert.ToInt32(data);
		
		OnNotificationIdLoaded(id);
		
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	

	
	private void SaveNotifications(List<LocalNotificationTemplate> notifications) {
		
		if(notifications.Count == 0) {
			PlayerPrefs.DeleteKey(PP_KEY);
			return;
		}
		
		string srialzedNotifications = "";
		int len = notifications.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				srialzedNotifications += DATA_SPLITTER;
			}
			
			srialzedNotifications += notifications[i].SerializedString;
		}
		
		PlayerPrefs.SetString(PP_KEY, srialzedNotifications);
		
	}
	
	
	public  List<LocalNotificationTemplate> LoadPendingNotifications(bool includeAll = false) {
		#if UNITY_ANDROID
		string data = string.Empty;
		if(PlayerPrefs.HasKey(PP_KEY)) {
			data = PlayerPrefs.GetString(PP_KEY);
		}
		List<LocalNotificationTemplate>  tpls = new List<LocalNotificationTemplate>();

		if(data != string.Empty) {
			string[] notifications = data.Split(DATA_SPLITTER [0]);
			foreach(string n in notifications) {
				
				String templateData = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(n) );
		
				try {
					LocalNotificationTemplate notification = new LocalNotificationTemplate(templateData);

					if(!notification.IsFired|| includeAll) {
						tpls.Add(notification);
					}
				} catch(Exception e) {
					Debug.Log("AndroidNative. AndroidNotificationManager loading notification data failed: " + e.Message);
				}

			}
		}
		return tpls;
		#else
		return null;
		#endif
		

	}
	
	
	
}
