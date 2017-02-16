//#define FBV7_API_ENABLED

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if FBV7_API_ENABLED
using Facebook.Unity;
#endif



public class SP_FB_API_v7 : SP_FB_API {

    private const string PUBLISH_PERMISSION_STRING = "publish_actions";

    private string _UserId = string.Empty;
	private string _AccessToken = string.Empty;


	//--------------------------------------
	//  API Methods
	//--------------------------------------


	public void Init() {
		#if FBV7_API_ENABLED
		FB.Init(OnInitComplete, OnHideUnity);
		#endif
	}

	public void Login(params string[] scopes) {
#if FBV7_API_ENABLED

        List<string> scopesList = new List<string>(scopes);
        if (scopesList.Contains(PUBLISH_PERMISSION_STRING))
        {
            scopesList.Remove(PUBLISH_PERMISSION_STRING);
            if (FB.IsLoggedIn)
            {                
                if (scopesList.Count > 0)
                {
                    Debug.Log("Logging with read permission");
                    FB.LogInWithReadPermissions(scopesList, ReadPermissionCallback);
                }
                else
                {
                    Debug.Log("Logging with write permission");
                    List<string> publishList = new List<string>();
                    publishList.Add(PUBLISH_PERMISSION_STRING);
                    FB.LogInWithPublishPermissions(publishList, LoginCallback);
                }
            }
            else
            {
                FB.LogInWithReadPermissions(scopesList, ReadPermissionCallback);
            }
        }
        else
        {
            FB.LogInWithReadPermissions(scopesList, LoginCallback);
        }
#endif
    }

#if FBV7_API_ENABLED
    private void ReadPermissionCallback(ILoginResult result)
    {
        LoginCallback(result);
        if (FB.IsLoggedIn)
        {
            Debug.Log("ReadPermissionCallback:Logging with write permission");
            List<string> publishList = new List<string>();
            publishList.Add(PUBLISH_PERMISSION_STRING);
            FB.LogInWithPublishPermissions(publishList, LoginCallback);
        }
    }
#endif

    public void Logout() {
		#if FBV7_API_ENABLED
		_UserId = string.Empty;
		_AccessToken = string.Empty;

		FB.LogOut();

		#endif
	}


	public void API(string query, FB_HttpMethod method, SPFacebook.FB_Delegate callback) {
		#if FBV7_API_ENABLED
		new FB_GrapRequest_V7(query, ConvertHttpMethod(method), callback);
		#endif
	}

	public void API(string query, FB_HttpMethod method, SPFacebook.FB_Delegate callback, WWWForm form) {
		#if FBV7_API_ENABLED
		new FB_GrapRequest_V7(query, ConvertHttpMethod(method), callback, form);
		#endif
	}

	public void AppRequest(string message, FB_RequestActionType actionType, string objectId, string[] to, string data = "", string title = "") {
		#if FBV7_API_ENABLED
		FB.AppRequest(message, ConvertActionType(actionType), objectId, to, data, title, AppRequestCallback);
		#endif
	}
	
	public void AppRequest(string message, FB_RequestActionType actionType, string objectId, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "") {
		#if FBV7_API_ENABLED
		FB.AppRequest(message, ConvertActionType(actionType), objectId, filters, excludeIds, maxRecipients, data, title, AppRequestCallback);
		#endif
	}
	
	
	public void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "") {
		#if FBV7_API_ENABLED
		FB.AppRequest(message, to, filters, excludeIds, maxRecipients, data, title, AppRequestCallback);
		#endif
	}


	public void FeedShare (string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string actionName = "", string actionLink = "", string reference = "")  {

		#if FBV7_API_ENABLED

		System.Uri linkUri = null;
		if (!string.IsNullOrEmpty(link))
			linkUri = new System.Uri(link);

		System.Uri pictureUri = null;
		if (!string.IsNullOrEmpty(picture))
			pictureUri = new System.Uri(picture);
		
		FB.FeedShare(
			toId: toId,
			link: linkUri,
			linkName: linkName,
			linkCaption: linkCaption,
			linkDescription: linkDescription,
			picture: pictureUri,
			mediaSource: reference,
			callback: PostCallback
		);

		#endif
		
	}




	//--------------------------------------
	//  Get / Set
	//--------------------------------------



	public bool IsLoggedIn {
		get {
			#if FBV7_API_ENABLED
			return FB.IsLoggedIn;
			#else
			return false;
			#endif
		}
	}

	public string UserId {
		get {
			return _UserId;
		}
	}

	public string AccessToken {
		get {
			return _AccessToken;
		}
	}

	public string AppId {
		get {
			#if FBV7_API_ENABLED
			return FB.AppId;
			#else
			return "";
			#endif
		}
	}

	public static bool IsAPIEnabled {
		get  {
			#if FBV7_API_ENABLED
			return true;
			#else
			return false;
			#endif
		}
	}

	//--------------------------------------
	//  Handlers
	//--------------------------------------

	#if FBV7_API_ENABLED

	private void AppRequestCallback(IAppRequestResult result) {
		FB_AppRequestResult res =  new FB_AppRequestResult(result.RawResult, result.Error);
		SPFacebook.Instance.AppRequestCallback(res);
	}

	
	private void LoginCallback(ILoginResult result) {
		FB_LoginResult res;

		if (result == null) {
			res = new FB_LoginResult(string.Empty, "Null Response", false);
		} else {
			res =  new FB_LoginResult(result.RawResult, result.Error, result.Cancelled);
		}

		if(res.IsSucceeded && !result.Cancelled && result.AccessToken != null) {
			_UserId 		= result.AccessToken.UserId;
			_AccessToken 	= result.AccessToken.TokenString;

			res.SetCredential(_UserId, _AccessToken);
		}

		SPFacebook.Instance.LoginCallback(res);

	}


	private void OnInitComplete() {
		if (Facebook.Unity.AccessToken.CurrentAccessToken != null) {
			_AccessToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
			_UserId = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
		}
		SPFacebook.Instance.OnInitComplete();
	}

	
	private void OnHideUnity(bool isGameShown) {
		SPFacebook.Instance.OnHideUnity(isGameShown);
	}

	private void PostCallback(IShareResult result) {
		FB_PostResult res =  new FB_PostResult(result.RawResult, result.Error);
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
			return HttpMethod.DELETE;
		}

		return HttpMethod.GET;
	}


	private OGActionType ConvertActionType(FB_RequestActionType actionType) {
		switch(actionType) {
		case FB_RequestActionType.AskFor:
			return OGActionType.ASKFOR;
		case FB_RequestActionType.Send:
			return OGActionType.SEND;
		case FB_RequestActionType.Turn:
			return OGActionType.TURN;
		}
		
		return OGActionType.ASKFOR;
	}

	#endif
}
