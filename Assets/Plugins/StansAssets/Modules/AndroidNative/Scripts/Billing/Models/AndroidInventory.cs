////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AndroidInventory  {

	private Dictionary<string, GooglePurchaseTemplate> _purchases;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public AndroidInventory () {
		_purchases = new  Dictionary<string, GooglePurchaseTemplate> ();
	} 

	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	

	public void addPurchase(GooglePurchaseTemplate purchase) {
		if(_purchases.ContainsKey(purchase.SKU)) {
			_purchases [purchase.SKU] = purchase;
		} else {
			_purchases.Add (purchase.SKU, purchase);
		}
	}

	public void removePurchase(GooglePurchaseTemplate purchase) {
		if(_purchases.ContainsKey(purchase.SKU)) {
			_purchases.Remove (purchase.SKU);
		}
	}

	public bool IsProductPurchased(string SKU) {
		if(_purchases.ContainsKey(SKU)) {
			GooglePurchaseTemplate tpl = GetPurchaseDetails(SKU);
			if(tpl.State == GooglePurchaseState.PURCHASED) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}


	public GoogleProductTemplate GetProductDetails(string SKU) {
		foreach(GoogleProductTemplate p in Products) {
			if(p.SKU.Equals(SKU)) {
				return p;
			}
		}

		GoogleProductTemplate product = new GoogleProductTemplate(){SKU = SKU};
		return product;
	}

	public GooglePurchaseTemplate GetPurchaseDetails(string SKU) {
		if(_purchases.ContainsKey(SKU)) {
			return _purchases [SKU];
		} else {
			return null;
		}
	}

	//--------------------------------------
	// GET / SET
	//--------------------------------------



	public List<GooglePurchaseTemplate> Purchases {
		get {
			return  new List<GooglePurchaseTemplate>(_purchases.Values);
		}
	}
		

	public List<GoogleProductTemplate> Products {
		get {
			return  AndroidNativeSettings.Instance.InAppProducts;
		}
	}


}
