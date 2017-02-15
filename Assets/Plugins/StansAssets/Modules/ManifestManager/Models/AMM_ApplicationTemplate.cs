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

	public class ApplicationTemplate : BaseTemplate {
		private Dictionary<int, ActivityTemplate> _activities = null;

		public ApplicationTemplate() : base(){
			_activities = new Dictionary<int, ActivityTemplate> ();
		}
		
		public void AddActivity(ActivityTemplate activity) {
			_activities.Add (activity.Id, activity);	
		}

		public void RemoveActivity(ActivityTemplate activity) {
			_activities.Remove (activity.Id);
		}


		public ActivityTemplate GetOrCreateActivityWithName(string name) {
			ActivityTemplate activity = GetActivityWithName(name);
			if(activity == null) {
				activity =  new ActivityTemplate(false, name);
				AddActivity(activity);
			}

			return activity;

		}

		public ActivityTemplate GetActivityWithName(string name)  {


			foreach(KeyValuePair<int, ActivityTemplate> entry in Activities) {
				if(entry.Value.Name.Equals(name)) {
					return entry.Value;
				}
			}

			return null;
		}

		public ActivityTemplate GetLauncherActivity() {
			foreach(KeyValuePair<int, ActivityTemplate> entry in Activities) {
				if(entry.Value.IsLauncher) {
					return entry.Value;
				}
			} 
			
			return null;
		}

		public override void ToXmlElement (XmlDocument doc, XmlElement parent)
		{
			AddAttributesToXml (doc, parent, this);
			AddPropertiesToXml (doc, parent, this);

			foreach (int id in _activities.Keys) {
				XmlElement activity = doc.CreateElement ("activity");
				_activities[id].ToXmlElement(doc, activity);
				parent.AppendChild (activity);
			}
		}

		public Dictionary<int, ActivityTemplate> Activities {
			get {
				return _activities;
			}
		}



	}

}
