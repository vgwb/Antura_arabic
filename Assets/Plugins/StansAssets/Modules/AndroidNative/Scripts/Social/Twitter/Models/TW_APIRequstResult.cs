using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TW_APIRequstResult  {

	public List<TweetTemplate> tweets  = new List<TweetTemplate>();
	public List<TwitterUserInfo> users = new List<TwitterUserInfo>() ;

	public List<string> ids = new List<string>() ;

	
	private bool _IsSucceeded = false;
	private string _data = string.Empty;
	
	
	public TW_APIRequstResult(bool IsResSucceeded, string resData) {
		_IsSucceeded = IsResSucceeded;
		_data = resData;	
	}

	public bool IsSucceeded {
		get {
			return _IsSucceeded;
		}
	} 

	public string responce {
		get {
			return _data;
		}
	}


}
