////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AndroidRateUsPopUp : BaseAndroidPopup {
	


	public string yes;
	public string later;
	public string no;
	public string url;

	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public static AndroidRateUsPopUp Create(string title, string message, string url) {
		return Create(title, message, url, "Rate app", "Later", "No, thanks");
	}

    public static AndroidRateUsPopUp Create(string title, string message, string url, AndroidDialogTheme theme)
    {
        return Create(title, message, url, "Rate app", "Later", "No, thanks", theme);
    }

    public static AndroidRateUsPopUp Create(string title, string message, string url, string yes, string later, string no) {
		AndroidRateUsPopUp rate = new GameObject("AndroidRateUsPopUp").AddComponent<AndroidRateUsPopUp>();
		rate.title = title;
		rate.message = message;
		rate.url = url;

		rate.yes = yes;
		rate.later = later;
		rate.no = no;

		rate.init();
			
		return rate;
	}

    public static AndroidRateUsPopUp Create(string title, string message, string url, string yes, string later, string no, AndroidDialogTheme theme)
    {
        AndroidRateUsPopUp rate = new GameObject("AndroidRateUsPopUp").AddComponent<AndroidRateUsPopUp>();
        rate.title = title;
        rate.message = message;
        rate.url = url;

        rate.yes = yes;
        rate.later = later;
        rate.no = no;

        rate.init(theme);

        return rate;
    }

    //--------------------------------------
    //  PUBLIC METHODS
    //--------------------------------------


    public void init() {
		AN_PoupsProxy.showRateDialog(title, message, yes, later, no, AndroidNativeSettings.Instance.DialogTheme);
	}


    public void init(AndroidDialogTheme theme)
    {
        AN_PoupsProxy.showRateDialog(title, message, yes, later, no, theme);
    }


    //--------------------------------------
    //  GET/SET
    //--------------------------------------

    //--------------------------------------
    //  EVENTS
    //--------------------------------------

    public void onPopUpCallBack(string buttonIndex) {
		int index = System.Convert.ToInt16(buttonIndex);
		switch(index) {
			case 0: 
				AN_PoupsProxy.OpenAppRatePage(url);
				DispatchAction(AndroidDialogResult.RATED);
				break;
			case 1:
				DispatchAction(AndroidDialogResult.REMIND);
				break;
			case 2:
				DispatchAction(AndroidDialogResult.DECLINED);
				break;
		}
		
		
		
		Destroy(gameObject);
	} 
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
