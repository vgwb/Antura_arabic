using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using ANMiniJSON;

public class FB_PostResult  :FB_Result {

	
	private string _PostId = string.Empty;
	
	public FB_PostResult(string RawData, string Error): base(RawData, Error) {
		if(_IsSucceeded) {
			try {
				Dictionary<string, object> data =   ANMiniJSON.Json.Deserialize(RawData) as Dictionary<string, object>;
				_PostId = System.Convert.ToString(data["id"]);
				_IsSucceeded = true;
			} catch(System.Exception ex) {
				_IsSucceeded = false;
				Debug.Log("No Post Id: "  + ex.Message);
			}
		}
	}


	public string PostId {
		get {
			return _PostId;
		}
	}
	
}
