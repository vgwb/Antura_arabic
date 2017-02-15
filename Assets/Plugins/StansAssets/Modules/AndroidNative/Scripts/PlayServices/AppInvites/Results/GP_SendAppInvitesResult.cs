using UnityEngine;
using System.Collections;

public class GP_SendAppInvitesResult : GooglePlayResult {


	private string[] _InvitationIds  = null;

	public GP_SendAppInvitesResult(string code):base(code) {}

	public GP_SendAppInvitesResult(string[] invites):base(GP_GamesStatusCodes.STATUS_OK) {
		_InvitationIds = invites;
	}


	public string[] InvitationIds {
		get {
			return _InvitationIds;
		}
	}
}
