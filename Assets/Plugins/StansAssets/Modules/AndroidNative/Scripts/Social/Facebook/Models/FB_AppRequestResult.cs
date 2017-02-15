using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FB_AppRequestResult : FB_Result  {



	//The request object ID. 
	private string _ReuqestId = string.Empty;

	//An array of the recipient user IDs for the request that was created.
	private List<string> _Recipients =  new List<string>();


	public FB_AppRequestResult(string RawData, string Error): base(RawData, Error) {
		if(_IsSucceeded) {
			try {
				Dictionary<string, object> JSON = ANMiniJSON.Json.Deserialize(RawData) as Dictionary<string, object>;
				if(JSON.ContainsKey("request")) {					
					_ReuqestId = System.Convert.ToString(JSON["request"]);
				} else {
					_IsSucceeded = false;
				}				
				
				if(JSON.ContainsKey("to")) {
					List<object> Users = JSON["to"]  as List<object>;
					//We have multiple Recipients here
					if (Users != null) {
						foreach(object userId in  Users) {
							_Recipients.Add(System.Convert.ToString(userId));
						}
					} else {
						//Looks like only one Recipient was selected
						//Let's try to parse his Id
						string id = System.Convert.ToString(JSON["to"]);
						if (id != null) {
							_Recipients.Add(id);
						}
					}
				}
			} catch(System.Exception ex) {
				_IsSucceeded = false;
				Debug.Log("FB_AppRequestResult parsing failed: "  + ex.Message);
			}
		}
	}

	public FB_AppRequestResult(string requestId, List<string> _recipients, string RawData) : base(RawData, null) {

		if(requestId.Length > 0) {
			_ReuqestId = requestId;
			_Recipients = _recipients;
			_IsSucceeded = true;
		} else {
			_IsSucceeded = false;
		}
	}
	
	public string ReuqestId {
		get {
			return _ReuqestId;
		}
	}

	public List<string> Recipients {
		get {
			return _Recipients;
		}
	}
}
