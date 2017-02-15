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
using System.Collections.Generic;

public class SPFacebook : SA.Common.Pattern.Singleton<SPFacebook> {


	public delegate void FB_Delegate(FB_Result result);
	
	
	private FB_UserInfo _userInfo = null;
	private Dictionary<string,  FB_UserInfo> _friends = new Dictionary<string, FB_UserInfo>();
	private Dictionary<string,  FB_UserInfo> _invitableFriends = new Dictionary<string, FB_UserInfo> ();
	private bool _IsInited = false;
	
	
	private  Dictionary<string,  FB_Score> _userScores =  new Dictionary<string, FB_Score>();
	private  Dictionary<string,  FB_Score> _appScores  =  new Dictionary<string, FB_Score>();
	
	private int lastSubmitedScore = 0;
	
	
	private  Dictionary<string,  Dictionary<string, FB_LikeInfo>> _likes =  new Dictionary<string, Dictionary<string, FB_LikeInfo>>();
	
	private List<FB_AppRequest> _AppRequests =  new List<FB_AppRequest>();
	
	
	//Actinos
	public static event Action OnPostStarted 					= delegate {};
	public static event Action OnLoginStarted 					= delegate {};
	public static event Action OnLogOut 						= delegate {};
	public static event Action OnFriendsRequestStarted 		= delegate {};


	public static event Action OnInitCompleteAction = delegate {};
	public static event Action<FB_PostResult> OnPostingCompleteAction = delegate {};
	
	
	public static event Action<bool> OnFocusChangedAction = delegate {};
	public static event Action<FB_LoginResult> OnAuthCompleteAction = delegate {};

	public static event Action<FB_Result> OnUserDataRequestCompleteAction = delegate {};
	public static event Action<FB_Result> OnFriendsDataRequestCompleteAction = delegate {};
	public static event Action<FB_Result> OnInvitableFriendsDataRequestCompleteAction = delegate {};
	
	
	public static event Action<FB_AppRequestResult> OnAppRequestCompleteAction = delegate {};
	public static event Action<FB_Result> OnAppRequestsLoaded = delegate {};

	
	//--------------------------------------
	//  Scores API 
	//  https://developers.facebook.com/docs/games/scores
	//------------------------------------

	public static event Action<FB_Result> OnAppScoresRequestCompleteAction 			= delegate {};
	public static event Action<FB_Result> OnPlayerScoresRequestCompleteAction   		= delegate {};
	public static event Action<FB_Result> OnSubmitScoreRequestCompleteAction   		= delegate {};
	public static event Action<FB_Result> OnDeleteScoresRequestCompleteAction   		= delegate {};
	
	
	//--------------------------------------
	//  Likes API 
	//  https://developers.facebook.com/docs/graph-api/reference/v2.0/user/likes
	//------------------------------------
	
	public static event Action<FB_Result> OnLikesListLoadedAction = delegate {};

