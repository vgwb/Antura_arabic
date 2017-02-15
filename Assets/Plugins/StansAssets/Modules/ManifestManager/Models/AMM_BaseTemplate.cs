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

	public abstract class BaseTemplate {
		protected Dictionary<string, List<PropertyTemplate>> _properties = null;
		protected Dictionary<string, string> _values = null;



		public PropertyTemplate GetOrCreateIntentFilterWithName(string name) {
			PropertyTemplate filter = GetIntentFilterWithName(name);
			if(filter == null) {
				filter =  new PropertyTemplate("intent-filter");
				PropertyTemplate action = new PropertyTemplate("action");
				action.SetValue("android:name", name);
				filter.AddProperty(action);
				AddProperty(filter);
			}

			return filter;
		}


		public PropertyTemplate GetIntentFilterWithName(string name) {
			string tag = "intent-filter";
			List<PropertyTemplate> filters =  GetPropertiesWithTag(tag);
			foreach(PropertyTemplate intent_filter in filters) {
				string filter_name = GetIntentFilterName(intent_filter);
				if(filter_name.Equals(name)) {
					return intent_filter;
				}
			}

			return null;

		}


		public string GetIntentFilterName(PropertyTemplate intent) {

			List<PropertyTemplate> actions = intent.GetPropertiesWithTag("action");
			if(actions.Count > 0) {
				return actions[0].GetValue("android:name");
			} else {
				return string.Empty;
			}

		}

		public PropertyTemplate GetOrCreatePropertyWithName(string tag, string name) {
			PropertyTemplate p =  GetPropertyWithName(tag, name);
			if(p == null) {
				p = new PropertyTemplate(tag);
				p.SetValue("android:name", name);
				AddProperty(p);
			}

			return p;
		}


		public PropertyTemplate GetPropertyWithName(string tag, string name) {

			List<PropertyTemplate> tags = GetPropertiesWithTag(tag);
			foreach(PropertyTemplate prop in tags) {
				if(prop.Values.ContainsKey("android:name")) {
					if(prop.Values["android:name"] == name) {
						return prop;
					}
				}
			}


			return null;
		}


		public PropertyTemplate GetOrCreatePropertyWithTag(string tag) {
			PropertyTemplate p = GetPropertyWithTag(tag);
			if(p == null) {
				p = new PropertyTemplate(tag);
				AddProperty(p);
			}

			return p;
		}


		public PropertyTemplate GetPropertyWithTag(string tag) {
			List<PropertyTemplate> props = GetPropertiesWithTag(tag);
			if(props.Count > 0) {
				return props[0];
			} else {
				return null;
			}
		}


		public List<PropertyTemplate> GetPropertiesWithTag(string tag) {
			if(Properties.ContainsKey(tag)) {
				return Properties[tag];
			} else {
				return new List<PropertyTemplate>();
			}

		} 
		
		public abstract void ToXmlElement(XmlDocument doc, XmlElement parent);

		public BaseTemplate(){
			_values = new Dictionary<string, string> ();
			_properties = new Dictionary<string, List<PropertyTemplate>> ();
		}
		

		public void AddProperty(PropertyTemplate property) {
			AddProperty(property.Tag, property);
		}

		public void AddProperty(string tag, PropertyTemplate property) {
			if (!_properties.ContainsKey(tag)) {
				List<PropertyTemplate> list = new List<PropertyTemplate>();
				_properties.Add(tag, list);
			}
			_properties [tag].Add (property);
		}
		
		public void SetValue(string key, string value) {
			if(_values.ContainsKey(key)) {
				_values[key] = value;
			} else {
				_values.Add (key, value);
			}
		}

		public string GetValue(string key) {
			if(_values.ContainsKey(key)) {
				return _values[key];
			} else {
				return string.Empty;
			}
		}

		public void RemoveProperty(PropertyTemplate property) {
			_properties [property.Tag].Remove (property);
		}
		
		public void RemoveValue(string key) {
			_values.Remove (key);
		}

		public void AddPropertiesToXml(XmlDocument doc, XmlElement parent, BaseTemplate template) {
			foreach (string key in template.Properties.Keys) {
				foreach (PropertyTemplate p in template.Properties[key]) {
					XmlElement n = doc.CreateElement(key);
					AddAttributesToXml(doc, n, p);
					AddPropertiesToXml(doc, n, p);
					parent.AppendChild(n);
				}
			}
		}
		
		public void AddAttributesToXml(XmlDocument doc, XmlElement parent, BaseTemplate template) {
			foreach (string key in template.Values.Keys) {

				string k = key;
				if (key.Contains("android:")) {
					k = key.Replace("android:", "android___");
				}
				XmlAttribute attr = doc.CreateAttribute (k);
				attr.Value = template.Values[key];

				parent.Attributes.Append(attr);
			}
		}

		public Dictionary<string, string> Values {
			get {
				return _values;
			}
		}
		
		public Dictionary<string, List<PropertyTemplate>> Properties {
			get {
				return _properties;
			}
		}
	}

}
