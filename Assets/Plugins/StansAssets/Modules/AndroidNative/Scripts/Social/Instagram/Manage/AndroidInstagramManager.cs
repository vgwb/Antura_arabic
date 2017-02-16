////////////////////////////////////////////////////////////////////////////////
//  
// @module Mobile Social Plugin 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;
using System;

public class AndroidInstagramManager : SA.Common.Pattern.Singleton<AndroidInstagramManager> {


	public static event Action<InstagramPostResult> OnPostingCompleteAction = delegate {};


	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	
	public void Share(Texture2D texture) {
		Share(texture, "");
	}
	
	
	public void Share(Texture2D texture, string message) {
		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);


		AN_SocialSharingProxy.InstagramPostImage(bytesString, message);

	}



	//--------------------------------------
	//  EVENTS
	//--------------------------------------



	private void OnPostSuccess() {
		OnPostingCompleteAction(InstagramPostResult.RESULT_OK);
	}
	
	
	private void OnPostFailed(string data) {
		
		int code = System.Convert.ToInt32(data);
		InstagramPostResult error = InstagramPostResult.NO_APPLICATION_INSTALLED;
		
		switch(code) {
		case 1:
			error = InstagramPostResult.NO_APPLICATION_INSTALLED;
			break;
		case 2:
			error = InstagramPostResult.INTERNAL_EXCEPTION;
			break;
		}
		
		OnPostingCompleteAction(error);
	}

}
