using UnityEngine;
using System.Collections;

public static class AndroidAdMob {
	private static GoogleMobileAdInterface _Client = null;
	public static GoogleMobileAdInterface Client{
		get {
			if(_Client == null) {
				_Client = AndroidAdMobController.Instance;
				_Client.InitEditorTesting(AndroidNativeSettings.Instance.Is_Ad_EditorTestingEnabled, AndroidNativeSettings.Instance.Ad_EditorFillRate);
			}

			return _Client;
		}
		
	}
}
