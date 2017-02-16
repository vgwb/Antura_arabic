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

public static class AndroidInAppPurchaseManager  {

	//Actions
	public static event Action<BillingResult>  ActionProductPurchased   = delegate {};
	public static event Action<BillingResult>  ActionProductConsumed    = delegate {};
	
	public static event Action<BillingResult>  ActionBillingSetupFinished   = delegate {};
	public static event Action<BillingResult>  ActionRetrieveProducsFinished = delegate {};

	

	public static AN_InAppClient _Client = null;
	public static AN_InAppClient Client {
		get {
			if(_Client ==  null) {

				GameObject go = new GameObject("AndroidInAppPurchaseManager");
				MonoBehaviour.DontDestroyOnLoad(go);

				if(Application.isEditor) {
					if(AndroidNativeSettings.Instance.Is_InApps_EditorTestingEnabled) {
						_Client = go.AddComponent<AN_InApp_EditorClient>();
					} 
				}

				if(_Client == null) {
					_Client = go.AddComponent<AN_InAppAndroidClient>();
				}
					
				_Client.ActionBillingSetupFinished += HandleActionBillingSetupFinished;
				_Client.ActionProductConsumed += HandleActionProductConsumed;
				_Client.ActionProductPurchased += HandleActionProductPurchased;
				_Client.ActionRetrieveProducsFinished += HandleActionRetrieveProducsFinished;
			}

		


			return _Client;
		}
	}

	static void HandleActionRetrieveProducsFinished (BillingResult res) {
		ActionRetrieveProducsFinished(res);
	}

	static void HandleActionProductPurchased (BillingResult res) {
		ActionProductPurchased(res);
	}

	static void HandleActionProductConsumed (BillingResult res) {
		ActionProductConsumed(res);
	}

	static void HandleActionBillingSetupFinished (BillingResult res) {
		ActionBillingSetupFinished(res);
	}

	[System.Obsolete("Instance is deprectaed, please use Client instead")]
	public static AN_InAppClient Instance {
		get {
			return Client;
		}
	}


}
