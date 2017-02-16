////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class BillingResult  {


	private int _response;
	private string _message;

	private GooglePurchaseTemplate _purchase = null;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------



	public BillingResult(int code, string msg, GooglePurchaseTemplate p) : this(code, msg) {
		_purchase = p;
	}


	public BillingResult(int code, string msg) {
		_response = code;
		_message = msg;
	}



	//--------------------------------------
	// GET / SET
	//--------------------------------------


	[System.Obsolete("purchase is deprectaed, plase use Purchase instead")]
	public GooglePurchaseTemplate purchase { get { return  Purchase; } }
	public GooglePurchaseTemplate Purchase {
		get {
			return _purchase;
		}
	}


	[System.Obsolete("response is deprectaed, plase use Response instead")]
	public int response { get { return  Response; } }
	public int Response {
		get {
			return _response;
		}
	}


	[System.Obsolete("message is deprectaed, plase use Message instead")]
	public string message { get { return  Message; } }
	public string Message {
		get {
			return _message;
		}
	}
	

	[System.Obsolete("isSuccess is deprectaed, plase use IsSuccess instead")]
	public bool isSuccess { get { return  IsSuccess; } }
	public bool IsSuccess  {
		get {
			return _response == BillingResponseCodes.BILLING_RESPONSE_RESULT_OK;
		}
	}

	[System.Obsolete("isFailure is deprectaed, plase use IsFailure instead")]
	public bool isFailure { get { return  IsFailure; } }
	public bool IsFailure {
		get {
			return !IsSuccess;
		}
	}


}
