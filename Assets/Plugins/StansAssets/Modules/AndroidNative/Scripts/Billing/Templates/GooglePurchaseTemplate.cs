////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class GooglePurchaseTemplate  {

	public string OrderId;
	public string PackageName;
	public string SKU;

	public string DeveloperPayload;
	public string Signature;
	public string Token;
	public long Time;
	public string OriginalJson;
	public GooglePurchaseState State;


	public void SetState(string code) {
		int c = System.Convert.ToInt32(code);
		switch(c) {
		case 0:
			State = GooglePurchaseState.PURCHASED;
			break;
		case 1:
			State = GooglePurchaseState.CANCELED;
			break;
		case 2:
			State = GooglePurchaseState.REFUNDED;
			break;
		}
	}

}
