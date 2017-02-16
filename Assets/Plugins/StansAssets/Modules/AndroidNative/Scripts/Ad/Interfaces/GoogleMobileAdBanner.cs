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

public interface GoogleMobileAdBanner  {
	
	void Hide();
	void Show();
	void Refresh();
	void SetBannerPosition(int x, int y);
	void SetBannerPosition(TextAnchor anchor);



	int id {get;}
	int width {get;}
	int height {get;}

	bool IsLoaded {get;}
	bool IsOnScreen {get;}
	bool ShowOnLoad{get; set;}

	GADBannerSize size {get;}
	TextAnchor anchor {get;}



	//Actions
	Action<GoogleMobileAdBanner> OnLoadedAction 			{ get; set; }
	Action<GoogleMobileAdBanner> OnFailedLoadingAction 		{ get; set; }
	Action<GoogleMobileAdBanner> OnOpenedAction 			{ get; set; }
	Action<GoogleMobileAdBanner> OnClosedAction 			{ get; set; }
	Action<GoogleMobileAdBanner> OnLeftApplicationAction  	{ get; set; }

}
