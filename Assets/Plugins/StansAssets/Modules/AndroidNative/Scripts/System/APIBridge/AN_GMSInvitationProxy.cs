using UnityEngine;
using System.Collections;

public class AN_GMSInvitationProxy : MonoBehaviour {
	
	private const string CLASS_NAME = "com.androidnative.gms.core.GameInvitationManager";
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	
	//--------------------------------------
	// INVITATIONS
	//--------------------------------------
	
	public static void registerInvitationListener() {
		CallActivityFunction("registerInvitationListener");
	}
	
	public static void unregisterInvitationListener() {
		CallActivityFunction("unregisterInvitationListener");
	}
	
	
	public static void LoadInvitations() {
		CallActivityFunction("loadInvitations");
	}
	
	
}
