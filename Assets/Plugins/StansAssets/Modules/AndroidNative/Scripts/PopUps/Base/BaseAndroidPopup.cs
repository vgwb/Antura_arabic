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

public class BaseAndroidPopup : MonoBehaviour {
	
	
	public string title;
	public string message;

	public event Action<AndroidDialogResult> ActionComplete = delegate{};
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	public void onDismissed(string data) {
		ActionComplete(AndroidDialogResult.CLOSED);
		Destroy(gameObject);
	}

	protected void DispatchAction(AndroidDialogResult res) {
		ActionComplete(res);
	}
	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------


}
