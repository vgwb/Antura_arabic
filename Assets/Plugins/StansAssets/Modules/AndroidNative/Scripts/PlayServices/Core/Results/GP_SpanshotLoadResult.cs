using UnityEngine;
using System.Collections;

public class GP_SpanshotLoadResult : GooglePlayResult {

	private GP_Snapshot _snapshot = null;


	public GP_SpanshotLoadResult(string code):base(code) {

	}


	public void SetSnapShot(GP_Snapshot s) {
		_snapshot = s;
	}


	public GP_Snapshot Snapshot {
		get {
			return _snapshot;
		}
	}
}
