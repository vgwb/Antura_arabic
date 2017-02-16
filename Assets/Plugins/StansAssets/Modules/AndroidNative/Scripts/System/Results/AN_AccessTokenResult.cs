using UnityEngine;
using System.Collections;

public class AN_AccessTokenResult :  SA.Common.Models.Result {

	private string _accessToken = string.Empty;
	private string _tokenType = string.Empty;
	private long _expiresIn = 0L;


	public AN_AccessTokenResult(string errorMessage) : base (new SA.Common.Models.Error(0, errorMessage)) {

	}

	public AN_AccessTokenResult(string accessToken, string tokenType, long expiresIn) : base() {
		_accessToken = accessToken;
		_tokenType = tokenType;
		_expiresIn = expiresIn;
	}

	public string AccessToken {
		get {
			return _accessToken;
		}
	}

	public string TokenType {
		get {
			return _tokenType;
		}
	}

	public long ExpiresIn {
		get {
			return _expiresIn;
		}
	}
		
}
