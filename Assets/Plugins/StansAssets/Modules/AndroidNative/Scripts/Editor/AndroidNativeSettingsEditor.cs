#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using SA.Common.Editor;

[CustomEditor(typeof(AndroidNativeSettings))]
public class AndroidNativeSettingsEditor : Editor {
	
	
	GUIContent PlusApiLabel   		= new GUIContent("Enable Plus API [?]:", "API used for account managment");
	GUIContent GamesApiLabel   		= new GUIContent("Enable Games API [?]:", "API used for achivements and leaderboards");
	GUIContent DriveApiLabel 		= new GUIContent("Enable Drive API [?]:", "API used for saved games");
	GUIContent AppInviteAPILabel 	= new GUIContent("Enable AppInvite API [?]:", "API used for invite");
	
	
	
	
	GUIContent Base64KeyLabel 	= new GUIContent("Base64 Key[?]:", "Base64 Key app key.");
	GUIContent SdkVersion   	= new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent GPSdkVersion   	= new GUIContent("Google Play SDK Version [?]", "Version of Google Play SDK used by the plugin");
	
	
	private AndroidNativeSettings settings;
	
	
	void Awake() {
		ApplaySettings();
		
		if(IsInstalled && IsUpToDate) {
			UpdateManifest();
		}
		
		#if !UNITY_WEBPLAYER
		UpdatePluginDefines();
		#endif
		
	}
	
	
	
	public static string AndroidNativeSettings_Path = SA.Common.Config.MODULS_PATH + "AndroidNative/Scripts/Core/AndroidNativeSettings.cs";
	public static string GoogleCloudMessageService_Path = SA.Common.Config.MODULS_PATH + "AndroidNative/Scripts/GCM/GoogleCloudMessageService.cs";
	public static string ParseCloudMessageService_Path = SA.Common.Config.MODULS_PATH + "AndroidNative/Scripts/System/Notifications/ParsePushesStub.cs";
	
	public static void UpdatePluginDefines() {
		
		SA.Common.Editor.Tools.ChnageDefineState(AndroidNativeSettings_Path, 	"ATC_SUPPORT_ENABLED", 	AndroidNativeSettings.Instance.EnableATCSupport);
		SA.Common.Editor.Tools.ChnageDefineState(GoogleCloudMessageService_Path, "ONE_SIGNAL_ENABLED", 	AndroidNativeSettings.Instance.OneSignalEnabled);
		SA.Common.Editor.Tools.ChnageDefineState(ParseCloudMessageService_Path, "PARSE_PUSH_ENABLED", 	AndroidNativeSettings.Instance.UseParsePushNotifications);
		
		SocialPlatfromSettingsEditor.UpdatePluginDefines();
	}
	
	
	
	
	
	private Texture[] _ToolbarImages = null;
	
	public Texture[] ToolbarImages {
		get {
			if(_ToolbarImages == null) {
				Texture2D market =  Resources.Load("market") as Texture2D;
				Texture2D googleplay =  Resources.Load("googleplay") as Texture2D;
				Texture2D notifications =  Resources.Load("notifications") as Texture2D;
				Texture2D sharing =  Resources.Load("sharing") as Texture2D;
				Texture2D other =  Resources.Load("other") as Texture2D;				
				Texture2D android =  Resources.Load("android") as Texture2D;
				Texture2D camera =  Resources.Load("gallery") as Texture2D;
				Texture2D editorTesting = Resources.Load("editorTesting") as Texture2D;
				
				List<Texture2D> textures =  new List<Texture2D>();
				textures.Add(android);
				textures.Add(googleplay);
				textures.Add(market);
				textures.Add(notifications);
				textures.Add(sharing);
				textures.Add(camera);
				textures.Add(other);
				textures.Add(editorTesting);
				
				_ToolbarImages = textures.ToArray();
				
			}
			return _ToolbarImages;
		}
	}
	
	
	private int _Width = 500;
	public int Width {
		get {
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			Rect scale = GUILayoutUtility.GetLastRect();
			
			if(scale.width != 1) {
				_Width = System.Convert.ToInt32(scale.width);
			}
			
			return _Width;
		}
	}
	
	public override void OnInspectorGUI() {
		#if UNITY_WEBPLAYER
		EditorGUILayout.HelpBox("Editing Android Native Settings not avaliable with web player platfrom. Please swith to any other platfrom under Build Seting menu", MessageType.Warning);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Switch To Android Platfrom",  GUILayout.Width(180))) {
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		}
		EditorGUILayout.EndHorizontal();
		
		if(Application.isEditor) {
			return;
		}
		
		
		
		#endif
		
		
		settings = target as AndroidNativeSettings;
		
		GUI.changed = false;
		
		InstallOptions();
		
		
		GUILayoutOption[] toolbarSize = new GUILayoutOption[]{GUILayout.Width(Width-5), GUILayout.Height(30)};
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		AndroidNativeSettings.Instance.ToolbarSelectedIndex =  GUILayout.Toolbar(AndroidNativeSettings.Instance.ToolbarSelectedIndex, ToolbarImages, toolbarSize);
		GUILayout.FlexibleSpace();
		
		EditorGUILayout.EndHorizontal();
		
