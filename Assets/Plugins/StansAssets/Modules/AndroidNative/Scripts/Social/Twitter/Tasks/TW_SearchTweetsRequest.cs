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

public class TW_SearchTweetsRequest : TW_APIRequest {
	

	public static TW_SearchTweetsRequest Create() {
		return new GameObject("TW_SearchTweetsRequest").AddComponent<TW_SearchTweetsRequest>();
	}

	void Awake() {
		//https://dev.twitter.com/docs/api/1.1/get/search/tweets
		SetUrl("https://api.twitter.com/1.1/search/tweets.json");

	}


	protected override void OnResult(string data) {

		List<TweetTemplate> loadedTweets =  new List<TweetTemplate>();


		Dictionary<string, object> statuses = ANMiniJSON.Json.Deserialize(data) as Dictionary<string, object>;

		List<object> tweets = statuses["statuses"] as List<object>;

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
