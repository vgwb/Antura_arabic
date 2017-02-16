using UnityEngine;
using System.Collections;
using System;

public interface AN_InAppClient  {

	//Actions
	event Action<BillingResult>  ActionProductPurchased;
	event Action<BillingResult>  ActionProductConsumed;
	
	event Action<BillingResult>  ActionBillingSetupFinished;
	event Action<BillingResult>  ActionRetrieveProducsFinished;



	/// <summary>
	/// Use this methods to define your app products
	/// before retriving InApps information
	/// As alternative to this method you may fill
	/// the InApp's data using Editor Settings Window
	/// </summary>
	/// <param name="SKU">New product SKU.</param>
	void AddProduct(string SKU);

	/// <summary>
	/// Use this methods to define your app products
	/// before retriving InApps information
	/// As alternative to this method you may fill
	/// the InApp's data using Editor Settings Window
	/// </summary>
	/// <param name="SKU">New product Template.</param>
	void AddProduct(GoogleProductTemplate template);

	/// <summary>
	/// Retrieve's product details and current user inventory
	/// ActionRetrieveProducsFinished Action fired when request
	/// is complete
	/// </summary>
	void RetrieveProducDetails();

	/// <summary>
	/// Start purchase flow for product
	/// ActionProductPurchased Action fired when flow
	/// is complete
	/// </summary>
	/// <param name="SKU">product SKU you want to purchase</param>
	void Purchase(string SKU);

	/// <summary>
	/// Start purchase flow for product
	/// ActionProductPurchased Action fired when flow
	/// is complete
	/// </summary>
	/// <param name="SKU">product SKU you want to purchase</param>
	/// <param name="DeveloperPayload">purchse developer payload string</param>
	void Purchase(string SKU, string DeveloperPayload);

	/// <summary>
	/// Start subscribe flow for product
	/// ActionProductPurchased Action fired when flow
	/// is complete
	/// </summary>
	/// <param name="SKU">product SKU you want to purchase</param>
	void Subscribe(string SKU);


	/// <summary>
	/// Start subscribe flow for product
	/// ActionProductPurchased Action fired when flow
	/// is complete
	/// </summary>
	/// <param name="DeveloperPayload">purchse developer payload string</param>
	 void Subscribe(string SKU, string DeveloperPayload);


	/// <summary>
	/// Start consume flow for product
	/// ActionProductConsumed Action fired when flow
	/// is complete
	/// </summary>
	/// <param name="SKU">product SKU you want to consume</param>
	void Consume(string SKU);

	/// <summary>
	/// Connect to Android InApp service
	/// ActionBillingSetupFinished Action fired when connect
	/// is complete
	/// </summary>
	void Connect();

	/// <summary>
	/// Connect to Android InApp service
	/// ActionBillingSetupFinished Action fired when connect
	/// is complete
	/// As alternative you may use Connect methods
	/// without parameters and set up app public key
	/// using AndroidNative Editor Settings
	/// </summary>
	/// <param name="base64EncodedPublicKey">add public key</param>
	void Connect(string base64EncodedPublicKey);



	[System.Obsolete("LoadStore is deprectaed, plase use Connect instead")]
	void LoadStore();
	
	[System.Obsolete("LoadStore is deprectaed, plase use Connect instead")]
	void LoadStore(string base64EncodedPublicKey);


	/// <summary>
	/// Current user Android inventory
	/// </summary>
	AndroidInventory Inventory {get;}

	/// <summary>
	/// Can be used to determine is connection to the Android
	/// billing services is in the progress
	/// </summary>
	bool IsConnectingToServiceInProcess {get;}

	/// <summary>
	/// Can be used to determine is products details
	/// load request is in the progress
	/// </summary>
	bool IsProductRetrievingInProcess {get;}
	
	/// <summary>
	/// Can be used to determine if app is connection
	/// to the Android billing services 
	/// </summary>
	bool IsConnected {get;}


	/// <summary>
	/// Can be used to determine if user Android inventory
	/// data is loaded
	/// </summary>
	bool IsInventoryLoaded {get;}

}
