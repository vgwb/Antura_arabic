////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////




using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class SPFacebookAnalytics  {



	//--------------------------------------
	//  App Events API 
	//  https://developers.facebook.com/docs/android/app-events
	//	https://developers.facebook.com/docs/unity/reference/current/fb.appevents.logevent
	//------------------------------------
	
	
	//An app is being activated
	//https://developers.facebook.com/docs/unity/reference/current/FB.ActivateApp
	public static void ActivateApp() {
		//FB.ActivateApp ();
	}

	
	
	//The user has achieved a level in the app.
	public static void AchievedLevel(int level) {
		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();
		EventParams[FBAppEventParameterName.Level] = level;

		FB.AppEvents.LogEvent (FBAppEventName.AchievedLevel, null, EventParams);
		*/
	}
	
	//The user has entered their payment info.
	public static void AddedPaymentInfo(bool IsPaymentInfoAvailable) {
		/*

		Dictionary<string, object> EventParams = new Dictionary<string, object>();
		
		int val = 0;
		if (IsPaymentInfoAvailable) {
			val = 1;
		}
		
		EventParams[FBAppEventParameterName.PaymentInfoAvailable] = val;

		FB.AppEvents.LogEvent (FBAppEventName.AddedPaymentInfo, null, EventParams);
		*/

	}
	
	
	public static void AddedToCart(float  price, string id, string type, string currency = "USD") {
		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();
		
		
		EventParams[FBAppEventParameterName.ContentID] 		= id;
		EventParams[FBAppEventParameterName.ContentType] 	= type;
		EventParams[FBAppEventParameterName.Currency] 		= currency;
		
		FB.AppEvents.LogEvent (FBAppEventName.AddedToCart, price, EventParams);
*/
	}
	
	
	public static void AddedToWishlist(float  price, string id, string type, string currency = "USD") {
		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();
		
		
		EventParams[FBAppEventParameterName.ContentID] 		= id;
		EventParams[FBAppEventParameterName.ContentType] 	= type;
		EventParams[FBAppEventParameterName.Currency] 		= currency;
		
		FB.AppEvents.LogEvent (FBAppEventName.AddedToWishlist, price, EventParams);
		*/
	}


	public static void CompletedRegistration(string RegistrationMethod) {
		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();
		
		
		EventParams[FBAppEventParameterName.RegistrationMethod] = RegistrationMethod;
		FB.AppEvents.LogEvent (FBAppEventName.CompletedRegistration, null, EventParams);

*/
	}


	public static void CompletedTutorial(bool IsIsSuccsessed, string ContentId) {
		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();

		int val = 0;
		if (IsIsSuccsessed) {
			val = 1;
		}
		
		
		EventParams[FBAppEventParameterName.Success] = val;
		EventParams[FBAppEventParameterName.ContentID] = ContentId;
		FB.AppEvents.LogEvent (FBAppEventName.CompletedTutorial, null, EventParams);
	*/
	}


	public static void InitiatedCheckout(float price, int itemsCount, string ContentType, string ContentId, bool IsPaymentInfoAvailable, string Currency = "USD") {

		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();
		
		int val = 0;
		if (IsPaymentInfoAvailable) {
			val = 1;
		}

		
		EventParams[FBAppEventParameterName.ContentType] 			= ContentType;
		EventParams[FBAppEventParameterName.ContentID] 				= ContentId;
		EventParams[FBAppEventParameterName.NumItems] 				= itemsCount;
		EventParams[FBAppEventParameterName.PaymentInfoAvailable] 	= val;
		EventParams[FBAppEventParameterName.Currency] 				= Currency;


		FB.AppEvents.LogEvent (FBAppEventName.InitiatedCheckout, price, EventParams);

*/
		
	}

	public static void Purchased(float price, int itemsCount, string ContentType, string ContentId,  string Currency = "USD") {

		/*

		Dictionary<string, object> EventParams = new Dictionary<string, object>();


		EventParams[FBAppEventParameterName.NumItems] 				= itemsCount;
		EventParams[FBAppEventParameterName.ContentType] 			= ContentType;
		EventParams[FBAppEventParameterName.ContentID] 				= ContentId;
		EventParams[FBAppEventParameterName.Currency] 				= Currency;

		FB.AppEvents.LogEvent (FBAppEventName.Purchased, price, EventParams);

*/
	}


	public static void Rated(int Rating, string ContentType, string ContentId, int MaxRating) {
		/*

		Dictionary<string, object> EventParams = new Dictionary<string, object>();
		

		EventParams[FBAppEventParameterName.ContentType] 			= ContentType;
		EventParams[FBAppEventParameterName.ContentID] 				= ContentId;
		EventParams[FBAppEventParameterName.MaxRatingValue] 		= MaxRating;
		
		FB.AppEvents.LogEvent (FBAppEventName.Rated, Rating, EventParams);
		*/
	}



	public static void Searched(string ContentType, string SearchString, bool IsIsSuccsessed) {
		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();

		int v = 0;
		if (IsIsSuccsessed) {
			v = 1;
		}
		
		EventParams[FBAppEventParameterName.ContentType] 	= ContentType;
		EventParams[FBAppEventParameterName.SearchString] 	= SearchString;
		EventParams[FBAppEventParameterName.Success] 		= v;
		
		FB.AppEvents.LogEvent (FBAppEventName.Searched, null, EventParams);
		*/
	}



	public static void SpentCredits(float credit, string ContentType, string ContentId) {
		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();
		
		
		EventParams[FBAppEventParameterName.ContentType] 			= ContentType;
		EventParams[FBAppEventParameterName.ContentID] 				= ContentId;
		
		FB.AppEvents.LogEvent (FBAppEventName.SpentCredits, credit, EventParams);
		*/
	}


	public static void UnlockedAchievement(string Describtion) {
		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();

		EventParams[FBAppEventParameterName.Description] 			= Describtion;
		FB.AppEvents.LogEvent (FBAppEventName.UnlockedAchievement, null, EventParams);
		*/
	}



	public static void LogEvent() {
		/*
		string logEvent,
		float? valueToSum = null,
		Dictionary<string, object> parameters = null) {

		FB.AppEvents.LogEvent(logEvent, valueToSum, parameters);
		*/
	}


	public static void ViewedContent(float price,  string ContentType, string ContentId,  string Currency = "USD") {
		/*
		Dictionary<string, object> EventParams = new Dictionary<string, object>();

		EventParams[FBAppEventParameterName.ContentType] 			= ContentType;
		EventParams[FBAppEventParameterName.ContentID] 				= ContentId;
		EventParams[FBAppEventParameterName.Currency] 				= Currency;
		
		FB.AppEvents.LogEvent (FBAppEventName.ViewedContent, price, EventParams);
		*/
	}
	

}
