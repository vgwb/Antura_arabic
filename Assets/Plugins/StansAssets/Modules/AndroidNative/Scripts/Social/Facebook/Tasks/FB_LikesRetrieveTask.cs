using UnityEngine;
using System;
using System.Collections;

public class FB_LikesRetrieveTask : MonoBehaviour {

	private string _userId;

	public event Action<FB_Result, FB_LikesRetrieveTask> ActionComplete = delegate{};


	public static FB_LikesRetrieveTask Create(){
		return new GameObject("FBLikesRetrieveTask").AddComponent<FB_LikesRetrieveTask>();
	}

	public void LoadLikes(string userId) {
		_userId =  userId;
		SPFacebook.Instance.FB.API("/" + userId + "/likes", FB_HttpMethod.GET, OnUserLikesResult);  
	}
	
	public void LoadLikes(string userId, string pageId ) {
		_userId =  userId;
		SPFacebook.Instance.FB.API("/" + userId + "/likes/" + pageId, FB_HttpMethod.GET, OnUserLikesResult);  
	}



	public string userId {
		get {
			return _userId;
		}
	}
	

	private void OnUserLikesResult(FB_Result result) {
		ActionComplete(result, this);
		Destroy(gameObject);
	}
}
