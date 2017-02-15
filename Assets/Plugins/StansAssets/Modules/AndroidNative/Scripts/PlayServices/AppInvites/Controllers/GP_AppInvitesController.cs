using UnityEngine;
using System;
using System.Collections;

public class GP_AppInvitesController : SA.Common.Pattern.Singleton<GP_AppInvitesController> {

	public static event Action<GP_SendAppInvitesResult> ActionAppInvitesSent	= delegate {};
	public static event Action<GP_RetrieveAppInviteResult> ActionAppInviteRetrieved	= delegate {};



	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	/// <summary>
	/// Opens the contact chooser where the user selects the contacts to invite.
	/// </summary>
	/// <param name="builder">GP_AppInviteBuilder builder object used to build invitation intent</param>
	public void StartInvitationDialog(GP_AppInviteBuilder builder) {
		AN_AppInvitesProxy.StartInvitationDialog(builder.Id);
	}


	/// <summary>
	/// Get the invitation data, if app was downloaded by invitation.
	/// </summary>
	/// <param name="autoLaunchDeepLink">If true, launch the app with the deep link set as the data of the launch intent if initial install.</param>
	public void GetInvitation(bool autoLaunchDeepLink = false) {
		AN_AppInvitesProxy.GetInvitation(autoLaunchDeepLink);
	}




	
	//--------------------------------------
	// Native Events
	//--------------------------------------


	private void OnInvitationDialogComplete(string InvitationIds) {
		string[] invites = AndroidNative.StringToArray(InvitationIds);
		GP_SendAppInvitesResult result =  new GP_SendAppInvitesResult(invites);
		ActionAppInvitesSent(result);
	}


	private void OnInvitationDialogFailed(string erroCode) {
		GP_SendAppInvitesResult result =  new GP_SendAppInvitesResult(erroCode);
		ActionAppInvitesSent(result);
	}


	private void OnInvitationLoadFailed(string erroCode) {
		GP_RetrieveAppInviteResult result =  new GP_RetrieveAppInviteResult(erroCode);
		ActionAppInviteRetrieved(result); 
	}

	private void OnInvitationLoaded(string data) {
		string[] Data;
		Data = data.Split(AndroidNative.DATA_SPLITTER [0]);

		string link = Data[0];
		string inivtationId = Data[1];
		bool isOpenedFromPlayStore = System.Convert.ToBoolean(Data[2]);


		GP_AppInvite appInvite =  new GP_AppInvite(inivtationId, link, isOpenedFromPlayStore);


		GP_RetrieveAppInviteResult result =  new GP_RetrieveAppInviteResult(appInvite);
		ActionAppInviteRetrieved(result); 
	}
	
	
	
	
	
	
	
	
	
	
	
	



}
