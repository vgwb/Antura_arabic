using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GP_AppInviteBuilder  {

	private int _Id;


	/// <summary>
	/// App invite invitation builder.
	/// </summary>
	/// <param name="title">Sets the title for the invitation Activity.</param>
	public GP_AppInviteBuilder(string title) {
		_Id = SA.Common.Util.IdFactory.NextId;
		AN_AppInvitesProxy.CreateBuilder(_Id, title);
	}

	/// <summary>
	/// Sets the invite message that is sent to all invitees. 
	/// </summary>
	/// <param name="msg">The invite message.</param>
	public void SetMessage(string msg) {
		AN_AppInvitesProxy.SetMessage(_Id, msg);
	}

	/// <summary>
	/// Sets the deep link that is made available to the app when opened from the invitation. This deep link is made available both to a newly installed application and an already installed application. The deep link can be sent to Android and other platforms so should be formatted to support deep links across platforms.
	/// </summary>
	/// <param name="url">The app deep link.</param>
	public void SetDeepLink(string url) {
		AN_AppInvitesProxy.SetDeepLink(_Id, url);
	}

	/// <summary>
	/// Text shown on the email invitation for the user to accept the invitation. Default install text used if not set.
	/// </summary>
	/// <param name="actionText">Text to use on the invitation button.</param>
	public void SetCallToActionText(string actionText) {
		AN_AppInvitesProxy.SetCallToActionText(_Id, actionText);
	}

	/// <summary>
	/// Sets the Google Analytics Tracking id. The tracking id should be created for the calling application under Google Analytics
	/// </summary>
	/// <param name="trackingId">String of the form UA-xxxx-y</param>
	public void SetGoogleAnalyticsTrackingId(string trackingId) {
		AN_AppInvitesProxy.SetGoogleAnalyticsTrackingId(_Id, trackingId);
	}

	/// <summary>
	/// Sets the minimum version of the android app installed on the receiving device. If this minimum version is not installed then the install flow will be triggered.
	/// </summary>
	/// <param name="versionCode">Minimum version of the android app. It will be compared with the android version code and if below this minimum the app will be considered not installed. Defaults to 0 which disables the version check.</param>
	public void SetAndroidMinimumVersionCode(int versionCode) {
		AN_AppInvitesProxy.SetAndroidMinimumVersionCode(_Id, versionCode);
	}

	///<summary>
	///<para>Adds query parameters to the play store referral URL when the app needs additional referral parameters for other application component referrals.</para>
	///<para>The parameters are set as name, value pairs that will be set as query parameter name and value on the referral URL.</para>
	///</summary>
	/// <param name="referralParameters">Referral parameters defined as name value pairs.</param>
	public void SetAdditionalReferralParameters(Dictionary<string, string> referralParameters) {

		List<string> values =  new List<string>();
		List<string> keys =  new List<string>();

		foreach(KeyValuePair<string, string> pair in referralParameters) {
			values.Add(pair.Value);
			keys.Add(pair.Key);
		}

		string valuesString = AndroidNative.ArrayToString(values.ToArray());
		string keysString = AndroidNative.ArrayToString(keys.ToArray());

	
		AN_AppInvitesProxy.SetAdditionalReferralParameters(_Id, keysString, valuesString);
	}


	public int Id {
		get {
			return _Id;
		}
	}
}
