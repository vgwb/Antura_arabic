////////////////////////////////////////////////////////////////////////////////
//  
// @module Manifest Manager
// @author Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

namespace SA.Manifest {

	public enum ManifestPermission {

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


	static class MenifestPermissionMethods {
		
		public static string GetFullName(this ManifestPermission permission) {

			string prefix = "android.permission.";

			switch(permission) {
			case ManifestPermission.SET_ALARM:
				prefix = "com.android.alarm.permission.";
				break;

			case ManifestPermission.INSTALL_SHORTCUT:
			case ManifestPermission.UNINSTALL_SHORTCUT:
				prefix = "com.android.launcher.permission.";
				break;

			case ManifestPermission.ADD_VOICEMAIL:
				prefix = "com.android.voicemail.permission.";
				break;
			}

			return prefix + permission.ToString();

		}
		
		public static bool IsNormalPermission(this ManifestPermission permission) {
			switch(permission) {
			case ManifestPermission.ACCESS_LOCATION_EXTRA_COMMANDS:
			case ManifestPermission.ACCESS_NETWORK_STATE:
			case ManifestPermission.ACCESS_NOTIFICATION_POLICY:
			case ManifestPermission.ACCESS_WIFI_STATE:
			case ManifestPermission.ACCESS_WIMAX_STATE:
			case ManifestPermission.BLUETOOTH:
			case ManifestPermission.BLUETOOTH_ADMIN:
			case ManifestPermission.BROADCAST_STICKY:
			case ManifestPermission.CHANGE_NETWORK_STATE:
			case ManifestPermission.CHANGE_WIFI_MULTICAST_STATE:
			case ManifestPermission.CHANGE_WIFI_STATE:
			case ManifestPermission.CHANGE_WIMAX_STATE:
			case ManifestPermission.DISABLE_KEYGUARD:
			case ManifestPermission.EXPAND_STATUS_BAR:
			case ManifestPermission.FLASHLIGHT:
			case ManifestPermission.GET_PACKAGE_SIZE:
			case ManifestPermission.INTERNET:
			case ManifestPermission.KILL_BACKGROUND_PROCESSES:
			case ManifestPermission.MODIFY_AUDIO_SETTINGS:
			case ManifestPermission.NFC:
			case ManifestPermission.READ_SYNC_SETTINGS:
			case ManifestPermission.READ_SYNC_STATS:
			case ManifestPermission.RECEIVE_BOOT_COMPLETED:
			case ManifestPermission.REORDER_TASKS:
			case ManifestPermission.REQUEST_INSTALL_PACKAGES:
			case ManifestPermission.SET_TIME_ZONE:
			case ManifestPermission.SET_WALLPAPER:
			case ManifestPermission.SET_WALLPAPER_HINTS:
			case ManifestPermission.SUBSCRIBED_FEEDS_READ:
			case ManifestPermission.TRANSMIT_IR:
			case ManifestPermission.USE_FINGERPRINT:
			case ManifestPermission.VIBRATE:
			case ManifestPermission.WAKE_LOCK:
			case ManifestPermission.WRITE_SYNC_SETTINGS:
			case ManifestPermission.SET_ALARM:
			case ManifestPermission.INSTALL_SHORTCUT:
			case ManifestPermission.UNINSTALL_SHORTCUT:
				return true;
			default:
				return false;
			}
		}



	}

}
