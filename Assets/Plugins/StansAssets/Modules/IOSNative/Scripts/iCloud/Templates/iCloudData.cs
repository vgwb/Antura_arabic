////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class iCloudData  {


	private string _key;
	private string _val;

	private bool _IsEmpty = false;

	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public iCloudData(string k, string v) {
		_key = k;
		_val = v;

		if(_val.Equals("null")) {
			if(!IOSNativeSettings.Instance.DisablePluginLogs) 
				ISN_Logger.Log ("ISN iCloud Empty set");
			_IsEmpty = true;
		}
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public string key {
		get {
			return _key;
		}
	}

	public string stringValue {
		get {
			if(_IsEmpty) {
				return null;
			}

			return _val;
		}
	}

	public float floatValue {
		get {

			if(_IsEmpty) {
				return 0f;
			}

			return System.Convert.ToSingle (_val);
		}
	}

	public byte[] bytesValue {
		get {

			if(_IsEmpty) {
				return null;
			}

			return System.Convert.FromBase64String(_val);
		}
	}


	public bool IsEmpty {
		get {
			return _IsEmpty;
		}
	}

	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
