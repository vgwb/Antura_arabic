////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class AndroidToast  {

	public const int LENGTH_SHORT = 0; // 2 seconds 
	public const int LENGTH_LONG  = 1; // 3.5 seconds

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public static void ShowToastNotification(string text) {
		ShowToastNotification (text, LENGTH_SHORT);
	}

	public static void ShowToastNotification(string text, int duration) {
		AN_NotificationProxy.ShowToastNotification (text, duration);
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
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
