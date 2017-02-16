////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


// The documentation can be foudn at:
//https://dev.twitter.com/docs/api/1.1/get/statuses/user_timeline

public class TW_UserTimeLineRequest : TW_APIRequest {



	public static TW_UserTimeLineRequest Create() {
		return new GameObject("TW_TimeLineRequest").AddComponent<TW_UserTimeLineRequest>();
	}

	void Awake() {
		//https://dev.twitter.com/docs/api/1/get/statuses/user_timeline
		SetUrl("https://api.twitter.com/1.1/statuses/user_timeline.json");
	}


	protected override void OnResult(string data) {


		List<TweetTemplate> loadedTweets =  new List<TweetTemplate>();
		List<object> tweets =  ANMiniJSON.Json.Deserialize(data) as List<object>;
		foreach(object tweet in tweets) {

			Dictionary<string, object> tweetJSON = (tweet as Dictionary<string, object>);

			TweetTemplate tpl =  new TweetTemplate();
			tpl.id 							= tweetJSON["id_str"] as string;
			tpl.created_at 					= tweetJSON["created_at"] as string;
			tpl.text 						= tweetJSON["text"] as string;
			tpl.source 						= tweetJSON["source"] as string;
			tpl.in_reply_to_status_id 		= tweetJSON["in_reply_to_status_id"] as string;
			tpl.in_reply_to_user_id 		= tweetJSON["in_reply_to_user_id"] as string;
			tpl.in_reply_to_screen_name 	= tweetJSON["in_reply_to_screen_name"] as string;
			tpl.geo 						= tweetJSON["geo"] as string;
			tpl.place 						= tweetJSON["place"] as string;
			tpl.lang 						= tweetJSON["lang"] as string;


			tpl.retweet_count 				= System.Convert.ToInt32(tweetJSON["retweet_count"] as string);
			tpl.favorite_count 				= System.Convert.ToInt32(tweetJSON["favorite_count"] as string);


			TwitterUserInfo user =  new TwitterUserInfo(tweetJSON["user"] as IDictionary);
			tpl.user_id = user.id;



			TwitterDataCash.AddTweet(tpl);
			TwitterDataCash.AddUser(user);
			loadedTweets.Add(tpl);
		}


		TW_APIRequstResult result = new TW_APIRequstResult(true, data);
		result.tweets = loadedTweets;

		SendCompleteResult(result);

	}



}