		switch(AndroidNativeSettings.Instance.ToolbarSelectedIndex) {
		case 0:
			PluginSetting();
			EditorGUILayout.Space();
			AboutGUI();
			break;
			
		case 1:
			PlayServiceSettings();
			break;
			
		case 2:
			BillingSettings();
			break;
			
		case 3:
			NotificationsSettings();
			break;
			
		case 4:
			SocialSettings();
			break;
		case 5:
			CameraAndGalleryParams();
			break;
			
		case 6:
			ThirdPartyParams ();
			break;
		case 7:
			EditorTestingParams();
			break;
		}
		
		
		if(GUI.changed) {
			DirtyEditor();
		}
		
	}
	
	
	public static bool IsInstalled {
		get {
			return VersionsManager.Is_AN_Installed;
		}
	}
	
	public static bool IsUpToDate {
		get {
			if(CurrentVersion == VersionsManager.AN_Version) {
				return true;
			} else {
				return false;
			}
		}
	}
	
	
	public static int CurrentVersion {
		get {
			return VersionsManager.ParceVersion(AndroidNativeSettings.VERSION_NUMBER);
		}
	}
	
	public static int CurrentMagorVersion {
		get {
			return VersionsManager.ParceMagorVersion(AndroidNativeSettings.VERSION_NUMBER);
		}
	}
	
	
	public static void UpdateVersionInfo() {
		SA.Common.Util.Files.Write(SA.Common.Config.AN_VERSION_INFO_PATH, AndroidNativeSettings.VERSION_NUMBER);
		SocialPlatfromSettingsEditor.UpdateVersionInfo();
		UpdateManifest();
	}
	
	
	
	
	
	private void DrawOpenManifestButton() {
		
		
		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		
		if(GUILayout.Button("Open Manifest ",  GUILayout.Width(120))) {
			UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal("Assets" +  SA.Manifest.Manager.MANIFEST_FILE_PATH, 1);
		}
		EditorGUILayout.EndHorizontal();
	}
	
	
	private void InstallOptions() {
		
		
		
		if(!IsInstalled) {
			EditorGUILayout.BeginVertical (GUI.skin.box);
			EditorGUILayout.HelpBox("Install Required ", MessageType.Error);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			Color c = GUI.color;
			GUI.color = Color.cyan;
			if(GUILayout.Button("Install Plugin",  GUILayout.Width(350))) {
				Instalation.Android_InstallPlugin();
				UpdataAndroidNativeAARs ();
				UpdateVersionInfo();
			}
			
			EditorGUILayout.Space();
			GUI.color = c;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			
		}
		
		if(IsInstalled) {
			if(!IsUpToDate) {
				EditorGUILayout.BeginVertical (GUI.skin.box);
				EditorGUILayout.HelpBox("Update Required \nResources version: " + VersionsManager.AN_StringVersionId + " Plugin version: " + AndroidNativeSettings.VERSION_NUMBER, MessageType.Warning);
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				Color c = GUI.color;
				GUI.color = Color.cyan;
				
				
				
				if(CurrentMagorVersion != VersionsManager.AN_MagorVersion) {
					if(GUILayout.Button("How to update",  GUILayout.Width(350))) {
						Application.OpenURL("https://goo.gl/Z9wgEI");
					}
				} else {
					if(GUILayout.Button("Upgrade Resources",  GUILayout.Width(350))) {
						AN_Plugin_Update();
						UpdateVersionInfo();
					}
				}
				
				
				GUI.color = c;
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
				
				
			} else {
				EditorGUILayout.HelpBox("Android Native Plugin v" + AndroidNativeSettings.VERSION_NUMBER + " is installed", MessageType.Info);
			}
		}
		
		
		EditorGUILayout.Space();
		
	}
	
	
	private void Actions() {
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("More Actions", EditorStyles.boldLabel);
		
		
		if(!Instalation.IsFacebookInstalled) {
			GUI.enabled = false;
		}	
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		
		if(GUILayout.Button("Remove Facebook SDK",  GUILayout.Width(160))) {
			Instalation.Remove_FB_SDK_WithDialog();
			UpdatePluginDefines();
		}
		
		GUI.enabled = true;
		
		if(GUILayout.Button("Open Manifest ",  GUILayout.Width(160))) {
			UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal("Assets" + SA.Manifest.Manager.MANIFEST_FILE_PATH, 1);
		}
		
		if(GUILayout.Button("Reset Settings",  GUILayout.Width(160))) {
			
			SocialPlatfromSettingsEditor.ResetSettings();
			
			SA.Common.Util.Files.DeleteFile(SA.Common.Config.SETTINGS_PATH + "AndroidNativeSettings.asset");
			AndroidNativeSettings.Instance.ShowActions = true;
			Selection.activeObject = AndroidNativeSettings.Instance;
			
			return;
		}
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		
		if(GUILayout.Button("Load Example Settings",  GUILayout.Width(160))) {
			LoadExampleSettings();
		}
		
		if(GUILayout.Button("Reinstall",  GUILayout.Width(160))) {
			AN_Plugin_Update();
			UpdateVersionInfo();
			
		}
		if(GUILayout.Button("Remove",  GUILayout.Width(160))) {
			RemoveTool.RemovePlugins();
		}
		
		EditorGUILayout.Space();
		
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		
		
	}
	
	public static void LoadExampleSettings()  {
		AndroidNativeSettings.Instance.base64EncodedPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsV676BTvO5djSDdUwotbLCIPtGZ5OVCbIn402RXuEpDwuHZMIOy5E6DQjUlQPKCiB7A1Vx+ePQI50Gk8NO1zuPRBgCgvW/oTTf863KkF34QLZD+Ii8fc6VE0UKp3GfApnLmq2qtr1fwDmRCteBUET1h0EcRn3/6R/BA5DMmF1aTv8yUY5LQETWqEPIjGdyNaAhmnWf2sTliYLANiR51WXsfbDdCNT4Ux3gQo/XJynGadfwRS7A9N9e5SgvMEFUR6EwnANOF9QXgE2d0HEitpS56D3uHH/2LwICrTWAmbLX3qPYlQ3Ncf1SRyjqiKae2wW8QUnDFU5BSozwGW6tcQvQIDAQAB";
		AndroidNativeSettings.Instance.InAppProducts =  new List<GoogleProductTemplate>();
		
		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "coins_bonus", 		Title = "Bonus Coins", IsOpen = false});
		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "small_coins_bag", 	Title = "Small Coins Bag", IsOpen = false});
		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "pm_coins", 		Title = "Coins Pack", IsOpen = false});
		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "pm_green_sphere", 	Title = "Green Sphere", IsOpen = false});
		AndroidNativeSettings.Instance.InAppProducts.Add(new GoogleProductTemplate(){ SKU = "pm_red_sphere", 	Title = "Red Sphere", IsOpen = false});
		
		
		AndroidNativeSettings.Instance.SoomlaEnvKey = "3c3df370-ad80-4577-8fe5-ca2c49b2c1b4";
		AndroidNativeSettings.Instance.SoomlaGameKey = "db24ba61-3aa7-4653-a3f7-9c613cb2c0f3";
		
		AndroidNativeSettings.Instance.GCM_SenderId = "216817929098";
		AndroidNativeSettings.Instance.GooglePlayServiceAppID = "216817929098";
		
		PlayerSettings.bundleIdentifier = "com.unionassets.android.plugin.preview";
		
		SocialPlatfromSettingsEditor.LoadExampleSettings();
	}
	
	
	
	private void PluginSetting() {
		
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Plugin Settings", MessageType.None);
		
		
		EditorGUILayout.LabelField("Android Native Libs", EditorStyles.boldLabel);
		
		
		EditorGUI.indentLevel++;
		EditorGUI.BeginChangeCheck();
		
		
		
		//Native Lib API
		EditorGUILayout.BeginHorizontal();
		settings.ExpandNativeAPI = EditorGUILayout.Foldout(settings.ExpandNativeAPI, "Enable Native Lib");
		SuperSpace();
		GUI.enabled = false;
		EditorGUILayout.Toggle(true);
		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();
		if(settings.ExpandNativeAPI) {
			EditorGUI.indentLevel++;
			
			
			EditorGUILayout.BeginHorizontal();
			settings.LocalNotificationsAPI = EditorGUILayout.Toggle(AN_API_NAME.LocalNotifications,  settings.LocalNotificationsAPI);
			settings.ImmersiveModeAPI = EditorGUILayout.Toggle(AN_API_NAME.ImmersiveMode,  settings.ImmersiveModeAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			settings.ApplicationInformationAPI = EditorGUILayout.Toggle(AN_API_NAME.ApplicationInformation,  settings.ApplicationInformationAPI);
			settings.ExternalAppsAPI = EditorGUILayout.Toggle(AN_API_NAME.RunExternalApp,  settings.ExternalAppsAPI);
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.BeginHorizontal();
			settings.PoupsandPreloadersAPI = EditorGUILayout.Toggle(AN_API_NAME.PoupsandPreloaders,  settings.PoupsandPreloadersAPI);
			settings.CheckAppLicenseAPI = EditorGUILayout.Toggle(AN_API_NAME.CheckAppLicense,  settings.CheckAppLicenseAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			settings.NetworkStateAPI = EditorGUILayout.Toggle(AN_API_NAME.NetworkInfo,  settings.NetworkStateAPI);
			settings.FirebaseAnalytics = EditorGUILayout.Toggle(AN_API_NAME.FirebaseAnalytics,  settings.FirebaseAnalytics);
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		
		
		
		EditorGUILayout.BeginHorizontal();
		settings.ExpandBillingAPI = EditorGUILayout.Foldout(settings.ExpandBillingAPI, "Enable Billing Lib");
		SuperSpace();
		settings.EnableBillingAPI	 	= EditorGUILayout.Toggle(settings.EnableBillingAPI);
		
		EditorGUILayout.EndHorizontal();
		if(settings.ExpandBillingAPI) {
			EditorGUI.indentLevel++;
			
			
			EditorGUILayout.BeginHorizontal();
			settings.InAppPurchasesAPI = EditorGUILayout.Toggle(AN_API_NAME.InAppPurchases,  settings.InAppPurchasesAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		EditorGUI.BeginChangeCheck ();
		//GOOGLE PLAY API
		EditorGUILayout.BeginHorizontal();
		settings.ExpandPSAPI = EditorGUILayout.Foldout(settings.ExpandPSAPI, "Enable Google Play Lib");
		SuperSpace();
		
		settings.EnablePSAPI = EditorGUILayout.Toggle(settings.EnablePSAPI);
		
		EditorGUILayout.EndHorizontal();
		
		if(settings.ExpandPSAPI) {
			EditorGUI.indentLevel++;
			
			EditorGUILayout.BeginHorizontal();
			settings.GooglePlayServicesAPI = EditorGUILayout.Toggle(AN_API_NAME.GooglePlayServices,  settings.GooglePlayServicesAPI);
			settings.PlayServicesAdvancedSignInAPI = EditorGUILayout.Toggle(AN_API_NAME.GooglePlayServicesAdvancedSignIn,  settings.PlayServicesAdvancedSignInAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			settings.PushNotificationsAPI = EditorGUILayout.Toggle(AN_API_NAME.PushNotifications,  settings.PushNotificationsAPI);
			settings.GoogleCloudSaveAPI = EditorGUILayout.Toggle(AN_API_NAME.GoogleCloudSave,  settings.GoogleCloudSaveAPI);
			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.BeginHorizontal();
			settings.AnalyticsAPI = EditorGUILayout.Toggle(AN_API_NAME.Analytics,  settings.AnalyticsAPI);
			settings.GoogleMobileAdAPI = EditorGUILayout.Toggle(AN_API_NAME.GoogleMobileAd,  settings.GoogleMobileAdAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			settings.GoogleButtonAPI = EditorGUILayout.Toggle(AN_API_NAME.GoogleButton,  settings.GoogleButtonAPI);
			settings.GoogleOAuthAPI = EditorGUILayout.Toggle (AN_API_NAME.GoogleOAuth, settings.GoogleOAuthAPI);
			EditorGUILayout.EndHorizontal();
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		if (EditorGUI.EndChangeCheck()) {
			UpdataAndroidNativeAARs ();
		}
		
		
		
		EditorGUILayout.BeginHorizontal();
		settings.ExpandSocialAPI = EditorGUILayout.Foldout(settings.ExpandSocialAPI, "Enable Social Lib");
		SuperSpace();
		
		
		settings.EnableSocialAPI	 	= EditorGUILayout.Toggle(settings.EnableSocialAPI);
		EditorGUILayout.EndHorizontal();
		if(settings.ExpandSocialAPI) {
			EditorGUI.indentLevel++;
			
			SocialPlatfromSettingsEditor.DrawAPIsList();
			
			
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		
		
		EditorGUILayout.BeginHorizontal();
		settings.ExpandCameraAPI = EditorGUILayout.Foldout(settings.ExpandCameraAPI, "Enable Camera Lib");
		SuperSpace();
		
		settings.EnableCameraAPI	 	= EditorGUILayout.Toggle(settings.EnableCameraAPI);
		
		
		EditorGUILayout.EndHorizontal();
		if(settings.ExpandCameraAPI) {
			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();
			settings.CameraAPI = EditorGUILayout.Toggle(AN_API_NAME.CameraAPI,  settings.CameraAPI);
			settings.GalleryAPI = EditorGUILayout.Toggle(AN_API_NAME.Gallery,  settings.GalleryAPI);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
			EditorGUILayout.Space();
		}
		
		
		EditorGUI.indentLevel--;
		
		if(EditorGUI.EndChangeCheck()) {
			UpdateAPIsInstalation();
			UpdataAndroidNativeAARs ();
		}
		
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Android Manifest", EditorStyles.boldLabel);
		
		
		EditorGUI.indentLevel++;
		
		
		SA.Manifest.Manager.Refresh();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Keep Android Mnifest Clean");
		
		EditorGUI.BeginChangeCheck();
		AndroidNativeSettings.Instance.KeepManifestClean = EditorGUILayout.Toggle(AndroidNativeSettings.Instance.KeepManifestClean);
		SocialPlatfromSettings.Instance.KeepManifestClean = AndroidNativeSettings.Instance.KeepManifestClean;
		if(EditorGUI.EndChangeCheck()) {
			UpdateManifest();
		}
		
		if(GUILayout.Button("[?]",  GUILayout.Width(27))) {
			Application.OpenURL("http://goo.gl/syIebl");
		}
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		EditorGUILayout.EndHorizontal();
		
		
		
		AndroidNativeSettings.Instance.ShowAppPermissions = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowAppPermissions, "Application Permissions");
		if(AndroidNativeSettings.Instance.ShowAppPermissions) {
			SA.Manifest.Manager.Refresh();
			
			
			EditorGUILayout.LabelField("Required By Android Native:", EditorStyles.boldLabel);
			List<string> permissions = GetRequiredPermissions();
			
			foreach(string p in permissions) {
				EditorGUILayout.BeginVertical (GUI.skin.box);
				EditorGUILayout.BeginHorizontal();
				
				EditorGUILayout.SelectableLabel(p, GUILayout.Height(16));
				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Other Permissions in Manifest:", EditorStyles.boldLabel);
			foreach(SA.Manifest.PropertyTemplate tpl in SA.Manifest.Manager.GetManifest().Permissions) {
				if(!permissions.Contains(tpl.Name)) {
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					EditorGUILayout.BeginHorizontal();
					
					EditorGUILayout.SelectableLabel(tpl.Name, GUILayout.Height(16));
					if(GUILayout.Button("x",  GUILayout.Width(20))) {
						SA.Manifest.Manager.GetManifest().RemovePermission(tpl);
						SA.Manifest.Manager.SaveManifest();
						return;
					}
					
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}
			} 
			
			
			//EditorGUI.indentLevel--;
		}
		
		
		EditorGUI.indentLevel--;
		
		
		Actions();
		
		EditorGUILayout.Space();
	}
	
	private static void SuperSpace() {
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
	}
	
	private void UpdataAndroidNativeAARs() {
		//Manipulate play-services-gcm.aar
		if (AndroidNativeSettings.Instance.GoogleMobileAdAPI)
			Instalation.EnableGoogleAdMobAPI ();
		else
			Instalation.DisableGoogleAdMobAPI ();

		//Monipulate play-services-games.aar
		if (AndroidNativeSettings.Instance.GooglePlayServicesAPI)
			Instalation.EnableGooglePlayServicesAPI ();
		else
			Instalation.DisableGooglePlayServicesAPI ();

		//Manipulate play-services-gcm.aar
		if (AndroidNativeSettings.Instance.PushNotificationsAPI)
			Instalation.EnablePushNotificationsAPI ();
		else
			Instalation.DisablePushNotificationsAPI ();

		//Manipulate play-services-plus.aar
		if (AndroidNativeSettings.Instance.EnablePlusAPI)
			Instalation.EnableGooglePlusAPI ();
		else
			Instalation.DisableGooglePlusAPI ();

		//Manipulate play-services-appinvite.aar
		if (AndroidNativeSettings.Instance.EnableAppInviteAPI)
			Instalation.EnableAppInvitesAPI ();
		else
			Instalation.DisableAppInvitesAPI ();

		//Manipulate play-services-analytics.aar
		if (AndroidNativeSettings.Instance.AnalyticsAPI)
			Instalation.EnableAnalyticsAPI ();
		else
			Instalation.DisableAnalyticsAPI ();

		//Manipulate play-services-auth.aar
		if (AndroidNativeSettings.Instance.GoogleOAuthAPI)
			Instalation.EnableOAuthAPI ();
		else
			Instalation.DisableOAuthAPI ();

		//Manipulate play-services-drive.aar
		if (AndroidNativeSettings.Instance.GoogleCloudSaveAPI)
			Instalation.EnableDriveAPI ();
		else
			Instalation.DisableDriveAPI ();
	}
	
	public static void UpdateAPIsInstalation() {
		
		
		if(AndroidNativeSettings.Instance.EnableBillingAPI) {
			Instalation.EnableBillingAPI();
		} else {
			Instalation.DisableBillingAPI();
			AndroidNativeSettings.Instance.InAppPurchasesAPI = false;
		}
		
		if(AndroidNativeSettings.Instance.EnablePSAPI) {
			Instalation.EnableGooglePlayAPI();
		} else {
			Instalation.DisableGooglePlayAPI();
			
			AndroidNativeSettings.Instance.GooglePlayServicesAPI = false;
			AndroidNativeSettings.Instance.PushNotificationsAPI = false;
			
			AndroidNativeSettings.Instance.GoogleCloudSaveAPI = false;
			AndroidNativeSettings.Instance.GoogleMobileAdAPI = false;
			
			AndroidNativeSettings.Instance.AnalyticsAPI = false;
			AndroidNativeSettings.Instance.GoogleButtonAPI = false;
		}
		
		
		if(AndroidNativeSettings.Instance.EnableSocialAPI) {
			Instalation.EnableSocialAPI();
		} else {
			Instalation.DisableSocialAPI();
			SocialPlatfromSettings.Instance.TwitterAPI = false;
			SocialPlatfromSettings.Instance.NativeSharingAPI = false;
			SocialPlatfromSettings.Instance.InstagramAPI = false;
		}
		
		
		if(AndroidNativeSettings.Instance.EnableCameraAPI) {
			Instalation.EnableCameraAPI();
		} else {
			Instalation.DisableCameraAPI();
			AndroidNativeSettings.Instance.CameraAPI = false;
			AndroidNativeSettings.Instance.GalleryAPI = false;
		}
		
		
		if(AndroidNativeSettings.Instance.GooglePlayServicesAPI == false) {
			AndroidNativeSettings.Instance.PlayServicesAdvancedSignInAPI = false;
		}
		
		
		if(AndroidNativeSettings.Instance.CheckAppLicenseAPI) {
			Instalation.EnableAppLicensingAPI();
		} else {
			Instalation.DisableAppLicensingAPI();
		}

		if (AndroidNativeSettings.Instance.FirebaseAnalytics) {
			Instalation.EnableFirebaseAnalytics ();
		} else {
			Instalation.DisableFirebaseAnalytics ();
		}
		
		UpdateManifest();
		
		
	}
	
	
	
	public static void UpdateManifest() {
		
		if(!AndroidNativeSettings.Instance.KeepManifestClean) {
			return;
		}
		
		UpdateAppID ();


		
		SA.Manifest.Manager.Refresh();
		
		int UpdateId = 0;
		SA.Manifest.Template Manifest =  SA.Manifest.Manager.GetManifest();
		SA.Manifest.ApplicationTemplate application =  Manifest.ApplicationTemplate;
		SA.Manifest.ActivityTemplate launcherActivity = application.GetLauncherActivity();

		SA.Manifest.PropertyTemplate targetSdk = Manifest.GetOrCreatePropertyWithTag ("uses-sdk");
		targetSdk.SetValue ("android:targetSdkVersion", "25");
		
		if(launcherActivity.Name == "com.androidnative.AndroidNativeBridge") {
			launcherActivity.SetName("com.unity3d.player.UnityPlayerNativeActivity");
		}
		
		foreach (KeyValuePair<int, SA.Manifest.ActivityTemplate> a in application.Activities) {
			if (a.Value.Name.Equals("com.unity3d.player.UnityPlayerNativeActivity") && !a.Value.IsLauncher) {
				application.RemoveActivity(a.Value);
				break;
			}
		}
		
		////////////////////////
		//REQUIRED
		////////////////////////
		SA.Manifest.ActivityTemplate AndroidNativeProxy = application.GetOrCreateActivityWithName("com.androidnative.AndroidNativeProxy");
		AndroidNativeProxy.SetValue("android:launchMode", "singleTask");
		AndroidNativeProxy.SetValue("android:label", "@string/app_name");
		AndroidNativeProxy.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
		AndroidNativeProxy.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");
		
		SA.Manifest.ActivityTemplate OAuthNativeProxy = application.GetOrCreateActivityWithName("com.androidnative.OAuthProxyActivity");
		OAuthNativeProxy.SetValue("android:launchMode", "singleTask");
		OAuthNativeProxy.SetValue("android:label", "@string/app_name");
		OAuthNativeProxy.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
		OAuthNativeProxy.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");		
		
		////////////////////////
		//Google Play Service API
		////////////////////////
		SA.Manifest.PropertyTemplate games_version = application.GetOrCreatePropertyWithName("meta-data",  "com.google.android.gms.version");
		if(AndroidNativeSettings.Instance.EnablePSAPI) {
			#if UNITY_4_6 || UNITY_4_7
			games_version.SetValue("android:value", AndroidNativeSettings.GOOGLE_PLAY_SDK_LEAGCY_VERSION_NUMBER);
			#else
			games_version.SetValue("android:value", "@integer/google_play_services_version");
			#endif
		} else {
			application.RemoveProperty(games_version);
		}

		////////////////////////
		//Firebase Analytics
		////////////////////////

		UpdateId++;
		SA.Manifest.PropertyTemplate internalReceiver = application.GetOrCreatePropertyWithName ("receiver", "com.google.firebase.iid.FirebaseInstanceIdInternalReceiver");
		SA.Manifest.PropertyTemplate provider = application.GetOrCreatePropertyWithName ("provider", "com.google.firebase.provider.FirebaseInitProvider");
		SA.Manifest.PropertyTemplate service = application.GetOrCreatePropertyWithName ("service", "com.google.firebase.iid.FirebaseInstanceIdService");
		SA.Manifest.PropertyTemplate receiver = application.GetOrCreatePropertyWithName ("receiver", "com.google.firebase.iid.FirebaseInstanceIdReceiver");

		if (AndroidNativeSettings.Instance.FirebaseAnalytics) {
			internalReceiver.SetValue ("android:exported", "false");

			receiver.SetValue ("android:permission", "com.google.android.c2dm.permission.SEND");
			receiver.SetValue ("android:exported", "true");
			SA.Manifest.PropertyTemplate filter = receiver.GetOrCreateIntentFilterWithName ("com.google.android.c2dm.intent.RECEIVE");
			filter.GetOrCreatePropertyWithName ("action", "com.google.android.c2dm.intent.REGISTRATION");
			filter.GetOrCreatePropertyWithName ("category", PlayerSettings.bundleIdentifier);

			service.SetValue ("android:exported", "true");
			SA.Manifest.PropertyTemplate intentFilter = service.GetOrCreateIntentFilterWithName ("com.google.firebase.INSTANCE_ID_EVENT");
			intentFilter.SetValue ("android:priority", "-500");

			provider.SetValue ("android:initOrder", "100");
			provider.SetValue ("android:exported", "false");
			provider.SetValue ("android:authorities", PlayerSettings.bundleIdentifier + ".firebaseinitprovider");
		} else {
			application.RemoveProperty (internalReceiver);
			application.RemoveProperty (provider);
			application.RemoveProperty (service);
			application.RemoveProperty (receiver);
		}
		
		////////////////////////
		//GooglePlayServicesAPI
		////////////////////////
		
		UpdateId++;
		SA.Manifest.PropertyTemplate games_APP_ID  = application.GetOrCreatePropertyWithName("meta-data",  "com.google.android.gms.games.APP_ID");
		if(!AndroidNativeSettings.Instance.GooglePlayServicesAPI) {
			application.RemoveProperty(games_APP_ID);
		} else {
			games_APP_ID.SetValue("android:value", "@string/app_id");
			
			SA.Manifest.PropertyTemplate property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.games.APP_ID");
			property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
		}
		
		////////////////////////
		//GoogleCloudSaveAPI
		////////////////////////
		UpdateId++;
		SA.Manifest.PropertyTemplate appstate_APP_ID = application.GetOrCreatePropertyWithName("meta-data",  "com.google.android.gms.appstate.APP_ID");
		if(AndroidNativeSettings.Instance.GoogleCloudSaveAPI) {
			appstate_APP_ID.SetValue("android:value", "@string/app_id");
			
			SA.Manifest.PropertyTemplate property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.appstate.APP_ID");
			property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
		} else {
			application.RemoveProperty(appstate_APP_ID);
		}
		
		
		////////////////////////
		//AnalyticsAPI
		////////////////////////
		UpdateId++;
		if(AndroidNativeSettings.Instance.AnalyticsAPI) {
			//Nothing to do
		}
		
		
		////////////////////////
		//PushNotificationsAPI
		////////////////////////
		UpdateId++;
		
		
		SA.Manifest.PropertyTemplate permission_C2D_MESSAGE_Old = Manifest.GetPropertyWithName ("permission", "com.example.gcm.permission.C2D_MESSAGE");
		if (permission_C2D_MESSAGE_Old != null) {
			Manifest.RemoveProperty(permission_C2D_MESSAGE_Old);
		}
		
		SA.Manifest.PropertyTemplate permission_C2D_MESSAGE = Manifest.GetOrCreatePropertyWithName("permission", PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE");
		permission_C2D_MESSAGE.SetValue("android:protectionLevel", "signature");
		
		SA.Manifest.PropertyTemplate GcmBroadcastReceiver = application.GetOrCreatePropertyWithName("receiver",  "com.androidnative.gcm.GcmBroadcastReceiver");
		SA.Manifest.PropertyTemplate GcmIntentService = application.GetOrCreatePropertyWithName("service",  "com.androidnative.gcm.GcmIntentService");
		
		SA.Manifest.ActivityTemplate gameThriveActivity = application.GetOrCreateActivityWithName ("com.onesignal.NotificationOpenedActivity");
		SA.Manifest.PropertyTemplate gameThriveService = application.GetOrCreatePropertyWithName("service", "com.onesignal.GcmIntentService");
		SA.Manifest.PropertyTemplate gameThriveReceiver = application.GetOrCreatePropertyWithName ("receiver", "com.onesignal.GcmBroadcastReceiver");
		
		SA.Manifest.PropertyTemplate ParseBroadcastReceiver = application.GetOrCreatePropertyWithName ("receiver",  "com.parse.ParsePushBroadcastReceiver");
		SA.Manifest.PropertyTemplate ParsePushService = application.GetOrCreatePropertyWithName ("service", "com.parse.ParsePushService");
		
		if (AndroidNativeSettings.Instance.PushNotificationsAPI) {
			
			switch (AndroidNativeSettings.Instance.PushService) {
			case AN_PushNotificationService.Google:
				
				GcmBroadcastReceiver.SetValue("android:exported", "true");
				GcmBroadcastReceiver.SetValue("android:permission", "com.google.android.c2dm.permission.SEND");
				
				SA.Manifest.PropertyTemplate intent_filter = GcmBroadcastReceiver.GetOrCreateIntentFilterWithName("com.google.android.c2dm.intent.RECEIVE");
				intent_filter.GetOrCreatePropertyWithName("action", "com.androidnative.push.intent.OPEN");
				SA.Manifest.PropertyTemplate category = intent_filter.GetOrCreatePropertyWithTag("category");
				category.SetValue("android:name", PlayerSettings.bundleIdentifier);
				
				//Clean Up other push notifications providers
				application.RemoveActivity(gameThriveActivity);
				application.RemoveProperty(gameThriveService);
				application.RemoveProperty(gameThriveReceiver);
				
				application.RemoveProperty(ParseBroadcastReceiver);
				application.RemoveProperty(ParsePushService);
				
				break;
			case AN_PushNotificationService.OneSignal:
				
				gameThriveActivity.SetValue("android:theme", "@android:style/Theme.NoDisplay");
				
				gameThriveReceiver.SetValue("android:permission", "com.google.android.c2dm.permission.SEND");
				
				SA.Manifest.PropertyTemplate gameThriveIntentFilter = gameThriveReceiver.GetOrCreateIntentFilterWithName("com.google.android.c2dm.intent.RECEIVE");
				gameThriveIntentFilter.GetOrCreatePropertyWithName("category", PlayerSettings.bundleIdentifier);
				
				//Clean Up other push notifications providers
				application.RemoveProperty(GcmBroadcastReceiver);
				application.RemoveProperty(GcmIntentService);
				
				application.RemoveProperty(ParseBroadcastReceiver);
				application.RemoveProperty(ParsePushService);
				
				break;
			case AN_PushNotificationService.Parse:
				
				ParseBroadcastReceiver.SetValue("android:permission", "com.google.android.c2dm.permission.SEND");
				
				SA.Manifest.PropertyTemplate parseIntentFilter = ParseBroadcastReceiver.GetOrCreateIntentFilterWithName("com.google.android.c2dm.intent.RECEIVE");
				parseIntentFilter.GetOrCreatePropertyWithName("action", "com.google.android.c2dm.intent.REGISTRATION");
				parseIntentFilter.GetOrCreatePropertyWithName("category", PlayerSettings.bundleIdentifier);
				
				//Clean Up other push notifications providers
				application.RemoveProperty(GcmBroadcastReceiver);
				application.RemoveProperty(GcmIntentService);
				
				application.RemoveActivity(gameThriveActivity);
				application.RemoveProperty(gameThriveService);
				application.RemoveProperty(gameThriveReceiver);
				
				break;
			default: break;
			}
			
		} else {
			//Clean Up ALL push notifications providers,
			// if Push Notifications APIs desabled in Android Native Settings
			application.RemoveProperty(GcmBroadcastReceiver);
			application.RemoveProperty(GcmIntentService);
			
			application.RemoveActivity(gameThriveActivity);
			application.RemoveProperty(gameThriveService);
			application.RemoveProperty(gameThriveReceiver);
			
			application.RemoveProperty(ParseBroadcastReceiver);
			application.RemoveProperty(ParsePushService);
			
			Manifest.RemoveProperty(permission_C2D_MESSAGE);
		}
		
		////////////////////////
		//In App Purchases API
		////////////////////////
		
		SA.Manifest.ActivityTemplate BillingProxyActivity = application.GetOrCreateActivityWithName("com.androidnative.billing.core.AN_BillingProxyActivity");
		if(AndroidNativeSettings.Instance.InAppPurchasesAPI) {
			
			BillingProxyActivity.SetValue("android:launchMode", "singleTask");
			BillingProxyActivity.SetValue("android:label", "@string/app_name");
			BillingProxyActivity.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
			BillingProxyActivity.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");
		} else {
			application.RemoveActivity(BillingProxyActivity);
		}
		
		
		
		SA.Manifest.ActivityTemplate GP_ProxyActivity = application.GetOrCreateActivityWithName("com.androidnative.gms.core.GooglePlaySupportActivity");
		if(AndroidNativeSettings.Instance.EnablePSAPI) {
			GP_ProxyActivity.SetValue("android:launchMode", "singleTask");
			GP_ProxyActivity.SetValue("android:label", "@string/app_name");
			GP_ProxyActivity.SetValue("android:configChanges", "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
			GP_ProxyActivity.SetValue("android:theme", "@android:style/Theme.Translucent.NoTitleBar");
		} else {
			application.RemoveActivity(GP_ProxyActivity);
		}
		
		
		
		////////////////////////
		//GoogleMobileAdAPI
		////////////////////////
		UpdateId++;
		SA.Manifest.ActivityTemplate AdActivity = application.GetOrCreateActivityWithName("com.google.android.gms.ads.AdActivity");
		
		
		
		if(AndroidNativeSettings.Instance.GoogleMobileAdAPI) {
			if(launcherActivity != null) {
				SA.Manifest.PropertyTemplate ForwardNativeEventsToDalvik = launcherActivity.GetOrCreatePropertyWithName("meta-data",  "unityplayer.ForwardNativeEventsToDalvik");
				
				#if !(UNITY_4_0	|| UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
				ForwardNativeEventsToDalvik.SetValue("android:value", "false");
				#else
				ForwardNativeEventsToDalvik.SetValue("android:value", "true");
				#endif
			}
			
			AdActivity.SetValue ("android:configChanges", "keyboard|keyboardHidden|orientation|screenLayout|uiMode|screenSize|smallestScreenSize");
			AdActivity.SetValue ("android:theme", "@android:style/Theme.Translucent");
		} else {
			application.RemoveActivity(AdActivity);
		}
		
		
		
		////////////////////////
		//GoogleButtonAPI
		////////////////////////
		UpdateId++;
		if(AndroidNativeSettings.Instance.GoogleButtonAPI) {
			//Nothing to do
		} 
		
		
		
		////////////////////////
		//Local Notification Tags
		////////////////////////
		SA.Manifest.PropertyTemplate LocalNotificationService = application.GetOrCreatePropertyWithName("service", "com.androidnative.features.notifications.LocalNotificationService");



		SA.Manifest.PropertyTemplate LocalNotificationReceiver = application.GetOrCreatePropertyWithName("receiver",  "com.androidnative.features.notifications.LocalNotificationReceiver");
		SA.Manifest.PropertyTemplate ReceiverIntentFilter = LocalNotificationReceiver.GetOrCreateIntentFilterWithName ("com.androidnative.local.intent.OPEN");
		ReceiverIntentFilter.GetOrCreatePropertyWithName ("category", "android.intent.category.DEFAULT");

		if(!AndroidNativeSettings.Instance.LocalNotificationsAPI) {
			application.RemoveProperty (LocalNotificationReceiver);
			application.RemoveProperty (LocalNotificationService);
		}
		
		////////////////////////
		//ImmersiveModeAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.ImmersiveModeAPI) {
			//Nothing to do
		}
		
		
		////////////////////////
		//ApplicationInformationAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.ApplicationInformationAPI) {
			//Nothing to do
		}
		
		////////////////////////
		//ExternalAppsAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.ExternalAppsAPI) {
			//Nothing to do
		}
		
		
		////////////////////////
		//PoupsandPreloadersAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.PoupsandPreloadersAPI) {
			//Nothing to do
		}
		
		
		////////////////////////
		//CameraAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.CameraAPI) {
			//Nothing to do
		}
		
		
		////////////////////////
		//GalleryAPI
		////////////////////////
		if(AndroidNativeSettings.Instance.GalleryAPI) {
			//Nothing to do
		}
		
		List<string> permissions = GetRequiredPermissions();
		foreach(string p in permissions) {
			Manifest.AddPermission(p);
		}
		
		////////////////////////
		//Check for C2D_MESSAGE <permission> duplicates
		////////////////////////
		bool duplicated = true;
		while (duplicated) {
			duplicated = false;
			List<SA.Manifest.PropertyTemplate> properties = Manifest.Properties["permission"];
			foreach (SA.Manifest.PropertyTemplate permission in properties) {
				if (permission.Name.EndsWith(".permission.C2D_MESSAGE")
				    && !permission.Name.Equals(PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE")) {
					properties.Remove(permission);
					duplicated = true;
					break;
				}
			}
		}
		
		////////////////////////
		//Check for C2D_MESSAGE <permission> <uses-permission> duplicates
		////////////////////////
		duplicated = true;
		while (duplicated) {
			duplicated = false;
			List<SA.Manifest.PropertyTemplate> properties = Manifest.Permissions;
			foreach (SA.Manifest.PropertyTemplate permission in properties) {
				if (permission.Name.EndsWith(".permission.C2D_MESSAGE")
				    && !permission.Name.Equals(PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE")) {
					properties.Remove(permission);
					duplicated = true;
					break;
				}
			}
		}
		
		SA.Manifest.Manager.SaveManifest();
		
		SocialPlatfromSettingsEditor.UpdateManifest();
	}
	
	
	private static List<string> GetRequiredPermissions() {
		List<string> permissions =  new List<string>();
		permissions.Add("android.permission.INTERNET");
		
		
		if(AndroidNativeSettings.Instance.AnalyticsAPI) {
			permissions.Add("android.permission.ACCESS_NETWORK_STATE");
		}
		
		if(AndroidNativeSettings.Instance.InAppPurchasesAPI) {
			permissions.Add("com.android.vending.BILLING");
		}
		
		if(AndroidNativeSettings.Instance.PushNotificationsAPI) {
			permissions.Add("com.google.android.c2dm.permission.RECEIVE");
			permissions.Add(PlayerSettings.bundleIdentifier + ".permission.C2D_MESSAGE");
			permissions.Add("android.permission.WAKE_LOCK");
		}
		
		if(AndroidNativeSettings.Instance.LocalNotificationsAPI || AndroidNativeSettings.Instance.PushNotificationsAPI) {
			permissions.Add("android.permission.VIBRATE");
			permissions.Add("android.permission.GET_TASKS");
		}
		
		
		
		if(SocialPlatfromSettings.Instance.EnableImageSharing) {
			permissions.Add("android.permission.WRITE_EXTERNAL_STORAGE");
			permissions.Add("android.permission.ACCESS_WIFI_STATE");
		}
		
		if(AndroidNativeSettings.Instance.PlayServicesAdvancedSignInAPI) {
			permissions.Add("android.permission.GET_ACCOUNTS");
		}
		
		if (AndroidNativeSettings.Instance.CheckAppLicenseAPI) {
			permissions.Add("com.android.vending.CHECK_LICENSE");
		}
		
		if (AndroidNativeSettings.Instance.NetworkStateAPI) {
			permissions.Add("android.permission.ACCESS_WIFI_STATE");
		}
		
		return permissions;
	}
	
	private bool IsDigitsOnly(string str) {
		foreach (char c in str) {
			if (!char.IsDigit(c)) {
				return false;
			}
		}
		
		return true;
	}
	
	private static void UpdateAppID() {
		if (!SA.Common.Util.Files.IsFolderExists("Plugins/Android/AN_Res/res/values")) {
			EditorGUILayout.HelpBox("Android resource folder DOESN'T exist", MessageType.Warning);
		} else {
			if (!SA.Common.Util.Files.IsFileExists ("Plugins/Android/AN_Res/res/values/ids.xml")) {
				EditorGUILayout.HelpBox("XML file with PlayService ID's DOESN'T exist", MessageType.Warning);
			} else {
				//Parse XML file with PlayService Settings ID's
				XmlDocument doc = new XmlDocument();
				doc.Load(Application.dataPath + "/Plugins/Android/AN_Res/res/values/ids.xml");
				
				bool bAppIdNodeExists = false;
				string appId = string.Empty;
				XmlNode rootResourcesNode = doc.DocumentElement;
				
				List<XmlNode> resources = new List<XmlNode>();
				foreach(XmlNode chn in rootResourcesNode.ChildNodes) {
					if (chn.Name.Equals("string")) {
						if (chn.Attributes["name"] != null) {
							if (chn.Attributes["name"].Value.Equals("app_id")) {
								bAppIdNodeExists = true;
								appId = chn.InnerText;
							} else {
								resources.Add(chn);
							}
						}
					}
				}
				
				if (bAppIdNodeExists) {
					//Save AppID to manifest file, if it has been changed
					if (!appId.Equals(AndroidNativeSettings.Instance.GooglePlayServiceAppID)) {
						AndroidNativeSettings.Instance.GooglePlayServiceAppID = appId;
						
						SA.Manifest.Manager.Refresh();
						SA.Manifest.Template manifest = SA.Manifest.Manager.GetManifest();
						SA.Manifest.ApplicationTemplate application = manifest.ApplicationTemplate;
						
						SA.Manifest.PropertyTemplate property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.games.APP_ID");
						property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
						property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.version");
						property.SetValue("android:value", AndroidNativeSettings.GOOGLE_PLAY_SDK_VERSION_NUMBER);
						property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.appstate.APP_ID");
						property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
						SA.Manifest.Manager.SaveManifest();
					}
				}
			}
		}
	}
	
	private void PlayServiceDrawXmlIDs() {
		if (!SA.Common.Util.Files.IsFolderExists("Plugins/Android/AN_Res/res/values")) {
			EditorGUILayout.HelpBox("Android resource folder DOESN'T exist", MessageType.Warning);
		} else {
			if (!SA.Common.Util.Files.IsFileExists ("Plugins/Android/AN_Res/res/values/ids.xml")) {
				EditorGUILayout.HelpBox("XML file with PlayService ID's DOESN'T exist", MessageType.Warning);
			} else {
				//Parse XML file with PlayService Settings ID's
				XmlDocument doc = new XmlDocument();
				doc.Load(Application.dataPath + "/Plugins/Android/AN_Res/res/values/ids.xml");
				
				bool bAppIdNodeExists = false;
				string appId = string.Empty;
				XmlNode rootResourcesNode = doc.DocumentElement;
				
				List<XmlNode> resources = new List<XmlNode>();
				foreach(XmlNode chn in rootResourcesNode.ChildNodes) {
					if (chn.Name.Equals("string")) {
						if (chn.Attributes["name"] != null) {
							if (chn.Attributes["name"].Value.Equals("app_id")) {
								bAppIdNodeExists = true;
								appId = chn.InnerText;
								
								EditorGUILayout.BeginHorizontal();
								GUI.enabled = true;
								EditorGUILayout.LabelField("App ID:");
								GUI.enabled = false;
								EditorGUILayout.TextField(chn.InnerText);
								EditorGUILayout.EndHorizontal();
							} else {
								resources.Add(chn);
							}
						}
					}
				}
				
				if (!bAppIdNodeExists) {
					//Warning in Inspector window if there is NO AppID info in XML file
					EditorGUILayout.HelpBox("XML file with DOESN'T contain information for App ID", MessageType.Warning);
				} else {
					//Save AppID to manifest file, if it has been changed
					if (!appId.Equals(AndroidNativeSettings.Instance.GooglePlayServiceAppID)) {
						AndroidNativeSettings.Instance.GooglePlayServiceAppID = appId;
						
						SA.Manifest.Manager.Refresh();
						SA.Manifest.Template manifest = SA.Manifest.Manager.GetManifest();
						SA.Manifest.ApplicationTemplate application = manifest.ApplicationTemplate;
						
						SA.Manifest.PropertyTemplate property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.games.APP_ID");
						property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
						property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.version");
						property.SetValue("android:value", AndroidNativeSettings.GOOGLE_PLAY_SDK_VERSION_NUMBER);
						property = application.GetOrCreatePropertyWithName("meta-data", "com.google.android.gms.appstate.APP_ID");
						property.SetValue("android:value", "\\ " + AndroidNativeSettings.Instance.GooglePlayServiceAppID);
						SA.Manifest.Manager.SaveManifest();
					}
				}
				
				EditorGUI.indentLevel++;
				if (resources.Count > 0) {
					GUI.enabled = true;
					AndroidNativeSettings.Instance.ShowPSSettingsResources = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowPSSettingsResources, "Resources IDs");
					
					if (AndroidNativeSettings.Instance.ShowPSSettingsResources) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Name", EditorStyles.boldLabel);
						EditorGUILayout.Space();
						EditorGUILayout.LabelField("ID", EditorStyles.boldLabel, GUILayout.Width(170.0f));
						EditorGUILayout.EndHorizontal();
					}
					GUI.enabled = false;
				}
				
				if (AndroidNativeSettings.Instance.ShowPSSettingsResources) {
					foreach (XmlNode r in resources) {
						EditorGUILayout.BeginHorizontal();
						GUI.enabled = true;
						EditorGUILayout.LabelField(r.Attributes["name"].Value);
						GUI.enabled = false;
						EditorGUILayout.TextField(r.InnerText, GUILayout.Width(170.0f));
						EditorGUILayout.EndHorizontal();
					}
					
					GUI.enabled = true;
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.Space ();
					if (GUILayout.Button("[?] How to GET Resources?", GUILayout.Width(200.0f))) {
						Application.OpenURL("https://unionassets.com/android-native-plugin/get-playservice-settings-resources-284");
					}
					EditorGUILayout.Space ();
					EditorGUILayout.EndHorizontal ();
				}
				EditorGUI.indentLevel--;
			}
		}
		
		GUI.enabled = true;
	}
	
	GUIContent LeaderboardIdDLabel 		= new GUIContent("LeaderboardId[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
	GUIContent LeaderboardNameLabel  	= new GUIContent("Display Name[?]:", "This is the name of the Leaderboard that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
	GUIContent LeaderboardDescriptionLabel 	= new GUIContent("Description[?]:", "This is the description of the Leaderboard. The description cannot be longer than 255 bytes.");
	
	GUIContent AchievementIdDLabel 		= new GUIContent("AchievementId[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
	GUIContent AchievementNameLabel  	= new GUIContent("Display Name[?]:", "This is the name of the Achievement that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
	GUIContent AchievementDescriptionLabel 	= new GUIContent("Description[?]:", "This is the description of the Achievement. The description cannot be longer than 255 bytes.");
	
	private void PlayServiceSettings() {
		
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Play Service API Settings", MessageType.None);
		
		
		PlayServiceDrawXmlIDs();
		EditorGUILayout.Space();
		
		EditorGUI.indentLevel++;
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			
			EditorGUILayout.BeginHorizontal();
			AndroidNativeSettings.Instance.ShowLeaderboards = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowLeaderboards, "Leaderboards");
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			if(AndroidNativeSettings.Instance.ShowLeaderboards) {
				
				foreach(GPLeaderBoard leaderboard in AndroidNativeSettings.Instance.Leaderboards) {
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					
					GUIStyle s =  new GUIStyle();
					s.padding =  new RectOffset();
					s.margin =  new RectOffset();
					s.border =  new RectOffset();
					
					if(leaderboard.Texture != null) {
						GUILayout.Box(leaderboard.Texture, s, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
					}
					
					leaderboard.IsOpen 	= EditorGUILayout.Foldout(leaderboard.IsOpen, leaderboard.Name);
					
					bool ItemWasRemoved = DrawSortingButtons((object) leaderboard, AndroidNativeSettings.Instance.Leaderboards);
					if(ItemWasRemoved) {
						return;
					}
					
					EditorGUILayout.EndHorizontal();
					
					if(leaderboard.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(LeaderboardIdDLabel);
						leaderboard.Id	 	= EditorGUILayout.TextField(leaderboard.Id);
						if(leaderboard.Id.Length > 0) {
							leaderboard.Id 		= leaderboard.Id.Trim();
						}
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(LeaderboardNameLabel);
						leaderboard.Name	 	= EditorGUILayout.TextField(leaderboard.Name);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(LeaderboardDescriptionLabel);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						leaderboard.Description	 = EditorGUILayout.TextArea(leaderboard.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
						leaderboard.Texture = (Texture2D) EditorGUILayout.ObjectField("", leaderboard.Texture, typeof (Texture2D), false);
						EditorGUILayout.EndHorizontal();
					}
					
					EditorGUILayout.EndVertical();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
					GPLeaderBoard leaderboard =  new GPLeaderBoard(string.Empty, "New Leaderboard");
					AndroidNativeSettings.Instance.Leaderboards.Add(leaderboard);
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			
			EditorGUILayout.EndVertical();
		}
		EditorGUI.indentLevel--;
		
		EditorGUI.indentLevel++;
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			
			EditorGUILayout.BeginHorizontal();
			AndroidNativeSettings.Instance.ShowAchievements = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowAchievements, "Achievements");
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			if(AndroidNativeSettings.Instance.ShowAchievements) {
				
				foreach(GPAchievement achievement in AndroidNativeSettings.Instance.Achievements) {
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					
					GUIStyle s =  new GUIStyle();
					s.padding =  new RectOffset();
					s.margin =  new RectOffset();
					s.border =  new RectOffset();
					
					if(achievement.Texture != null) {
						GUILayout.Box(achievement.Texture, s, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
					}
					
					achievement.IsOpen 	= EditorGUILayout.Foldout(achievement.IsOpen, achievement.Name);
					
					bool ItemWasRemoved = DrawSortingButtons((object) achievement, AndroidNativeSettings.Instance.Achievements);
					if(ItemWasRemoved) {
						return;
					}
					
					EditorGUILayout.EndHorizontal();
					
					if(achievement.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(AchievementIdDLabel);
						achievement.Id	 	= EditorGUILayout.TextField(achievement.Id);
						if(achievement.Id.Length > 0) {
							achievement.Id 		= achievement.Id.Trim();
						}
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(AchievementNameLabel);
						achievement.Name	 	= EditorGUILayout.TextField(achievement.Name);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(AchievementDescriptionLabel);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						achievement.Description	 = EditorGUILayout.TextArea(achievement.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
						achievement.Texture = (Texture2D) EditorGUILayout.ObjectField("", achievement.Texture, typeof (Texture2D), false);
						EditorGUILayout.EndHorizontal();
					}
					
					EditorGUILayout.EndVertical();
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
					GPAchievement achievement =  new GPAchievement(string.Empty, "New Achievement");
					AndroidNativeSettings.Instance.Achievements.Add(achievement);
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			
			EditorGUILayout.EndVertical();
		}
		EditorGUI.indentLevel--;
		
		
		
		
		
		
		
		
		EditorGUILayout.LabelField("API:", EditorStyles.boldLabel);
		EditorGUI.indentLevel++;
		
		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(PlusApiLabel);
		settings.EnablePlusAPI	 	= EditorGUILayout.Toggle(settings.EnablePlusAPI);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(GamesApiLabel);
		settings.EnableGamesAPI	 	= EditorGUILayout.Toggle(settings.EnableGamesAPI);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(AppInviteAPILabel);
		settings.EnableAppInviteAPI	 	= EditorGUILayout.Toggle(settings.EnableAppInviteAPI);
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(DriveApiLabel);
		settings.EnableDriveAPI	 	= EditorGUILayout.Toggle(settings.EnableDriveAPI);
		EditorGUILayout.EndHorizontal();

		if (EditorGUI.EndChangeCheck()) {
			//Manipulate play-services-plus.aar
			if (AndroidNativeSettings.Instance.EnablePlusAPI)
				Instalation.EnableGooglePlusAPI ();
			else
				Instalation.DisableGooglePlusAPI ();

			//Manipulate play-services-appinvite.aar
			if (AndroidNativeSettings.Instance.EnableAppInviteAPI)
				Instalation.EnableAppInvitesAPI ();
			else
				Instalation.DisableAppInvitesAPI ();

			//Manipulate play-services-drive.aar
			if (AndroidNativeSettings.Instance.EnableDriveAPI)
				Instalation.EnableDriveAPI ();
			else
				Instalation.DisableDriveAPI ();
		}
		
		EditorGUI.indentLevel--;
		
		
		EditorGUILayout.LabelField("Auto Image Loading:", EditorStyles.boldLabel);
		
		EditorGUI.indentLevel++;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Profile Icons");
		settings.LoadProfileIcons	 	= EditorGUILayout.Toggle(settings.LoadProfileIcons);
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Profile Hi-res Images");
		settings.LoadProfileImages	 	= EditorGUILayout.Toggle(settings.LoadProfileImages);
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Event Icons");
		settings.LoadEventsIcons	 	= EditorGUILayout.Toggle(settings.LoadEventsIcons);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Quest Icons");
		settings.LoadQuestsIcons	 	= EditorGUILayout.Toggle(settings.LoadQuestsIcons);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Quest Banners");
		settings.LoadQuestsImages	 	= EditorGUILayout.Toggle(settings.LoadQuestsImages);
		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		
		EditorGUILayout.LabelField("Extras:", EditorStyles.boldLabel);
		
		EditorGUI.indentLevel++;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Show Connecting Popup");
		settings.ShowConnectingPopup	= EditorGUILayout.Toggle(settings.ShowConnectingPopup);
		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
		
		
		
		
		
	}
	
	GUIContent ProductIdDLabel 		= new GUIContent("ProductId[?]:", "A unique identifier that will be used for reporting. It can be composed of letters and numbers.");
	GUIContent IsConsLabel 			= new GUIContent("Is Consumable[?]:", "Is prodcut allowed to be purchased more than once?");
	GUIContent DisplayNameLabel  	= new GUIContent("Display Name[?]:", "This is the name of the In-App Purchase that will be seen by customers (if this is their primary language). For automatically renewable subscriptions, don’t include a duration in the display name. The display name can’t be longer than 75 characters.");
	GUIContent DescriptionLabel 	= new GUIContent("Description[?]:", "This is the description of the In-App Purchase that will be used by App Review during the review process. If indicated in your code, this description may also be seen by customers. For automatically renewable subscriptions, do not include a duration in the description. The description cannot be longer than 255 bytes.");
	
	private void BillingSettings() {
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Billing Settings", MessageType.None);
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(Base64KeyLabel);
		settings.base64EncodedPublicKey	 	= EditorGUILayout.TextField(settings.base64EncodedPublicKey);
		
		if(settings.base64EncodedPublicKey.ToString().Length > 0) {
			settings.base64EncodedPublicKey 	= settings.base64EncodedPublicKey.ToString().Trim();
		}
		
		EditorGUILayout.EndHorizontal();
		
		EditorGUI.indentLevel++;
		{
			EditorGUILayout.BeginVertical (GUI.skin.box);
			
			
			EditorGUILayout.BeginHorizontal();
			AndroidNativeSettings.Instance.ShowStoreProducts = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowStoreProducts, "Products");
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			if(AndroidNativeSettings.Instance.ShowStoreProducts) {
				
				foreach(GoogleProductTemplate product in AndroidNativeSettings.Instance.InAppProducts) {
					
					EditorGUILayout.BeginVertical (GUI.skin.box);
					
					EditorGUILayout.BeginHorizontal();
					
					GUIStyle s =  new GUIStyle();
					s.padding =  new RectOffset();
					s.margin =  new RectOffset();
					s.border =  new RectOffset();
					
					if(product.Texture != null) {
						GUILayout.Box(product.Texture, s, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)});
					}
					
					product.IsOpen 	= EditorGUILayout.Foldout(product.IsOpen, product.Title);
					
					
					EditorGUILayout.LabelField(product.Price + "$");
					bool ItemWasRemoved = DrawSortingButtons((object) product, AndroidNativeSettings.Instance.InAppProducts);
					if(ItemWasRemoved) {
						return;
					}
					
					EditorGUILayout.EndHorizontal();
					
					if(product.IsOpen) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(ProductIdDLabel);
						product.SKU	 	= EditorGUILayout.TextField(product.SKU);
						if(product.SKU.Length > 0) {
							product.SKU 		= product.SKU.Trim();
						}
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(DisplayNameLabel);
						product.Title	 	= EditorGUILayout.TextField(product.Title);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(IsConsLabel);
						product.ProductType	 	= (AN_InAppType) EditorGUILayout.EnumPopup(product.ProductType);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.Space();
						EditorGUILayout.Space();
						
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(DescriptionLabel);
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.BeginHorizontal();
						product.Description	 = EditorGUILayout.TextArea(product.Description,  new GUILayoutOption[]{GUILayout.Height(60), GUILayout.Width(200)} );
						product.Texture = (Texture2D) EditorGUILayout.ObjectField("", product.Texture, typeof (Texture2D), false);
						EditorGUILayout.EndHorizontal();
					}
					
					
					EditorGUILayout.EndVertical();
					
				}
				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if(GUILayout.Button("Add new", EditorStyles.miniButton, GUILayout.Width(250))) {
					GoogleProductTemplate product =  new GoogleProductTemplate();
					AndroidNativeSettings.Instance.InAppProducts.Add(product);
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
			}
			
			EditorGUILayout.EndVertical();
		}
		
		EditorGUI.indentLevel--;
		
	}
	
	private bool DrawSortingButtons(object currentObject, IList ObjectsList) {
		
		int ObjectIndex = ObjectsList.IndexOf(currentObject);
		if(ObjectIndex == 0) {
			GUI.enabled = false;
		} 
		
		bool up 		= GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
		if(up) {
			object c = currentObject;
			ObjectsList[ObjectIndex]  		= ObjectsList[ObjectIndex - 1];
			ObjectsList[ObjectIndex - 1] 	=  c;
		}
		
		
		if(ObjectIndex >= ObjectsList.Count -1) {
			GUI.enabled = false;
		} else {
			GUI.enabled = true;
		}
		
		bool down 		= GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
		if(down) {
			object c = currentObject;
			ObjectsList[ObjectIndex] =  ObjectsList[ObjectIndex + 1];
			ObjectsList[ObjectIndex + 1] = c;
		}
		
		
		GUI.enabled = true;
		bool r 			= GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
		if(r) {
			ObjectsList.Remove(currentObject);
		}
		
		return r;
	}
	
	private void SocialSettings () {
		SocialPlatfromSettingsHelper.FacebookSettings();
		if(!Instalation.IsFacebookInstalled) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			
			
			if(GUILayout.Button("Native Sharing",  GUILayout.Width(150))) {
				Application.OpenURL("https://goo.gl/5Hv5zD");
			}
			
			if(GUILayout.Button("Download FB SDK",  GUILayout.Width(150))) {
				Application.OpenURL("https://goo.gl/tDmNO3");
			}
			
			EditorGUILayout.EndHorizontal();
		}
		
		
		
		EditorGUILayout.Space ();
		SocialPlatfromSettingsHelper.TwitterSettings();
	}
	
	private void NotificationsSettings() {
		EditorGUILayout.Space ();
		
		EditorGUILayout.HelpBox("Local Notifications", MessageType.None);
		LocalNotificationParams();
		
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.HelpBox("Push Notifications", MessageType.None);
		PushNotificationParams();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Pop Ups", MessageType.None);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Android Dialog Theme");
        EditorGUILayout.Space();
        AndroidNativeSettings.Instance.DialogTheme = (AndroidDialogTheme)EditorGUILayout.EnumPopup(AndroidNativeSettings.Instance.DialogTheme);
        EditorGUILayout.EndHorizontal();
	}

	private static int[] rates = new int[]{0, 20, 50, 80, 100};
	private static string[] FillRateToolbarStrings = new string[] {"0%", "20%", "50%", "80%", "100%"};

	public static void EditorTestingParams() {

		SA.Common.Editor.Tools.BlockHeader("In-App Purchases");

		AndroidNativeSettings.Instance.Is_InApps_EditorTestingEnabled = SA.Common.Editor.Tools.ToggleFiled("Editor Testing", AndroidNativeSettings.Instance.Is_InApps_EditorTestingEnabled);
		GUI.enabled = AndroidNativeSettings.Instance.Is_InApps_EditorTestingEnabled;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Service Connection Rate:");
		AndroidNativeSettings.Instance.InApps_EditorFillRateIndex = GUILayout.Toolbar(AndroidNativeSettings.Instance.InApps_EditorFillRateIndex, FillRateToolbarStrings, EditorStyles.radioButton);
		AndroidNativeSettings.Instance.InApps_EditorFillRate = rates[AndroidNativeSettings.Instance.InApps_EditorFillRateIndex];
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("");
		EditorGUILayout.LabelField("0% - Always Error");
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("");
		EditorGUILayout.LabelField("100% - Always Provide Success Response");
		EditorGUILayout.EndHorizontal();


		GUI.enabled = true;

		SA.Common.Editor.Tools.BlockHeader("Ad-Mob Advertising");

	
		AndroidNativeSettings.Instance.Is_Ad_EditorTestingEnabled = SA.Common.Editor.Tools.ToggleFiled("Editor Testing", AndroidNativeSettings.Instance.Is_Ad_EditorTestingEnabled);
		
		GUI.enabled = AndroidNativeSettings.Instance.Is_Ad_EditorTestingEnabled;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Fill Rate:");
		AndroidNativeSettings.Instance.Ad_EditorFillRateIndex = GUILayout.Toolbar(AndroidNativeSettings.Instance.Ad_EditorFillRateIndex, FillRateToolbarStrings, EditorStyles.radioButton);
		AndroidNativeSettings.Instance.Ad_EditorFillRate = rates[AndroidNativeSettings.Instance.Ad_EditorFillRateIndex];
		EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("");
		EditorGUILayout.LabelField("0% - Always Error");
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("");
		EditorGUILayout.LabelField("100% - Always Provide Ad");
		EditorGUILayout.EndHorizontal();
		
		GUI.enabled = true;

		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Notifications", MessageType.None);
		AndroidNativeSettings.Instance.Is_Leaderboards_Editor_Notifications_Enabled = SA.Common.Editor.Tools.ToggleFiled("Leaderboards", AndroidNativeSettings.Instance.Is_Leaderboards_Editor_Notifications_Enabled);
		AndroidNativeSettings.Instance.Is_Achievements_Editor_Notifications_Enabled = SA.Common.Editor.Tools.ToggleFiled("Achievements", AndroidNativeSettings.Instance.Is_Achievements_Editor_Notifications_Enabled);
	}
	
	public static void ThirdPartyParams(bool showTitle = false) {

		if(showTitle) {
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Third-Party Plug-Ins Support Seettings", MessageType.None);
		}

		
		EditorGUI.BeginChangeCheck ();
		
		
		SA.Common.Editor.Tools.BlockHeader("Anti-Cheat Toolkit");

		EditorGUI.indentLevel++; {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Anti-Cheat Toolkit Support");
			AndroidNativeSettings.Instance.EnableATCSupport = EditorGUILayout.Toggle ("", AndroidNativeSettings.Instance.EnableATCSupport);
			
			
			EditorGUILayout.Space ();
			EditorGUILayout.EndHorizontal ();
			
			if(EditorGUI.EndChangeCheck()) {
				UpdatePluginDefines();
			}
			
			
			
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space ();
			if (GUILayout.Button("[?] Read More", GUILayout.Width(100.0f))) {
				Application.OpenURL("http://goo.gl/dokdpv");
			}
			
			EditorGUILayout.EndHorizontal ();
		} EditorGUI.indentLevel--;
		
		SA.Common.Editor.Tools.BlockHeader("One Signal Configuration");


		EditorGUI.indentLevel++; {
			
			EditorGUI.BeginChangeCheck(); 
			bool prevSoomlaState = AndroidNativeSettings.Instance.OneSignalEnabled;
			AndroidNativeSettings.Instance.OneSignalEnabled = ToggleFiled("Enable One Signal", AndroidNativeSettings.Instance.OneSignalEnabled);
			if(EditorGUI.EndChangeCheck())  {
				
				if(AndroidNativeSettings.Instance.OneSignalEnabled) {
					if(!(SA.Common.Util.Files.IsFolderExists("Plugins/OneSignal") || SA.Common.Util.Files.IsFolderExists("OneSignal"))) {
						bool res = EditorUtility.DisplayDialog("One Signal not found", "Android Native wasn't able to find One Signal libraryes in your project. Would you like to donwload and install it?", "Download", "No Thanks");
						if(res) {
							Application.OpenURL(AndroidNativeSettings.Instance.OneSignalDownloadLink);
						}
						AndroidNativeSettings.Instance.OneSignalEnabled = false;
					}
				}
				
				UpdateManifest();
				UpdatePluginDefines();
			}
			
			if(!prevSoomlaState && AndroidNativeSettings.Instance.OneSignalEnabled) {
				bool res = EditorUtility.DisplayDialog("One Signal", "Make sure you have read the Documentation before you proceed with the implementation", "Documentation", "Got it");
				if(res) {
					Application.OpenURL(AndroidNativeSettings.Instance.OneSignalDocLink);
				}
			}
			
			GUI.enabled = AndroidNativeSettings.Instance.OneSignalEnabled;
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("OneSignal App ID");
			AndroidNativeSettings.Instance.OneSignalAppID = EditorGUILayout.TextField(AndroidNativeSettings.Instance.OneSignalAppID);
			EditorGUILayout.EndHorizontal();
			
			
			
			GUI.enabled = true;
			
		} EditorGUI.indentLevel--;
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.Space ();
		if (GUILayout.Button("[?] Read More", GUILayout.Width(100.0f))) {
			Application.OpenURL(AndroidNativeSettings.Instance.OneSignalDocLink);
		}
		
		EditorGUILayout.EndHorizontal ();
		
		
		
		
		SA.Common.Editor.Tools.BlockHeader("Parse Configuration");
	
		EditorGUI.indentLevel++; {
			
			EditorGUI.BeginChangeCheck(); 
			bool prevSoomlaState = AndroidNativeSettings.Instance.UseParsePushNotifications;
			AndroidNativeSettings.Instance.UseParsePushNotifications = ToggleFiled("Enable Parse", AndroidNativeSettings.Instance.UseParsePushNotifications);
			if(EditorGUI.EndChangeCheck())  {				
				if(AndroidNativeSettings.Instance.UseParsePushNotifications) {
					if(!SA.Common.Util.Files.IsFolderExists("Parse")) {
						bool res = EditorUtility.DisplayDialog("Parse SDK not found", "Android Native wasn't able to find Parse SDK libraries in your project. Would you like to donwload and install it?", "Download", "No Thanks");
						if(res) {
							Application.OpenURL(AndroidNativeSettings.Instance.ParseDownloadLink);
						}
						AndroidNativeSettings.Instance.UseParsePushNotifications = false;
					}
				}
				
				UpdateManifest();
				UpdatePluginDefines();
			}
			
			if(!prevSoomlaState && AndroidNativeSettings.Instance.UseParsePushNotifications) {
				bool res = EditorUtility.DisplayDialog("Parse SDK", "Make sure you have read the Documentation before you proceed with the implementation", "Documentation", "Got it");
				if(res) {
					Application.OpenURL(AndroidNativeSettings.Instance.ParseDocLink);
				}
			}
			
			GUI.enabled = AndroidNativeSettings.Instance.UseParsePushNotifications;
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Parse Application ID");
			AndroidNativeSettings.Instance.ParseAppId = EditorGUILayout.TextField(AndroidNativeSettings.Instance.ParseAppId);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Parse .NET Key");
			AndroidNativeSettings.Instance.DotNetKey = EditorGUILayout.TextField(AndroidNativeSettings.Instance.DotNetKey);
			EditorGUILayout.EndHorizontal();
			
			
			
			GUI.enabled = true;
			
		} EditorGUI.indentLevel--;
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.Space ();
		if (GUILayout.Button("[?] Read More", GUILayout.Width(100.0f))) {
			Application.OpenURL(AndroidNativeSettings.Instance.ParseDocLink);
		}
		EditorGUILayout.EndHorizontal();
		
		
		
		SA.Common.Editor.Tools.BlockHeader("Soomla Configuration");

		EditorGUI.indentLevel++; {
			
			
			EditorGUI.BeginChangeCheck(); 
			bool prevSoomlaState = AndroidNativeSettings.Instance.EnableSoomla;
			AndroidNativeSettings.Instance.EnableSoomla = ToggleFiled("Enable GROW", AndroidNativeSettings.Instance.EnableSoomla);
			if(EditorGUI.EndChangeCheck())  {
				
				if(!AndroidNativeSettings.Instance.EnableSoomla) {
					Instalation.DisableSoomlaAPI();
				} else {
					
					if(SA.Common.Util.Files.IsFileExists("Plugins/IOS/libSoomlaGrowLite.a")) {
						Instalation.EnableSoomlaAPI();
					} else {
						
						
						bool res = EditorUtility.DisplayDialog("Soomla Grow not found", "Android Native wasn't able to find Soomla Grow libraryes in your project. Would you like to donwload and install it?", "Download", "No Thanks");
						if(res) {
							Application.OpenURL(AndroidNativeSettings.Instance.SoomlaDownloadLink);
						}
						
						AndroidNativeSettings.Instance.EnableSoomla = false;
					}
				}
			}
			
			if(!prevSoomlaState && AndroidNativeSettings.Instance.EnableSoomla) {
				bool res = EditorUtility.DisplayDialog("Soomla Grow", "Make sure you initialize SoomlaGrow when your games starts: \nAN_SoomlaGrow.Init();", "Documentation", "Got it");
				if(res) {
					Application.OpenURL(AndroidNativeSettings.Instance.SoomlaDocsLink);
				}
			}
			
			
			
			
			GUI.enabled = AndroidNativeSettings.Instance.EnableSoomla;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Game Key");
			AndroidNativeSettings.Instance.SoomlaGameKey =  EditorGUILayout.TextField(AndroidNativeSettings.Instance.SoomlaGameKey);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Env Key");
			AndroidNativeSettings.Instance.SoomlaEnvKey =  EditorGUILayout.TextField(AndroidNativeSettings.Instance.SoomlaEnvKey);
			EditorGUILayout.EndHorizontal();
			GUI.enabled = true;
			
		}EditorGUI.indentLevel--;
		
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.Space ();
		if (GUILayout.Button("[?] Read More", GUILayout.Width(100.0f))) {
			Application.OpenURL(AndroidNativeSettings.Instance.SoomlaDocsLink);
		}
		
		EditorGUILayout.EndHorizontal ();
		
	}
	
	public static void LocalNotificationParams() {
		EditorGUI.BeginChangeCheck ();
		
		AndroidNativeSettings.Instance.ShowWhenAppIsForeground = ToggleFiled("Show in foreground", AndroidNativeSettings.Instance.ShowWhenAppIsForeground);
		AndroidNativeSettings.Instance.EnableVibrationLocal = SA.Common.Editor.Tools.ToggleFiled("Vibration", AndroidNativeSettings.Instance.EnableVibrationLocal);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Wake Lock Timeout [milliseconds]");
		AndroidNativeSettings.Instance.LocalNotificationWakeLockTimer = EditorGUILayout.IntField(AndroidNativeSettings.Instance.LocalNotificationWakeLockTimer);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Large Icon");
		
		Texture2D icon = (Texture2D)EditorGUILayout.ObjectField (AndroidNativeSettings.Instance.LocalNotificationLargeIcon, typeof(Texture2D), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.LocalNotificationLargeIcon != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.LocalNotificationLargeIcon);
				if (AndroidNativeSettings.Instance.PushNotificationLargeIcon != null) {
					if (!AndroidNativeSettings.Instance.PushNotificationLargeIcon.name.Equals(AndroidNativeSettings.Instance.LocalNotificationLargeIcon.name)) {
						SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/drawable/" + AndroidNativeSettings.Instance.LocalNotificationLargeIcon.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/drawable/" + AndroidNativeSettings.Instance.LocalNotificationLargeIcon.name.ToLower() + Path.GetExtension(path));
				}
			}
			
			if (icon != null) {
				string path = AssetDatabase.GetAssetPath(icon);
				SA.Common.Util.Files.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/AN_Res/res/drawable/" + icon.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.LocalNotificationLargeIcon = icon;
			if (AndroidNativeSettings.Instance.LocalNotificationSmallIcon == null) {
				AndroidNativeSettings.Instance.LocalNotificationSmallIcon = AndroidNativeSettings.Instance.LocalNotificationLargeIcon;
			}
		}
		
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Small Icon");
		EditorGUI.BeginChangeCheck ();
		Texture2D sIcon = (Texture2D)EditorGUILayout.ObjectField (AndroidNativeSettings.Instance.LocalNotificationSmallIcon, typeof(Texture2D), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.LocalNotificationSmallIcon != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.LocalNotificationSmallIcon);
				if (AndroidNativeSettings.Instance.LocalNotificationSmallIcon != null) {
					if (!AndroidNativeSettings.Instance.LocalNotificationSmallIcon.name.Equals(AndroidNativeSettings.Instance.LocalNotificationSmallIcon.name)) {
						SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/drawable/" + AndroidNativeSettings.Instance.LocalNotificationSmallIcon.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/drawable/" + AndroidNativeSettings.Instance.LocalNotificationLargeIcon.name.ToLower() + Path.GetExtension(path));
				}
			}
			
			if (sIcon != null) {
				string path = AssetDatabase.GetAssetPath(sIcon);
				SA.Common.Util.Files.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/AN_Res/res/drawable/" + sIcon.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.LocalNotificationSmallIcon = sIcon;
		}
		
		EditorGUILayout.EndHorizontal();
		
		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Notification Sound");
		AudioClip sound = (AudioClip)EditorGUILayout.ObjectField (AndroidNativeSettings.Instance.LocalNotificationSound, typeof(AudioClip), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.LocalNotificationSound != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.LocalNotificationSound);
				if (AndroidNativeSettings.Instance.PushNotificationSound != null) {
					if (!AndroidNativeSettings.Instance.PushNotificationSound.name.Equals(AndroidNativeSettings.Instance.LocalNotificationSound.name)) {
						SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/raw/" + AndroidNativeSettings.Instance.LocalNotificationSound.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/raw/" + AndroidNativeSettings.Instance.LocalNotificationSound.name.ToLower() + Path.GetExtension(path));
				}
			}
			
			if (sound != null) {
				string path = AssetDatabase.GetAssetPath(sound);
				SA.Common.Util.Files.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/AN_Res/res/raw/" + sound.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.LocalNotificationSound = sound;
		}
		
		EditorGUILayout.EndHorizontal();
	}
	
	public static void PushNotificationParams() {
		EditorGUI.BeginChangeCheck ();
		
		AndroidNativeSettings.Instance.ShowPushWhenAppIsForeground = ToggleFiled("Show in foreground", AndroidNativeSettings.Instance.ShowPushWhenAppIsForeground);
		AndroidNativeSettings.Instance.ReplaceOldNotificationWithNew = ToggleFiled("Replace old notification with new one", AndroidNativeSettings.Instance.ReplaceOldNotificationWithNew);
		AndroidNativeSettings.Instance.EnableVibrationPush =  SA.Common.Editor.Tools.ToggleFiled("Vibration", AndroidNativeSettings.Instance.EnableVibrationPush);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Color");
		AndroidNativeSettings.Instance.PushNotificationColor = EditorGUILayout.ColorField(AndroidNativeSettings.Instance.PushNotificationColor);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Large Icon");
		
		Texture2D icon = (Texture2D)EditorGUILayout.ObjectField (AndroidNativeSettings.Instance.PushNotificationLargeIcon, typeof(Texture2D), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.PushNotificationLargeIcon != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.PushNotificationLargeIcon);
				if (AndroidNativeSettings.Instance.LocalNotificationLargeIcon != null) {
					if (!AndroidNativeSettings.Instance.PushNotificationLargeIcon.name.Equals(AndroidNativeSettings.Instance.LocalNotificationLargeIcon.name)) {
						SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/drawable/" + AndroidNativeSettings.Instance.PushNotificationLargeIcon.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/drawable/" + AndroidNativeSettings.Instance.PushNotificationLargeIcon.name.ToLower() + Path.GetExtension(path));
				}
			}
			
			if (icon != null) {
				string path = AssetDatabase.GetAssetPath(icon);
				SA.Common.Util.Files.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/AN_Res/res/drawable/" + icon.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.PushNotificationLargeIcon = icon;
			if (AndroidNativeSettings.Instance.PushNotificationSmallIcon == null) {
				AndroidNativeSettings.Instance.PushNotificationSmallIcon = AndroidNativeSettings.Instance.PushNotificationLargeIcon;
			}
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Small Icon");
		EditorGUI.BeginChangeCheck ();
		Texture2D sIcon = (Texture2D)EditorGUILayout.ObjectField (AndroidNativeSettings.Instance.PushNotificationSmallIcon, typeof(Texture2D), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.PushNotificationSmallIcon != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.PushNotificationSmallIcon);
				if (AndroidNativeSettings.Instance.PushNotificationSmallIcon != null) {
					if (!AndroidNativeSettings.Instance.PushNotificationSmallIcon.name.Equals(AndroidNativeSettings.Instance.PushNotificationSmallIcon.name)) {
						SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/drawable/" + AndroidNativeSettings.Instance.PushNotificationSmallIcon.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/drawable/" + AndroidNativeSettings.Instance.PushNotificationSmallIcon.name.ToLower() + Path.GetExtension(path));
				}
			}
			
			if (sIcon != null) {
				string path = AssetDatabase.GetAssetPath(sIcon);
				SA.Common.Util.Files.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/AN_Res/res/drawable/" + sIcon.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.PushNotificationSmallIcon = sIcon;
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Notification Sound");
		AudioClip sound = (AudioClip)EditorGUILayout.ObjectField (AndroidNativeSettings.Instance.PushNotificationSound, typeof(AudioClip), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (AndroidNativeSettings.Instance.PushNotificationSound != null) {
				string path = AssetDatabase.GetAssetPath(AndroidNativeSettings.Instance.PushNotificationSound);
				if (AndroidNativeSettings.Instance.LocalNotificationSound != null) {
					if (!AndroidNativeSettings.Instance.PushNotificationSound.name.Equals(AndroidNativeSettings.Instance.LocalNotificationSound.name)) {
						SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/raw/" + AndroidNativeSettings.Instance.PushNotificationSound.name.ToLower() + Path.GetExtension(path));
					}
				} else {
					SA.Common.Util.Files.DeleteFile("Plugins/Android/AN_Res/res/raw/" + AndroidNativeSettings.Instance.PushNotificationSound.name.ToLower() + Path.GetExtension(path));
				}
			}
			
			if (sound != null) {
				string path = AssetDatabase.GetAssetPath(sound);
				SA.Common.Util.Files.CopyFile(path.Substring(path.IndexOf("/"), path.Length - path.IndexOf("/")),
				                       "Plugins/Android/AN_Res/res/raw/" + sound.name.ToLower() + Path.GetExtension(path));
			}
			AndroidNativeSettings.Instance.PushNotificationSound = sound;
		}
		EditorGUILayout.EndHorizontal();
		
		
		if(AndroidNativeSettings.Instance.PushService == AN_PushNotificationService.Google) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Sender Id");
			AndroidNativeSettings.Instance.GCM_SenderId	 	= EditorGUILayout.TextField(AndroidNativeSettings.Instance.GCM_SenderId);
			if(AndroidNativeSettings.Instance.GCM_SenderId.Length > 0) {
				AndroidNativeSettings.Instance.GCM_SenderId		= AndroidNativeSettings.Instance.GCM_SenderId.Trim();
			}
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Push Service");
		AndroidNativeSettings.Instance.PushService = (AN_PushNotificationService) EditorGUILayout.EnumPopup(AndroidNativeSettings.Instance.PushService);
		EditorGUILayout.EndHorizontal();
		if (EditorGUI.EndChangeCheck()) {
			UpdateManifest();
		}
		
		switch(AndroidNativeSettings.Instance.PushService) {
		case AN_PushNotificationService.OneSignal:
			if(!AndroidNativeSettings.Instance.OneSignalEnabled) {
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("One Signal SDK isn't Configured", MessageType.Error);
				
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.Space ();
				if (GUILayout.Button("One Signal SDK Settings", GUILayout.Width(150.0f))) {
					AndroidNativeSettings.Instance.ToolbarSelectedIndex = 6;
				}
				
				EditorGUILayout.EndHorizontal ();
			}
			break;
			
		case AN_PushNotificationService.Parse:
			if(!AndroidNativeSettings.Instance.UseParsePushNotifications) {
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Parse SDK isn't Configured", MessageType.Error);
				
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.Space ();
				if (GUILayout.Button("Parse SDK Settings", GUILayout.Width(150.0f))) {
					AndroidNativeSettings.Instance.ToolbarSelectedIndex = 6;
				}
				
				EditorGUILayout.EndHorizontal ();
			}
			break;
		}
		
		
		
	}
	
	public static void CameraAndGalleryParams(bool showTitle = true) {

		if(showTitle) {
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Camera and Gallery", MessageType.None);
		}

		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Camera Capture Mode");
		AndroidNativeSettings.Instance.CameraCaptureMode	 	= (AN_CameraCaptureType) EditorGUILayout.EnumPopup(AndroidNativeSettings.Instance.CameraCaptureMode);
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Max Loaded Image Size");
		AndroidNativeSettings.Instance.MaxImageLoadSize	 	= EditorGUILayout.IntField(AndroidNativeSettings.Instance.MaxImageLoadSize);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Image Format");
		AndroidNativeSettings.Instance.ImageFormat = (AndroidCameraImageFormat) EditorGUILayout.EnumPopup(AndroidNativeSettings.Instance.ImageFormat);
		EditorGUILayout.EndHorizontal();
		
		GUI.enabled = !AndroidNativeSettings.Instance.UseProductNameAsFolderName;
		if(AndroidNativeSettings.Instance.UseProductNameAsFolderName) {
			if(PlayerSettings.productName.Length > 0) {
				AndroidNativeSettings.Instance.GalleryFolderName = PlayerSettings.productName.Trim();
			}
			
			
		}
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("App Gallery Folder");
		AndroidNativeSettings.Instance.GalleryFolderName	 	= EditorGUILayout.TextField(AndroidNativeSettings.Instance.GalleryFolderName);
		if(AndroidNativeSettings.Instance.GalleryFolderName.Length > 0) {
			AndroidNativeSettings.Instance.GalleryFolderName		= AndroidNativeSettings.Instance.GalleryFolderName.Trim();
			AndroidNativeSettings.Instance.GalleryFolderName		= AndroidNativeSettings.Instance.GalleryFolderName.Trim('/');
		}
		
		EditorGUILayout.EndHorizontal();
		
		GUI.enabled = true;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Use Product Name As Folder Name");
		AndroidNativeSettings.Instance.UseProductNameAsFolderName	 	= EditorGUILayout.Toggle(AndroidNativeSettings.Instance.UseProductNameAsFolderName);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Save Camera Image To Gallery");
		AndroidNativeSettings.Instance.SaveCameraImageToGallery	 	= EditorGUILayout.Toggle(AndroidNativeSettings.Instance.SaveCameraImageToGallery);
		EditorGUILayout.EndHorizontal();
	}
	
	
	
	
	
	
	private void AboutGUI() {
		
		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		
		SA.Common.Editor.Tools.SelectableLabelField(SdkVersion,   AndroidNativeSettings.VERSION_NUMBER);
		SA.Common.Editor.Tools.SelectableLabelField(GPSdkVersion, AndroidNativeSettings.GOOGLE_PLAY_SDK_VERSION_NUMBER);
		
		
		SA.Common.Editor.Tools.FBSdkVersionLabel();
		SA.Common.Editor.Tools.SupportMail();
		
		SA.Common.Editor.Tools.DrawSALogo();
	}
	
	
	private void ApplaySettings() {
		if(AndroidNativeSettings.Instance.UseProductNameAsFolderName) {
			AndroidNativeSettings.Instance.GalleryFolderName = PlayerSettings.productName;
		}
	}
	
	public static void AN_Plugin_Update() {
		Instalation.Android_UpdatePlugin();
		AndroidNativeSettingsEditor.UpdateAPIsInstalation();
	}
	
	private static bool ToggleFiled(string titile, bool value) {
		
		AN_Bool initialValue = AN_Bool.Yes;
		if(!value) {
			initialValue = AN_Bool.No;
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(titile);
		
		initialValue = (AN_Bool) EditorGUILayout.EnumPopup(initialValue);
		if(initialValue == AN_Bool.Yes) {
			value = true;
		} else {
			value = false;
		}
		EditorGUILayout.EndHorizontal();
		
		return value;
	}
	
	private static void DirtyEditor() {
		#if UNITY_EDITOR
		EditorUtility.SetDirty(SocialPlatfromSettings.Instance);
		EditorUtility.SetDirty(AndroidNativeSettings.Instance);
		#endif
	}
	
	
}
#endif
