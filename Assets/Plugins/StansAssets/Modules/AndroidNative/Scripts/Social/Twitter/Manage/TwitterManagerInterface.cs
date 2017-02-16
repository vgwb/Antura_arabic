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

public interface TwitterManagerInterface  {


	void Init();
	void Init(string consumer_key, string consumer_secret);
	void AuthenticateUser();
	void LoadUserData();
	void Post(string status);
	void Post(string status, Texture2D texture);
	TwitterPostingTask PostWithAuthCheck(string status);
	TwitterPostingTask PostWithAuthCheck(string status, Texture2D texture);


	void LogOut();

	bool IsAuthed {get;}
	bool IsInited {get;}
	TwitterUserInfo userInfo  {get;}

	event Action<TWResult> OnTwitterInitedAction;
	event Action<TWResult> OnAuthCompleteAction;
	event Action<TWResult> OnPostingCompleteAction;
	event Action<TWResult> OnUserDataRequestCompleteAction; 

}
