////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class GPScore  {


	private int _rank = 0;
	private long _score = 0;

	private string _playerId;
	private string _leaderboardId;

	private GPCollectionType _collection;
	private GPBoardTimeSpan _timeSpan;

	

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	public GPScore(long vScore, int vRank, GPBoardTimeSpan vTimeSpan, GPCollectionType sCollection, string lid, string pid) {
		_score = vScore;
		_rank = vRank;

		_playerId = pid;
		_leaderboardId = lid;
	

		_timeSpan  = vTimeSpan;
		_collection = sCollection;

	}


	public void UpdateScore(long vScore) {
		_score = vScore;
	}



	//--------------------------------------
	// GET / SET
	//--------------------------------------

	[System.Obsolete("rank is deprectaed, plase use Rank instead")]
	public int rank {
		get {
			return _rank;
		}
	}

	public int Rank {
		get {
			return _rank;
		}
	}

	[System.Obsolete("score is deprectaed, plase use LongScore instead")]
	public long score {
		get {
			return _score;
		}
	}

	public long LongScore {
		get {
			return _score;
		}
	}

	public float CurrencyScore {
		get {
			return _score / 100.0f;
		}
	}

	public System.TimeSpan TimeScore {
		get {
			return System.TimeSpan.FromMilliseconds(_score);
		}
	}




	[System.Obsolete("playerId is deprectaed, plase use PlayerId instead")]
	public string playerId {
		get {
			return _playerId;
		}
	}

	public string PlayerId {
		get {
			return _playerId;
		}
	}

	public GooglePlayerTemplate Player {
		get {
			return GooglePlayManager.Instance.GetPlayerById(PlayerId);
		}
	}

	[System.Obsolete("leaderboardId is deprectaed, plase use LeaderboardId instead")]
	public string leaderboardId {
		get {
			return _leaderboardId;
		}
	}

	public string LeaderboardId {
		get {
			return _leaderboardId;
		}
	}
	
	[System.Obsolete("collection is deprectaed, plase use Collection instead")]
	public GPCollectionType collection {
		get {
			return _collection;
		}
	}

	public GPCollectionType Collection {
		get {
			return _collection;
		}
	}

	[System.Obsolete("timeSpan is deprectaed, plase use TimeSpan instead")]
	public GPBoardTimeSpan timeSpan {
		get {
			return _timeSpan;
		}
	}

	public GPBoardTimeSpan TimeSpan {
		get {
			return _timeSpan;
		}
	}
}
