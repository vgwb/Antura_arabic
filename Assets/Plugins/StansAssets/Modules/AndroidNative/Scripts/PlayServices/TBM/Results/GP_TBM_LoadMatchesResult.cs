using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GP_TBM_LoadMatchesResult : GooglePlayResult {
	
	public Dictionary<string, GP_TBM_Match> LoadedMatches;

	public GP_TBM_LoadMatchesResult(string code):base(code) {
		
	}
}
