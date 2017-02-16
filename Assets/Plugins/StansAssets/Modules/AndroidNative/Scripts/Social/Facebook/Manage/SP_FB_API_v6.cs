//#define FBV6_API_ENABLED

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if FBV6_API_ENABLED
using Facebook;
#endif

public class SP_FB_API_v6  : SP_FB_API {

	public void Init() {
		#if FBV6_API_ENABLED
		FB.Init(OnInitComplete, OnHideUnity);
		#endif
	}
	
	public void Login(params string[] scopes) {
		#if FBV6_API_ENABLED


		string scopesList = string.Empty;
		foreach(string s in scopes) {
			if (!scopesList.Equals(string.Empty)) {
				scopesList += ",";
			}
			scopesList += s;
		}

		FB.Login(scopesList, LoginCallback);

	
		#endif
	}
	
	public void Logout() {
		#if FBV6_API_ENABLED
		
		FB.Logout();
		
		#endif
	}
	
	
	public void API(string query, FB_HttpMethod method, SPFacebook.FB_Delegate callback) {
		#if FBV6_API_ENABLED
		new FB_GrapRequest_V6(query, ConvertHttpMethod(method), callback);
		#endif
	}
	
	public void API(string query, FB_HttpMethod method, SPFacebook.FB_Delegate callback, WWWForm form) {
		#if FBV6_API_ENABLED
		new FB_GrapRequest_V6(query, ConvertHttpMethod(method), callback, form);
		#endif
	}
	
	public void AppRequest(string message, FB_RequestActionType actionType, string objectId, string[] to, string data = "", string title = "") {
		#if FBV6_API_ENABLED
		FB.AppRequest(message, ConvertActionType(actionType), objectId, to, data, title, AppRequestCallback);
		#endif
	}
	
	public void AppRequest(string message, FB_RequestActionType actionType, string objectId, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "") {
		#if FBV6_API_ENABLED
		FB.AppRequest(message, ConvertActionType(actionType), objectId, filters, excludeIds, maxRecipients, data, title, AppRequestCallback);
		#endif
	}
	
	
	public void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "") {
		#if FBV6_API_ENABLED
		FB.AppRequest(message, to, filters, excludeIds, maxRecipients, data, title, AppRequestCallback);
		#endif
	}
	
	
	public void FeedShare (string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string actionName = "", string actionLink = "", string reference = "")  {
		
		#if FBV6_API_ENABLED


		FB.Feed(
			toId: toId,
			link: link,
			linkName: linkName,
			linkCaption: linkCaption,
			linkDescription: linkDescription,
			picture: picture,
			actionName: actionName,
			actionLink: actionLink,
			reference: reference,
			callback: PostCallback
			);
		
		#endif
		
	}
	
	
	
	
	//--------------------------------------
	//  Get / Set
	//--------------------------------------
	
	
	
	public bool IsLoggedIn {
		get {
			#if FBV6_API_ENABLED
			return FB.IsLoggedIn;
			#else
			return false;
			#endif
		}
	}
	
	public string UserId {
		get {
			#if FBV6_API_ENABLED
			return FB.UserId;
			#else
			return "";
			#endif
		}
	}
	
	public string AccessToken {
		get {
			#if FBV6_API_ENABLED
			return FB.AccessToken;
			#else
			return "";
			#endif
		}
	}
	
	public string AppId {
		get {
			#if FBV6_API_ENABLED
			return FB.AppId;
			#else
			return "";
			#endif
		}
	}

	public static bool IsAPIEnabled {
		get  {
			#if FBV6_API_ENABLED
			return true;
			#else
			return false;
			#endif
		}
	}
	
	//--------------------------------------
	//  Handlers
	//--------------------------------------
	
	#if FBV6_API_ENABLED
	
	private void AppRequestCallback(FBResult result) {
		FB_AppRequestResult res =  new FB_AppRequestResult(result.Text, result.Error);
		SPFacebook.Instance.AppRequestCallback(res);
	}
	
	
	private void LoginCallback(FBResult result) {

		bool IsCancelled = false;
		if(result.Error == null && !IsLoggedIn) {
			IsCancelled = true;
		}


		FB_LoginResult res = new FB_LoginResult(result.Text, result.Error, IsCancelled);
		res.SetCredential(UserId, AccessToken);
	
		
		SPFacebook.Instance.LoginCallback(res);
		
	}
	
	
	private void OnInitComplete() {
		SPFacebook.Instance.OnInitComplete();
	}
	
	
	private void OnHideUnity(bool isGameShown) {
		SPFacebook.Instance.OnHideUnity(isGameShown);
	}
	
	private void PostCallback(FBResult result) {
		FB_PostResult res =  new FB_PostResult(result.Text, result.Error);
		SPFacebook.Instance.PostCallback(res);
	}
	
	
	//--------------------------------------
	//  Utils
	//--------------------------------------
	
	private HttpMethod ConvertHttpMethod(FB_HttpMethod method) {
		switch(method) {
		case FB_HttpMethod.GET:
			return HttpMethod.GET;
		case FB_HttpMethod.POST:
			return HttpMethod.POST;
		case FB_HttpMethod.DELETE:
			return HttpMethod.POST;
		}
		
		return HttpMethod.GET;
	}
	
	
	private OGActionType ConvertActionType(FB_RequestActionType actionType) {
		switch(actionType) {
		case FB_RequestActionType.AskFor:
			return OGActionType.AskFor;
		case FB_RequestActionType.Send:
			return OGActionType.Send;
		}
		
		return OGActionType.AskFor;
	}
	
	#endif
}
