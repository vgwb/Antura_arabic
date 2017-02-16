using UnityEngine;
using System.Collections;

public class TWAPITest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TW_OAuthAPIRequest request =  TW_OAuthAPIRequest.Create();

		request.AddParam("count", 1);
		request.Send("https://api.twitter.com/1.1/statuses/home_timeline.json");

		request.OnResult += OnResult;
	}
	


	void OnResult (TW_APIRequstResult result) {
		Debug.Log("Is Request Succeeded: " + result.IsSucceeded);
		Debug.Log("Responce data:");
		Debug.Log(result.responce);
	}
}
