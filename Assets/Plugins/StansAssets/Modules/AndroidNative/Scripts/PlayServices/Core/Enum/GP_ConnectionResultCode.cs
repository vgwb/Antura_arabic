using UnityEngine;
using System.Collections;

public enum GP_ConnectionResultCode  {

	//results description can be found at:
	//http://developer.android.com/reference/com/google/android/gms/common/ConnectionResult.html

	CANCELED = 13,
	DATE_INVALID = 12,
	DEVELOPER_ERROR = 10,
	DRIVE_EXTERNAL_STORAGE_REQUIRED = 1500,
	INTERNAL_ERROR = 8,
	INTERRUPTED = 15,
	INVALID_ACCOUNT = 5,
	LICENSE_CHECK_FAILED = 11,
	NETWORK_ERROR = 7,
	RESOLUTION_REQUIRED = 6,
	SERVICE_DISABLED = 3,
	SERVICE_INVALID = 9,
	SERVICE_MISSING = 1,
	SERVICE_VERSION_UPDATE_REQUIRED = 2,
	SIGN_IN_REQUIRED = 4,
	SUCCESS = 0,
	TIMEOUT = 14

}