	private SP_FB_API _FB = null;
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	public void Init() {

		#if UNITY_ANDROID

		try {
			Type soomla = Type.GetType("AN_SoomlaGrow");
			System.Reflection.MethodInfo method  = soomla.GetMethod("Init", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			
			method.Invoke(null, null);
		} catch(Exception ex) {
			Debug.LogError("AndroidNative. Soomla Initalization failed" + ex.Message);
		}
	
		#endif

		FB.Init();
	}
	
	
	public void Login() {
		Login(SocialPlatfromSettings.Instance.fb_scopes_list.ToArray());
	}
	
	public void Login(params string[] scopes) {
		OnLoginStarted();

		FB.Login(scopes);
	}
	
	
	//--------------------------------------
	//  API METHODS
	//--------------------------------------


	/*
	public void CallPermissionCheck() {
		FB.API("/me/permissions", FB_HttpMethod.GET, PermissionCallback);
	}

	public void RevokePermission(FBPermission permission) {
		FB.API ("me/permissions/" + permission.Name, FB_HttpMethod.DELETE, RemovePermissionCallback);
	}
	*/
	
	public void Logout() {
		OnLogOut();
		FB.Logout();
	}
	
	
	
	public void LoadUserData() {
		if(IsLoggedIn) {
			
			FB.API("/me?fields=id,birthday,name,first_name,last_name,link,email,locale,location,gender,age_range,picture", FB_HttpMethod.GET, UserDataCallBack);
			
		} else {
			
			Debug.LogWarning("Auth user before loadin data, fail event generated");
			FB_Result res = new FB_Result("","User isn't authed");
			OnUserDataRequestCompleteAction(res);
		}
	}

	public void LoadInvitableFrientdsInfo(int limit) {
		if(IsLoggedIn) {
			
			FB.API("/me?fields=invitable_friends.limit(" + limit.ToString() + ").fields(first_name,id,last_name,name,link,locale,location)", FB_HttpMethod.GET, InvitableFriendsDataCallBack);  
			
		} else {
			Debug.LogWarning("Auth user before loadin data, fail event generated");
			FB_Result res = new FB_Result("","User isn't authed");
			OnInvitableFriendsDataRequestCompleteAction(res);
		}
	}

	public FB_UserInfo GetInvitableFriendById(string id) {
		if(_invitableFriends != null) {
			if(_invitableFriends.ContainsKey(id)) {
				return _invitableFriends[id];
			}
		}
		
		return null;
	}

	public void LoadFrientdsInfo(int limit) {

		OnFriendsRequestStarted();

		if(IsLoggedIn) {
			
			FB.API("/me?fields=friends.limit(" + limit.ToString() + ").fields(first_name,id,last_name,name,link,locale,location)", FB_HttpMethod.GET, FriendsDataCallBack);  
			
		} else {
			Debug.LogWarning("Auth user before loadin data, fail event generated");
			FB_Result res = new FB_Result("","User isn't authed");
			OnFriendsDataRequestCompleteAction(res);
		}
	}
	
	public FB_UserInfo GetFriendById(string id) {
		if(_friends != null) {
			if(_friends.ContainsKey(id)) {
				return _friends[id];
			}
		}
		
		return null;
	}
	
	
	
	public void PostImage(string caption, Texture2D image) {

		OnPostStarted();
		
		byte[] imageBytes = image.EncodeToPNG();
		
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("message", caption);
		wwwForm.AddBinaryData("image", imageBytes, "picture.png");
		wwwForm.AddField("name", caption);
		
		FB.API("me/photos", FB_HttpMethod.POST, PostCallback_Internal, wwwForm);
	}
	
	public void PostText(string message) {
		OnPostStarted();

		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("message", message);
		
		FB.API("me/feed", FB_HttpMethod.POST, PostCallback_Internal, wwwForm);
	}
	
	
	public void FeedShare (string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string actionName = "", string actionLink = "", string reference = "")  {

		FB_PostingTask task = FB_PostingTask.Cretae();
		task.FeedShare(toId,
		          link,
		          linkName,
		          linkCaption,
		          linkDescription,
		          picture,
		          actionName,
		          actionLink,
		          reference);
	} 
	
	

	public void SendTrunRequest(string title, string message, string data = "", string[] to = null) {
		
		string resp = "";
		if(to != null) {
			resp = string.Join(",", to);
		}
		
		AN_FBProxy.SendTrunRequest(title, message, data, resp);
	}
	
	public void SendGift(string title, string message, string objectId, string data = "", string[] to = null ) {
		
		AppRequest(message, FB_RequestActionType.Send, objectId,  to, data, title);
	}
	
	public void AskGift(string title, string message, string objectId, string data = "", string[] to = null ) {
		AppRequest(message, FB_RequestActionType.AskFor, objectId,  to, data, title);
	}
	
	public void SendInvite(string title, string message, string data = "", string[] to = null) {
		AppRequest(message, to, null, null, default(int?), data, title);
	}
	
	

	
	private void OnAppRequestFailed_AndroidCB(string error) {
		FB_AppRequestResult res =  new FB_AppRequestResult("",error);
		OnAppRequestCompleteAction(res);
	}
	
	
	private void OnAppRequestCompleted_AndroidCB(string data) {
		Debug.Log("OnAppRequestCompleted_AndroidCB: " + data);
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		string requestId = storeData[0];
		string to = storeData[1];
		string[] list = to.Split(',');

		FB_AppRequestResult res =  new FB_AppRequestResult(requestId, new List<string>(list), data);
		OnAppRequestCompleteAction(res);
		
	}
	
	
	public void AppRequest(string message, FB_RequestActionType actionType, string objectId, string[] to, string data = "", string title = ""){
		if(!IsLoggedIn) { 
			Debug.LogWarning("Auth user before AppRequest, fail event generated");
			FB_AppRequestResult r =  new FB_AppRequestResult("","User isn't authed");
			OnAppRequestCompleteAction(r);
			return;
		}
		
		FB.AppRequest(message, actionType, objectId, to, data, title);
	}
	
	public void AppRequest(string message, FB_RequestActionType actionType, string objectId, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = ""){
		if(!IsLoggedIn) { 
			Debug.LogWarning("Auth user before AppRequest, fail event generated");
			FB_AppRequestResult r =  new FB_AppRequestResult("","User isn't authed");
			OnAppRequestCompleteAction(r);
			return;
		}
		
		FB.AppRequest(message, actionType, objectId, filters, excludeIds, maxRecipients, data, title);
	}
	
	
	
	public void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = ""){
		if(!IsLoggedIn) { 
			Debug.LogWarning("Auth user before AppRequest, fail event generated");
			FB_AppRequestResult r =  new FB_AppRequestResult("","User isn't authed");
			OnAppRequestCompleteAction(r);
			return;
		}
		
		FB.AppRequest(message, to, filters, excludeIds, maxRecipients, data, title);
	}
	
	
	
