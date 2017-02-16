#define FACEBOOK_ENABLED

using UnityEngine;
using System;
using System.Collections;

public class AN_SoomlaGrow : SA.Common.Pattern.Singleton<AN_SoomlaGrow> {


	private static bool _IsInitialized = false;

	public static event Action ActionInitialized 	= delegate {};
	public static event Action ActionConnected 		= delegate {};
	public static event Action ActionDisconnected 	= delegate {};





	// --------------------------------------
	// INITIALIZATION
	// --------------------------------------

	public void CreateListner() {
		
	}

	public static void Init() {

		if(_IsInitialized || !AndroidNativeSettings.Instance.EnableSoomla) {
			return;
		}

		Instance.CreateListner();
		AN_SoomlaProxy.Initalize(AndroidNativeSettings.Instance.SoomlaGameKey, AndroidNativeSettings.Instance.SoomlaEnvKey);

		AndroidTwitterManager.Instance.OnTwitterLoginStarted += OnTwitterLoginStarted;
		AndroidTwitterManager.Instance.OnTwitterLogOut += OnTwitterLogOut;
		AndroidTwitterManager.Instance.OnAuthCompleteAction += HandleOnAuthCompleteAction;
		AndroidTwitterManager.Instance.OnUserDataRequestCompleteAction += HandleOnUserDataRequestCompleteAction;
		AndroidTwitterManager.Instance.OnTwitterPostStarted += TW_PostStarted;
		AndroidTwitterManager.Instance.OnPostingCompleteAction += TW_PostCompleted;

		#if FACEBOOK_ENABLED

		SPFacebook.OnLoginStarted += FB_OnLoginStarted;
		SPFacebook.OnLogOut += FB_OnLogOut;
		SPFacebook.OnAuthCompleteAction += FB_OnAuthCompleteAction;
		SPFacebook.OnPostStarted += FB_PostStarted;
		SPFacebook.OnPostingCompleteAction += FB_PostCompleted;
		SPFacebook.OnFriendsDataRequestCompleteAction += FB_HandleOnFriendsDataRequestCompleteAction;
		SPFacebook.OnFriendsRequestStarted += FB_OnFriendsRequestStarted;

		#endif

		_IsInitialized = true;
	}






	// --------------------------------------
	// Billing
	// --------------------------------------


	public static void PurchaseStarted(string prodcutId) {

		if (!AN_SoomlaGrow.CheckState ()) { return; }

		AN_SoomlaProxy.OnMarketPurchaseStarted(prodcutId);
	}
	
	public static void PurchaseFinished(string prodcutId, long priceInMicros, string currency) {

		if (!AN_SoomlaGrow.CheckState ()) { return; }

		AN_SoomlaProxy.OnMarketPurchaseFinished(prodcutId, priceInMicros, currency);
	}
	
	public static void PurchaseCanceled(string prodcutId) {
		if (!AN_SoomlaGrow.CheckState ()) { return; }

		AN_SoomlaProxy.OnMarketPurchaseCancelled(prodcutId);
	}
	
	public static void SetPurhsesSupportedState(bool isSupported) {
		if (!AN_SoomlaGrow.CheckState ()) { return; }

		AN_SoomlaProxy.SetBillingState(isSupported);
	}
	
	
	public static void PurchaseError() {
		if (!AN_SoomlaGrow.CheckState ()) { return; }

		AN_SoomlaProxy.OnMarketPurchaseFailed();
	}

	private static void FriendsRequest(AN_SoomlaEventType eventType, AN_SoomlaSocialProvider provider) {
		if (!AN_SoomlaGrow.CheckState ()) { return; }
		
		AN_SoomlaProxy.OnFriendsRequest( (int) eventType, (int) provider);
	}



	public static void SocialLogin(AN_SoomlaEventType eventType, AN_SoomlaSocialProvider provider) {
		if (!AN_SoomlaGrow.CheckState ()) { return; }

		AN_SoomlaProxy.OnSocialLogin( (int) eventType, (int) provider);
	}


	public static void SocialLoginFinished(AN_SoomlaSocialProvider provider, string ProfileId) {
		if (!AN_SoomlaGrow.CheckState ()) { return; }
		
		AN_SoomlaProxy.OnSocialLoginFinished((int) provider, ProfileId);
	}

