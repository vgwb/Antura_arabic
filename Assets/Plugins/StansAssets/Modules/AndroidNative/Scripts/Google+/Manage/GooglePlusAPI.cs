using UnityEngine;
using System;
using System.Collections;

public class GooglePlusAPI : SA.Common.Pattern.Singleton<GooglePlusAPI> {


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	[Obsolete("clearDefaultAccount is deprecated, please use ClearDefaultAccount instead.")]
	public void clearDefaultAccount() {
		ClearDefaultAccount();
	}

	public void ClearDefaultAccount() {
		if (!GooglePlayConnection.CheckState ()) { return; }

		AN_GMSGeneralProxy.clearDefaultAccount();
		GooglePlayConnection.Instance.Disconnect();
	}
}
