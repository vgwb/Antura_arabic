using UnityEngine;
using System.Collections;

public class GP_LeaderboardResult : GooglePlayResult {

	private GPLeaderBoard _Leaderboard;
	
	public GP_LeaderboardResult(GPLeaderBoard leaderboard, string code):base(code) {
		SetInfo(leaderboard);
	}

	private void SetInfo(GPLeaderBoard leaderboard) {
		_Leaderboard = leaderboard;
	}

	public GPLeaderBoard Leaderboard {
		get {
			return _Leaderboard;
		}
	}
}
