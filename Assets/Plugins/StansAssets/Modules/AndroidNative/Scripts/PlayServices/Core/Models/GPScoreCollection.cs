using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GPScoreCollection {

	public Dictionary<int, GPScore> AllTimeScores 	=  new Dictionary<int, GPScore>();
	public Dictionary<int, GPScore> WeekScores 		=  new Dictionary<int, GPScore>();
	public Dictionary<int, GPScore> TodayScores		=  new Dictionary<int, GPScore>();

}

