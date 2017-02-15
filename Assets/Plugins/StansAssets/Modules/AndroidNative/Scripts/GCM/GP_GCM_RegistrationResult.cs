using UnityEngine;
using System.Collections;

public class GP_GCM_RegistrationResult : GooglePlayResult {

	private string _RegistrationDeviceId = string.Empty;


	public GP_GCM_RegistrationResult():base(GP_GamesStatusCodes.STATUS_INTERNAL_ERROR) {
		
	}

	public GP_GCM_RegistrationResult(string id):base(GP_GamesStatusCodes.STATUS_OK) {
		_RegistrationDeviceId = id;
	}



	public string RegistrationDeviceId {
		get {
			return _RegistrationDeviceId;
		}
	}
}
