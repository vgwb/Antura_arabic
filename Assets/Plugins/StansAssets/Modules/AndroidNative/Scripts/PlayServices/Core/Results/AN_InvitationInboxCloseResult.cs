using UnityEngine;
using System.Collections;

public class AN_InvitationInboxCloseResult : MonoBehaviour {
	private AdroidActivityResultCodes _resultCode;

	public AN_InvitationInboxCloseResult(string result) {
		_resultCode = (AdroidActivityResultCodes) System.Convert.ToInt32(result);
	}

	public AdroidActivityResultCodes ResultCode {
		get {
			return _resultCode;
		}
	}
}
