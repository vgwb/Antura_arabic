////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FB_UserInfo {

	private string _id 			= string.Empty;
	private string _name 		= string.Empty;
	private string _first_name  = string.Empty;
	private string _last_name 	= string.Empty;
	private string _username 	= string.Empty;

	private string _profile_url = string.Empty;
	private string _email 		= string.Empty;
	
	private string _location 	= string.Empty;
	private string _locale 		= string.Empty;
	
	private string _rawJSON 	= string.Empty;

	private DateTime _Birthday 	= new DateTime();

	private FB_Gender _gender 	= FB_Gender.Male;

	private string _ageRange 	= string.Empty;
	private string _picUrl		= string.Empty;

	private Dictionary<FB_ProfileImageSize, Texture2D> profileImages =  new Dictionary<FB_ProfileImageSize, Texture2D>();


	public event Action<FB_UserInfo> OnProfileImageLoaded = delegate {};



	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	



	public FB_UserInfo(string data) {
		_rawJSON = data;

		IDictionary JSON =  ANMiniJSON.Json.Deserialize(_rawJSON) as IDictionary;	

		InitializeData(JSON);

	}


	public FB_UserInfo(IDictionary JSON) {
		InitializeData(JSON);
	}

	public void InitializeData(IDictionary JSON) {

		

		if(JSON.Contains("id")) {
			_id 								= System.Convert.ToString(JSON["id"]);
		}


		if(JSON.Contains("birthday")) {
			_Birthday 							=DateTime.Parse(System.Convert.ToString(JSON["birthday"])); 
		}


		if(JSON.Contains("name")) {
			_name 								= System.Convert.ToString(JSON["name"]);
		}

		if(JSON.Contains("first_name")) {
			_first_name 								= System.Convert.ToString(JSON["first_name"]);
		}

		if(JSON.Contains("last_name")) {
			_last_name 								= System.Convert.ToString(JSON["last_name"]);
		}

		if(JSON.Contains("username")) {
			_username 								= System.Convert.ToString(JSON["username"]);
		}

		if(JSON.Contains("link")) {
			_profile_url 								= System.Convert.ToString(JSON["link"]);
		}

		if(JSON.Contains("email")) {
			_email 								= System.Convert.ToString(JSON["email"]);
		}

		if(JSON.Contains("locale")) {
			_locale 								= System.Convert.ToString(JSON["locale"]);
		}

		if(JSON.Contains("location")) {
			IDictionary loc = JSON["location"] as IDictionary;
			_location							= System.Convert.ToString(loc["name"]);
		}

		if(JSON.Contains("gender")) {
			string g = System.Convert.ToString(JSON["gender"]);
			if(g.Equals("male")) {
				_gender = FB_Gender.Male;
			} else {
				_gender = FB_Gender.Female;
			}
		}

		if(JSON.Contains("age_range")) {
			IDictionary age = JSON["age_range"] as IDictionary;
			_ageRange = (age.Contains("min")) ? age["min"].ToString() : "0";
			_ageRange += "-";
			_ageRange += (age.Contains("max")) ? age["max"].ToString() : "1000";
		} 


		if(JSON.Contains("picture")) {
			IDictionary picDict = JSON["picture"] as IDictionary;
			if (picDict != null && picDict.Contains("data")) {
				IDictionary data = picDict["data"] as IDictionary;
				if (data != null && data.Contains("url")) {
					_picUrl =  System.Convert.ToString(data["url"]);
				}
			}
		}

	}


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public string GetProfileUrl(FB_ProfileImageSize size) {
		if (!string.IsNullOrEmpty(_picUrl)) {
			return _picUrl;
		}

		return  "https://graph.facebook.com/" + Id + "/picture?type=" + size.ToString();
	} 

	public Texture2D  GetProfileImage(FB_ProfileImageSize size) {
		if(profileImages.ContainsKey(size)) {
			return profileImages[size];
		} else {
			return null;
		}
	}

	public void LoadProfileImage(FB_ProfileImageSize size) {
		if(GetProfileImage(size) != null) {
			Debug.LogWarning("Profile image already loaded, size: " + size);
			OnProfileImageLoaded(this);
		}


		switch(size) {
		case FB_ProfileImageSize.large:
			SA.Common.Util.Loader.LoadWebTexture(GetProfileUrl(size), OnLargeImageLoaded);
			break;
		case FB_ProfileImageSize.normal:
			SA.Common.Util.Loader.LoadWebTexture(GetProfileUrl(size), OnNormalImageLoaded);
			break;
		case FB_ProfileImageSize.small:
			SA.Common.Util.Loader.LoadWebTexture(GetProfileUrl(size), OnSmallImageLoaded);
			break;
		case FB_ProfileImageSize.square:
			SA.Common.Util.Loader.LoadWebTexture(GetProfileUrl(size), OnSquareImageLoaded);
			break;

		}
		Debug.Log("LOAD IMAGE URL: " + GetProfileUrl(size));


	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public string RawJSON {
		get {
			return _rawJSON;
		}
	}
	
	
	public string Id {
		get {
			return _id;
		}
	}

	public DateTime Birthday {
		get {
			return _Birthday;
		}
	}

	public string Name {
		get {
			return _name;
		}
	}

	public string FirstName {
		get {
			return _first_name;
		}
	}

	public string LastName {
		get {
			return _last_name;
		}
	}


	public string UserName {
		get {
			return _username;
		}
	}


	public string ProfileUrl {
		get {
			return _profile_url;
		}
	}

	public string Email {
		get {
			return _email;
		}
	}


	public string Locale {
		get {
			return _locale;
		}
	}

	public string Location {
		get {
			return _location;
		}
	}


	public FB_Gender Gender {
		get {
			return _gender;
		}
	}

	public string AgeRange {
		get {
			return _ageRange;
		}
	}

	public string PictureUrl {
		get {
			return _picUrl;
		}
	}
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnSquareImageLoaded(Texture2D image) {

		if(this == null) {return;}

		if(image != null && !profileImages.ContainsKey(FB_ProfileImageSize.square)) {
			profileImages.Add(FB_ProfileImageSize.square, image);
		}
		
		OnProfileImageLoaded(this);
	}

	private void OnLargeImageLoaded(Texture2D image) {

		if(this == null) {return;}

		if(image != null && !profileImages.ContainsKey(FB_ProfileImageSize.large)) {
			profileImages.Add(FB_ProfileImageSize.large, image);
		}
		
		OnProfileImageLoaded(this);
	}


	private void OnNormalImageLoaded(Texture2D image) {

		if(this == null) {return;}

		if(image != null && !profileImages.ContainsKey(FB_ProfileImageSize.normal)) {
			profileImages.Add(FB_ProfileImageSize.normal, image);
		}
		
		OnProfileImageLoaded(this);
	}

	private void OnSmallImageLoaded(Texture2D image) {

		if(this == null) {return;}

		if(image != null && !profileImages.ContainsKey(FB_ProfileImageSize.small)) {
			profileImages.Add(FB_ProfileImageSize.small, image);
		}
		
		OnProfileImageLoaded(this);
	}




	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
