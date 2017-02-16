////////////////////////////////////////////////////////////////////////////////
//  
// @module Manifest Manager
// @author Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.IO;

#if !(UNITY_WP8 || UNITY_METRO)
using System.Xml;
#endif

using System.Collections;
using System.Text.RegularExpressions;

#if UNITY_EDITOR


namespace SA.Manifest {


static public class Manager {
	
	private const string DEFAULT_MANIFEST_PATH 	= "/" + SA.Common.Config.MODULS_PATH + "ManifestManager/default.xml";
	public const string MANIFEST_FILE_PATH	 	= "/" + SA.Common.Config.ANDROID_DESTANATION_PATH + "AndroidManifest.xml";

	private static Template _Template = null;

	public static bool HasManifest {
		get {

			if (File.Exists (Application.dataPath + MANIFEST_FILE_PATH)) {
				return true;
			}
			
			return false;
		}
	}

	private static void ReadManifest(string manifestPath) {

#if !(UNITY_WP8 || UNITY_METRO)
		//Read XML file
		_Template = new Template ();
			
		XmlDocument doc = new XmlDocument ();
		doc.Load (Application.dataPath + manifestPath);
		XmlNode rootNode = doc.DocumentElement;
			
		foreach (XmlAttribute attr in rootNode.Attributes) {				
			_Template.SetValue(attr.Name, attr.Value);
		}
			
		foreach (XmlNode childNode in rootNode.ChildNodes) {
			if (!childNode.Name.Equals("application")
			    && !childNode.Name.Equals("uses-permission")
			    && !childNode.Name.Equals("#comment")) {
				_Template.AddProperty(childNode.Name, ParseProperty(childNode));
			}
		}
			
		XmlNode applicationNode = null;
		foreach (XmlNode childNode in rootNode.ChildNodes) {
			if (childNode.Name.Equals("application")) {
				applicationNode = childNode;
				break;
			}
		}
			
		foreach (XmlAttribute attr in applicationNode.Attributes) {
			_Template.ApplicationTemplate.SetValue(attr.Name, attr.Value);
		}
		foreach (XmlNode childNode in applicationNode.ChildNodes) {
			if(!childNode.Name.Equals("#comment")
			   && !childNode.Name.Equals("activity")) {
				_Template.ApplicationTemplate.AddProperty(childNode.Name, ParseProperty(childNode));
			}
		}
			
		foreach (XmlNode childNode in applicationNode.ChildNodes) {
			if(childNode.Name.Equals("activity")
			   && !childNode.Name.Equals("#comment")) {

				string activityName = "";
				if(childNode.Attributes["android:name"] != null) {
					activityName = childNode.Attributes["android:name"].Value;
				} else {
					Debug.LogWarning("Android Manifest contains activity tag without android:name attribute.");
				}
					
				XmlNode launcher = null;
				bool isLauncher = false;
				foreach (XmlNode actNode in childNode.ChildNodes) {
					if (actNode.Name.Equals("intent-filter")) {
						foreach (XmlNode intentNode in actNode.ChildNodes) {
							if (intentNode.Name.Equals("category")) {
								if (intentNode.Attributes["android:name"].Value.Equals("android.intent.category.LAUNCHER")) {
									isLauncher = true;
									launcher = actNode;
								}
							}
						}
					}
				}
					
				ActivityTemplate activity = new ActivityTemplate(isLauncher, activityName);
				foreach (XmlAttribute attr in childNode.Attributes) {
					activity.SetValue(attr.Name, attr.Value);
				}

				foreach (XmlNode actNode in childNode.ChildNodes) {
					if (!actNode.Name.Equals("#comment")) {
						if (actNode != launcher) {
							activity.AddProperty(actNode.Name, ParseProperty(actNode));
						}
					}
				}
					
				_Template.ApplicationTemplate.AddActivity(activity);
			}
		}
			
		//Load Manifest Permissions
		foreach (XmlNode node in rootNode.ChildNodes) {
			if (node.Name.Equals("uses-permission")) {
				PropertyTemplate permission = new PropertyTemplate("uses-permission");
				permission.SetValue("android:name", node.Attributes["android:name"].Value);
				_Template.AddPermission(permission);
			}
		}
#endif

	}

	public static void CreateDefaultManifest() {
		ReadManifest (DEFAULT_MANIFEST_PATH);
		SaveManifest ();
	}

	public static void SaveManifest() {
#if !(UNITY_WP8 || UNITY_METRO)
		SA.Common.Util.Files.CreateFolder (SA.Common.Config.ANDROID_DESTANATION_PATH);

		XmlDocument newDoc = new XmlDocument ();
		//Create XML header
		XmlNode docNode = newDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
		newDoc.AppendChild(docNode);

		XmlElement child = newDoc.CreateElement ("manifest");
		_Template.ToXmlElement (newDoc, child);
		newDoc.AppendChild (child);

		newDoc.Save (Application.dataPath  + MANIFEST_FILE_PATH);

		//Replace 'android___' pattern with 'android:'
		TextReader reader = new StreamReader (Application.dataPath + MANIFEST_FILE_PATH);
		string src = reader.ReadToEnd ();
		string pattern = @"android___";
		string replacement = "android:";
		Regex regex = new Regex (pattern);
		src = regex.Replace (src, replacement);
		reader.Close ();

		TextWriter writer = new StreamWriter(Application.dataPath + MANIFEST_FILE_PATH);
		writer.Write (src);
		writer.Close ();

		AssetDatabase.Refresh ();
#endif
	}

	public static Template GetManifest() {
		if (_Template == null) {
			ReadManifest(MANIFEST_FILE_PATH);
		}
		return _Template;
	}

	public static void Refresh() {
		if(HasManifest) {
			ReadManifest(MANIFEST_FILE_PATH);
		} else {
			CreateDefaultManifest();
		}

	} 

#if !(UNITY_WP8 || UNITY_METRO)
	private static PropertyTemplate ParseProperty(XmlNode node) {
		PropertyTemplate property = new PropertyTemplate (node.Name);
		//Get Values
		foreach (XmlAttribute attr in node.Attributes) {
			property.SetValue(attr.Name, attr.Value);
		}
		//Get Properties
		foreach (XmlNode n in node.ChildNodes) {
			if (!n.Name.Equals("#comment")) {
				property.AddProperty(n.Name, ParseProperty(n));
			}
		}

		return property;
	}
#endif
}


}

#endif