	//--------------------------------------
	//  Requests API 
	//--------------------------------------
	
	
	public void LoadPendingRequests() {
		FB.API("/" + UserId + "/apprequests?fields=id,application,data,message,action_type,created_time,from,object", FB_HttpMethod.GET, OnRequestsLoadComplete);
		
	}
	
	private void OnRequestsLoadComplete(FB_Result result) {
		if(result.IsSucceeded) {
			
			Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(result.RawData) as Dictionary<string, object>;
			List<object> data = JSON["data"]  as List<object>;			
			
			AppRequests.Clear();
			foreach(object row in data) {
				
				FB_AppRequest request =  new FB_AppRequest();
				Dictionary<string, object> dataRow = row as Dictionary<string, object>;
				
				Dictionary<string, object> AppInfo = dataRow["application"]  as Dictionary<string, object>;
				request.ApplicationId = System.Convert.ToString(AppInfo["id"]);
				if(!request.ApplicationId.Equals(AppId)) {
					break;
				}
				
				Dictionary<string, object> FromInfo = dataRow["from"]  as Dictionary<string, object>;
				request.FromId = System.Convert.ToString(FromInfo["id"]);
				request.FromName = System.Convert.ToString(FromInfo["name"]);
				
				request.Id = System.Convert.ToString(dataRow["id"]);

				if (dataRow.ContainsKey("created_time")) {
					request.SetCreatedTime(System.Convert.ToString(dataRow["created_time"]));
				}
				
				if(dataRow.ContainsKey("data")) {
					request.Data = System.Convert.ToString(dataRow["data"]);
				}
				
				if(dataRow.ContainsKey("message")) {
					request.Message = System.Convert.ToString(dataRow["message"]);
				}				
				
				if(dataRow.ContainsKey("message")) {
					request.Message = System.Convert.ToString(dataRow["message"]);
				}
				
				if(dataRow.ContainsKey("action_type")) {
					string action_type = System.Convert.ToString(dataRow["action_type"]);
					switch(action_type) {
					case "send":
						request.ActionType = FB_RequestActionType.Send;
						break;
						
					case "askfor":
						request.ActionType = FB_RequestActionType.AskFor;
						break;
						
					case "turn":
						request.ActionType = FB_RequestActionType.Turn;
						break;
					}
					
				}
				
				if(dataRow.ContainsKey("object")) {
					FB_Object obj = new FB_Object();
					Dictionary<string, object> objectData = dataRow["object"] as Dictionary<string, object>;
					
					obj.Id = System.Convert.ToString(objectData["id"]);
					obj.Title = System.Convert.ToString(objectData["id"]);
					obj.Type = System.Convert.ToString(objectData["id"]);
					obj.SetCreatedTime(System.Convert.ToString(objectData["created_time"]));
					
					if(objectData.ContainsKey("image")) {
						
						List<object> images = objectData["image"] as List<object>;
						Debug.Log(objectData["image"]);
						foreach(object img in images) {
							Dictionary<string, object>imgData = img as Dictionary<string, object>;
							obj.AddImageUrl(System.Convert.ToString(imgData["url"]));
						}						
					}
					request.Object = obj;					
				}
				
				AppRequests.Add(request);
			}
			
			Debug.Log("SPFacebook: " + AppRequests.Count +  "App request Loaded");
			
		} else {
			Debug.LogWarning("SPFacebook: App requests failed to load");
			Debug.LogWarning(result.Error.ToString());
		}

		OnAppRequestsLoaded(result);
	}
	
	
	//--------------------------------------
	//  Scores API 
	//  https://developers.facebook.com/docs/games/scores
	//------------------------------------
	
	
	
