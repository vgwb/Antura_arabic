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

public interface GoogleMobileAdInterface  {

	void Init(string ad_unit_id);
	void Init(string banners_unit_id, string interstisial_unit_id);
	void InitEditorTesting(bool isEditorTestingEnabled, int fillRate);

	void SetBannersUnitID(string ad_unit_id);
	void SetInterstisialsUnitID(string ad_unit_id);
	void SetRewardedVideoAdUnitID(string ad_unit_id);
	
	void AddKeyword(string keyword);
	void AddTestDevice(string deviceId);
	void AddTestDevices(params string[] ids);
	void SetGender(GoogleGender gender);
	void SetBirthday(int year, AndroidMonth month, int day);
	void TagForChildDirectedTreatment(bool tagForChildDirectedTreatment);
	
	GoogleMobileAdBanner CreateAdBanner(TextAnchor anchor, GADBannerSize size);
	GoogleMobileAdBanner CreateAdBanner(int x, int y, GADBannerSize size);	
	
	void DestroyBanner(int id);

	void StartRewardedVideo();
	void LoadRewardedVideo();
	void ShowRewardedVideo();
	
	void StartInterstitialAd();
	void LoadInterstitialAd();
	void ShowInterstitialAd();
	void RecordInAppResolution(GADInAppResolution resolution);
	
	GoogleMobileAdBanner GetBanner(int id);
	List<GoogleMobileAdBanner> banners {get;}
	bool IsInited {get;}
	string BannersUunitId {get;}
	string InterstisialUnitId {get;}
	string RewardedVideoAdUnitId {get;}

	//Actions
	event Action OnInterstitialLoaded;
	event Action OnInterstitialFailedLoading;
	event Action OnInterstitialOpened;
	event Action OnInterstitialClosed;
	event Action OnInterstitialLeftApplication;
	event Action<string> OnAdInAppRequest;

	event Action<string, int> OnRewarded;
	event Action OnRewardedVideoAdClosed;
	event Action<int> OnRewardedVideoAdFailedToLoad;
	event Action OnRewardedVideoAdLeftApplication;
	event Action OnRewardedVideoLoaded;
	event Action OnRewardedVideoAdOpened;
	event Action OnRewardedVideoStarted;

}
