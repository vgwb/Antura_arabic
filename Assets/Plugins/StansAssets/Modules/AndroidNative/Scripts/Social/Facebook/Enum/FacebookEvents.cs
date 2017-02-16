////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;

public class FacebookEvents {
	
	public const string FACEBOOK_INITED 				= "facebook_inited";


	public const string AUTHENTICATION_SUCCEEDED 		= "authentication_succeeded";
	public const string AUTHENTICATION_FAILED   		= "authentication_failed";


	public const string PAYMENT_SUCCEEDED 				= "payment_succeeded";
	public const string PAYMENT_FAILED   				= "payment_failed";


	public const string POST_SUCCEEDED 					= "post_succeeded";
	public const string POST_FAILED   					= "post_failed";


	public const string APP_REQUEST_COMPLETE   			= "app_request_complete";


	public const string GAME_FOCUS_CHANGED 				= "game_focus_changed";

	public const string USER_DATA_LOADED 				= "user_data_loaded";
	public const string USER_DATA_FAILED_TO_LOAD   		= "user_data_failed_to_load";


	public const string FRIENDS_DATA_LOADED 			= "friends_data_loaded";
	public const string FRIENDS_FAILED_TO_LOAD   		= "friends_failed_to_load";



	//--------------------------------------
	//  Scores API 
	//  https://developers.facebook.com/docs/games/scores
	//------------------------------------

	public const string APP_SCORES_REQUEST_COMPLETE 			= "app_scores_request_complete";
	public const string PLAYER_SCORES_REQUEST_COMPLETE   		= "player_scores_request_complete";
	public const string SUBMIT_SCORE_REQUEST_COMPLETE   		= "submit_score_request_complete";
	public const string DELETE_SCORES_REQUEST_COMPLETE   		= "delete_scores_request_complete";


	//--------------------------------------
	//  Likes API 
	//  https://developers.facebook.com/docs/graph-api/reference/v2.0/user/likes
	//------------------------------------

	public const string LIKES_LIST_LOADED   		= "likes_list_loaded";

}
