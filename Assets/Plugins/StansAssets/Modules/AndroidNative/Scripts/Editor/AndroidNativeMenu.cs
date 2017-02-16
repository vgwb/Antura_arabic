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

public class AndroidNativeMenu : EditorWindow {
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	//--------------------------------------
	//  EDIT
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Edit Settings", false, 101)]
	public static void Edit() {
		Selection.activeObject = AndroidNativeSettings.Instance;
	}

	//--------------------------------------
	//  GETTING STARTED
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Getting Started/Plugin setup", false, 101)]
	public static void GettingStartedPluginSetup() {
		string url = "https://unionassets.com/android-native-plugin/plugin-setup-79";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Getting Started/Updates", false, 101)]
	public static void GettingStartedUpdates() {
		string url = "https://unionassets.com/android-native-plugin/updates-81";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Getting Started/Compatibility", false, 101)]
	public static void GettingStartedCompatibility() {
		string url = "https://unionassets.com/android-native-plugin/compatibility-154";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  IN-APP-Purchases
	//--------------------------------------
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/In-App Purchases/Setup In Developer Console", false, 101)]
	public static void InAppSetupInConsole() {
		string url = "https://unionassets.com/android-native-plugin/setup-in-developer-console-87";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/In-App Purchases/Setup In Editor", false, 101)]
	public static void InAppSetupInEditor() {
		string url = "https://unionassets.com/android-native-plugin/setup-in-editor-88";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/In-App Purchases/Coding Guidelines", false, 101)]
	public static void InAppCodingGuidelines() {
		string url = "https://unionassets.com/android-native-plugin/coding-guidelines-89";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/In-App Purchases/Video Tutorials", false, 101)]
	public static void InAppVideoTutorials() {
		string url = "https://unionassets.com/android-native-plugin/video-tutorials-179";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  GOOGLE PLAY SERVICES
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Getting Started", false, 101)]
	public static void GPSGettingStarted() {
		string url = "https://unionassets.com/android-native-plugin/getting-started-131";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Sign In", false, 101)]
	public static void GPSSignIn() {
		string url = "https://unionassets.com/android-native-plugin/sign-in-132";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Achievements", false, 101)]
	public static void GPSAchievements() {
		string url = "https://unionassets.com/android-native-plugin/achievements-139";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Leaderboards", false, 101)]
	public static void GPSLeaderboards() {
		string url = "https://unionassets.com/android-native-plugin/leaderboards-140";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Friends", false, 101)]
	public static void GPSFriends() {
		string url = "https://unionassets.com/android-native-plugin/friends-141";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Saved Games", false, 101)]
	public static void GPSSavedGames() {
		string url = "https://unionassets.com/android-native-plugin/saved-games-142";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Game Gifts", false, 101)]
	public static void GPSGameGifts() {
		string url = "https://unionassets.com/android-native-plugin/game-gifts-143";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Quests And Events", false, 101)]
	public static void GPSQuestsAndEvents() {
		string url = "https://unionassets.com/android-native-plugin/quests-and-events-144";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Real-Time Multiplayer", false, 101)]
	public static void GPSRealTimeMultiplayer() {
		string url = "https://unionassets.com/android-native-plugin/real-time-multiplayer-145";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Turn-Based Multiplayer", false, 101)]
	public static void GPSTurnBasedMultiplayer() {
		string url = "https://unionassets.com/android-native-plugin/turn-based-multiplayer-306";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Multiplayer Invitations", false, 101)]
	public static void GPSTMultiplayerInvitations() {
		string url = "https://unionassets.com/android-native-plugin/multiplayer-invitations-338";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Google Play Utilities", false, 101)]
	public static void GPSGooglePlayUtilities() {
		string url = "https://unionassets.com/android-native-plugin/google-play-utilities-273";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  GOOGLE +
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google+/Google+ Button", false, 101)]
	public static void GooglePlusButton() {
		string url = "https://unionassets.com/android-native-plugin/google-button-171";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  SOCIAL
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Social/Native Sharing", false, 101)]
	public static void SocialNativeSharing() {
		string url = "https://unionassets.com/android-native-plugin/native-sharing-164";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Social/Facebook", false, 101)]
	public static void SocialFacebook() {
		string url = "https://unionassets.com/android-native-plugin/facebook-165";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Social/Twitter", false, 101)]
	public static void SocialTwitter() {
		string url = "https://unionassets.com/android-native-plugin/twitter-167";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Social/Instagram Sharing", false, 101)]
	public static void SocialInstagramSharing() {
		string url = "https://unionassets.com/android-native-plugin/instagram-sharing-199";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  MORE FEATURES
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Local Notifications", false, 101)]
	public static void FeaturesNotifications() {
		string url = "https://unionassets.com/android-native-plugin/local-notifications-90";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Camera And Gallery", false, 101)]
	public static void FeaturesCameraAndGallery() {
		string url = "https://unionassets.com/android-native-plugin/camera-and-gallery-93";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Immersive Mode", false, 101)]
	public static void FeaturesImmersiveMode() {
		string url = "https://unionassets.com/android-native-plugin/immersive-mode-91";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Application Information", false, 101)]
	public static void FeaturesApplicationInformation() {
		string url = "https://unionassets.com/android-native-plugin/application-information-94";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Run External App", false, 101)]
	public static void FeaturesRunExternalApp() {
		string url = "https://unionassets.com/android-native-plugin/run-external-app-95";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Analytics", false, 101)]
	public static void FeaturesAnalytics() {
		string url = "https://unionassets.com/android-native-plugin/analytics-96";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Google Cloud Save", false, 101)]
	public static void FeaturesGoogleCloudSave() {
		string url = "https://unionassets.com/android-native-plugin/google-cloud-save-150";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Poups and Pre-loaders", false, 101)]
	public static void FeaturesPoupsPreloaders() {
		string url = "https://unionassets.com/android-native-plugin/poups-and-pre-loaders-174";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Google Mobile Ad", false, 101)]
	public static void FeaturesGoogleMobilAd() {
		string url = "https://unionassets.com/android-native-plugin/google-mobile-ad-178";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Native System Events", false, 101)]
	public static void FeaturesNativeSystemEvents() {
		string url = "https://unionassets.com/android-native-plugin/native-system-events-180";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/App Licensing", false, 101)]
	public static void FeaturesAppLicensing() {
		string url = "https://unionassets.com/android-native-plugin/app-licensing-261";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/AddressBook", false, 101)]
	public static void FeaturesAddressBook() {
		string url = "https://unionassets.com/android-native-plugin/addressbook-262";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Android TV API", false, 101)]
	public static void FeaturesAndroidTVAPI() {
		string url = "https://unionassets.com/android-native-plugin/android-tv-api-366";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/System Utilities", false, 101)]
	public static void FeaturesSystemUtilities() {
		string url = "https://unionassets.com/android-native-plugin/system-utilities-372";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  PUSH NOTIFICATIONS
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Push Notifications/Push Notifications", false, 101)]
	public static void PNPushNotifications() {
		string url = "https://unionassets.com/android-native-plugin/push-notifications-169";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Push Notifications/Push with Parse", false, 101)]
	public static void PNPushWithParse() {
		string url = "https://unionassets.com/android-native-plugin/push-notifications-with-parse-358";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Push Notifications/Push with OneSignal", false, 101)]
	public static void PNPushWithOneSignal() {
		string url = "https://unionassets.com/android-native-plugin/push-notifications-with-gamethrive-359";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  THIRD PARTY PLUGINS SUPPORT
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Third-Party Plug-Ins Support/Anti-Cheat Toolkit", false, 101)]
	public static void TPPSAntiCheatToolkit() {
		string url = "https://unionassets.com/android-native-plugin/anti-cheat-toolkit-420";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  PLAYMAKER
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Playmaker/Actions List", false, 101)]
	public static void PlaymakerActionsList() {
		string url = "https://unionassets.com/android-native-plugin/actions-list-98";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Playmaker/In-App Purchasing with Playmaker", false, 101)]
	public static void PlaymakerInApp() {
		string url = "https://unionassets.com/android-native-plugin/in-app-purchasing-with-playmaker-99";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Playmaker/Google Ad With Playmaker", false, 101)]
	public static void PlaymakerGoogleAd() {
		string url = "https://unionassets.com/android-native-plugin/google-ad-with-playmaker-100";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  FAQ
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/FAQ", false, 101)]
	public static void FAQ() {
		string url = "https://unionassets.com/android-native-plugin/manual#faq";
		Application.OpenURL(url);
	}
	
	//--------------------------------------
	//  TROUBLESHOOTING
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Troubleshooting", false, 101)]
	public static void Troubleshooting() {
		string url = "https://unionassets.com/android-native-plugin/manual#troubleshooting";
		Application.OpenURL(url);
	}
}

#endif
