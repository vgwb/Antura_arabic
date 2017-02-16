//#define FBV7_API_ENABLED

using UnityEngine;
using System.Collections;

#if FBV7_API_ENABLED
using Facebook.Unity;



public class FB_GrapRequest_V7  {


	SPFacebook.FB_Delegate _Callback;
	

	public FB_GrapRequest_V7(string query, HttpMethod method, SPFacebook.FB_Delegate callback) {

		_Callback = callback;
		FB.API(query, method, GraphCallback);
	}

	public FB_GrapRequest_V7(string query, HttpMethod method, SPFacebook.FB_Delegate callback, WWWForm form) {
		_Callback = callback;
		FB.API(query, method, GraphCallback, form);
	}


	void GraphCallback (IGraphResult result) {
		FB_Result fb_result;

		if (result == null) {
			fb_result = new FB_Result(string.Empty, "Null Response");
		} else {
			fb_result =  new FB_Result(result.RawResult, result.Error);
		}

		_Callback(fb_result);
	}
}

#endif