	//Read score for a player
	public void LoadPlayerScores() {
		FB.API("/" + UserId + "/scores", FB_HttpMethod.GET, OnLoaPlayrScoresComplete);  
	}
	
	//Read scores for players and friends
	public void LoadAppScores() {
		FB.API("/" + AppId + "/scores", FB_HttpMethod.GET, OnAppScoresComplete);  
	}
	
	//Create or update a score
	public void SubmitScore(int score) {
		lastSubmitedScore = score;
		FB.API("/" + UserId + "/scores?score=" + score, FB_HttpMethod.POST, OnScoreSubmited);  
	}
	
	//Delete scores for a player
	public void DeletePlayerScores() {
		FB.API("/" + UserId + "/scores", FB_HttpMethod.DELETE, OnScoreDeleted); 
		
		
	}
	
	
	
	//--------------------------------------
	//  Likes API 
	//  https://developers.facebook.com/docs/graph-api/reference/v2.0/user/likes
	//------------------------------------
	
	public void LoadCurrentUserLikes() {
		LoadLikes(FB.UserId);
	}
	
	
	public void LoadLikes(string userId) {
		FB_LikesRetrieveTask task = FB_LikesRetrieveTask.Create();
		task.ActionComplete += OnUserLikesResult;
		task.LoadLikes(userId);
	}
	
	public void LoadLikes(string userId, string pageId) {
		FB_LikesRetrieveTask task = FB_LikesRetrieveTask.Create();
		task.ActionComplete += OnUserLikesResult;
		task.LoadLikes(userId, pageId);
	}
	
	

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	
	
	
	public FB_Score GetCurrentPlayerScoreByAppId(string appId) {
		if(_userScores.ContainsKey(appId)) {
			return _userScores[appId];
		} else {
			FB_Score score =  new FB_Score();
			score.UserId = FB.UserId;
			score.AppId = appId;
			score.value = 0;
			
			return score;
		}
	}
	
	
	public int GetCurrentPlayerIntScoreByAppId(string appId) {
		return GetCurrentPlayerScoreByAppId(appId).value;
	}
	
	
	
	
	public int GetScoreByUserId(string userId) {
		if(_appScores.ContainsKey(userId)) {
			return _appScores[userId].value;
		} else {
			return 0;
		}
	}
	
	public FB_Score GetScoreObjectByUserId(string userId) {
		if(_appScores.ContainsKey(userId)) {
			return _appScores[userId];
		} else {
			return null;
		}
	}
	
	
	
	public List<FB_LikeInfo> GerUserLikesList(string userId){
		
		List<FB_LikeInfo>  result = new List<FB_LikeInfo>();
		
		if(_likes.ContainsKey(userId)) {
			foreach(KeyValuePair<string,  FB_LikeInfo>  pair in _likes[userId]) {
				result.Add(pair.Value);
			}
		}
		
		return result;
	}
	
	public bool IsUserLikesPage(string userId, string pageId) {
		if(_likes.ContainsKey(userId)) {
			if(_likes[userId].ContainsKey(pageId)) {
				return true;
			}
		}
		
		return false;
	}
	
	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	
	public bool IsInited  {
		get {
			return _IsInited;
		}
	}
	
