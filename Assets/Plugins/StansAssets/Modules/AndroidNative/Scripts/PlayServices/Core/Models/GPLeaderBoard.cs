////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GPLeaderBoard  {

	//Editor Only
	public bool IsOpen = true;

	[SerializeField]
	private string _id = string.Empty;

	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private string _description = string.Empty;

	[SerializeField]
	private Texture2D _Texture;

	private bool _CurrentPlayerScoreLoaded = false;
	
	public GPScoreCollection SocsialCollection =  new GPScoreCollection();
	public GPScoreCollection GlobalCollection  =  new GPScoreCollection();

	public List<GPScore> CurrentPlayerScore =  new List<GPScore>();
	private Dictionary<int, GP_LocalPlayerScoreUpdateListener> _ScoreUpdateListners;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------
	

	public GPLeaderBoard(string lId, string lName) {
		_id = lId;
		_name = lName;
	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	public void UpdateName(string lName) {
		_name = lName;
	}


	public List<GPScore> GetScoresList(GPBoardTimeSpan timeSpan, GPCollectionType collection) {
		GPScoreCollection col = GlobalCollection;
		
		switch(collection) {
		case GPCollectionType.GLOBAL:
			col = GlobalCollection;
			break;
		case GPCollectionType.FRIENDS:
			col = SocsialCollection;
			break;
		}
		
		
		Dictionary<int, GPScore> scoreDict = col.AllTimeScores;
		
		switch(timeSpan) {
		case GPBoardTimeSpan.ALL_TIME:
			scoreDict = col.AllTimeScores;
			break;
		case GPBoardTimeSpan.TODAY:
			scoreDict = col.TodayScores;
			break;
		case GPBoardTimeSpan.WEEK:
			scoreDict = col.WeekScores;
			break;
		}

		List<GPScore> scores = new List<GPScore>();
		scores.AddRange(scoreDict.Values);


		return scores;
	}



	public GPScore GetScoreByPlayerId(string playerId, GPBoardTimeSpan timeSpan, GPCollectionType collection) {
		List<GPScore> scores = GetScoresList(timeSpan, collection);
		foreach(GPScore s in scores) {
			if(s.PlayerId.Equals(playerId)) {
				return s;
			}
		}

		return null;
	}


	public GPScore GetScore(int rank, GPBoardTimeSpan timeSpan, GPCollectionType collection) {
		
		GPScoreCollection col = GlobalCollection;
		
		switch(collection) {
		case GPCollectionType.GLOBAL:
			col = GlobalCollection;
			break;
		case GPCollectionType.FRIENDS:
			col = SocsialCollection;
			break;
		}
		

		Dictionary<int, GPScore> scoreDict = col.AllTimeScores;
		
		switch(timeSpan) {
		case GPBoardTimeSpan.ALL_TIME:
			scoreDict = col.AllTimeScores;
			break;
		case GPBoardTimeSpan.TODAY:
			scoreDict = col.TodayScores;
			break;
		case GPBoardTimeSpan.WEEK:
			scoreDict = col.WeekScores;
			break;
		}


		if(scoreDict.ContainsKey(rank)) {
			return scoreDict[rank];
		} else {
			return null;
		}
		
	}

	public GPScore GetCurrentPlayerScore(GPBoardTimeSpan timeSpan, GPCollectionType collection) {
		foreach (GPScore score in CurrentPlayerScore) {
			if (score.TimeSpan == timeSpan && score.Collection == collection) {
				return score;
			}
		}

		return null;
	}

	public void CreateScoreListener(int requestId) {
		GP_LocalPlayerScoreUpdateListener listener = new GP_LocalPlayerScoreUpdateListener(requestId, Id);
		ScoreUpdateListners.Add(listener.RequestId, listener);
	}

	public void ReportLocalPlayerScoreUpdate (GPScore score, int requestId) {
		GP_LocalPlayerScoreUpdateListener listener = _ScoreUpdateListners[requestId];
		listener.ReportScoreUpdate(score);
	}

	public void ReportLocalPlayerScoreUpdateFail(string errorData, int requestId) {
		GP_LocalPlayerScoreUpdateListener listener = _ScoreUpdateListners[requestId];
		listener.ReportScoreUpdateFail(errorData);
	}

	public void UpdateCurrentPlayerScore(List<GPScore> newScores) {
		CurrentPlayerScore.Clear();
		foreach (GPScore score in newScores) {
			CurrentPlayerScore.Add(score);
		}
		_CurrentPlayerScoreLoaded = true;
	}

	public void UpdateCurrentPlayerScore(GPScore score) {		
		GPScore currentScore = GetCurrentPlayerScore(score.TimeSpan, score.Collection);
		if (currentScore != null) {
			CurrentPlayerScore.Remove(currentScore);
		}		
		CurrentPlayerScore.Add(score);
		_CurrentPlayerScoreLoaded = true;
	}
	
	public void UpdateScore(GPScore score) {
	
		GPScoreCollection col = GlobalCollection;
		
		switch(score.Collection) {
		case GPCollectionType.GLOBAL:
			col = GlobalCollection;
			break;
		case GPCollectionType.FRIENDS:
			col = SocsialCollection;
			break;
		}

		
		Dictionary<int, GPScore> scoreDict = col.AllTimeScores;
		
		switch(score.TimeSpan) {
		case GPBoardTimeSpan.ALL_TIME:
			scoreDict = col.AllTimeScores;
			break;
		case GPBoardTimeSpan.TODAY:
			scoreDict = col.TodayScores;
			break;
		case GPBoardTimeSpan.WEEK:
			scoreDict = col.WeekScores;
			break;
		}
	
		if(scoreDict.ContainsKey(score.Rank)) {
			scoreDict[score.Rank] = score;
		} else {
			scoreDict.Add(score.Rank, score);
		}
	}

	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public string Id {
		get {
			return _id;
		}
		set {
			_id = value;
		}
	}

	public string Name {
		get {
			return _name;
		}
		set {
			_name = value;
		}
	}

	public string Description {
		get {
			return _description;
		}
		set {
			_description = value;
		}
	}

	public Texture2D Texture {
		get {
			return _Texture;
		}
		set {
			_Texture = value;
		}
	}

	public bool CurrentPlayerScoreLoaded {
		get {
			return _CurrentPlayerScoreLoaded;
		}
	}

	private Dictionary<int, GP_LocalPlayerScoreUpdateListener> ScoreUpdateListners {
		get {
			if(_ScoreUpdateListners == null) {
				_ScoreUpdateListners	=  new Dictionary<int, GP_LocalPlayerScoreUpdateListener>();
			}

			return _ScoreUpdateListners;
		}
	}
}
