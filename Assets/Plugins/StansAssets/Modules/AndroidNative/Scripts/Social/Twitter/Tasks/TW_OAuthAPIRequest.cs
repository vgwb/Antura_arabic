using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;

public class TW_OAuthAPIRequest : MonoBehaviour {


	public event Action<TW_APIRequstResult> OnResult =  delegate {};

	private bool IsFirst = true;
	private string GetParams = string.Empty;




	private string requestUrl;

	#if UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 
	private Hashtable Headers = new Hashtable();
	#else
	private  Dictionary<string, string> Headers = new Dictionary<string, string>();
	#endif


	private SortedDictionary<string, string> requestParams = new SortedDictionary<string, string>();

	// --------------------------------------
	// Pulic Methods
	// --------------------------------------


	public static TW_OAuthAPIRequest Create() {
		return new GameObject("TW_OAuthAPIRequest").AddComponent<TW_OAuthAPIRequest>();
	}



	public void Send(string url) {
		requestUrl = url;
		StartCoroutine(Request());

	}
	

	public void AddParam(string name, int value) {
		AddParam(name, value.ToString());
	}

	public void AddParam(string name, string value) {


		if(!IsFirst) {
			GetParams += "&";
		} else {
			GetParams += "?";
		}

		GetParams += name + "=" + value;


		IsFirst = false;

		requestParams.Add(name, value);
	}




	// --------------------------------------
	// Protected Methods
	// --------------------------------------


	protected void SetUrl(string url) {
		requestUrl = url;
	}

	private IEnumerator Request () {


		TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);



		string oauth_consumer_key = SocialPlatfromSettings.Instance.TWITTER_CONSUMER_KEY;
		string oauth_token = string.Empty;
		#if UNITY_EDITOR
		oauth_token = SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN;
		#else
		oauth_token = AndroidTwitterManager.Instance.AccessToken;
		#endif


		string oauth_signature_method = "HMAC-SHA1";
		string oauth_timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
		string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
		string oauth_version = "1.0";



		requestParams.Add("oauth_version", oauth_version);
		requestParams.Add("oauth_consumer_key", oauth_consumer_key);
		requestParams.Add("oauth_nonce", oauth_nonce);
		requestParams.Add("oauth_signature_method", oauth_signature_method);
		requestParams.Add("oauth_timestamp", oauth_timestamp);
		requestParams.Add("oauth_token", oauth_token);


		string baseString = String.Empty;
		baseString += "GET" + "&";
		baseString += Uri.EscapeDataString(requestUrl) + "&";
		foreach (KeyValuePair<string,string> entry in requestParams){
			baseString += Uri.EscapeDataString(entry.Key +  "=" + entry.Value + "&");
		}
		
		//GS - Remove the trailing ambersand char, remember 
		//it's been urlEncoded so you have to remove the 
		//last 3 chars - %26
		baseString = baseString.Substring(0, baseString.Length - 3);

		//GS - Build the signing key
		string consumerSecret = SocialPlatfromSettings.Instance.TWITTER_CONSUMER_SECRET;
		string oauth_token_secret = string.Empty;

		#if UNITY_EDITOR
		oauth_token_secret = SocialPlatfromSettings.Instance.TWITTER_ACCESS_TOKEN_SECRET;
		#else
		oauth_token_secret = AndroidTwitterManager.Instance.AccessTokenSecret;
		#endif


#if !UNITY_WP8 && !UNITY_METRO
		string signingKey = Uri.EscapeDataString(consumerSecret) + "&" + Uri.EscapeDataString(oauth_token_secret);
		//GS - Sign the request
		HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
		
		string signatureString = Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));
		
	
		string authorizationHeaderParams = String.Empty;
		authorizationHeaderParams += "OAuth ";
		authorizationHeaderParams += "oauth_nonce=" + "\"" + Uri.EscapeDataString(oauth_nonce) + "\",";
		authorizationHeaderParams += "oauth_signature_method=" + "\"" + Uri.EscapeDataString(oauth_signature_method) + "\",";
		authorizationHeaderParams += "oauth_timestamp=" + "\"" + Uri.EscapeDataString(oauth_timestamp) + "\",";
		authorizationHeaderParams += "oauth_consumer_key=" 	+ "\"" + Uri.EscapeDataString(oauth_consumer_key) + "\",";
		authorizationHeaderParams += "oauth_token=" + "\"" + Uri.EscapeDataString(oauth_token) + "\",";
		authorizationHeaderParams += "oauth_signature=" + "\"" + Uri.EscapeDataString(signatureString) + "\",";
		authorizationHeaderParams += "oauth_version=" + "\"" + 	Uri.EscapeDataString(oauth_version) + "\"";


		requestUrl = requestUrl + GetParams;
		Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
		Headers.Add("Authorization", authorizationHeaderParams);

#endif

		WWW www = new WWW(requestUrl, null,  Headers);
		yield return www;


		TW_APIRequstResult result;

		if(www.error == null) {
			result =  new TW_APIRequstResult(true, www.text);
		} else {
			result =  new TW_APIRequstResult(false, www.error);
		}

		OnResult(result);


		Destroy(gameObject);
	}



	// --------------------------------------
	// Events
	// --------------------------------------



}
