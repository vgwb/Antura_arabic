////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////
/// 
using UnityEngine;
using System.Collections;

public class AndroidGoogleAnalytics : SA.Common.Pattern.Singleton<AndroidGoogleAnalytics> {


	private bool IsStarted = false;



	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void StartTracking() {
		if(IsStarted) {
			return;
		}

		IsStarted = true;
		AN_GoogleAnalyticsProxy.StartAnalyticsTracking();
	}


	public void SetTrackerID(string trackingID)  {
		AN_GoogleAnalyticsProxy.SetTrackerID(trackingID);
	}

	public void SendView(string appScreen) {
		AN_GoogleAnalyticsProxy.SendView(appScreen);
	}
	
	public void SendView() {
		AN_GoogleAnalyticsProxy.SendView();
	}


	public void SendEvent(string category, string action, string label) {
		AN_GoogleAnalyticsProxy.SendEvent(category, action, label, "null");
	}

	public void SendEvent(string category, string action, string label, long value) {
		AN_GoogleAnalyticsProxy.SendEvent(category, action, label, value.ToString());
	}

	public void SendEvent(string category, string action, string label, string key, string val) {
		AN_GoogleAnalyticsProxy.SendEvent(category, action, label, "null", key, val);
	}

	public void SendEvent(string category, string action, string label, long value, string key, string val) {
		AN_GoogleAnalyticsProxy.SendEvent(category, action, label, value.ToString(), key, val);
	}


	public void SendTiming(string category, long intervalInMilliseconds) {
		AN_GoogleAnalyticsProxy.SendTiming(category, intervalInMilliseconds.ToString(), "null", "null");
	}

	public void SendTiming(string category, long intervalInMilliseconds, string name) {
		AN_GoogleAnalyticsProxy.SendTiming(category, intervalInMilliseconds.ToString(), name, "null");
	}
	

	public void SendTiming(string category, long intervalInMilliseconds, string name, string label) {
		AN_GoogleAnalyticsProxy.SendTiming(category, intervalInMilliseconds.ToString(), name, label);
	}


	public void CreateTransaction(string transactionId, string affiliation, float revenue, float tax, float shipping, string currencyCode) {
		AN_GoogleAnalyticsProxy.CreateTransaction(transactionId, affiliation, revenue.ToString(), tax.ToString(), shipping.ToString(), currencyCode);
	}
	
	public void CreateItem(string transactionId, string name, string sku, string category, float price, int quantity, string currencyCode) {
		AN_GoogleAnalyticsProxy.CreateItem(transactionId, name, sku, category, price.ToString(), quantity.ToString(), currencyCode);
	}


	public void SetKey(string key, string value) {
		AN_GoogleAnalyticsProxy.SetKey(key, value);
	}


	public  void ClearKey(string key) {
		AN_GoogleAnalyticsProxy.ClearKey(key);
	}

	public void SetLogLevel(GPLogLevel logLevel) {
		AN_GoogleAnalyticsProxy.SetLogLevel((int) logLevel);
	}

	public void SetDryRun(bool mode) {
		if(mode) {
			AN_GoogleAnalyticsProxy.SetDryRun("true");
		} else {
			AN_GoogleAnalyticsProxy.SetDryRun("false");
		}
	}

	public void EnableAdvertisingIdCollection(bool mode) {
		if(mode) {
			AN_GoogleAnalyticsProxy.EnableAdvertisingIdCollection("true");
		} else {
			AN_GoogleAnalyticsProxy.EnableAdvertisingIdCollection("false");
		}
	}

	
	
	



		
}
