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

	public class Template : BaseTemplate {
		private ApplicationTemplate _applicationTemplate = null;
		private List<PropertyTemplate> _permissions = null; 

		public Template() : base() {
			_applicationTemplate = new ApplicationTemplate ();
			_permissions = new List<PropertyTemplate> ();
		}

		public bool HasPermission(string name) {

			foreach(PropertyTemplate permission in Permissions) {
				if(permission.Name.Equals(name)) {
					return true;
				}
			}

			return false;
		}


		public void RemovePermission(string name) {
			while(HasPermission(name)) {
				foreach(PropertyTemplate permission in Permissions) {
					if(permission.Name.Equals(name)) {
						RemovePermission(permission);
						break;
					}
				}
			}
		}

		public void RemovePermission(PropertyTemplate permission) {
			_permissions.Remove (permission);
		}


		public void AddPermission(string name) {
			if(!HasPermission(name)) {
				PropertyTemplate uses_permission = new PropertyTemplate("uses-permission");
				uses_permission.Name = name;
				AddPermission(uses_permission);
			}
		}
		

		public void AddPermission(PropertyTemplate permission) {
			_permissions.Add (permission);
		}
		



		public override void ToXmlElement (XmlDocument doc, XmlElement parent) {
			AddAttributesToXml (doc, parent, this);
			AddPropertiesToXml (doc, parent, this);

			XmlElement app = doc.CreateElement ("application");
			_applicationTemplate.ToXmlElement (doc, app);
			parent.AppendChild (app);

			foreach (PropertyTemplate permission in Permissions) {
				XmlElement p = doc.CreateElement("uses-permission");
				permission.ToXmlElement(doc, p);
				parent.AppendChild(p);
			}
		}

		public ApplicationTemplate ApplicationTemplate {
			get {
				return _applicationTemplate;
			}
		}

		public List<PropertyTemplate> Permissions {
			get {
				return _permissions;
			}
		}
	}
}
