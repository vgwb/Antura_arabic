using UnityEngine;
using System.Collections;

public class GP_RetrieveAppInviteResult : GooglePlayResult {

	private GP_AppInvite _AppInvite = null;

	public GP_RetrieveAppInviteResult(string code):base(code) {}


	public GP_RetrieveAppInviteResult(GP_AppInvite invite):base(GP_GamesStatusCodes.STATUS_OK) {
		_AppInvite = invite;
	}



	public GP_AppInvite AppInvite {
		get {
			return _AppInvite;
		}
	}
}