	public bool IsLoggedIn {
		get {
			return FB.IsLoggedIn;
		}
	}
	
	
	public string UserId {
		get {
			return FB.UserId;
		}
	}
	
	public string AccessToken {
		get {
			return FB.AccessToken;
		}
	}

	public string AppId {
		get {
			return FB.AppId;
		}
	}
	
	public FB_UserInfo userInfo {
		get {
			return _userInfo;
		}
	}
	
	public Dictionary<string,  FB_UserInfo> friends {
		get {
			return _friends;
		}
	}

	public Dictionary<string,  FB_UserInfo> InvitableFriends {
		get {
			return _invitableFriends;
		}
	}
	
	public List<string> friendsIds {
		get {
			if(_friends == null) {
				return null;
			}
			
			List<string> ids = new List<string>();
			foreach(KeyValuePair<string, FB_UserInfo> item in _friends) {
				ids.Add(item.Key);
			}
			
			return ids;
		}
	}

	public List<string> InvitableFriendsIds {
		get {
			if(_invitableFriends == null) {
				return null;
			}
			
			List<string> ids = new List<string>();
			foreach(KeyValuePair<string, FB_UserInfo> item in _invitableFriends) {
				ids.Add(item.Key);
			}
			
			return ids;
		}
	}
	
	public List<FB_UserInfo> friendsList {
		get {
			if(_friends == null) {
				return null;
			}
			
			List<FB_UserInfo> flist = new List<FB_UserInfo>();
			foreach(KeyValuePair<string, FB_UserInfo> item in _friends) {
				flist.Add(item.Value);
			}
			
			return flist;
		}
	}
	
	public List<FB_UserInfo> InvitableFriendsList {
		get {
			if(_invitableFriends == null) {
				return null;
			}
			
			List<FB_UserInfo> flist = new List<FB_UserInfo>();
			foreach(KeyValuePair<string, FB_UserInfo> item in _invitableFriends) {
				flist.Add(item.Value);
			}
			
			return flist;
		}
	}
	
	public  Dictionary<string,  FB_Score> userScores  {
		get {
			return _userScores;
		}
	}
	
	public  Dictionary<string,  FB_Score>  appScores{
		get {
			return _appScores;
		}
	}
	
	
	public List<FB_Score> applicationScoreList {
		get {
			List<FB_Score>  result = new List<FB_Score>();
			foreach(KeyValuePair<string,  FB_Score>  pair in _appScores) {
				result.Add(pair.Value);
			}
			
			return result;
		}
	}
	
	public List<FB_AppRequest> AppRequests {
		get {
			return _AppRequests;
		}
	}

	public SP_FB_API FB {
		get {
			if(_FB == null) {

				if(SP_FB_API_v7.IsAPIEnabled) {
					_FB =  new SP_FB_API_v7();
				}

				if(_FB == null) {
					_FB =  new SP_FB_API_v6();
				}
			}
			return _FB;
		}
	}	
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private void OnUserLikesResult(FB_Result result, FB_LikesRetrieveTask task) {
		

		if(result.IsFailed) {
			OnLikesListLoadedAction(result);
			return;
		}
		
		
		Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(result.RawData) as Dictionary<string, object>;
		List<object> data = JSON["data"]  as List<object>;
		
		
		Dictionary<string, FB_LikeInfo> userLikes = null;
		if(_likes.ContainsKey(task.userId)) {
			userLikes = _likes[task.userId];
		} else {
			userLikes =  new Dictionary<string, FB_LikeInfo>();
			_likes.Add(task.userId, userLikes);
		}
		
		foreach(object row in data) {
			Dictionary<string, object> dataRow = row as Dictionary<string, object>;
			
			FB_LikeInfo tpl 	=  new FB_LikeInfo();
			tpl.Id 				= System.Convert.ToString(dataRow["id"]);
			tpl.Name 			= System.Convert.ToString(dataRow["name"]);
			tpl.CreatedTime 	= System.Convert.ToString(dataRow["created_time"]);
			
			if(userLikes.ContainsKey(tpl.Id)) {
				userLikes[tpl.Id] = tpl;
			} else {
				userLikes.Add(tpl.Id, tpl);
			}
		}

		OnLikesListLoadedAction(result);
	}
	
	
	
	
	private void OnScoreDeleted(FB_Result result) {

		if(result.IsFailed) {
			OnDeleteScoresRequestCompleteAction(result);
			return;
		}
		
		
		if(result.RawData.Equals("true")) {
			
			FB_Score score = new FB_Score();
			score.AppId = AppId;
			score.UserId = UserId;
			score.value = 0;
			
			if(_appScores.ContainsKey(UserId)) {
				_appScores[UserId].value = 0;
			}  else {
				_appScores.Add(score.UserId, score);
			}
			
			
			if(_userScores.ContainsKey(AppId)) {
				_userScores[AppId].value = 0;
			} else {
				_userScores.Add(AppId, score); 
			}

		} 

		OnDeleteScoresRequestCompleteAction(result);
		
		
	}
	
