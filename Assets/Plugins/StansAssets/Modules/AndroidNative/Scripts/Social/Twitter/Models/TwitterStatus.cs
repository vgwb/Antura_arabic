////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public class TwitterStatus  {

	private string _rawJSON;
	
	private string _text;
	private string _geo;

	
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public TwitterStatus(IDictionary JSON) {
		_rawJSON = ANMiniJSON.Json.Serialize(JSON) ;


		_text = System.Convert.ToString(JSON["text"]);
		_geo  = System.Convert.ToString(JSON["geo"]);

	}
	
	
	
	
	//--------------------------------------
	// GET / SET
	//--------------------------------------
	
	public string rawJSON {
		get {
			return _rawJSON;
		}
	}

	public string text {
		get {
			return _text;
		}
	}

	public string geo {
		get {
			return _geo;
		}
	}

}

