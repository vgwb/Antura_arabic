using UnityEngine;
using System.Collections;

public enum AN_Permission {

	ACCESS_LOCATION_EXTRA_COMMANDS,
	ACCESS_NETWORK_STATE,
	ACCESS_NOTIFICATION_POLICY,
	ACCESS_WIFI_STATE,
	ACCESS_WIMAX_STATE,
	BLUETOOTH,
	BLUETOOTH_ADMIN,
	BROADCAST_STICKY,
	CHANGE_NETWORK_STATE,
	CHANGE_WIFI_MULTICAST_STATE,
	CHANGE_WIFI_STATE,
	CHANGE_WIMAX_STATE,
	DISABLE_KEYGUARD,
	EXPAND_STATUS_BAR,
	FLASHLIGHT,
	GET_PACKAGE_SIZE,
	INTERNET,
	KILL_BACKGROUND_PROCESSES,
	MODIFY_AUDIO_SETTINGS,
	NFC,
	READ_SYNC_SETTINGS,
	READ_SYNC_STATS,
	RECEIVE_BOOT_COMPLETED,
	REORDER_TASKS,
	REQUEST_INSTALL_PACKAGES,
	SET_TIME_ZONE,
	SET_WALLPAPER,
	SET_WALLPAPER_HINTS,
	SUBSCRIBED_FEEDS_READ,
	TRANSMIT_IR,
	USE_FINGERPRINT,
	VIBRATE,
	WAKE_LOCK,
	WRITE_SYNC_SETTINGS,
	SET_ALARM,
	INSTALL_SHORTCUT,
	UNINSTALL_SHORTCUT,



	READ_CALENDAR,
	WRITE_CALENDAR,

	CAMERA,

	READ_CONTACTS,
	WRITE_CONTACTS,
	GET_ACCOUNTS,

	ACCESS_FINE_LOCATION,
	ACCESS_COARSE_LOCATION,

	RECORD_AUDIO,


	READ_PHONE_STATE,
	CALL_PHONE,
	READ_CALL_LOG,
	WRITE_CALL_LOG,
	ADD_VOICEMAIL,
	USE_SIP,
	PROCESS_OUTGOING_CALLS,

	BODY_SENSORS,

	SEND_SMS,
	READ_SMS,
	RECEIVE_SMS,
	RECEIVE_WAP_PUSH,
	RECEIVE_MMS,




	READ_EXTERNAL_STORAGE,
	WRITE_EXTERNAL_STORAGE,


	UNDEFINED

}


static class AN_PermissionMethods {

	public static string GetFullName(this AN_Permission permission) {

		string prefix = "android.permission.";

		switch(permission) {
		case AN_Permission.SET_ALARM:
			prefix = "com.android.alarm.permission.";
			break;

		case AN_Permission.INSTALL_SHORTCUT:
		case AN_Permission.UNINSTALL_SHORTCUT:
			prefix = "com.android.launcher.permission.";
			break;

		case AN_Permission.ADD_VOICEMAIL:
			prefix = "com.android.voicemail.permission.";
			break;
		}

		return prefix + permission.ToString();

	}

	public static bool IsNormalPermission(this AN_Permission permission) {
		switch(permission) {
		case AN_Permission.ACCESS_LOCATION_EXTRA_COMMANDS:
		case AN_Permission.ACCESS_NETWORK_STATE:
		case AN_Permission.ACCESS_NOTIFICATION_POLICY:
		case AN_Permission.ACCESS_WIFI_STATE:
		case AN_Permission.ACCESS_WIMAX_STATE:
		case AN_Permission.BLUETOOTH:
		case AN_Permission.BLUETOOTH_ADMIN:
		case AN_Permission.BROADCAST_STICKY:
		case AN_Permission.CHANGE_NETWORK_STATE:
		case AN_Permission.CHANGE_WIFI_MULTICAST_STATE:
		case AN_Permission.CHANGE_WIFI_STATE:
		case AN_Permission.CHANGE_WIMAX_STATE:
		case AN_Permission.DISABLE_KEYGUARD:
		case AN_Permission.EXPAND_STATUS_BAR:
		case AN_Permission.FLASHLIGHT:
		case AN_Permission.GET_PACKAGE_SIZE:
		case AN_Permission.INTERNET:
		case AN_Permission.KILL_BACKGROUND_PROCESSES:
		case AN_Permission.MODIFY_AUDIO_SETTINGS:
		case AN_Permission.NFC:
		case AN_Permission.READ_SYNC_SETTINGS:
		case AN_Permission.READ_SYNC_STATS:
		case AN_Permission.RECEIVE_BOOT_COMPLETED:
		case AN_Permission.REORDER_TASKS:
		case AN_Permission.REQUEST_INSTALL_PACKAGES:
		case AN_Permission.SET_TIME_ZONE:
		case AN_Permission.SET_WALLPAPER:
		case AN_Permission.SET_WALLPAPER_HINTS:
		case AN_Permission.SUBSCRIBED_FEEDS_READ:
		case AN_Permission.TRANSMIT_IR:
		case AN_Permission.USE_FINGERPRINT:
		case AN_Permission.VIBRATE:
		case AN_Permission.WAKE_LOCK:
		case AN_Permission.WRITE_SYNC_SETTINGS:
		case AN_Permission.SET_ALARM:
		case AN_Permission.INSTALL_SHORTCUT:
		case AN_Permission.UNINSTALL_SHORTCUT:
			return true;
		default:
			return false;
		}
	}



}
