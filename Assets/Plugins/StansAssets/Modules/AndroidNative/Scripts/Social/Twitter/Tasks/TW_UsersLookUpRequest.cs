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

public class TW_UsersLookUpRequest : TW_APIRequest {
	

	public static TW_UsersLookUpRequest Create() {
		return new GameObject("TW_UersLookUpRequest").AddComponent<TW_UsersLookUpRequest>();
	}

	void Awake() {

		//https://dev.twitter.com/docs/api/1.1/get/users/lookup
		SetUrl("https://api.twitter.com/1.1/users/lookup.json");
	}


	protected override void OnResult(string data) {



		List<TwitterUserInfo> loadedUsers =  new List<TwitterUserInfo>();
		foreach(object user in ANMiniJSON.Json.Deserialize(data) as List<object>) {
			TwitterUserInfo userInfo =  new TwitterUserInfo(user as IDictionary);
			TwitterDataCash.AddUser(userInfo);

			loadedUsers.Add(userInfo);
		}


		TW_APIRequstResult result = new TW_APIRequstResult(true, data);
		result.users = loadedUsers;

		SendCompleteResult(result);

	}



}
