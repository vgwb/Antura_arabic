using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public interface SP_FB_API  {

	void Init();
	void Login(params string[] scopes);
	void Logout();


	void API(string query, FB_HttpMethod method, SPFacebook.FB_Delegate callback);
	void API(string query, FB_HttpMethod method, SPFacebook.FB_Delegate callback, WWWForm form);


	void AppRequest(string message, FB_RequestActionType actionType, string objectId, string[] to, string data = "", string title = "");
	void AppRequest(string message, FB_RequestActionType actionType, string objectId, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "");
	void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "");


	void FeedShare(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string actionName = "", string actionLink = "", string reference = "");


	bool IsLoggedIn {get;}
	string UserId {get;}
	string AccessToken {get;}
	string AppId {get;}

}