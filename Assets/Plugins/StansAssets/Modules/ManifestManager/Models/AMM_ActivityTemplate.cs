////////////////////////////////////////////////////////////////////////////////
//  
// @module Manifest Manager
// @author Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Xml;
using System.Collections.Generic;


namespace SA.Manifest {

	public class ActivityTemplate : BaseTemplate {
		public bool IsOpen = false;

		private int _id = 0;
		private bool _isLauncher = false;
		private string _name = string.Empty;

		public ActivityTemplate(bool isLauncher, string name) : base() {
			_isLauncher = isLauncher;
			_name = name;
			_id = GetHashCode ();

			_values = new Dictionary<string, string> ();
			_properties = new Dictionary<string, List<PropertyTemplate>> ();
			SetValue("android:name", name);
		}

		public void SetName(string name) {
			_name = name;
			SetValue ("android:name", name);
		}

		public void SetAsLauncher(bool isLauncher) {
			_isLauncher = isLauncher;
		}

		public static PropertyTemplate GetLauncherPropertyTemplate() {
			PropertyTemplate launcher = new PropertyTemplate ("intent-filter");

			PropertyTemplate prop = new PropertyTemplate ("action");
			prop.SetValue ("android:name", "android.intent.action.MAIN");
			launcher.AddProperty ("action", prop);

			prop = new PropertyTemplate ("category");
			prop.SetValue ("android:name", "android.intent.category.LAUNCHER");
			launcher.AddProperty ("category", prop);

			return launcher;
		}

		public bool IsLauncherProperty(PropertyTemplate property) {
			if(property.Tag.Equals("intent-filter")) {
				foreach (PropertyTemplate p in property.Properties["category"]) {
					if (p.Values.ContainsKey("android:name")) {
						if (p.Values["android:name"].Equals("android.intent.category.LAUNCHER")) {
							return true;
						}
					}
				}
			}

			return false;
		}

		public override void ToXmlElement (XmlDocument doc, XmlElement parent)
		{
			AddAttributesToXml (doc, parent, this);

			PropertyTemplate launcher = null;
			if (_isLauncher) {
				launcher = GetLauncherPropertyTemplate();
				AddProperty(launcher.Tag, launcher);
			}
			AddPropertiesToXml (doc, parent, this);
			if (_isLauncher) {
				_properties["intent-filter"].Remove(launcher);
			}
		}

		public bool IsLauncher {
			get {
				return _isLauncher;
			}
		}

		public string Name {
			get {
				return _name;
			}
		}

		public int Id {
			get {
				return _id;
			}
		}
	}
}
