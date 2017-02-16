using UnityEngine;
using System.Collections;

public class AN_RefreshTokenResult : SA.Common.Models.Result {

	private string _accessToken = string.Empty;
	private string _refreshToken = string.Empty;
	private string _tokenType = string.Empty;
	private long _expiresIn = 0L;


	public AN_RefreshTokenResult(string errorMessage) : base(new SA.Common.Models.Error(errorMessage)) {

	}

	public AN_RefreshTokenResult(string accessToken, string refreshToken, string tokenType, long expiresIn) : base() {
		_accessToken = accessToken;
		_refreshToken = refreshToken;
		_tokenType = tokenType;
		_expiresIn = expiresIn;
	}
	
	public string AccessToken {
		get {
			return _accessToken;
		}
	}

	public string RefreshToken {
		get {
			return _refreshToken;
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
