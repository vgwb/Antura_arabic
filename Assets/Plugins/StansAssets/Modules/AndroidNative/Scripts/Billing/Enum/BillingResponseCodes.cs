////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class BillingResponseCodes {

	// Billing response codes
	public const int BILLING_RESPONSE_RESULT_OK = 0;
	public const int BILLING_RESPONSE_RESULT_USER_CANCELED = 1;
	public const int BILLING_RESPONSE_RESULT_BILLING_UNAVAILABLE = 3;
	public const int BILLING_RESPONSE_RESULT_ITEM_UNAVAILABLE = 4;
	public const int BILLING_RESPONSE_RESULT_DEVELOPER_ERROR = 5;
	public const int BILLING_RESPONSE_RESULT_ERROR = 6;
	public const int BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED = 7;
	public const int BILLING_RESPONSE_RESULT_ITEM_NOT_OWNED = 8;

	// IAB Helper error codes
	public const int BILLINGHELPER_ERROR_BASE = -1000;
	public const int BILLINGHELPER_REMOTE_EXCEPTION = -1001;
	public const int BILLINGHELPER_BAD_RESPONSE = -1002;
	public const int BILLINGHELPER_VERIFICATION_FAILED = -1003;
	public const int BILLINGHELPER_SEND_INTENT_FAILED = -1004;
	public const int BILLINGHELPERR_USER_CANCELLED = -1005;
	public const int BILLINGHELPER_UNKNOWN_PURCHASE_RESPONSE = -1006;
	public const int BILLINGHELPER_MISSING_TOKEN = -1007;
	public const int BILLINGHELPER_UNKNOWN_ERROR = -1008;
	public const int BILLINGHELPER_SUBSCRIPTIONS_NOT_AVAILABLE = -1009;
	public const int BILLINGHELPER_INVALID_CONSUMPTION = -1010;
}
