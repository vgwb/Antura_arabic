#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;

public class IOSNativePostProcess  {

	#if UNITY_IPHONE
	[PostProcessBuild(50)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {


		if(IOSNativeSettings.Instance.EnableInAppsAPI) {

			string StoreKit = "StoreKit.framework";



			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(StoreKit)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = StoreKit;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}

		}

		if(IOSNativeSettings.Instance.EnableGameCenterAPI) {
			
			string GameKit = "GameKit.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(GameKit)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = GameKit;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}

			SA.IOSDeploy.Variable UIRequiredDeviceCapabilities =  new SA.IOSDeploy.Variable();
			UIRequiredDeviceCapabilities.Name = "UIRequiredDeviceCapabilities";
			UIRequiredDeviceCapabilities.Type = SA.IOSDeploy.PlistValueTypes.Array;

			SA.IOSDeploy.Variable gamekit =  new SA.IOSDeploy.Variable();
			gamekit.StringValue = "gamekit";
			gamekit.Type = SA.IOSDeploy.PlistValueTypes.String;
			UIRequiredDeviceCapabilities.AddChild(gamekit);


			SA.IOSDeploy.Variable armv7 =  new SA.IOSDeploy.Variable();
			armv7.StringValue = "armv7";
			armv7.Type = SA.IOSDeploy.PlistValueTypes.String;
			UIRequiredDeviceCapabilities.AddChild(armv7);


			SA.IOSDeploy.ISD_Settings.Instance.AddOrReplaceNewVariable(UIRequiredDeviceCapabilities);

		}

		if(IOSNativeSettings.Instance.UrlTypes.Count > 0) {
			SA.IOSDeploy.Variable CFBundleURLTypes =  new SA.IOSDeploy.Variable();
			CFBundleURLTypes.Name = "CFBundleURLTypes";
			CFBundleURLTypes.Type = SA.IOSDeploy.PlistValueTypes.Array;



			foreach(SA.IOSNative.Models.UrlType url in IOSNativeSettings.Instance.UrlTypes) {
				SA.IOSDeploy.Variable URLTypeHolder =  new SA.IOSDeploy.Variable();
				URLTypeHolder.Type = SA.IOSDeploy.PlistValueTypes.Dictionary;

				CFBundleURLTypes.AddChild (URLTypeHolder);


				SA.IOSDeploy.Variable CFBundleURLName =  new SA.IOSDeploy.Variable();
				CFBundleURLName.Type = SA.IOSDeploy.PlistValueTypes.String;
				CFBundleURLName.Name = "CFBundleURLName";
				CFBundleURLName.StringValue = url.Identifier;
				URLTypeHolder.AddChild (CFBundleURLName);


				SA.IOSDeploy.Variable CFBundleURLSchemes =  new SA.IOSDeploy.Variable();
				CFBundleURLSchemes.Type = SA.IOSDeploy.PlistValueTypes.Array;
				CFBundleURLSchemes.Name = "CFBundleURLSchemes";
				URLTypeHolder.AddChild (CFBundleURLSchemes);

				foreach(string scheme in url.Schemes) {
					SA.IOSDeploy.Variable Scheme =  new SA.IOSDeploy.Variable();
					Scheme.Type = SA.IOSDeploy.PlistValueTypes.String;
					Scheme.StringValue = scheme;

					CFBundleURLSchemes.AddChild (Scheme);
				}

			}


			foreach(SA.IOSDeploy.Variable v in  SA.IOSDeploy.ISD_Settings.Instance.PlistVariables) {
				if(v.Name.Equals(CFBundleURLTypes.Name)) {
					SA.IOSDeploy.ISD_Settings.Instance.PlistVariables.Remove (v);
					break;
				}
			}

			SA.IOSDeploy.ISD_Settings.Instance.PlistVariables.Add (CFBundleURLTypes);
		}




		if(IOSNativeSettings.Instance.EnableSocialSharingAPI) {
		
			string Accounts = "Accounts.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(Accounts)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = Accounts;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}

			
			
			string SocialF = "Social.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(SocialF)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = SocialF;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}
			
			string MessageUI = "MessageUI.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(MessageUI)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = MessageUI;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}

			string QueriesSchemesName = "LSApplicationQueriesSchemes";
			SA.IOSDeploy.Variable LSApplicationQueriesSchemes = SA.IOSDeploy.ISD_Settings.Instance.GetVariableByName (QueriesSchemesName);
			if(LSApplicationQueriesSchemes == null) {
				LSApplicationQueriesSchemes = new SA.IOSDeploy.Variable();
				LSApplicationQueriesSchemes.Name = QueriesSchemesName;
				LSApplicationQueriesSchemes.Type = SA.IOSDeploy.PlistValueTypes.Array;
			}	

			SA.IOSDeploy.Variable instagram =  new SA.IOSDeploy.Variable();
			instagram.StringValue = "instagram";
			instagram.Type = SA.IOSDeploy.PlistValueTypes.String;
			LSApplicationQueriesSchemes.AddChild(instagram);

			SA.IOSDeploy.Variable whatsapp =  new SA.IOSDeploy.Variable();
			whatsapp.StringValue = "whatsapp";
			whatsapp.Type = SA.IOSDeploy.PlistValueTypes.String;
			LSApplicationQueriesSchemes.AddChild(whatsapp);


			SA.IOSDeploy.ISD_Settings.Instance.PlistVariables.Remove(LSApplicationQueriesSchemes);
			SA.IOSDeploy.ISD_Settings.Instance.PlistVariables.Add(LSApplicationQueriesSchemes);

		}
			

		if(IOSNativeSettings.Instance.ApplicationQueriesSchemes.Count > 0) {
			string QueriesSchemesName = "LSApplicationQueriesSchemes";
			SA.IOSDeploy.Variable LSApplicationQueriesSchemes = SA.IOSDeploy.ISD_Settings.Instance.GetVariableByName (QueriesSchemesName);
			if(LSApplicationQueriesSchemes == null) {
				LSApplicationQueriesSchemes = new SA.IOSDeploy.Variable();
				LSApplicationQueriesSchemes.Name = QueriesSchemesName;
				LSApplicationQueriesSchemes.Type = SA.IOSDeploy.PlistValueTypes.Array;
			}	


			foreach(var scheme in IOSNativeSettings.Instance.ApplicationQueriesSchemes) {
				SA.IOSDeploy.Variable schemeName =  new SA.IOSDeploy.Variable();
				schemeName.StringValue = scheme.Identifier;
				schemeName.Type = SA.IOSDeploy.PlistValueTypes.String;
				LSApplicationQueriesSchemes.AddChild(schemeName);
			}

			SA.IOSDeploy.ISD_Settings.Instance.AddOrReplaceNewVariable(LSApplicationQueriesSchemes);
		}




		if(IOSNativeSettings.Instance.EnableMediaPlayerAPI) {
			string MediaPlayer = "MediaPlayer.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(MediaPlayer)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = MediaPlayer;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}
				

			var NSAppleMusicUsageDescription =  new SA.IOSDeploy.Variable();
			NSAppleMusicUsageDescription.Name = "NSAppleMusicUsageDescription";
			NSAppleMusicUsageDescription.StringValue = IOSNativeSettings.Instance.AppleMusicUsageDescription;
			NSAppleMusicUsageDescription.Type = SA.IOSDeploy.PlistValueTypes.String;


			SA.IOSDeploy.ISD_Settings.Instance.AddOrReplaceNewVariable(NSAppleMusicUsageDescription);

		}
	

		if(IOSNativeSettings.Instance.EnableCameraAPI) {
			string MobileCoreServices = "MobileCoreServices.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(MobileCoreServices)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = MobileCoreServices;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}



			var NSCameraUsageDescription =  new SA.IOSDeploy.Variable();
			NSCameraUsageDescription.Name = "NSCameraUsageDescription";
			NSCameraUsageDescription.StringValue = IOSNativeSettings.Instance.CameraUsageDescription;
			NSCameraUsageDescription.Type = SA.IOSDeploy.PlistValueTypes.String;

			SA.IOSDeploy.ISD_Settings.Instance.AddOrReplaceNewVariable(NSCameraUsageDescription);



			var NSPhotoLibraryUsageDescription =  new SA.IOSDeploy.Variable();
			NSPhotoLibraryUsageDescription.Name = "NSPhotoLibraryUsageDescription";
			NSPhotoLibraryUsageDescription.StringValue = IOSNativeSettings.Instance.PhotoLibraryUsageDescription;
			NSPhotoLibraryUsageDescription.Type = SA.IOSDeploy.PlistValueTypes.String;


			SA.IOSDeploy.ISD_Settings.Instance.AddOrReplaceNewVariable(NSPhotoLibraryUsageDescription);

		}

		if(IOSNativeSettings.Instance.EnableReplayKit) {
			Debug.Log ("Replay Kit enabled");

			string ReplayKit = "ReplayKit.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(ReplayKit)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = ReplayKit;
				F.IsOptional = true;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}
		}


