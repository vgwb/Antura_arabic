using UnityEngine;
using System.Collections;

public class AN_DeviceCodeResult : SA.Common.Models.Result {

	private string _deviceCode = string.Empty;
	private string _userCode = string.Empty;
	private string _verificationUrl = string.Empty;
	private long _expiresIn = 0L;
	private long _interval = 0L;

	public AN_DeviceCodeResult(string errorMessage) : base(new SA.Common.Models.Error(0, errorMessage)) {
		
	}

	public AN_DeviceCodeResult(string deviceCode, string userCode, string verificationUrl, long expiresIn, long interval) : base() {
		_deviceCode = deviceCode;
		_userCode = userCode;
		_verificationUrl = verificationUrl;
		_expiresIn = expiresIn;
		_interval = interval;
	}

	public string DeviceCode {
		get {
			return _deviceCode;
		}
	}

	public string UserCode {
		get {
			return _userCode;
		}
	}

	public string VerificationUrl {
		get {
			return _verificationUrl;
		}
	}

	public long ExpiresIn {
		get {
			return _expiresIn;
		}
	}

	public long Interval {
		get {
			return _interval;
		}
	}


}
