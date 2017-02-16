using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FB_Score  {
	public string UserId;
	public string UserName;

	public string AppId;
	public string AppName;


	public int value;

	private Dictionary<FB_ProfileImageSize, Texture2D> profileImages =  new Dictionary<FB_ProfileImageSize, Texture2D>();
	public event Action<FB_Score> OnProfileImageLoaded = delegate {};


	public string GetProfileUrl(FB_ProfileImageSize size) {
		return  "https://graph.facebook.com/" + UserId + "/picture?type=" + size.ToString();
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


}
