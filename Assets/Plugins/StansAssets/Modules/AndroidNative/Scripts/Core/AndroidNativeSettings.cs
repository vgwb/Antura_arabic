//#define ATC_SUPPORT_ENABLED

using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if ATC_SUPPORT_ENABLED
using CodeStage.AntiCheat.ObscuredTypes;
#endif

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif



public class AndroidNativeSettings : ScriptableObject {

	public const string VERSION_NUMBER = "9.4/17";
	public const string GOOGLE_PLAY_SDK_VERSION_NUMBER = "10.0.1";
	public const string GOOGLE_PLAY_SDK_LEAGCY_VERSION_NUMBER = "10084000";

	public bool EnablePlusAPI 		= true;
	public bool EnableGamesAPI 		= true;
	public bool EnableAppInviteAPI	= true;
	public bool EnableDriveAPI 		= false;
	public bool LoadProfileIcons 	= true;
	public bool LoadProfileImages 	= true;

	public bool LoadQuestsImages 	= true;
	public bool LoadQuestsIcons 	= true;
	public bool LoadEventsIcons 	= true;
	public bool ShowConnectingPopup = true;


	//ATC:
	public bool EnableATCSupport = false;


	//One Signal
	public bool OneSignalEnabled = false;
	public string OneSignalAppID = "YOUR_ONESIGNAL_APP_ID";
	public string OneSignalDownloadLink = "https://goo.gl/Vc6tfK";
	public string OneSignalDocLink =  "https://goo.gl/aZjkxV";


	//Parce
	public bool UseParsePushNotifications = false;
	public string ParseAppId = "YOUR_PARSE_APP_ID";
	public string DotNetKey = "YOUR_PARSE_DOT_NET_KEY";
	public string ParseDocLink =  "http://goo.gl/9BgQ8r";
	public string ParseDownloadLink =  "https://goo.gl/dm7jYL";

    //PopUps
    public AndroidDialogTheme DialogTheme = AndroidDialogTheme.ThemeDeviceDefaultDark;

	//Soomla
	public bool EnableSoomla = false;
	public string SoomlaDownloadLink = "http://goo.gl/7LYwuj";
	public string SoomlaDocsLink =  "https://goo.gl/es5j1N";
	public string SoomlaGameKey = "" ;
	public string SoomlaEnvKey = "" ;


	//Google AdMob Editor Testing
	public int Ad_EditorFillRateIndex = 4;
	public int Ad_EditorFillRate = 100;
	public bool Is_Ad_EditorTestingEnabled = true;


	//Inn Apps Editor Testing
	public int InApps_EditorFillRateIndex = 4;
	public int InApps_EditorFillRate = 100;
	public bool Is_InApps_EditorTestingEnabled = true;


	//Notifications Editor Testing
	public bool Is_Leaderboards_Editor_Notifications_Enabled = true;
	public bool Is_Achievements_Editor_Notifications_Enabled = true;


	//Google Push
	public string GCM_SenderId = "YOUR_SENDER_ID_HERE";
	public AN_PushNotificationService PushService = AN_PushNotificationService.Google;


	public bool SaveCameraImageToGallery = false;
	public bool UseProductNameAsFolderName = true;
	public string GalleryFolderName = string.Empty;
	public int MaxImageLoadSize = 512;
	public AN_CameraCaptureType CameraCaptureMode;
	public AndroidCameraImageFormat ImageFormat = AndroidCameraImageFormat.JPG;


	public bool ShowAppPermissions = false;
	public bool EnableBillingAPI = true;
	public bool EnablePSAPI = true;
	public bool EnableSocialAPI = true;
	public bool EnableCameraAPI = true;


	public bool ExpandNativeAPI = false;
	public bool ExpandPSAPI = false;
	public bool ExpandBillingAPI = false;
	public bool ExpandSocialAPI = false;
	public bool ExpandCameraAPI = false;



	public bool ThirdPartyParams = false;
	public bool ShowPSSettingsResources = false;
	public bool ShowActions = false;
	public bool GCMSettingsActinve = false;


	//APIs:
	public bool LocalNotificationsAPI = true;
	public bool ImmersiveModeAPI = true;
	public bool ApplicationInformationAPI = true;
	public bool ExternalAppsAPI = true;
	public bool PoupsandPreloadersAPI = true;
	public bool CheckAppLicenseAPI = true;
	public bool NetworkStateAPI = false;
	public bool FirebaseAnalytics = false;

	public bool InAppPurchasesAPI = true;


	public bool GooglePlayServicesAPI = true;
	public bool PlayServicesAdvancedSignInAPI = true;
	public bool GoogleButtonAPI = true;
	public bool AnalyticsAPI = true;
	public bool GoogleCloudSaveAPI = true;
	public bool PushNotificationsAPI = true;
	public bool GoogleMobileAdAPI = true;
	public bool GoogleOAuthAPI = true;

	public bool GalleryAPI = true;
	public bool CameraAPI = true;

	public bool KeepManifestClean = true;





	public string GooglePlayServiceAppID = "0";

	public int ToolbarSelectedIndex = 0;


	#if ATC_SUPPORT_ENABLED
	public  ObscuredString base64EncodedPublicKey = "REPLACE_WITH_YOUR_PUBLIC_KEY";
	#else
	public  string base64EncodedPublicKey = "REPLACE_WITH_YOUR_PUBLIC_KEY";
	#endif

	public bool ShowStoreProducts = true;
	public List<GoogleProductTemplate> InAppProducts = new List<GoogleProductTemplate>();

	public bool ShowLeaderboards = true;
	public List<GPLeaderBoard> Leaderboards = new List<GPLeaderBoard>();

	public bool ShowAchievements = true;
	public List<GPAchievement> Achievements = new List<GPAchievement>();

	public bool ShowWhenAppIsForeground = true;
	public bool EnableVibrationLocal = false;

	public Texture2D LocalNotificationSmallIcon = null;
	public Texture2D LocalNotificationLargeIcon = null;
	public AudioClip LocalNotificationSound = null;
	public int LocalNotificationWakeLockTimer = 10000;

	public bool ReplaceOldNotificationWithNew = false;
	public bool ShowPushWhenAppIsForeground = true;
	public bool EnableVibrationPush = false;

	public Color PushNotificationColor = Color.white;
	public Texture2D PushNotificationSmallIcon = null;
	public Texture2D PushNotificationLargeIcon = null;
	public AudioClip PushNotificationSound = null;

	public const string ANSettingsAssetName = "AndroidNativeSettings";
	public const string ANSettingsAssetExtension = ".asset";

	private static AndroidNativeSettings instance = null;


	public static AndroidNativeSettings Instance {

		get {
			if (instance == null) {
				instance = Resources.Load(ANSettingsAssetName) as AndroidNativeSettings;

				if (instance == null) {

					// If not found, autocreate the asset object.
					instance = CreateInstance<AndroidNativeSettings>();
					#if UNITY_EDITOR
					SA.Common.Util.Files.CreateFolder(SA.Common.Config.SETTINGS_PATH);

					string fullPath = Path.Combine(Path.Combine("Assets", SA.Common.Config.SETTINGS_PATH),
					                               ANSettingsAssetName + ANSettingsAssetExtension
					                               );

					AssetDatabase.CreateAsset(instance, fullPath);
					#endif
				}
			}
			return instance;
		}
	}


	public bool IsBase64KeyWasReplaced {
		get {
			if(base64EncodedPublicKey.Equals("REPLACE_WITH_YOUR_PUBLIC_KEY") || (base64EncodedPublicKey.Equals(""))) {
				return false;
			} else {
				return true;
			}
		}
	}



}
