using UnityEngine;
using System.Collections.Generic;

public class GP_LocalPlayerScoreUpdateListener {

	private int _RequestId;
	private string _leaderboardId;

	private string _ErrorData = null;
	
	private List<GPScore> Scores =  new List<GPScore>();

	//--------------------------------------
	// Initialization
	//--------------------------------------
	
	public GP_LocalPlayerScoreUpdateListener (int requestId, string leaderboardId) {
		_RequestId = requestId;
		_leaderboardId = leaderboardId;
	}

	//--------------------------------------
	// Public Methods
	//--------------------------------------
	
	public void ReportScoreUpdate(GPScore score) {
		Scores.Add(score);
		DispatchUpdate();
	}
	
	public void ReportScoreUpdateFail(string errorData) {
		Debug.Log("ReportScoreUpdateFail");
		_ErrorData = errorData;
		Scores.Add(null);
		
		DispatchUpdate();
	}

	//--------------------------------------
	// Get / Set
	//--------------------------------------
	
	public int RequestId {
		get {
			return _RequestId;
		}
	}

	//--------------------------------------
	// Private Methods
	//--------------------------------------
	
	private void DispatchUpdate() {
		if(Scores.Count == 6) {
			
			GPLeaderBoard board = GooglePlayManager.Instance.GetLeaderBoard(_leaderboardId);
			GP_LeaderboardResult result;			
			
			if(_ErrorData != null) {
				result =  new GP_LeaderboardResult(board, _ErrorData);
			} else {
				board.UpdateCurrentPlayerScore(Scores);
				result =  new GP_LeaderboardResult(board, _ErrorData);
			}

			GooglePlayManager.Instance.DispatchLeaderboardUpdateEvent(result);
			
		}
	}

}
