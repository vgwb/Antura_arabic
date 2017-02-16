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

public class AndroidMessage : BaseAndroidPopup {
	
	public string ok;
	public Action OnComplete = delegate {} ;
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	public static AndroidMessage Create(string title, string message) {
		return Create(title, message, "Ok");
	}

    public static AndroidMessage Create(string title, string message, AndroidDialogTheme theme)
    {
        return Create(title, message, "Ok", theme);
    }

    public static AndroidMessage Create(string title, string message, string ok) {
		AndroidMessage dialog;
		dialog  = new GameObject("AndroidPopUp").AddComponent<AndroidMessage>();
		dialog.title = title;
		dialog.message = message;
		dialog.ok = ok;
		
		dialog.init();
		
		return dialog;
	}

    public static AndroidMessage Create(string title, string message, string ok, AndroidDialogTheme theme)
    {
        AndroidMessage dialog;
        dialog = new GameObject("AndroidPopUp").AddComponent<AndroidMessage>();
        dialog.title = title;
        dialog.message = message;
        dialog.ok = ok;

        dialog.init(theme);

        return dialog;
    }

    //--------------------------------------
    //  PUBLIC METHODS
    //--------------------------------------

    public void init() {
		AN_PoupsProxy.showMessage(title, message, ok, AndroidNativeSettings.Instance.DialogTheme);
	}

    public void init(AndroidDialogTheme theme)
    {
        AN_PoupsProxy.showMessage(title, message, ok, theme);
    }

    //--------------------------------------
    //  GET/SET
    //--------------------------------------

    //--------------------------------------
    //  EVENTS
    //--------------------------------------

    public void onPopUpCallBack(string buttonIndex) {
		DispatchAction(AndroidDialogResult.CLOSED);
		Destroy(gameObject);	
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
