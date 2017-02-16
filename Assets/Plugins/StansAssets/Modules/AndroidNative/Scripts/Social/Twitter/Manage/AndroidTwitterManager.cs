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

public class AndroidTwitterManager : SA.Common.Pattern.Singleton<AndroidTwitterManager>, TwitterManagerInterface {


	private bool _IsAuthed = false;
	private bool _IsInited = false;

	private string _AccessToken = string.Empty;
	private string _AccessTokenSecret = string.Empty;

	private TwitterUserInfo _userInfo;


	//Actinos
	public event Action OnTwitterLoginStarted 						= delegate {};
	public event Action OnTwitterLogOut 							= delegate {};
	public event Action OnTwitterPostStarted						= delegate {};


	public event Action<TWResult> OnTwitterInitedAction 			= delegate {};
	public event Action<TWResult> OnAuthCompleteAction 				= delegate {};
	public event Action<TWResult> OnPostingCompleteAction 			= delegate {};
	public event Action<TWResult> OnUserDataRequestCompleteAction 	= delegate {};





	// --------------------------------------
	// INITIALIZATION
	// --------------------------------------

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
			Debug.LogError("AndroidNative: Soomla Initalization failed" + ex.Message);
		}
		
		#endif


		Init(SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY, SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET);
	}

	public void Init(string consumer_key, string consumer_secret) {
		if(_IsInited) {
			return;
		}

		_IsInited = true;
		AndroidNative.TwitterInit(consumer_key, consumer_secret);
	}


	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------

	


	public void AuthenticateUser() {
		OnTwitterLoginStarted();

		if(_IsAuthed) {
			OnAuthSuccess();
		} else {
			AndroidNative.AuthificateUser();
		}
	}

	public void LoadUserData() {
		if(_IsAuthed) {
			AndroidNative.LoadUserData();
		} else {
			Debug.LogWarning("Auth user before loadin data, fail event generated");

			TWResult res =  new TWResult(false, null);
			OnUserDataRequestCompleteAction(res);
		}
	}
	
	public void Post(string status) {
		OnTwitterPostStarted();


		if(!_IsAuthed) {
			Debug.LogWarning("Auth user before posting data, fail event generated");
			TWResult res =  new TWResult(false, null);
			OnPostingCompleteAction(res);
			return;
		} 


		AndroidNative.TwitterPost(status);
	}

	public void Post(string status, Texture2D texture) {

		OnTwitterPostStarted();

		if(!_IsAuthed) {
			Debug.LogWarning("Auth user before posting data, fail event generated");
			TWResult res =  new TWResult(false, null);
			OnPostingCompleteAction(res);
			return;
		} 


		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);

		AndroidNative.TwitterPostWithImage(status, bytesString);
	}


	public TwitterPostingTask PostWithAuthCheck(string status)  {
		return PostWithAuthCheck(status, null);
	}

	public TwitterPostingTask PostWithAuthCheck(string status, Texture2D texture) {
		TwitterPostingTask task =  TwitterPostingTask.Cretae();
		task.Post(status, texture, this);
		return task;

	}

	public void LogOut() {
		OnTwitterLogOut();

		_IsAuthed = false;
		AndroidNative.LogoutFromTwitter();
	}

	// --------------------------------------
	// GET / SET
	// --------------------------------------

	public bool IsAuthed {
		get {
			return _IsAuthed;
		}
	}

	public bool IsInited {
		get {
			return _IsInited;
		}
	}

	public TwitterUserInfo userInfo {
		get {
			return _userInfo;
		}
	}

	public string AccessToken {
		get {
			return _AccessToken;
		}
	}

	public string AccessTokenSecret {
		get {
			return _AccessTokenSecret;
		}
	}
	
	// --------------------------------------
	// EVENTS
	// --------------------------------------



	private void OnInited(string data) {
		if(data.Equals("1")) {
			_IsAuthed = true;
		}

		TWResult res =  new TWResult(true, null);
		OnTwitterInitedAction(res);
	}

	private void OnAuthSuccess() {
		_IsAuthed = true;
		TWResult res =  new TWResult(true, null);
		OnAuthCompleteAction(res);
	}


	private void OnAuthFailed() {
		TWResult res =  new TWResult(false, null);
		OnAuthCompleteAction(res);
	}

	private void OnPostSuccess() {
		TWResult res =  new TWResult(true, null);
		OnPostingCompleteAction(res);
	}
	
	
	private void OnPostFailed() {
		TWResult res =  new TWResult(false, null);
		OnPostingCompleteAction(res);
	}


	private void OnUserDataLoaded(string data) {
		_userInfo =  new TwitterUserInfo(data);

		TWResult res =  new TWResult(true, data);
		OnUserDataRequestCompleteAction(res);


	}


	private void OnUserDataLoadFailed() {
		TWResult res =  new TWResult(false, null);
		OnUserDataRequestCompleteAction(res);
	}

	private void OnAuthInfoReceived(string data) {

		Debug.Log("OnAuthInfoReceived");

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		_AccessToken = storeData[0];
		_AccessTokenSecret = storeData[1];

	}

}

