////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;

public enum GPConnectionState  {
	STATE_UNCONFIGURED,  //the default sate, means we never tried to connect in current session
	STATE_DISCONNECTED,
	STATE_CONNECTING,
	STATE_CONNECTED
}