	private void OnScoreSubmited(FB_Result result) {

		if(result.IsFailed) {
			OnSubmitScoreRequestCompleteAction(result);
			return;
		}
		
		
		if(result.RawData.Equals("true")) {
			
			FB_Score score = new FB_Score();
			score.AppId = AppId;
			score.UserId = UserId;
			score.value = lastSubmitedScore;
			
			if(_appScores.ContainsKey(UserId)) {
				_appScores[UserId].value = lastSubmitedScore;
			}  else {
				_appScores.Add(score.UserId, score);
			}
			
			
			if(_userScores.ContainsKey(AppId)) {
				_userScores[AppId].value = lastSubmitedScore;
			} else {
				_userScores.Add(AppId, score); 
			}
		
		} 
			
		OnSubmitScoreRequestCompleteAction(result);

	}
	
	
	private void OnAppScoresComplete(FB_Result result) {
		if(result.IsFailed) {
			OnAppScoresRequestCompleteAction(result);
			return;
		}
		
		Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(result.RawData) as Dictionary<string, object>;
		List<object> data = JSON["data"]  as List<object>;
		
		foreach(object row in data) {
			FB_Score score =  new FB_Score();
			Dictionary<string, object> dataRow = row as Dictionary<string, object>;

			if (dataRow.ContainsKey("user")) {
			
				Dictionary<string, object> userInfo = dataRow["user"]  as Dictionary<string, object>;

				if (userInfo.ContainsKey("id")) {
					score.UserId = System.Convert.ToString(userInfo["id"]);
				}

				if (userInfo.ContainsKey("name")) {
					score.UserName = System.Convert.ToString(userInfo["name"]);
				}
				
				
			}


			if (dataRow.ContainsKey("score")) {
				score.value = System.Convert.ToInt32(dataRow["score"]); 
			}


			if (dataRow.ContainsKey("application")) {
				Dictionary<string, object> AppInfo = dataRow["application"]  as Dictionary<string, object>;

				if (AppInfo.ContainsKey("id")) {
					score.AppId = System.Convert.ToString(AppInfo["id"]);
				}

				if (AppInfo.ContainsKey("name")) {
					score.AppName = System.Convert.ToString(AppInfo["name"]);
				}

			}

			
			AddToAppScores(score);
			
			
		}

		OnAppScoresRequestCompleteAction(result);
	}
	
	private void AddToAppScores(FB_Score score) {
		
		if(_appScores.ContainsKey(score.UserId)) {
			_appScores[score.UserId] = score;
		} else {
			_appScores.Add(score.UserId, score);
		}
		
		if(_userScores.ContainsKey(score.AppId)) {
			_userScores[score.AppId] = score;
		} else {
			_userScores.Add(score.AppId, score);
		}
		
		
		
		
	}
	
	private void AddToUserScores(FB_Score score) {
		if(_userScores.ContainsKey(score.AppId)) {
			_userScores[score.AppId] = score;
		} else {
			_userScores.Add(score.AppId, score);
		}
		
		
		if(_appScores.ContainsKey(score.UserId)) {
			_appScores[score.UserId] = score;
		} else {
			_appScores.Add(score.UserId, score);
		}
		
	}
	