		if(IOSNativeSettings.Instance.EnableCloudKit) {

			Debug.Log ("Cloud Kit enabled");

			string CloudKit = "CloudKit.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(CloudKit)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = CloudKit;
				F.IsOptional = true;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}


			string inheritedflag = "$(inherited)";

			SA.IOSDeploy.ISD_Settings.Instance.AddLinkerFlag (inheritedflag);

		}

		if(IOSNativeSettings.Instance.EnablePickerAPI) {
			string AssetsLibrary = "AssetsLibrary.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(AssetsLibrary)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = AssetsLibrary;
				F.IsOptional = true;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}
		}


		if(IOSNativeSettings.Instance.EnableContactsAPI) {
			string Contacts = "Contacts.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(Contacts)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = Contacts;
				F.IsOptional = true;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}


			string ContactsUI = "ContactsUI.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(ContactsUI)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = ContactsUI;
				F.IsOptional = true;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}


			var NSContactsUsageDescription =  new SA.IOSDeploy.Variable();
			NSContactsUsageDescription.Name = "NSContactsUsageDescription";
			NSContactsUsageDescription.StringValue = IOSNativeSettings.Instance.CameraUsageDescription;
			NSContactsUsageDescription.Type = SA.IOSDeploy.PlistValueTypes.String;


			SA.IOSDeploy.ISD_Settings.Instance.AddOrReplaceNewVariable(NSContactsUsageDescription);

		}

		if(IOSNativeSettings.Instance.EnableSoomla) {
			string AdSupport = "AdSupport.framework";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsFreamworkWithName(AdSupport)) {
				SA.IOSDeploy.Framework F =  new SA.IOSDeploy.Framework();
				F.Name = AdSupport;
				SA.IOSDeploy.ISD_Settings.Instance.Frameworks.Add(F);
			}

			string libsqlite3 = "libsqlite3.dylib";
			if(!SA.IOSDeploy.ISD_Settings.Instance.ContainsLibWithName(libsqlite3)) {
				SA.IOSDeploy.Lib L =  new SA.IOSDeploy.Lib();
				L.Name = libsqlite3;
				SA.IOSDeploy.ISD_Settings.Instance.Libraries.Add(L);
			}



			#if UNITY_5
				string soomlaLinkerFlag = "-force_load Libraries/Plugins/iOS/libSoomlaGrowLite.a";
			#else
				string soomlaLinkerFlag = "-force_load Libraries/libSoomlaGrowLite.a";
			#endif



			SA.IOSDeploy.ISD_Settings.Instance.AddLinkerFlag (soomlaLinkerFlag);
		}


		Debug.Log("ISN Postprocess Done");

	
	}
	#endif
}
#endif