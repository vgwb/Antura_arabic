using UnityEngine;
using System.Collections;

public class AN_SoomlaProxy  {

	private const string CLASS_NAME = "com.stansassets.sa_soomla.SoomlaBridge";
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	

	
	public static void Initalize(string gameKey, string envtKey) {
		CallActivityFunction("initalize", gameKey, envtKey);
	}


	public static  void SetBillingState(bool state) {
		CallActivityFunction("setBillingState", state);
	}
	
	public static void OnMarketPurchaseStarted(string productId) {
		CallActivityFunction("onMarketPurchaseStarted", productId);
	}
	
	public static void OnMarketPurchaseFinished(string marketProductId, long marketPriceMicros, string marketCurrencyCode) {
		CallActivityFunction("onMarketPurchaseFinished", marketProductId, marketPriceMicros, marketCurrencyCode);
	}

	public static void OnMarketPurchaseFailed() {
		CallActivityFunction("onMarketPurchaseFailed");
	}

	public static void OnMarketPurchaseCancelled(string productId) {
		CallActivityFunction("onMarketPurchaseCancelled", productId);
	}
	

	public static void OnSocialLogin(int eventType, int providerId) {
		CallActivityFunction("onSocialLogin", eventType, providerId);
	}
	
	public  static  void OnSocialLoginFinished(int providerId, string profileId) {
		CallActivityFunction("OnSocialLoginFinished", providerId, profileId);
	}
	
	public static void OnSocialLogout(int eventType, int providerId) {
		CallActivityFunction("onSocialLogout", eventType, providerId);
	}

	public static void OnSocialShare(int eventType, int providerId) {
		CallActivityFunction("onSocialShare", eventType, providerId);
	}

	public static void OnFriendsRequest(int eventType, int providerId) {
		CallActivityFunction("onFriendsRequest", eventType, providerId);
	}






}
