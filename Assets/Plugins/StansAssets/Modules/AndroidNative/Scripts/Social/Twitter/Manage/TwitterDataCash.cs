using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TwitterDataCash  {


	private static Dictionary<string, TweetTemplate> tweets 	=  new Dictionary<string, TweetTemplate>();
	private static Dictionary<string, TwitterUserInfo> users 	=  new Dictionary<string, TwitterUserInfo>();



	public static void AddTweet(TweetTemplate t) {
		if(!tweets.ContainsKey(t.id)) {
			tweets.Add(t.id, t);
		}
	}

	public static TweetTemplate GetTweetsById(string id) {
		if(tweets.ContainsKey(id)) {
			return tweets[id];
		}

		return null;
	}


	public static void AddUser(TwitterUserInfo u) {
		if(!users.ContainsKey(u.id)) {
			users.Add(u.id, u);
		}
	}


	public static TwitterUserInfo GetUserById(string id) {
		if(users.ContainsKey(id)) {
			return users[id];
		}

		return null;
	}

}
