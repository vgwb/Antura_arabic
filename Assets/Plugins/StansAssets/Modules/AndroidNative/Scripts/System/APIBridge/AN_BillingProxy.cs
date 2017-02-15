using UnityEngine;
using System.Collections;

public class AN_BillingProxy  {

	private const string CLASS_NAME = "com.androidnative.billing.core.BillingManager";
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	
	// --------------------------------------
	// Social
	// --------------------------------------

	public static void Connect(string ids, string base64EncodedPublicKey) {
		CallActivityFunction("AN_Connect", ids, base64EncodedPublicKey);
	}

	public static void RetrieveProducDetails() {
		CallActivityFunction("AN_RetrieveProducDetails");
	}


	public static void Consume(string SKU) {
		CallActivityFunction("AN_Consume", SKU);
	}
	
	public static void Purchase(string SKU, string developerPayload) {
		CallActivityFunction("AN_Purchase", SKU, developerPayload);
	}
	
	
	public static void Subscribe(string SKU, string developerPayload) {
		CallActivityFunction("AN_Subscribe", SKU, developerPayload);
	}






}
