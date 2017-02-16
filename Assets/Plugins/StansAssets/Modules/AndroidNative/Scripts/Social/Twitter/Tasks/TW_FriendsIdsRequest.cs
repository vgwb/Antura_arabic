////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// The documentation can be foudn at:
//https://dev.twitter.com/docs/api/1.1/get/statuses/user_timeline

public class TW_FriendsIdsRequest : TW_APIRequest {
	

	public static TW_FriendsIdsRequest Create() {
		return new GameObject("TW_FriendsIdsRequest").AddComponent<TW_FriendsIdsRequest>();
	}

	void Awake() {

		//https://dev.twitter.com/docs/api/1.1/get/friends/ids
		SetUrl("https://api.twitter.com/1.1/friends/ids.json");

	}


	protected override void OnResult(string data) {

		Dictionary<string, object>  ids = ANMiniJSON.Json.Deserialize(data) as Dictionary<string, object>;
		TW_APIRequstResult result = new TW_APIRequstResult(true, data);


		foreach(object id in (ids["ids"] as List<object>) ) {
		    string val =  System.Convert.ToString(id);
			result.ids.Add(val);
		}

	

		SendCompleteResult(result);

	}



}
