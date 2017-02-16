using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GooglePlayQuests : SA.Common.Pattern.Singleton<GooglePlayQuests> {

	//Actions
	public Action<GP_QuestResult> OnQuestsLoaded =  delegate{};
	public Action<GP_QuestResult> OnQuestsAccepted =  delegate{};
	public Action<GP_QuestResult> OnQuestsCompleted =  delegate{};



	public static GP_QuestsSelect[] SELECT_ALL_QUESTS =  {
												GP_QuestsSelect.SELECT_ACCEPTED,
												GP_QuestsSelect.SELECT_COMPLETED,
												GP_QuestsSelect.SELECT_COMPLETED_UNCLAIMED,
												GP_QuestsSelect.SELECT_ENDING_SOON,
												GP_QuestsSelect.SELECT_EXPIRED,
												GP_QuestsSelect.SELECT_FAILED,
												GP_QuestsSelect.SELECT_OPEN,
												GP_QuestsSelect.SELECT_UPCOMING
											};


	private Dictionary<string, GP_Quest> _Quests =  new Dictionary<string, GP_Quest>();


	//--------------------------------------
	// INITIALIZATION
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public void Init() {
		//empty for now, used juts to create GO
	}

	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------


	public void LoadQuests(GP_QuestSortOrder sortOrder) {
		LoadQuests(sortOrder, SELECT_ALL_QUESTS);
	}

	public void LoadQuests(GP_QuestSortOrder sortOrder, params GP_QuestsSelect[] selectors) {
		if (!GooglePlayConnection.CheckState ()) { return; }

		string questSelectors = "";
		for(int i= 0; i <  selectors.Length; i++) {
			int v = (int) selectors[i];
			questSelectors += v.ToString();
			questSelectors += ",";
		}
		
		questSelectors = questSelectors.TrimEnd(',');

		AN_GMSQuestsEventsProxy.loadQuests(questSelectors, (int) sortOrder);
	}


	public void ShowQuests() {
		ShowQuests(SELECT_ALL_QUESTS);
	}

	public void ShowQuests(params GP_QuestsSelect[] selectors) {
		if (!GooglePlayConnection.CheckState ()) { return; }


		string questSelectors = "";
		for(int i= 0; i <  selectors.Length; i++) {
			int v = (int) selectors[i];
			questSelectors += v.ToString();
			questSelectors += ",";
		}

		questSelectors = questSelectors.TrimEnd(',');


		AN_GMSQuestsEventsProxy.showSelectedQuests(questSelectors);
	}

	public void AcceptQuest(string id) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSQuestsEventsProxy.acceptQuest(id);
	}


	public GP_Quest GetQuestById(string id) {
		if(_Quests.ContainsKey(id)) {
			return _Quests[id];
		} else {
			GP_Quest q =  new GP_Quest();
			q.Id = id;
			return q;
		}
	}

	public List<GP_Quest> GetQuests() {
		List<GP_Quest> quests =  new List<GP_Quest>();
		foreach(KeyValuePair<string, GP_Quest> entry in _Quests) {
			quests.Add(entry.Value);
		}

		return quests;
	}


	public List<GP_Quest> GetQuestsByState(GP_QuestState state) {
		List<GP_Quest> quests =  new List<GP_Quest>();
		foreach(KeyValuePair<string, GP_Quest> entry in _Quests) {
			if(state == entry.Value.state) {
				quests.Add(entry.Value);
			}
		}
		
		return quests;
	}



	//--------------------------------------
	// LISTNERS
	//--------------------------------------
	

	private void OnGPQuestAccepted(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GP_QuestResult result = new GP_QuestResult (storeData [0]);
		result.quest = GetQuestById (storeData [1]);

		OnQuestsAccepted(result);

	}


	private void OnGPQuestCompleted(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		GP_QuestResult result = new GP_QuestResult (storeData [0]);
		result.quest = GetQuestById (storeData [1]);

		OnQuestsCompleted(result);
	
	}


	private void OnGPQuestUpdated(string data) {

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		int i = 0;
		
		UpdateQuestInfo(
			storeData[i],
			storeData[i + 1],
			storeData[i + 2],
			storeData[i + 3],
			storeData[i + 4],
			storeData[i + 5],
			storeData[i + 6],
			storeData[i + 7],
			storeData[i + 8],
			storeData[i + 9],
			storeData[i + 10],
			storeData[i + 11]
			);

	}


	private void OnGPQuestsLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);


		GP_QuestResult result = new GP_QuestResult (storeData [0]);
		if(result.IsSucceeded) {
			
			for(int i = 1; i < storeData.Length; i+=12) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}

				UpdateQuestInfo(
					storeData[i],
					storeData[i + 1],
					storeData[i + 2],
					storeData[i + 3],
					storeData[i + 4],
					storeData[i + 5],
					storeData[i + 6],
					storeData[i + 7],
					storeData[i + 8],
					storeData[i + 9],
					storeData[i + 10],
					storeData[i + 11]
					);

			}
		}


		OnQuestsLoaded(result);
		Debug.Log ("OnGPQuestsLoaded, total:" + _Quests.Count.ToString());
		
	}



	private void UpdateQuestInfo(string id, string name, string descr, string icon, string banner, string state, 
	                             string timeUpdated, string timeAccepted, string timeEnded, string rewardData, string currentProgress, string targetProgress) {

		GP_Quest quest;
		if(_Quests.ContainsKey(id)) {
			quest = _Quests[id];
		} else {
			quest = new GP_Quest();
			quest.Id = id;
			_Quests.Add(quest.Id, quest);
		}

		quest.Name = name;
		quest.Description = descr;
		quest.IconImageUrl = icon;
		quest.BannerImageUrl = banner;

		int intState = System.Convert.ToInt32(state);
		quest.state = (GP_QuestState) intState;

		quest.LastUpdatedTimestamp  = System.Convert.ToInt64(timeUpdated);
		quest.AcceptedTimestamp 	= System.Convert.ToInt64(timeAccepted);
		quest.EndTimestamp 			= System.Convert.ToInt64(timeEnded);

		quest.RewardData = System.Text.Encoding.UTF8.GetBytes(rewardData);
		quest.CurrentProgress = System.Convert.ToInt64(currentProgress);
		quest.TargetProgress = System.Convert.ToInt64(targetProgress);
		
		if(AndroidNativeSettings.Instance.LoadQuestsIcons) {
			quest.LoadIcon();
		}

		if(AndroidNativeSettings.Instance.LoadQuestsImages) {
			quest.LoadBanner();
		}
	}

}
