using UnityEngine;
using System.Collections;

public class FB_LoginResult : FB_Result {

	private bool _IsCanceled;
	private string _UserId;
	private string _AccessToken;


	public FB_LoginResult(string RawData, string Error, bool isCanceled):base(RawData, Error) {
		
		_IsCanceled = isCanceled;
	}


	public void SetCredential(string userId, string accessToken) {
		_UserId = userId;
		_AccessToken = accessToken;
	}


	public string UserId {
		get {
			return _UserId;
		}
	}

	public string AccessToken {
		get {
			return _AccessToken;
		}
	}

	public bool IsCanceled {
		get {
			return _IsCanceled;
		}
	}
}
