////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;

public enum GADBannerSize  {

	//Banner sizes indicated at 160 DPI, at a different DPI value pixel size will change accordingly
	//You may use width and height getters to find out current banner pixel size
	//Phones and Tablets
	BANNER = 1, //  (320x50)
	SMART_BANNER = 2, 

	//Tablets
	FULL_BANNER = 3, //  (468x60)
	LEADERBOARD = 4, //  (728x90)
	MEDIUM_RECTANGLE = 5, //  (300x250)
}
