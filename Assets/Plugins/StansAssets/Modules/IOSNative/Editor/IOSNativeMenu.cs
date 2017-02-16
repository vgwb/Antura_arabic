////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class IOSNativeMenu : EditorWindow {
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	#if UNITY_EDITOR


	//--------------------------------------
	//  EDIT
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Edit Settings", false, 100)]
	public static void Edit() {
		Selection.activeObject = IOSNativeSettings.Instance;
	}

	//--------------------------------------
	//  Setup
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Setup/Setup and Update", false, 100)]
	public static void SetupPluginSetUp() {
		string url = "https://unionassets.com/iosnative/plugin-set-up-2";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Setup/Compatibility and Dependencies", false, 100)]
	public static void SetupCompatibilityAndDependencies() {
		string url = "https://unionassets.com/iosnative/compatibility-and-dependencies-470";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Setup/Certificate and Provisioning", false, 100)]
	public static void SetupCertificateAndProvisioning() {
		string url = "https://unionassets.com/iosnative/certificate-and-provisioning-5";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Setup/Creating iTunes app", false, 100)]
	public static void SetupCreatingiTunesApp() {
		string url = "https://unionassets.com/iosnative/creating-itunes-app-6";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Setup/IOS 64-bit Support", false, 100)]
	public static void SetupIOS64bitSupport() {
		string url = "https://unionassets.com/iosnative/ios-64-bit-support-462";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  In-App Purchases
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/In-App Purchases/Manage In-App Purchases", false, 100)]
	public static void InAppManagePurchases() {
		string url = "https://unionassets.com/iosnative/manage-in-app-purchases-8";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/In-App Purchases/Coding Guidelines", false, 100)]
	public static void InAppCodingGuidelines() {
		string url = "https://unionassets.com/iosnative/coding-guidelines-15";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/In-App Purchases/Transactions Validation", false, 100)]
	public static void InAppTransactionsValidation() {
		string url = "https://unionassets.com/iosnative/transactions-validation-16";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/In-App Purchases/Restoring Purchases", false, 100)]
	public static void InAppRestoringPurchases() {
		string url = "https://unionassets.com/iosnative/restoring-purchases-21";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/In-App Purchases/Store Product View", false, 100)]
	public static void InAppStoreProductView() {
		string url = "https://unionassets.com/iosnative/store-product-view-438";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  Game Center
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/Manage Game Center", false, 100)]
	public static void GameCenterManage() {
		string url = "https://unionassets.com/iosnative/manage-game-center-7";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/Initialize Game Center", false, 100)]
	public static void GameCenterInit() {
		string url = "https://unionassets.com/iosnative/init-the-game-center-44";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/Leaderboards", false, 100)]
	public static void GameCenterLeaderboards() {
		string url = "https://unionassets.com/iosnative/leaderboards-45";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/Leaderboard Sets", false, 100)]
	public static void GameCenterLeaderboardSets() {
		string url = "https://unionassets.com/iosnative/leaderboard-sets-370";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/Achievements", false, 100)]
	public static void GameCenterAchievements() {
		string url = "https://unionassets.com/iosnative/achievements-46";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/Challenges", false, 100)]
	public static void GameCenterChallenges() {
		string url = "https://unionassets.com/iosnative/challenges-47";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/Friends", false, 100)]
	public static void GameCenterFriends() {
		string url = "https://unionassets.com/iosnative/friends-48";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/Turn-Based Multiplayer", false, 100)]
	public static void GameCenterTurnBasedMultiplayer() {
		string url = "https://unionassets.com/iosnative/turn-based-multiplayer-461";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/Real-Time Multiplayer", false, 100)]
	public static void GameCenterRealTimeMultiplayer() {
		string url = "https://unionassets.com/iosnative/real-time-multiplayer-49";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Game Center/API Reference", false, 100)]
	public static void GameCenterAPIReference() {
		string url = "https://unionassets.com/iosnative/api-reference-463";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  iAd App Network
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/iAd App Network/iAd Setup", false, 100)]
	public static void iAdAppSetup() {
		string url = "https://unionassets.com/iosnative/iad-setup-19";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/iAd App Network/Coding Guidelines", false, 100)]
	public static void iAdAppNetworkCodingGuidelines() {
		string url = "https://unionassets.com/iosnative/coding-guidelines-20";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  iCloud
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/iCloud/iCloud Setup", false, 100)]
	public static void iCloudSetup() {
		string url = "https://unionassets.com/iosnative/icloud-setup-25";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/iCloud/Coding Guidelines", false, 100)]
	public static void iCloudCodingGuidelines() {
		string url = "https://unionassets.com/iosnative/coding-guidelines-26";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  Push Notifications
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Push Notifications/Push Notifications", false, 100)]
	public static void PushNotifications() {
		string url = "https://unionassets.com/iosnative/push-notifications-14";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Push Notifications/Push with OneSignal", false, 100)]
	public static void PushWithOneSignal() {
		string url = "https://unionassets.com/iosnative/push-with-onesignal-419";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  More Features
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Social Sharing", false, 100)]
	public static void FeaturesSocialSharing() {
		string url = "https://unionassets.com/iosnative/social-sharing-9";
		Application.OpenURL(url);
	}
	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Replay Kit", false, 100)]
	public static void FeaturesReplayKit() {
		string url = "https://unionassets.com/iosnative/replaykit-480";
		Application.OpenURL(url);
	}


	
	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Camera And Gallery", false, 100)]
	public static void FeaturesCameraAndGallery() {
		string url = "https://unionassets.com/iosnative/camera-and-gallery-10";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Shared App URL", false, 100)]
	public static void FeaturesSharedAppUrl() {
		string url = "https://unionassets.com/iosnative/shared-app-url-11";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Local Notifications", false, 100)]
	public static void FeaturesLocalNotifications() {
		string url = "https://unionassets.com/iosnative/local-notifications-12";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Pop-ups and Pre-loaders", false, 100)]
	public static void FeaturesPopUpsAndPreLoaders() {
		string url = "https://unionassets.com/iosnative/poups-and-pre-loaders-24";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Native System Events", false, 100)]
	public static void FeaturesNativeSystemEvents() {
		string url = "https://unionassets.com/iosnative/native-system-events-33";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Video API", false, 100)]
	public static void FeaturesVideoAPI() {
		string url = "https://unionassets.com/iosnative/video-api-73";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Date Time Picker", false, 100)]
	public static void FeaturesDateTimePicker() {
		string url = "https://unionassets.com/iosnative/date-time-picker-292";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Validating Receipts Locally", false, 100)]
	public static void FeaturesValidatingReceiptsLocally() {
		string url = "https://unionassets.com/iosnative/validating-receipts-locally-310";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/Media Playback", false, 100)]
	public static void FeaturesMediaPlayback() {
		string url = "https://unionassets.com/iosnative/media-playback-469";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/More Features/System Utilities", false, 100)]
	public static void FeaturesSystemUtilities() {
		string url = "https://unionassets.com/iosnative/system-utilities-371";
		Application.OpenURL(url);
	}
	//--------------------------------------
	//  PLAYMAKER
	//--------------------------------------
	
	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Playmaker/Actions List", false, 100)]
	public static void PlaymakerActionsList() {
		string url = "https://unionassets.com/iosnative/actions-list-18";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Playmaker/iAd With Playmaker", false, 100)]
	public static void PlaymakerIAd() {
		string url = "https://unionassets.com/iosnative/iad-with-playmaker-22";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Playmaker/IOS Native InApp Purchasing with Playmaker", false, 100)]
	public static void PlaymakerInAppPurchasing() {
		string url = "https://unionassets.com/iosnative/ios-native-inapp-purchasing-with-playmaker-28";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  FAQ
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/FAQ", false, 100)]
	public static void FAQ() {
		string url = "https://unionassets.com/iosnative/manual#faq";
		Application.OpenURL(url);
	}	

	//--------------------------------------
	//  TROUBLESHOOTING
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/IOS Native/Documentation/Troubleshooting", false, 100)]
	public static void Troubleshooting() {
		string url = "https://unionassets.com/iosnative/manual#troubleshooting";
		Application.OpenURL(url);
	}	

	#endif

}
#endif