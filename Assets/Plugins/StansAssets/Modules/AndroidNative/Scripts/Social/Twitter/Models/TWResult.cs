using UnityEngine;
using System.Collections;

public class TWResult {

	private bool _IsSucceeded = false;
	private string _data = string.Empty;


	public TWResult(bool IsResSucceeded, string resData) {
		_IsSucceeded = IsResSucceeded;
		_data = resData;

	}

	public bool IsSucceeded {
		get {
			return _IsSucceeded;
		}
	} 


	public string data  {
		get {
			return _data;
		}
	}
		
}

