using UnityEngine;
using System;
using System.Collections;

public class AN_InApp_EditorClient : MonoBehaviour, AN_InAppClient  {

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


	private float _RequestsSuccessRate = 100f;
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	
	void Awake() {
		_inventory = new AndroidInventory ();

		_RequestsSuccessRate = AndroidNativeSettings.Instance.InApps_EditorFillRate;
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
	
	public void Connect() {
		if(AndroidNativeSettings.Instance.IsBase64KeyWasReplaced) {
			_IsConnectingToServiceInProcess = true;
			Connect(AndroidNativeSettings.Instance.base64EncodedPublicKey);
		} else {
			SA_EditorNotifications.ShowNotification("Billing Connection Failed", "Wrong Public Key", SA_EditorNotificationType.Error);

			BillingResult result = new BillingResult(BillingResponseCodes.BILLINGHELPER_MISSING_TOKEN, "Connection Failed");
			ActionBillingSetupFinished(result);
		}
	}
	
	public void Connect(string base64EncodedPublicKey) {

		foreach(GoogleProductTemplate tpl in AndroidNativeSettings.Instance.InAppProducts) {
			AddProduct(tpl);
		}

		Invoke("GenerateConnectionResult", UnityEngine.Random.Range(0.5f, 3f));
	}

	private void GenerateConnectionResult() {

		bool IsSucceeded = SA_EditorTesting.HasFill(_RequestsSuccessRate);

		BillingResult result;
		if(IsSucceeded) {
			_IsConnected = true;
			result = new BillingResult(BillingResponseCodes.BILLING_RESPONSE_RESULT_OK, "Connection Successful");
			SA_EditorNotifications.ShowNotification("Billing Connected", "Connection successful", SA_EditorNotificationType.Message);
		} else {
			result = new BillingResult(BillingResponseCodes.BILLINGHELPER_UNKNOWN_ERROR, "Connection Failed");
			SA_EditorNotifications.ShowNotification("Billing Connection Failed", "Connection Failed", SA_EditorNotificationType.Error);
		}

		_IsConnectingToServiceInProcess = false;
		ActionBillingSetupFinished(result);
	}

	
	public void RetrieveProducDetails() {
		_IsProductRetrievingInProcess = true;
		Invoke("OnQueryInventoryFinishedCallBack", UnityEngine.Random.Range(0.5f, 3f));
	}

	public void OnQueryInventoryFinishedCallBack() {

		BillingResult result = new BillingResult(BillingResponseCodes.BILLING_RESPONSE_RESULT_OK, "BILLING_RESPONSE_RESULT_OK");
		_IsInventoryLoaded = true;
		_IsProductRetrievingInProcess = false;
		
		ActionRetrieveProducsFinished(result);
	}

	
	private string _CurrentSKU = "";
	public void Purchase(string SKU) {
		Purchase(SKU, "");
	}
	
	public void Purchase(string SKU, string DeveloperPayload) {
		_processedSKU = SKU;
		GoogleProductTemplate product = _inventory.GetProductDetails(SKU);



		string title = SKU;
		string describtion = "???";
		string price = "?.??$";

		_CurrentSKU = SKU;

		if(product != null) {
			title = product.Title;
			describtion = product.Description;
			price = product.LocalizedPrice;
		} 

		SA_EditorInApps.ShowInAppPopup(title, describtion, price, OnPurchaseComplete);
	}


	private void OnPurchaseComplete(bool IsSucceeded) {

		GooglePurchaseTemplate purchase = null;
		
		BillingResult result;

		if(IsSucceeded) {
			purchase = new GooglePurchaseTemplate ();
			purchase.SKU = _CurrentSKU;

			result = new BillingResult(BillingResponseCodes.BILLING_RESPONSE_RESULT_OK, "BILLING_RESPONSE_RESULT_OK", purchase);
		} else {
			result = new BillingResult(BillingResponseCodes.BILLINGHELPERR_USER_CANCELLED, "BILLINGHELPERR_USER_CANCELLED");
		}

		ActionProductPurchased(result);
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

		GooglePurchaseTemplate purchase = null;
		purchase = new GooglePurchaseTemplate ();
		purchase.SKU = SKU;
		BillingResult result = new BillingResult(BillingResponseCodes.BILLING_RESPONSE_RESULT_OK, "BILLING_RESPONSE_RESULT_OK", purchase);

		ActionProductConsumed(result);
	}
	
	public void LoadStore(){ Connect(); }
	public void LoadStore(string base64EncodedPublicKey){ Connect(base64EncodedPublicKey);}
	
	

	
	
	
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

	
	

}
