using UnityEngine;
using System.Collections;

public class GP_SnapshotConflict  {

	private GP_Snapshot _s1;
	private GP_Snapshot _s2;


	public GP_SnapshotConflict (GP_Snapshot s1, GP_Snapshot s2) {
		_s1 = s1;
		_s2 = s2;
	}


	public GP_Snapshot Snapshot {
		get {
			return _s1;
		}
	}


	public GP_Snapshot ConflictingSnapshot {
		get {
			return _s2;
		}
	}

	public void Resolve(GP_Snapshot snapshot) {
		if(snapshot.Equals(_s1)) {
			AN_GMSGeneralProxy.ResolveSnapshotsConflict_Bridge(0);
		} else {
			AN_GMSGeneralProxy.ResolveSnapshotsConflict_Bridge(1);
		}
	}
}
