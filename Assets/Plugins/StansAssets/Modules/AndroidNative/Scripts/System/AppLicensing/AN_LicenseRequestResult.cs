using UnityEngine;
using System.Collections;

public class AN_LicenseRequestResult {

	private AN_LicenseStatusCode _status = AN_LicenseStatusCode.RESULT_ALLOW;
	private AN_LicenseErrorCode _error = AN_LicenseErrorCode.ERROR_NONE;

	private AN_LicenseRequestResult() {}

	public AN_LicenseRequestResult(AN_LicenseStatusCode status, AN_LicenseErrorCode error = AN_LicenseErrorCode.ERROR_NONE) {
		_status = status;
		_error = error;
	}

	public AN_LicenseStatusCode Status {
		get {
			return _status;
		}
	}

	public AN_LicenseErrorCode Error {
		get {
			return _error;
		}
	}
}
