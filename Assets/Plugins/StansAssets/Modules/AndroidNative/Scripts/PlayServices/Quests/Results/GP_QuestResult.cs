using UnityEngine;
using System.Collections;

public class GP_QuestResult : GooglePlayResult {

	public GP_Quest quest;
	
	public GP_QuestResult(string code):base(code) {
	}

	public GP_Quest GetQuest() {
		return quest;
	}
}
