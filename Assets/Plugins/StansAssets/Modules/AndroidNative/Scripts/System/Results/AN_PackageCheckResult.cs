using UnityEngine;
using System.Collections;

public class AN_PackageCheckResult : SA.Common.Models.Result {


	private string _packageName;

	public AN_PackageCheckResult (string packId):base() {
		_packageName = packId;
	}

	public AN_PackageCheckResult(string packId, SA.Common.Models.Error error):base(error)  {
		_packageName = packId;
	}



	public string packageName {
		get {
			return _packageName;
		}
	}
}
