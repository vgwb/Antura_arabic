using UnityEngine;
using System;
using System.Collections;

public class AN_PlusShareListener : MonoBehaviour {
	public delegate void PlusShareCallback(SA.Common.Models.Result result);
	private PlusShareCallback builderCallback = delegate {};

	public void AttachBuilderCallback(PlusShareCallback callback) {
		builderCallback = callback;
	}
	
	private void OnPlusShareCallback(string res) {
		bool val = Boolean.Parse (res);

		SA.Common.Models.Result result;
		if(val) {
			result =  new SA.Common.Models.Result();
		} else {
			result = new SA.Common.Models.Result (new SA.Common.Models.Error ());
		}

		builderCallback(result);
	}
}
