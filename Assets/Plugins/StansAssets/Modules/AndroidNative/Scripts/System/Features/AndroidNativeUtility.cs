using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class AndroidNativeUtility : SA.Common.Pattern.Singleton<AndroidNativeUtility> {
	

	//Actions
	public static event Action<AN_PackageCheckResult> OnPackageCheckResult = delegate{};
	public static event Action<string> OnAndroidIdLoaded = delegate{};

	public static event Action<string> InternalStoragePathLoaded = delegate{};
	public static event Action<string> ExternalStoragePathLoaded = delegate{};


	public static event Action<AN_Locale> LocaleInfoLoaded = delegate{};
	public static event Action<string[]> ActionDevicePackagesListLoaded = delegate{};
	public static event Action<AN_NetworkInfo> ActionNetworkInfoLoaded = delegate{};

	public static event Action<AN_RefreshTokenResult> OnOAuthRefreshTokenLoaded = delegate {};
	public static event Action<AN_AccessTokenResult> OnOAuthAccessTokenLoaded = delegate {};
	public static event Action<AN_DeviceCodeResult> OnDeviceCodeLoaded = delegate {};
	
	private string _redirectUrl = string.Empty;
	private string _clientId = string.Empty;
	private string _clientSecret = string.Empty;
	
	//--------------------------------------
	// Init
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	//--------------------------------------
	// Public Methods
	//--------------------------------------

	public void GenerateRefreshToken(string redirectUrl, string clientId, string clientSecret) {
		_redirectUrl = redirectUrl;
		_clientId = clientId;
		_clientSecret = clientSecret;

		AndroidNative.GenerateRefreshToken (_redirectUrl, _clientId);
	}

	public void RefreshOAuthToken(string refreshToken, string clientId, string clientSecret) {
		StartCoroutine(RefreshOAuthTokenRequest(clientId, clientSecret, refreshToken));
	}

	public void ObtainUserDeviceCode(string clientId) {
		StartCoroutine (ObtainUserDeviceCodeRequest (clientId));
	}
	
	public void CheckIsPackageInstalled(string packageName) {
		AndroidNative.isPackageInstalled(packageName);
	}
	

	public void StartApplication(string bundle) {
		AndroidNative.runPackage(bundle);
	}

	public void StartApplication(string packageName, Dictionary<string, string> extras) {
		StringBuilder builder = new StringBuilder();
		foreach (KeyValuePair<string, string> extra in extras) {
			builder.AppendFormat("{0}{1}{2}", extra.Key, AndroidNative.DATA_SPLITTER, extra.Value);
			builder.Append(AndroidNative.DATA_SPLITTER2);
		}
		builder.Append(AndroidNative.DATA_EOF);

		Debug.Log("[StartApplication] with Extras " + builder.ToString());

		AndroidNative.LaunchApplication(packageName, builder.ToString());
	}

	public void LoadAndroidId() {
		AndroidNative.LoadAndroidId();
	}

	public void GetInternalStoragePath() {
		AndroidNative.GetInternalStoragePath();
	}
	
	public void GetExternalStoragePath() {
		AndroidNative.GetExternalStoragePath();
	}

	public string GetExternalStoragePublicDirectory(AN_ExternalStorageType type) {
		return AndroidNative.GetExternalStoragePublicDirectory(type.ToString());
	}

	public void LoadLocaleInfo() {
		AndroidNative.LoadLocaleInfo();
	}

	public void LoadPackagesList() {
		AndroidNative.LoadPackagesList();
	}


	public void LoadNetworkInfo() {
		AndroidNative.LoadNetworkInfo();
	}


	
	//--------------------------------------
	// Static Methods
	//--------------------------------------

	public static void OpenSettingsPage(string action) {
		AndroidNative.OpenSettingsPage(action);
	}

	public static void ShowPreloader(string title, string message) {
		AN_PoupsProxy.ShowPreloader(title, message, AndroidNativeSettings.Instance.DialogTheme);
	}

    public static void ShowPreloader(string title, string message, AndroidDialogTheme theme) {
        AN_PoupsProxy.ShowPreloader(title, message, AndroidNativeSettings.Instance.DialogTheme);
    }

    public static void HidePreloader() {
		AN_PoupsProxy.HidePreloader();
	}


	public static void OpenAppRatingPage(string url) {
		AN_PoupsProxy.OpenAppRatePage(url);
	}


	public static void RedirectToGooglePlayRatingPage(string url) {
		OpenAppRatingPage(url);
	}


	public static void HideCurrentPopup() {
		AN_PoupsProxy.HideCurrentPopup();
	}

	
	public static int SDKLevel {
		get {
			#if UNITY_ANDROID
			var clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
			var fieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
			var sdkLevel = AndroidJNI.GetStaticIntField(clazz, fieldID);
			return sdkLevel;
			#else
			return 0;
			#endif
		}
	}


	public static void InvitePlusFriends() {
		AndroidNative.InvitePlusFriends();
	}

	


	//--------------------------------------
	// Event Handlers
	//--------------------------------------

	private void RefreshTokenCodeReceived(string data) {
		Debug.Log (data);
		string[] rawData = data.Split (new string[] { "|" }, StringSplitOptions.None);
		int status = Int32.Parse (rawData [0]);

		if (status == 1) {
			StartCoroutine (GenerateRefreshTokenRequest (rawData [1], _clientId, _clientSecret, _redirectUrl));
		} else {
			AN_RefreshTokenResult result = new AN_RefreshTokenResult ("Request Authorization Code error");
			OnOAuthRefreshTokenLoaded (result);
		}
	}

	private IEnumerator GenerateRefreshTokenRequest(string code, string clientId, string clientSecret, string redirectUrl) {

		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("grant_type", "authorization_code");
		requestForm.AddField ("code", code);
		requestForm.AddField ("client_id", clientId);
		requestForm.AddField ("client_secret", clientSecret);
		requestForm.AddField ("redirect_uri", redirectUrl);

		WWW response = new WWW ("https://accounts.google.com/o/oauth2/token", requestForm);
		yield return response;

		if (string.IsNullOrEmpty (response.error)) {
			Dictionary<string, object> data = ANMiniJSON.Json.Deserialize (response.text) as Dictionary<string, object>;
			string access_token = data.ContainsKey ("access_token") ? data ["access_token"].ToString () : string.Empty;
			string refresh_token = data.ContainsKey ("refresh_token") ? data ["refresh_token"].ToString () : string.Empty;
			string token_type = data.ContainsKey ("token_type") ? data ["token_type"].ToString () : string.Empty;
			long expiresIn = data.ContainsKey ("expires_in") ? (long)data ["expires_in"] : 0L;

			AN_RefreshTokenResult result = new AN_RefreshTokenResult (access_token, refresh_token, token_type, expiresIn);
			OnOAuthRefreshTokenLoaded (result);
		} else {
			AN_RefreshTokenResult result = new AN_RefreshTokenResult (response.error);
			OnOAuthRefreshTokenLoaded (result);
		}
	}

	private IEnumerator RefreshOAuthTokenRequest(string clientId, string clientSecret, string refreshToken) {

		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("grant_type", "refresh_token");
		requestForm.AddField ("client_id", clientId);
		requestForm.AddField ("client_secret", clientSecret);
		requestForm.AddField ("refresh_token", refreshToken);

		WWW response = new WWW ("https://accounts.google.com/o/oauth2/token", requestForm);
		yield return response;

		if (string.IsNullOrEmpty (response.error)) {
			Dictionary<string, object> data = ANMiniJSON.Json.Deserialize (response.text) as Dictionary<string, object>;
			string access_token = data.ContainsKey ("access_token") ? data ["access_token"].ToString () : string.Empty;
			string token_type = data.ContainsKey ("token_type") ? data ["token_type"].ToString () : string.Empty;
			long expiresIn = data.ContainsKey ("expires_in") ? (long)data ["expires_in"] : 0L;

			AN_AccessTokenResult result = new AN_AccessTokenResult (access_token, token_type, expiresIn);
			OnOAuthAccessTokenLoaded (result);
		} else {
			AN_AccessTokenResult result = new AN_AccessTokenResult (response.error);
			OnOAuthAccessTokenLoaded (result);
		}
	}

	private IEnumerator ObtainUserDeviceCodeRequest(string clientId) {

		WWWForm requestForm = new WWWForm ();
		requestForm.AddField ("client_id", clientId);
		requestForm.AddField ("scope", "email profile");

		WWW response = new WWW ("https://accounts.google.com/o/oauth2/device/code", requestForm);
		yield return response;

		Debug.Log (response.text);

		if (string.IsNullOrEmpty (response.error)) {
			Dictionary<string, object> data = ANMiniJSON.Json.Deserialize (response.text) as Dictionary<string, object>;
			string device_code = data.ContainsKey ("device_code") ? data ["device_code"].ToString () : string.Empty;
			string user_code = data.ContainsKey ("user_code") ? data ["user_code"].ToString () : string.Empty;
			string verification_url = data.ContainsKey ("verification_url") ? data ["verification_url"].ToString () : string.Empty;
			long expires_in = data.ContainsKey ("expires_in") ? (long)data ["expires_in"] : 0L;
			long interval = data.ContainsKey ("interval") ? (long)data ["interval"] : 0L;

			AN_DeviceCodeResult result = new AN_DeviceCodeResult (device_code, user_code, verification_url, expires_in, interval);
			OnDeviceCodeLoaded (result);
		} else {
			AN_DeviceCodeResult result = new AN_DeviceCodeResult (response.error);
			OnDeviceCodeLoaded (result);
		}
	}

	private void OnAndroidIdLoadedEvent(string id) {
		OnAndroidIdLoaded(id);
	}

	private void OnPacakgeFound(string packageName) {
		AN_PackageCheckResult result = new AN_PackageCheckResult(packageName);
		OnPackageCheckResult(result);
	}

	private void OnPacakgeNotFound(string packageName) {
		AN_PackageCheckResult result = new AN_PackageCheckResult(packageName, new SA.Common.Models.Error(0, "Pacakge not Found"));
		OnPackageCheckResult(result);
	}


	private void OnExternalStoragePathLoaded(string path) {
		ExternalStoragePathLoaded(path);
	}

	private void OnInternalStoragePathLoaded(string path) {
		InternalStoragePathLoaded(path);
	}


	private void OnLocaleInfoLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		AN_Locale locale =  new AN_Locale();
		locale.CountryCode = storeData[0];
		locale.DisplayCountry = storeData[1];

		locale.LanguageCode = storeData[2];
		locale.DisplayLanguage = storeData[3];

		LocaleInfoLoaded(locale);

	}

	private void OnPackagesListLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		ActionDevicePackagesListLoaded(storeData);
	}

	private void OnNetworkInfoLoaded (string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		AN_NetworkInfo info =  new AN_NetworkInfo();
		info.SubnetMask = storeData[0];
		info.IpAddress = storeData[1];
		info.MacAddress =  storeData[2];
		info.SSID = storeData[3];
		info.BSSID = storeData[4];

		info.LinkSpeed = System.Convert.ToInt32(storeData[5]);
		info.NetworkId = System.Convert.ToInt32(storeData[6]);

		ActionNetworkInfoLoaded(info);
	
	}
	


}

