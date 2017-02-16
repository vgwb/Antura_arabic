#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using SA.Common.Editor;


public class SocialPlatfromSettingsHelper  {


	static GUIContent TConsumerKey   = new GUIContent("API Key [?]:", "Twitter register app consumer key");
	static GUIContent TConsumerSecret   = new GUIContent("API Secret [?]:", "Twitter register app consumer secret");
	
	static GUIContent TAccessToken   = new GUIContent("Access Token [?]:", "Twitter Access Token for editor testing only");
	static GUIContent TAccessTokenSecret   = new GUIContent("Access Token Secret [?]:", "Twitter Access Token Secret for editor testing only");





	private static string newPermition = "";
	public static void FacebookSettings() {
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Facebook Settings", MessageType.None);


		
		if(!Instalation.IsFacebookInstalled) {
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Facebook SDK is not found in your project. However you can steel use the device native sharing API.", MessageType.Info);
			

			return;
		}

		string FBAppId = "0";
		string FBAppName = "App Name";
		object instance = null;

		System.Reflection.MethodInfo idSetter = null;
		System.Reflection.MethodInfo labelSetter = null;

		List<string> ids = null;
		List<string> labels = null;

		try {
			ScriptableObject FB_Resourse = 	Resources.Load("FacebookSettings") as ScriptableObject;
			
			if(FB_Resourse != null) {
				Type fb_settings = FB_Resourse.GetType();

				if (SA.Common.Config.FB_SDK_MajorVersionCode == 6) {

					System.Reflection.PropertyInfo propert  = fb_settings.GetProperty("AppId", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
					FBAppId = (string) propert.GetValue(null, null);
					
					System.Reflection.PropertyInfo Instance  = fb_settings.GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
					instance = Instance.GetValue(null, null);

					System.Reflection.PropertyInfo appLabelsProperty = fb_settings.GetProperty("AppLabels", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
					string[] appLabels = (string[])appLabelsProperty.GetValue(instance, null);
					
					if (appLabels.Length >= 1) {
						FBAppName = appLabels[0];
					}
					
					idSetter = fb_settings.GetMethod("SetAppId", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
					labelSetter = fb_settings.GetMethod("SetAppLabel", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

				} else if (SA.Common.Config.FB_SDK_MajorVersionCode == 7) {

					System.Reflection.PropertyInfo idsProperty = fb_settings.GetProperty("AppIds", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
					ids = (List<string>)idsProperty.GetValue(null, null);
					if (ids.Count >= 1) {
						FBAppId = ids[0];
					}

					System.Reflection.PropertyInfo labelsProperty = fb_settings.GetProperty("AppLabels", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
					labels = (List<string>)labelsProperty.GetValue(null, null);
					if (labels.Count >= 1) {
						FBAppName = labels[0];
					}
				}
			}			
		} catch(Exception ex) {
			Debug.LogError("AndroidNative. FBSettings.AppId reflection failed: " + ex.Message);
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("App Name:");
		EditorGUI.BeginChangeCheck();
		string newName = EditorGUILayout.TextField(FBAppName);
		EditorGUILayout.EndHorizontal();

		if (EditorGUI.EndChangeCheck()) {
			if (SA.Common.Config.FB_SDK_MajorVersionCode == 6) {
				if (labelSetter != null && instance != null) {
					labelSetter.Invoke(instance, new object[] { 0, newName });
				}
			} else if (SA.Common.Config.FB_SDK_MajorVersionCode == 7) {
				if (labels.Count >= 1) {
					labels[0] = newName;
				}
			}
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("App ID:");
		EditorGUI.BeginChangeCheck();
		string newId = EditorGUILayout.TextField(FBAppId);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		if (EditorGUI.EndChangeCheck()) {
			if (SA.Common.Config.FB_SDK_MajorVersionCode == 6) {
				if (idSetter != null && instance != null) {
					idSetter.Invoke(instance, new object[] { 0, newId });
				}
			} else if (SA.Common.Config.FB_SDK_MajorVersionCode == 7) {
				if (ids.Count >= 1) {
					ids[0] = newId;
				}
			}
		}
		
		if (SocialPlatfromSettings.Instance.fb_scopes_list.Count == 0) {
			SocialPlatfromSettings.Instance.AddDefaultScopes();
		}
		
		SocialPlatfromSettings.Instance.showPermitions = EditorGUILayout.Foldout(SocialPlatfromSettings.Instance.showPermitions, "Facebook Permissions");
		if(SocialPlatfromSettings.Instance.showPermitions) {
			foreach(string s in SocialPlatfromSettings.Instance.fb_scopes_list) {
				EditorGUILayout.BeginVertical (GUI.skin.box);
				EditorGUILayout.BeginHorizontal();
				
				EditorGUILayout.SelectableLabel(s, GUILayout.Height(16));
				
				if(GUILayout.Button("x",  GUILayout.Width(20))) {
					SocialPlatfromSettings.Instance.fb_scopes_list.Remove(s);
					return;
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.EndVertical();
			}
			
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Add new permition: ");
			newPermition = EditorGUILayout.TextField(newPermition);
			
			
			EditorGUILayout.EndHorizontal();
			
			
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.Space();
			if(GUILayout.Button("Documentation",  GUILayout.Width(100))) {
				Application.OpenURL("https://developers.facebook.com/docs/facebook-login/permissions/v2.0");
			}
			
			
			
			if(GUILayout.Button("Add",  GUILayout.Width(100))) {
				
				if(newPermition != string.Empty) {
					newPermition = newPermition.Trim();
					if(!SocialPlatfromSettings.Instance.fb_scopes_list.Contains(newPermition)) {
						SocialPlatfromSettings.Instance.fb_scopes_list.Add(newPermition);
					}
					
					newPermition = string.Empty;
				}
			}
			EditorGUILayout.EndHorizontal();
			
		}
		
	}
	
	public static void TwitterSettings() {
		EditorGUILayout.HelpBox("Twitter Settings", MessageType.None);
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(TConsumerKey);
		SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY	 	= EditorGUILayout.TextField(SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY);
		SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY 		= SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY.Trim();
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(TConsumerSecret);
		SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET	 	= EditorGUILayout.TextField(SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET);
		SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET	 	= SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET.Trim();
		EditorGUILayout.EndHorizontal();
		
		EditorGUI.indentLevel++;
		SocialPlatfromSettings.Instance.ShowEditorOauthTestingBlock = EditorGUILayout.Foldout(SocialPlatfromSettings.Instance.ShowEditorOauthTestingBlock, "OAuth Testing In Editor");
		if(SocialPlatfromSettings.Instance.ShowEditorOauthTestingBlock) {
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(TAccessToken);
			SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN	 	= EditorGUILayout.TextField(SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN);
			SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN 		= SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN.Trim();
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(TAccessTokenSecret);
			SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN_SECRET	 	= EditorGUILayout.TextField(SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN_SECRET);
			SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN_SECRET	 	= SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN_SECRET.Trim();
			EditorGUILayout.EndHorizontal();
			
		}
		
		EditorGUI.indentLevel--;
	}

}
#endif
