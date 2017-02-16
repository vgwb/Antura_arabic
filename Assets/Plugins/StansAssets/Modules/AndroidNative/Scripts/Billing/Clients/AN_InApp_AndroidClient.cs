using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AN_InAppAndroidClient : MonoBehaviour, AN_InAppClient {

	//Actions
	public event Action<BillingResult>  ActionProductPurchased   = delegate {};
	public event Action<BillingResult>  ActionProductConsumed    = delegate {};
	
	public event Action<BillingResult>  ActionBillingSetupFinished   = delegate {};
	public event Action<BillingResult>  ActionRetrieveProducsFinished = delegate {};
	
	private string _processedSKU;
	private AndroidInventory _inventory;
	
	
	private bool _IsConnectingToServiceInProcess 	= false;
	private bool _IsProductRetrievingInProcess 		= false;
	
	private bool _IsConnected = false;
	private bool _IsInventoryLoaded = false;
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {
		_inventory = new AndroidInventory ();
	}
	
	
	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------
	

	public void AddProduct(string SKU) {
		GoogleProductTemplate template = new GoogleProductTemplate(){SKU = SKU};
		AddProduct(template);
	}
	
	public void AddProduct(GoogleProductTemplate template) {
		
		bool IsPordcutAlreadyInList = false;
		int replaceIndex = 0;
		foreach(GoogleProductTemplate p in _inventory.Products) {
			if(p.SKU.Equals(template.SKU)) {
				IsPordcutAlreadyInList = true;
				replaceIndex = _inventory.Products.IndexOf(p);
				break;
			}
		}
		
		if(IsPordcutAlreadyInList) {
			_inventory.Products[replaceIndex] = template;
		} else {
			_inventory.Products.Add(template);
		}
	}
	

	
	public void RetrieveProducDetails() {
		_IsProductRetrievingInProcess = true;
		AN_BillingProxy.RetrieveProducDetails();
	}
	

	
	public void Purchase(string SKU) {
		Purchase(SKU, "");
	}
	
	public void Purchase(string SKU, string DeveloperPayload) {
		_processedSKU = SKU;
		AN_SoomlaGrow.PurchaseStarted(SKU);
		AN_BillingProxy.Purchase (SKU, DeveloperPayload);
		
	}
	

	
	public void Subscribe(string SKU) {
		Subscribe(SKU, "");
	}
	
	public void Subscribe(string SKU, string DeveloperPayload) {
		_processedSKU = SKU;
		AN_SoomlaGrow.PurchaseStarted(SKU);
		AN_BillingProxy.Subscribe (SKU, DeveloperPayload);
	}


	public void Consume(string SKU) {
		_processedSKU = SKU;
		AN_BillingProxy.Consume (SKU);
	}
	
	public void LoadStore(){ Connect(); }
	public void LoadStore(string base64EncodedPublicKey){ Connect(base64EncodedPublicKey);}


	public void Connect() {
		if(AndroidNativeSettings.Instance.IsBase64KeyWasReplaced) {
			Connect(AndroidNativeSettings.Instance.base64EncodedPublicKey);
			_IsConnectingToServiceInProcess = true;
		} else {
			Debug.LogError("Replace base64EncodedPublicKey in Androdi Native Setting menu");
		}
	}
	
	public void Connect(string base64EncodedPublicKey) {
		
		foreach(GoogleProductTemplate pid in AndroidNativeSettings.Instance.InAppProducts) {
			AddProduct(pid.SKU);
		}
		
		string ids = "";
		int len = AndroidNativeSettings.Instance.InAppProducts.Count;
		for(int i = 0; i < len; i++) {
			if(i != 0) {
				ids += ",";
			}
			
			ids += AndroidNativeSettings.Instance.InAppProducts[i].SKU;
		}
		
		AN_BillingProxy.Connect (ids, base64EncodedPublicKey);
		
	}
	
	
	
	//--------------------------------------
	// GET / SET
	//--------------------------------------

	
	public AndroidInventory Inventory {
		get {
			return _inventory;
		}
	}
	
	public bool IsConnectingToServiceInProcess {
		get {
			return _IsConnectingToServiceInProcess;
		}
	}
	
	public bool IsProductRetrievingInProcess {
		get {
			return _IsProductRetrievingInProcess;
		}
	}
	
	public bool IsConnected {
		get {
			return _IsConnected;
		}
	}
	
	public bool IsInventoryLoaded {
		get {
			return _IsInventoryLoaded;
		}
	}
	
	
	//--------------------------------------
	// EVENTS
	//--------------------------------------
	
	
	
	public void OnPurchaseFinishedCallback(string data) {
		Debug.Log(data);
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		int resp = System.Convert.ToInt32 (storeData[0]);
		GooglePurchaseTemplate purchase = new GooglePurchaseTemplate ();
		
		
		if(resp == BillingResponseCodes.BILLING_RESPONSE_RESULT_OK) {
			
			purchase.SKU 						= storeData[2];
			purchase.PackageName 				= storeData[3];
			purchase.DeveloperPayload 			= storeData[4];
			purchase.OrderId 	       			= storeData[5];
			purchase.SetState(storeData[6]);
			purchase.Token 	        			= storeData[7];
			purchase.Signature 	        		= storeData[8];
			purchase.Time						= System.Convert.ToInt64(storeData[9]);
			purchase.OriginalJson 				= storeData[10];
			
			if(_inventory != null) {
				_inventory.addPurchase (purchase);
			}
			
		} else {
			purchase.SKU 						= _processedSKU;
		}
		
		
		//Soomla Analytics
		if(resp == BillingResponseCodes.BILLING_RESPONSE_RESULT_OK) {
			GoogleProductTemplate tpl = Inventory.GetProductDetails(purchase.SKU);
			if(tpl != null) {
				AN_SoomlaGrow.PurchaseFinished(tpl.SKU, tpl.PriceAmountMicros, tpl.PriceCurrencyCode);
			} else {
				AN_SoomlaGrow.PurchaseFinished(purchase.SKU, 0, "USD");
			}
		} else {
			
			
			if(resp == BillingResponseCodes.BILLINGHELPERR_USER_CANCELLED) {
				AN_SoomlaGrow.PurchaseCanceled(purchase.SKU);
			} else {
				AN_SoomlaGrow.PurchaseError();
			}
		}
		
		BillingResult result = new BillingResult (resp, storeData [1], purchase);
		
		
		ActionProductPurchased(result);
	}
	
	
	public void OnConsumeFinishedCallBack(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		int resp = System.Convert.ToInt32 (storeData[0]);
		GooglePurchaseTemplate purchase = null;
		
		
		if(resp == BillingResponseCodes.BILLING_RESPONSE_RESULT_OK) {
			purchase = new GooglePurchaseTemplate ();
			purchase.SKU 				= storeData[2];
			purchase.PackageName 		= storeData[3];
			purchase.DeveloperPayload 	= storeData[4];
			purchase.OrderId 	        = storeData[5];
			purchase.SetState(storeData[6]);
			purchase.Token 	        		= storeData[7];
			purchase.Signature 	        	= storeData[8];
			purchase.Time					= System.Convert.ToInt64(storeData[9]);
			purchase.OriginalJson 	        = storeData[10];
			
			if(_inventory != null) {
				_inventory.removePurchase (purchase);
			}
			
		}
		
		BillingResult result = new BillingResult (resp, storeData [1], purchase);
		
		ActionProductConsumed(result);
	}
	
	
	
	public void OnBillingSetupFinishedCallback(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		int resp = System.Convert.ToInt32 (storeData[0]);
		
		

	
		BillingResult result = new BillingResult (resp, storeData [1]);

		if(result.IsSuccess) {
			_IsConnected = true;
		}
		_IsConnectingToServiceInProcess = false;
		
		AN_SoomlaGrow.SetPurhsesSupportedState(result.IsSuccess);
		
		ActionBillingSetupFinished(result);
	}
	
	
	public void OnQueryInventoryFinishedCallBack(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		int resp = System.Convert.ToInt32 (storeData[0]);
		
		BillingResult result = new BillingResult (resp, storeData [1]);
		
		_IsInventoryLoaded = true;
		_IsProductRetrievingInProcess = false;
		
		ActionRetrieveProducsFinished(result);
	}
	
	
	
	public void OnPurchasesRecive(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("InAppPurchaseManager, no purchases avaiable");
			return;
		}
		
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		for(int i = 0; i < storeData.Length; i+=9) {
			GooglePurchaseTemplate tpl =  new GooglePurchaseTemplate();
			tpl.SKU 				= storeData[i];
			tpl.PackageName 		= storeData[i + 1];
			tpl.DeveloperPayload 	= storeData[i + 2];
			tpl.OrderId 	        = storeData[i + 3];
			tpl.SetState(storeData[i + 4]);
			tpl.Token 	        	= storeData[i + 5];
			tpl.Signature 	        = storeData[i + 6];
			tpl.Time 	        	= System.Convert.ToInt64(storeData[i + 7]); 
			tpl.OriginalJson 	    = storeData[i + 8];
			
			_inventory.addPurchase (tpl);
		}
		
		Debug.Log("InAppPurchaseManager, total purchases loaded: " + _inventory.Purchases.Count);
		
	}
	
	
	public void OnProducttDetailsRecive(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("InAppPurchaseManager, no products avaiable");
			return;
		}
		
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		
		for(int i = 0; i < storeData.Length; i+=7) {
			GoogleProductTemplate tpl =  _inventory.GetProductDetails(storeData[i]);
			if(tpl == null) {
				tpl =  new GoogleProductTemplate();
				tpl.SKU = storeData[i];
				_inventory.Products.Add(tpl);
			}
			
			tpl.LocalizedPrice 		  		= storeData[i + 1];
			tpl.Title 	      				= storeData[i + 2];
			tpl.Description   				= storeData[i + 3];
			tpl.PriceCurrencyCode   		= storeData[i + 5];
			tpl.OriginalJson   				= storeData[i + 6];

			long priceAmountMicros = 0L;
			if (!System.Int64.TryParse(storeData[i + 4], out priceAmountMicros)) {
				priceAmountMicros = GoogleProductTemplate.DEFAULT_PRICE_AMOUNT_MICROS;
			}
			tpl.PriceAmountMicros = priceAmountMicros;
			
			Debug.Log("Prodcut originalJson: " + tpl.OriginalJson);
		}
		
		Debug.Log("InAppPurchaseManager, total products loaded: " + _inventory.Products.Count);
	}
}
