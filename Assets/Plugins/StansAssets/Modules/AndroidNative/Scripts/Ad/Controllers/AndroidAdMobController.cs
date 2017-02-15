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
using System.Collections.Generic;

public class AndroidAdMobController : SA.Common.Pattern.Singleton<AndroidAdMobController>, GoogleMobileAdInterface  {

	
	private bool _IsInited = false ;
	private Dictionary<int, AndroidADBanner> _banners; 

	private bool _IsEditorTestingEnabled = true;
	private int _EditorFillRate = 100;
	
	private string _BannersUunitId;
	private string _InterstisialUnitId;
	private string _RewardedVideoAdUnitId;

	//Actions
	public event Action<string, int> OnRewarded 				= delegate {};
	public event Action OnRewardedVideoAdClosed 				= delegate {};
	public event Action<int> OnRewardedVideoAdFailedToLoad 		= delegate {};
	public event Action OnRewardedVideoAdLeftApplication 		= delegate {};
	public event Action OnRewardedVideoLoaded 					= delegate {};
	public event Action OnRewardedVideoAdOpened 				= delegate {};
	public event Action OnRewardedVideoStarted 					= delegate {};

	public event Action OnInterstitialLoaded 			= delegate {};
	public event Action OnInterstitialFailedLoading 	= delegate {};
	public event Action OnInterstitialOpened 			= delegate {};
	public event Action OnInterstitialClosed 			= delegate {};
	public event Action OnInterstitialLeftApplication 	= delegate {};
	public event Action<string> OnAdInAppRequest		= delegate {};



	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus) {
			foreach (KeyValuePair<int, AndroidADBanner> banner in _banners) {
				banner.Value.Pause();
			}
		} else {
			foreach (KeyValuePair<int, AndroidADBanner> banner in _banners) {
				banner.Value.Resume();
			}
		}
	}

	public void Init(string ad_unit_id) {
		if(_IsInited) {
			Debug.LogWarning ("Init shoudl be called only once. Call ignored");
			return;
		}
		_IsInited = true;

		_BannersUunitId 	= ad_unit_id;
		_InterstisialUnitId = ad_unit_id;
		_RewardedVideoAdUnitId = ad_unit_id;

		_banners =  new Dictionary<int, AndroidADBanner>();

		if (IsEditorTestingEnabled) {
			Debug.Log("Initialized with Editor Testing Profile");
			SA_EditorAd.Instance.SetFillRate(_EditorFillRate);
			return;
		}

		AN_GoogleAdProxy.InitMobileAd(ad_unit_id);
	}


	public void Init(string banners_unit_id, string interstisial_unit_id) {
		if(_IsInited) {
			Debug.LogWarning ("Init shoudl be called only once. Call ignored");
			return;
		}
		
		Init(banners_unit_id);
		SetInterstisialsUnitID(interstisial_unit_id);
	}

	public void InitEditorTesting (bool isTestingEnabled, int editorFillRate) {
		_IsEditorTestingEnabled = isTestingEnabled;
		_EditorFillRate = editorFillRate;
	}

	public void SetBannersUnitID(string ad_unit_id) {
		_BannersUunitId = ad_unit_id;
		AN_GoogleAdProxy.ChangeBannersUnitID(ad_unit_id);
	}

	public void SetInterstisialsUnitID(string ad_unit_id) {
		_InterstisialUnitId = ad_unit_id;
		AN_GoogleAdProxy.ChangeInterstisialsUnitID(ad_unit_id);
	}

	public void SetRewardedVideoAdUnitID(string id) {
		_RewardedVideoAdUnitId = id;
		AN_GoogleAdProxy.ChangeRewardedVideoUnitID(_RewardedVideoAdUnitId);
	}

	//--------------------------------------
	//  BUILDER METHODS
	//--------------------------------------



	//Add a keyword for targeting purposes.
	public void AddKeyword(string keyword)  {
		if(!_IsInited) {
			Debug.LogWarning ("AddKeyword shoudl be called only after Init function. Call ignored");
			return;
		}

		AN_GoogleAdProxy.AddKeyword(keyword);
	}


	public void SetBirthday(int year, AndroidMonth month, int day)  {
		if(!_IsInited) {
			Debug.LogWarning ("SetBirthday shoudl be called only after Init function. Call ignored");
			return;
		}
		
		AN_GoogleAdProxy.SetBirthday(year, (int) month, day);
	}

	public void TagForChildDirectedTreatment(bool tagForChildDirectedTreatment)  {
		if(!_IsInited) {
			Debug.LogWarning ("TagForChildDirectedTreatment shoudl be called only after Init function. Call ignored");
			return;
		}

		AN_GoogleAdProxy.TagForChildDirectedTreatment(tagForChildDirectedTreatment);
	}



	//Causes a device to receive test ads. The deviceId can be obtained by viewing the logcat output after creating a new ad.
	public void AddTestDevice(string deviceId) {
		if(!_IsInited) {
			Debug.LogWarning ("AddTestDevice shoudl be called only after Init function. Call ignored");
			return;
		}

		AN_GoogleAdProxy.AddTestDevice(deviceId);
	}


	private const string DEVICES_SEPARATOR = ",";
	//Causes a device to receive test ads. The deviceId can be obtained by viewing the logcat output after creating a new ad.
	public void AddTestDevices(params string[] ids) {
		if(!_IsInited) {
			Debug.LogWarning ("AddTestDevice shoudl be called only after Init function. Call ignored");
			return;
		}

		if(ids.Length == 0) {
			return;
		}


		AN_GoogleAdProxy.AddTestDevices(string.Join(DEVICES_SEPARATOR, ids));
	}



	//Set the user's gender for targeting purposes. This should be GADGenger.GENDER_MALE, GADGenger.GENDER_FEMALE, or GADGenger.GENDER_UNKNOWN
	public void SetGender(GoogleGender gender) {
		if(!_IsInited) {
			Debug.LogWarning ("SetGender shoudl be called only after Init function. Call ignored");
			return;
		}

		AN_GoogleAdProxy.SetGender((int) gender);
	}




	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	public GoogleMobileAdBanner CreateAdBanner(TextAnchor anchor, GADBannerSize size)  {
		if(!_IsInited) {
			Debug.LogWarning ("CreateBannerAd shoudl be called only after Init function. Call ignored");
			return null;
		}

		AndroidADBanner bannner = new AndroidADBanner(anchor, size, GADBannerIdFactory.nextId);
		_banners.Add(bannner.id, bannner);

		return bannner;
		
	}


	public GoogleMobileAdBanner CreateAdBanner(int x, int y, GADBannerSize size)  {
		if(!_IsInited) {
			Debug.LogWarning ("CreateBannerAd shoudl be called only after Init function. Call ignored");
			return null;
		}
		
		AndroidADBanner bannner = new AndroidADBanner(x, y, size, GADBannerIdFactory.nextId);
		_banners.Add(bannner.id, bannner);
		
		return bannner;
		
	}



	public void DestroyBanner(int id) {
		if(_banners != null) {
			if(_banners.ContainsKey(id)) {

				AndroidADBanner banner = _banners[id];
				if(banner.IsLoaded) {
					_banners.Remove(id);
					AN_GoogleAdProxy.DestroyBanner(id);
				} else {
					banner.DestroyAfterLoad();
				}

			}
		}
	}

	private bool _InterstitialShowOnLoad = false;
	public void StartInterstitialAd() {
		if(!_IsInited) {
			Debug.LogWarning ("StartInterstitialAd shoudl be called only after Init function. Call ignored");
			return;
		}

		if (IsEditorTestingEnabled) {
			_InterstitialShowOnLoad = true;
			SA_EditorAd.OnInterstitialLoadComplete += HandleOnInterstitialLoadComplete_Editor;
			SA_EditorAd.Instance.LoadInterstitial();
			return;
		}

		AN_GoogleAdProxy.StartInterstitialAd();
	}
	
	public void LoadInterstitialAd() {
		if(!_IsInited) {
			Debug.LogWarning ("LoadInterstitialAd shoudl be called only after Init function. Call ignored");
			return;
		}

		if (IsEditorTestingEnabled) {
			SA_EditorAd.OnInterstitialLoadComplete += HandleOnInterstitialLoadComplete_Editor;
			SA_EditorAd.Instance.LoadInterstitial();
			return;
		}

		AN_GoogleAdProxy.LoadInterstitialAd();
	}

	private void HandleOnInterstitialLoadComplete_Editor (bool success)
	{
		SA_EditorAd.OnInterstitialLoadComplete -= HandleOnInterstitialLoadComplete_Editor;
		if (success) {
			OnInterstitialLoaded();
			if (_InterstitialShowOnLoad) {
				_InterstitialShowOnLoad = false;
				ShowInterstitialAd();
			}
		} else {
			OnInterstitialFailedLoading();
		}
	}
	
	public void ShowInterstitialAd() {
		if(!_IsInited) {
			Debug.LogWarning ("ShowInterstitialAd shoudl be called only after Init function. Call ignored");
			return;
		}

		if (IsEditorTestingEnabled) {
			SA_EditorAd.OnInterstitialLeftApplication += HandleOnInterstitialLeftApplication_Editor;
			SA_EditorAd.OnInterstitialFinished += HandleOnInterstitialFinished_Editor;
			SA_EditorAd.Instance.ShowInterstitial();
			OnInterstitialOpened();
			return;
		}

		AN_GoogleAdProxy.ShowInterstitialAd();
	}

	void HandleOnInterstitialFinished_Editor (bool isRewarded)
	{
		SA_EditorAd.OnInterstitialLeftApplication -= HandleOnInterstitialLeftApplication_Editor;
		SA_EditorAd.OnInterstitialFinished -= HandleOnInterstitialFinished_Editor;
		OnInterstitialClosed();
	}

	void HandleOnInterstitialLeftApplication_Editor ()
	{
		OnInterstitialLeftApplication();
	}

	private bool _RewardedVideoShowOnLoad = false;
	public void StartRewardedVideo() {
		if(!_IsInited) {
			Debug.LogWarning ("StartRewardedVideo shoudl be called only after Init function. Call ignored");
			return;
		}

		_RewardedVideoShowOnLoad = true;
		LoadRewardedVideo ();
	}

	public void LoadRewardedVideo() {
		if(!_IsInited) {
			Debug.LogWarning ("ShowRewardedVideo shoudl be called only after Init function. Call ignored");
			return;
		}

		if (IsEditorTestingEnabled) {
			SA_EditorAd.OnVideoLoadComplete += HandleOnVideoLoadComplete_Editor;
			SA_EditorAd.Instance.LoadVideo();
			return;
		}
		
		AN_GoogleAdProxy.LoadRewardedVideo();
	}

	void HandleOnVideoLoadComplete_Editor (bool success)
	{
		SA_EditorAd.OnVideoLoadComplete -= HandleOnVideoLoadComplete_Editor;

		if (success) {
			OnRewardedVideoLoaded();

			if (_RewardedVideoShowOnLoad) {
				_RewardedVideoShowOnLoad = false;
				ShowRewardedVideo ();
			}
		} else {
			OnRewardedVideoAdFailedToLoad(-1);
		}
	}

	public void ShowRewardedVideo() {
		if(!_IsInited) {
			Debug.LogWarning ("ShowRewardedVideo shoudl be called only after Init function. Call ignored");
			return;
		}

		if (IsEditorTestingEnabled) {
			SA_EditorAd.OnVideoLeftApplication += HandleOnVideoLeftApplication_Editor;
			SA_EditorAd.OnVideoFinished += HandleOnVideoFinished_Editor;
			SA_EditorAd.Instance.ShowVideo();
			OnRewardedVideoAdOpened();
			return;
		}
		
		AN_GoogleAdProxy.ShowRewardedVideo();
	}

	void HandleOnVideoFinished_Editor (bool isRewarded)
	{
		SA_EditorAd.OnVideoLeftApplication -= HandleOnVideoLeftApplication_Editor;
		SA_EditorAd.OnVideoFinished -= HandleOnVideoFinished_Editor;
		OnRewardedVideoAdClosed();
	}

	void HandleOnVideoLeftApplication_Editor ()
	{
		OnRewardedVideoAdLeftApplication();
	}

	public void RecordInAppResolution(GADInAppResolution resolution) {
		AN_GoogleAdProxy.RecordInAppResolution((int) resolution);
	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	public GoogleMobileAdBanner GetBanner(int id) {
		if(_banners.ContainsKey(id)) {
			return _banners[id];
		} else {
			Debug.LogWarning("Banner id: " + id.ToString() + " not found");
			return null;
		}
	}


	public List<GoogleMobileAdBanner> banners {
		get {

			List<GoogleMobileAdBanner> allBanners =  new List<GoogleMobileAdBanner>();
			if(_banners ==  null) {
				return allBanners;
			}

			foreach(KeyValuePair<int, AndroidADBanner> entry in _banners) {
				allBanners.Add(entry.Value);
			}

			return allBanners;


		}
	}

	public bool IsInited {
		get {
			return _IsInited;
		}
	}

	public string BannersUunitId {
		get {
			return _BannersUunitId;
		}
	}

	public string InterstisialUnitId {
		get {
			return _InterstisialUnitId;
		}
	}	

	public string RewardedVideoAdUnitId {
		get {
			return _RewardedVideoAdUnitId;
		}
	}

	public bool IsEditorTestingEnabled {
		get {
			return SA_EditorTesting.IsInsideEditor && _IsEditorTestingEnabled;
		}
	}

	//--------------------------------------
	//  EVENTS BANNER AD
	//--------------------------------------
	

	private void OnBannerAdLoaded(string data)  {

		string[] bannerData;
		bannerData = data.Split(AndroidNative.DATA_SPLITTER [0]);


		int id = System.Convert.ToInt32(bannerData[0]);
		int w = System.Convert.ToInt32(bannerData[1]);
		int h = System.Convert.ToInt32(bannerData[2]);

		AndroidADBanner banner = GetBanner(id) as AndroidADBanner;
		if(banner != null) {
			banner.SetDimentions(w, h);
			banner.OnBannerAdLoaded();
		}
	
	}
	
	private void OnBannerAdFailedToLoad(string bannerID) {
		int id = System.Convert.ToInt32(bannerID);
		AndroidADBanner banner = GetBanner(id) as AndroidADBanner;
		if(banner != null) {
			banner.OnBannerAdFailedToLoad();
		}
	}
	
	private void OnBannerAdOpened(string bannerID) {
		int id = System.Convert.ToInt32(bannerID);
		AndroidADBanner banner = GetBanner(id) as AndroidADBanner;
		if(banner != null) {
			banner.OnBannerAdOpened();
		}
	}

	private void OnBannerAdClosed(string bannerID) {
		int id = System.Convert.ToInt32(bannerID);
		AndroidADBanner banner = GetBanner(id) as AndroidADBanner;
		if(banner != null) {
			banner.OnBannerAdClosed();
		}
	}

	private void OnBannerAdLeftApplication(string bannerID) {
		int id = System.Convert.ToInt32(bannerID);
		AndroidADBanner banner = GetBanner(id) as AndroidADBanner;
		if(banner != null) {
			banner.OnBannerAdLeftApplication();
		}
	}


	
	//--------------------------------------
	//  EVENTS INTERSTITIAL AD
	//--------------------------------------

	
	private void OnInterstitialAdLoaded()  {
		OnInterstitialLoaded();
	}
	
	private void OnInterstitialAdFailedToLoad() {
		OnInterstitialFailedLoading();;
	}
	
	private void OnInterstitialAdOpened() {
		OnInterstitialOpened();
	}
	
	private void OnInterstitialAdClosed() {
		OnInterstitialClosed();
	}
	
	private void OnInterstitialAdLeftApplication() {
		OnInterstitialLeftApplication();
	}

	//--------------------------------------
	//  EVENTS REWARDED VIDEO AD
	//--------------------------------------

	private void RewardedCallback(string data) {
		string[] rawData = data.Split(new string[] {"|"}, StringSplitOptions.None);
		OnRewarded(rawData[0], Int32.Parse(rawData[1]));
	}

	private void RewardedVideoAdClosed() {
		OnRewardedVideoAdClosed();
	}

	private void RewardedVideoAdFailedToLoad(string errorCode) {
		OnRewardedVideoAdFailedToLoad(Int32.Parse(errorCode));
	}

	private void RewardedVideoAdLeftApplication() {
		OnRewardedVideoAdLeftApplication();
	}

	private void RewardedVideoLoaded() {
		OnRewardedVideoLoaded();
	}

	private void RewardedVideoAdOpened() {
		OnRewardedVideoAdOpened();
	}

	private void RewardedVideoStarted() {
		OnRewardedVideoStarted();
	}
	
	//--------------------------------------
	//  GENERAL EVENTS
	//--------------------------------------

	private void OnInAppPurchaseRequested(string productId) {
		OnAdInAppRequest(productId);
	}



	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}