	public static void SocialLogOut(AN_SoomlaEventType eventType, AN_SoomlaSocialProvider provider) {
		if (!AN_SoomlaGrow.CheckState ()) { return; }

		AN_SoomlaProxy.OnSocialLogout((int) eventType, (int) provider);
	}

	public static void SocialShare(AN_SoomlaEventType eventType, AN_SoomlaSocialProvider provider) {
		if (!AN_SoomlaGrow.CheckState ()) { return; }
		
		AN_SoomlaProxy.OnSocialShare((int) eventType, (int) provider);
	}

	// --------------------------------------
	// Facebook
	// --------------------------------------

	#if FACEBOOK_ENABLED

	private static void FB_OnFriendsRequestStarted() {
		FriendsRequest(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.FACEBOOK);
	}


	private static void FB_HandleOnFriendsDataRequestCompleteAction (FB_Result res){
		if(res.IsSucceeded) {
			FriendsRequest(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.FACEBOOK);
		} else {
			FriendsRequest(AN_SoomlaEventType.SOOMLA_EVENT_FAILED, AN_SoomlaSocialProvider.FACEBOOK);
		}
	}
	

	private static void FB_OnAuthCompleteAction (FB_Result res) {
		if(res.IsSucceeded) {
			SocialLoginFinished(AN_SoomlaSocialProvider.FACEBOOK, SPFacebook.Instance.UserId);
		} else {
			SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_FAILED, AN_SoomlaSocialProvider.FACEBOOK);
		}
	}
	
	private static void FB_OnLoginStarted () {
		SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.FACEBOOK);
	}
	
	private static void FB_OnLogOut () {
		SocialLogOut(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.FACEBOOK);
		SocialLogOut(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.FACEBOOK);
	}

	private static void FB_PostStarted () {
		SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.FACEBOOK);
	}

	private static void FB_PostCompleted (FB_PostResult res) {
		Debug.Log("FB_PostCompleted");
		if(res.IsSucceeded) {
			Debug.Log("SOOMLA_EVENT_FINISHED");
			SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.FACEBOOK);
		} else {
			Debug.Log("SOOMLA_EVENT_CNACELED");
			SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_CNACELED, AN_SoomlaSocialProvider.FACEBOOK);
		}

	}

	#endif
	
	// --------------------------------------
	// Twitter
	// --------------------------------------

	private static void HandleOnAuthCompleteAction (TWResult res) {
		if(!res.IsSucceeded) {
			SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_CNACELED, AN_SoomlaSocialProvider.TWITTER);
		} else {
			AndroidTwitterManager.Instance.LoadUserData();
		}
	}
	
	private static void HandleOnUserDataRequestCompleteAction (TWResult res) {
		if(res.IsSucceeded) {
			SocialLoginFinished(AN_SoomlaSocialProvider.TWITTER, AndroidTwitterManager.Instance.userInfo.id);
		} else {
			SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_FAILED, AN_SoomlaSocialProvider.TWITTER);
		}
	}
	
	private static void OnTwitterLoginStarted () {
		SocialLogin(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.TWITTER);
	}
	
	private static void OnTwitterLogOut () {
		SocialLogOut(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.TWITTER);
		SocialLogOut(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.TWITTER);
	}

	private static void TW_PostStarted () {
		SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_STARTED, AN_SoomlaSocialProvider.TWITTER);
	}

	private static void TW_PostCompleted (TWResult res) {
		if(res.IsSucceeded) {
			SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_FINISHED, AN_SoomlaSocialProvider.TWITTER);
		} else {
			SocialShare(AN_SoomlaEventType.SOOMLA_EVENT_FAILED, AN_SoomlaSocialProvider.TWITTER);
		}
	}

	// --------------------------------------
	// Private Methods
	// --------------------------------------


	private static bool CheckState() {

		if(AndroidNativeSettings.Instance.EnableSoomla) {
			Init();
		}

		return AndroidNativeSettings.Instance.EnableSoomla;
	}



	// --------------------------------------
	// Events
	// --------------------------------------


	private void OnInitialized() {
		Debug.Log("AN_SOOMAL OnInitialized");
		ActionInitialized();
	}

	private void OnConnected() {
		ActionConnected();
	}

	private void OnDisconnected() {
		ActionDisconnected();
	}
	

}
