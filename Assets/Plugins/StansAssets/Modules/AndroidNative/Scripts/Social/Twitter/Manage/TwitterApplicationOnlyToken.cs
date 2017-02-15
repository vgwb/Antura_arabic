using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TwitterApplicationOnlyToken : SA.Common.Pattern.Singleton<TwitterApplicationOnlyToken> {

	public event Action ActionComplete = delegate{};


	private string _currentToken = null;

	private const string  TWITTER_CONSUMER_KEY = "wEvDyAUr2QabVAsWPDiGwg";
	private const string  TWITTER_CONSUMER_SECRET = "igRxZbOrkLQPNLSvibNC3mdNJ5tOlVOPH3HNNKDY0";


	private const string  BEARER_TOKEN_KEY = "bearer_token_key";


	#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 
	private Hashtable Headers = new Hashtable();
	#else
	private  Dictionary<string, string> Headers = new Dictionary<string, string>();
	#endif



	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public void RetrieveToken() {
		StartCoroutine(Load());
	}

	public string currentToken {
		get {
			 
			if(_currentToken == null) {
				if(PlayerPrefs.HasKey(BEARER_TOKEN_KEY)) {
					_currentToken = PlayerPrefs.GetString(BEARER_TOKEN_KEY);
				}
			}

			return _currentToken;
		}
	}




	private IEnumerator Load () {
		
		string url = "https://api.twitter.com/oauth2/token";
		
		
		//byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(TWITTER_CONSUMER_KEY +  ":" + TWITTER_CONSUMER_SECRET);
		byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY +  ":" + SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET);
		string encodedAccessToken =  System.Convert.ToBase64String (plainTextBytes);
		
		Headers.Clear();
		Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
		Headers.Add("Authorization", "Basic " + encodedAccessToken);
		
		
		
		WWWForm form = new WWWForm();
		form.AddField("grant_type", "client_credentials");
		
		
		
		WWW www = new WWW(url, form.data, Headers);
		
		yield return www;



		if(www.error == null) {
			Dictionary<string, object> map =  ANMiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
			_currentToken = map["access_token"] as string;
			PlayerPrefs.SetString(BEARER_TOKEN_KEY, _currentToken);
		} 

		ActionComplete();
		
	}
}
