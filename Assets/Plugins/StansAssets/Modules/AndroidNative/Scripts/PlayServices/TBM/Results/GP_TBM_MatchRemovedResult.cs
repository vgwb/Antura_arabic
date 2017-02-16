using UnityEngine;
using System.Collections;

public class GP_TBM_MatchRemovedResult  {
	private string _MatchId;

	public GP_TBM_MatchRemovedResult(string mId) {
		_MatchId = mId;
	}

	public string MatchId {
		get {
			return _MatchId;
		}
	}
}
