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

public class GooglePlayerTemplate {
	
	private string _id;
	private string _name;
	private string _iconImageUrl;
	private string _hiResImageUrl;

	private Texture2D _icon = null;
	private Texture2D _image = null;

	private bool _hasIconImage = false;
	private bool _hasHiResImage = false;


	public event Action<Texture2D> BigPhotoLoaded =  delegate {};
	public event Action<Texture2D> SmallPhotoLoaded =  delegate {};

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	public GooglePlayerTemplate(string pId, string pName, string iconUrl, string imageUrl, string pHasIconImage, string pHasHiResImage) {
		_id = pId;
		_name = pName;

		_iconImageUrl = iconUrl;
		_hiResImageUrl = imageUrl;

		if(pHasIconImage.Equals("1")) {
			_hasIconImage = true;
		}

		if(pHasHiResImage.Equals("1")) {
			_hasHiResImage = true;
		}


		if(AndroidNativeSettings.Instance.LoadProfileIcons) {
			LoadIcon();
		}

		if(AndroidNativeSettings.Instance.LoadProfileImages) {
			LoadImage();
		}
	} 


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------


	public void LoadImage() {
		
		if(image != null) {
			BigPhotoLoaded(image);
			return;
		}

		SA.Common.Util.Loader.LoadWebTexture(_hiResImageUrl, OnProfileImageLoaded);
	}


	public void LoadIcon() {
		
		if(icon != null) {
			SmallPhotoLoaded(icon);
			return;
		}

		SA.Common.Util.Loader.LoadWebTexture(_iconImageUrl, OnProfileIconLoaded);
	}

	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public string playerId {
		get {
			return _id;
		}
	}

	public string name {
		get {
			return _name;
		}
	}


	public bool hasIconImage {
		get {
			return _hasIconImage;
		}
	}
	
	public bool hasHiResImage {
		get {
			return _hasHiResImage;
		}
	}


	public string iconImageUrl {
		get {
			return _iconImageUrl;
		}
	}

	public string hiResImageUrl {
		get {
			return _hiResImageUrl;
		}
	}


	public Texture2D icon {
		get {
			return _icon;
		}
	}


	public Texture2D image {
		get {
			return _image;
		}
	}
	

	//--------------------------------------
	// EVENTS
	//--------------------------------------


	private void OnProfileImageLoaded(Texture2D tex) {
		if(this == null) {return;}

		_image = tex;
		BigPhotoLoaded(_image);
	}

	private void OnProfileIconLoaded(Texture2D tex) {
		if(this == null) {return;}

		_icon = tex;
		SmallPhotoLoaded(_icon);

	}



}