	private void OnLoaPlayrScoresComplete(FB_Result result) {
		

		if(result.IsFailed) {
			OnPlayerScoresRequestCompleteAction(result);
			return;
		}
		
		Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(result.RawData) as Dictionary<string, object>;
		List<object> data = JSON["data"]  as List<object>;
		
		foreach(object row in data) {
			FB_Score score =  new FB_Score();
			Dictionary<string, object> dataRow = row as Dictionary<string, object>;
			
			Dictionary<string, object> userInfo = dataRow["user"]  as Dictionary<string, object>;
			
			score.UserId = System.Convert.ToString(userInfo["id"]);
			score.UserName = System.Convert.ToString(userInfo["name"]);
			
			
			score.value = System.Convert.ToInt32(dataRow["score"]);
			score.AppId = System.Convert.ToString(FB.AppId);
			
			AddToUserScores(score);
			
		}
		

		OnPlayerScoresRequestCompleteAction(result);
		
	}



	public void ParseInvitableFriendsData(string data) {
		ParseFriendsFromJson (data, _invitableFriends, true);		
	}

	private void ParseFriendsFromJson(string data, Dictionary<string, FB_UserInfo> friends, bool invitable = false) {
		Debug.Log("ParceFriendsData");
		Debug.Log(data);
		
		try {
			_friends.Clear();
			IDictionary JSON =  ANMiniJSON.Json.Deserialize(data) as IDictionary;	
			IDictionary f = invitable ? JSON["invitable_friends"] as IDictionary : JSON["friends"] as IDictionary;
			IList flist = f["data"] as IList;
			
			
			for(int i = 0; i < flist.Count; i++) {
				FB_UserInfo user = new FB_UserInfo(flist[i] as IDictionary);
				_friends.Add(user.Id, user);
			}
			
		} catch(System.Exception ex) {
			Debug.LogWarning("Parceing Friends Data failed");
			Debug.LogWarning(ex.Message);
		}
	}
	
	


	
	public void ParseFriendsData(string data) {
		ParseFriendsFromJson (data, _friends);		
	}


	

	//--------------------------------------
	//  External Event Handlets
	//--------------------------------------

	public void OnInitComplete() {
		_IsInited = true;
		OnInitCompleteAction();
		Debug.Log("FB.Init completed: Is user logged in? " + IsLoggedIn);
	}
	
	
	
	public void OnHideUnity(bool isGameShown) {
		OnFocusChangedAction(isGameShown);
	}

	public void LoginCallback(FB_LoginResult result) {
		OnAuthCompleteAction(result);
	}

	private void PostCallback_Internal(FB_Result result) {
		FB_PostResult pr = new FB_PostResult(result.RawData, result.Error);
		OnPostingCompleteAction(pr);
	}

	public void PostCallback(FB_PostResult result) {
		OnPostingCompleteAction(result);
	}

	public void AppRequestCallback(FB_AppRequestResult result) {
		OnAppRequestCompleteAction(result);
	}
	
	//--------------------------------------
	//  Internal Event Handlets
	//--------------------------------------
	
	private void UserDataCallBack(FB_Result result) {
		
		if (result.IsFailed)  {         
			Debug.LogWarning(result.Error);
		}   else {
			Debug.Log("[UserDataCallBack] result.RawData " + result.RawData);
			_userInfo = new FB_UserInfo(result.RawData);
		}       
		OnUserDataRequestCompleteAction(result);
	}

	private void InvitableFriendsDataCallBack(FB_Result result) {
		
		if (result.IsFailed)  {                                                                                                                          
			Debug.LogWarning(result.Error);
		}  else {
			ParseInvitableFriendsData(result.RawData);
		}        
		
		OnInvitableFriendsDataRequestCompleteAction(result);
	}


	private void FriendsDataCallBack(FB_Result result) {
		if (result.IsFailed)  {                                                                                                                          
			Debug.LogWarning(result.Error);
		}  else {
			ParseFriendsData(result.RawData);
		}        
		
		OnFriendsDataRequestCompleteAction(result);
	}



	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------
	
}