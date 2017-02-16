using UnityEngine;
using System.Collections;

public class AN_GoogleAdProxy  {

	private const string CLASS_NAME = "com.androidnative.gms.ad.ANMobileAd";
		
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}

	public static void InitMobileAd(string id) {
		CallActivityFunction("Bridge_Init", id);
	}
	
	public static void ChangeBannersUnitID(string id) {
		CallActivityFunction("Bridge_ChangeBannersUnitID", id);
	}
	
	public static void ChangeInterstisialsUnitID(string id) {
		CallActivityFunction("Bridge_ChangeInterstisialsUnitID", id);
	}

	public static void ChangeRewardedVideoUnitID(string id) {
		CallActivityFunction("Bridge_ChangeRewardedVideoUnitID", id);
	}
	
	public static void CreateBannerAd(int gravity, int size, int id) {
		CallActivityFunction("Bridge_CreateBannerAd", gravity.ToString(), size.ToString(), id.ToString());
	}
	
	public static void CreateBannerAdPos(int x, int y, int size, int id) {
		CallActivityFunction("Bridge_CreateBannerAdPos", x.ToString(), y.ToString(), size.ToString(), id.ToString());
	}	
	
	// By nastrandsky
	public static void SetBannerPosition(int gravity, int bannerId) {
		CallActivityFunction ("Bridge_SetBannerPosition", gravity.ToString(), bannerId.ToString());
	}
	
	// By nastrandsky
	public static void SetBannerPosition(int x, int y, int bannerId) {
		CallActivityFunction ("Bridge_SetBannerPosition", x.ToString(), y.ToString(), bannerId.ToString());
	}	
	
	public static void HideAd(int id) { 
		CallActivityFunction ("Bridge_HideAd", id.ToString());
	}
	
	public static void ShowAd(int id) { 
		CallActivityFunction ("Bridge_ShowAd", id.ToString());
	}
	
	public static void RefreshAd(int id) { 
		CallActivityFunction ("Bridge_RefreshAd", id.ToString());
	}

	public static void PauseAd(int id) {
		CallActivityFunction("Bridge_PauseBanner", id.ToString());
	}

	public static void ResumeAd(int id) {
		CallActivityFunction("Bridge_ResumeBanner", id.ToString());
	}
	
	public static void DestroyBanner(int id) { 
		CallActivityFunction ("Bridge_DestroyBanner", id.ToString());
	}
		
	public static void StartInterstitialAd() {
		CallActivityFunction ("Bridge_StartInterstitialAd");
	}
	
	public static void LoadInterstitialAd() {
		CallActivityFunction ("Bridge_LoadInterstitialAd");
	}
	
	public static void ShowInterstitialAd() {
		CallActivityFunction ("Bridge_ShowInterstitialAd");
	}

	public static void LoadRewardedVideo () {
		CallActivityFunction("Bridge_LoadRewardedVideo");
	}

	public static void ShowRewardedVideo () {
		CallActivityFunction("Bridge_ShowRewardedVideo");
	}
	
	public static void RecordInAppResolution(int res) {
		CallActivityFunction ("Bridge_RecordInAppResolution", res.ToString());
	}
	
	public static void AddKeyword(string keyword) {
		CallActivityFunction ("Bridge_AddKeyword", keyword);
	}
	
	
	public static void SetBirthday(int year, int month, int day) {
		CallActivityFunction ("Bridge_SetBirthday", year.ToString(), month.ToString(), day.ToString());
	}
	
	public static void TagForChildDirectedTreatment(bool tagForChildDirectedTreatment) {
		if(tagForChildDirectedTreatment) {
			CallActivityFunction ("Bridge_TagForChildDirectedTreatment", "1");
		} else {
			CallActivityFunction ("Bridge_TagForChildDirectedTreatment", "0");
		}		
	}
	
	public static void AddTestDevice(string deviceId) {
		CallActivityFunction ("Bridge_AddTestDevice", deviceId);
	}
	
	// By nastrandsky: Ad various test devices at once.
	public static void AddTestDevices(string cvsDeviceIds) {
		CallActivityFunction ("Bridge_AddTestDevices", cvsDeviceIds);
	}
	
	public static void SetGender(int gender) {
		CallActivityFunction ("Bridge_SetGender", gender.ToString());
	}


}
