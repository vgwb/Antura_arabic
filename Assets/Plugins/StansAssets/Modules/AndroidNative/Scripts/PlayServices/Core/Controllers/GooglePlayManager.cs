////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GooglePlayManager : SA.Common.Pattern.Singleton<GooglePlayManager> {
	
	//Actions
	public static event Action<GP_LeaderboardResult> ActionScoreSubmited 					= delegate {};
	public static event Action<GP_LeaderboardResult> ActionScoresListLoaded 				= delegate {};

	public static event Action<GooglePlayResult> ActionLeaderboardsLoaded 					= delegate {};

	public static event Action<GP_AchievementResult> ActionAchievementUpdated 				= delegate {};
	public static event Action<GooglePlayResult> ActionFriendsListLoaded 					= delegate {};
	public static event Action<GooglePlayResult> ActionAchievementsLoaded 					= delegate {};

	public static event Action<GooglePlayGiftRequestResult> ActionSendGiftResultReceived 	= delegate {};
	public static event Action ActionRequestsInboxDialogDismissed 							= delegate {};
	public static event Action<List<GPGameRequest>> ActionPendingGameRequestsDetected 		= delegate {};
	public static event Action<List<GPGameRequest>> ActionGameRequestsAccepted 				= delegate {};

	public static event Action<List<string>> ActionAvailableDeviceAccountsLoaded 			= delegate {};
	public static event Action<string> ActionOAuthTokenLoaded 								= delegate {};
	public static event Action<GooglePlayResult, string> ActionServerAuthCodeLoaded			= delegate {};

	private GooglePlayerTemplate _player = null ;

	private Dictionary<string, GooglePlayerTemplate> _players = new Dictionary<string, GooglePlayerTemplate>();

	private List<string> _friendsList 		  				=  new List<string>();
	private List<string> _deviceGoogleAccountList 		 	=  new List<string>();
	private List<GPGameRequest> _gameRequests 				=  new List<GPGameRequest>();

	private string _loadedAuthToken = "";
	private string _currentAccount = "";

	private static bool _IsLeaderboardsDataLoaded = false;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public void Create() {
		Debug.Log ("GooglePlayManager was created");

		//Creating sub managers
		GooglePlayQuests.Instance.Init();
	}


	//--------------------------------------
	// PUBLIC API CALL METHODS
	//--------------------------------------

	public GP_PlayServicesStatus GetPlayServicesStatus() {
		return AN_GMSGeneralProxy.GetPlayServicesStatus();
	}

	public void RetrieveDeviceGoogleAccounts() {
		AN_GMSGeneralProxy.loadGoogleAccountNames();
	}

	public void LoadToken(string accountName,  string scopes) {
		AN_GMSGeneralProxy.loadToken(accountName, scopes);

	}

	public void LoadToken() {
		LoadToken(currentAccount, "oauth2:https://www.googleapis.com/auth/games");
	}

	public void GetGamesServerAuthCode(string webClientAppId) {
		AN_GMSGeneralProxy.GetGamesServerAuthCode (webClientAppId);
	}

	public void InvalidateToken(string token) {
		AN_GMSGeneralProxy.invalidateToken(token);
	}

	

	public void ShowAchievementsUI() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.showAchievementsUI ();
	}
	

	public void ShowLeaderBoardsUI() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.showLeaderBoardsUI ();
	}
	


	public void ShowLeaderBoard(string leaderboardName) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.showLeaderBoard (leaderboardName);
	}
	

	public void ShowLeaderBoardById(string leaderboardId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.showLeaderBoardById (leaderboardId);
	}

	

	public void SubmitScore(string leaderboardName, long score) {

		if(AndroidNativeSettings.Instance.Is_Leaderboards_Editor_Notifications_Enabled)
			SA_EditorNotifications.ShowNotification(leaderboardName, score + " Scores Submitted", SA_EditorNotificationType.Achievement);

		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.submitScore (leaderboardName, score);
	}
	

	public void SubmitScoreById(string leaderboardId, long score) {

		if(AndroidNativeSettings.Instance.Is_Leaderboards_Editor_Notifications_Enabled)
			SA_EditorNotifications.ShowNotification(leaderboardId, score + " Scores Submitted", SA_EditorNotificationType.Achievement);

		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.submitScoreById (leaderboardId, score);
	}

	

	public void LoadLeaderBoards() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.loadLeaderBoards ();
	}

	public void UpdatePlayerScoreLocal(GPLeaderBoard leaderboard) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		int requestId = SA.Common.Util.IdFactory.NextId;
		leaderboard.CreateScoreListener(requestId);

		AN_GMSGeneralProxy.loadLeaderboardInfoLocal(leaderboard.Id, requestId);
	}
	

	public void LoadPlayerCenteredScores(string leaderboardId, GPBoardTimeSpan span, GPCollectionType collection, int maxResults) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.loadPlayerCenteredScores(leaderboardId, (int) span, (int) collection, maxResults);
	}
	

	public void LoadTopScores(string leaderboardId, GPBoardTimeSpan span, GPCollectionType collection, int maxResults) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.loadTopScores(leaderboardId, (int) span, (int) collection, maxResults);
	}





	public void UnlockAchievement(string achievementName) {
		if(AndroidNativeSettings.Instance.Is_Achievements_Editor_Notifications_Enabled)
			SA_EditorNotifications.ShowNotification(achievementName, "Unlock Method Called", SA_EditorNotificationType.Achievement);

		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.reportAchievement (achievementName);


	}
	
	public void UnlockAchievementById(string achievementId) {
		if(AndroidNativeSettings.Instance.Is_Achievements_Editor_Notifications_Enabled)
			SA_EditorNotifications.ShowNotification(achievementId, "Unlock Method Called", SA_EditorNotificationType.Achievement);

		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.reportAchievementById (achievementId);
	}

	

	public void RevealAchievement(string achievementName) {
		if(AndroidNativeSettings.Instance.Is_Achievements_Editor_Notifications_Enabled)
			SA_EditorNotifications.ShowNotification(achievementName, "Reveal Method Called", SA_EditorNotificationType.Achievement);

		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.revealAchievement (achievementName);
	}

	

	public void RevealAchievementById(string achievementId) {
		if(AndroidNativeSettings.Instance.Is_Achievements_Editor_Notifications_Enabled)
			SA_EditorNotifications.ShowNotification(achievementId, "Reveal Method Called", SA_EditorNotificationType.Achievement);

		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.revealAchievementById (achievementId);
	}
	


	public void IncrementAchievement(string achievementName, int numsteps) {
		if(AndroidNativeSettings.Instance.Is_Achievements_Editor_Notifications_Enabled)
			SA_EditorNotifications.ShowNotification(achievementName, "Incremented " + numsteps + " Steps", SA_EditorNotificationType.Achievement);

		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.incrementAchievement (achievementName, numsteps.ToString());
	}
	
	public void IncrementAchievementById(string achievementId, int numsteps) {
		if(AndroidNativeSettings.Instance.Is_Achievements_Editor_Notifications_Enabled)
			SA_EditorNotifications.ShowNotification(achievementId, "Incremented " + numsteps + " Steps", SA_EditorNotificationType.Achievement);

		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.incrementAchievementById (achievementId, numsteps.ToString());
	}

	public void SetStepsImmediate(string achievementId, int numsteps) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.setStepsImmediate (achievementId, numsteps.ToString ());
	}
	

	public void LoadAchievements() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.loadAchievements ();
	}
	

	public void ResetAchievement(string achievementId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.resetAchievement(achievementId);

	}

	public void ResetAllAchievements() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.ResetAllAchievements();
		
	}

	

	public void ResetLeaderBoard(string leaderboardId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.resetLeaderBoard(leaderboardId);

		foreach (GPLeaderBoard lb in LeaderBoards) {
			if (lb.Id.Equals(leaderboardId)) {
				LeaderBoards.Remove(lb);
				return;
			}
		}
	}
	

	public void LoadFriends() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSGeneralProxy.loadConnectedPlayers ();
	}


	//--------------------------------------
	// GIFTS
	//--------------------------------------
	
	public void SendGiftRequest(GPGameRequestType type, int requestLifetimeDays, Texture2D icon, string description, string playload = "") {
		if (!GooglePlayConnection.CheckState ()) { return; }

		byte[] val = icon.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);

		AN_GMSGiftsProxy.sendGiftRequest((int) type, playload, requestLifetimeDays, bytesString, description);

	}

	public string currentAccount {
		get {
			return _currentAccount;
		}
	}
	
	public void ShowRequestsAccepDialog() {
		if (!GooglePlayConnection.CheckState ()) { return; }

		AN_GMSGiftsProxy.showRequestAccepDialog();
	}

	public void AcceptRequests(params string[] ids) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		
		if(ids.Length == 0) {
			return;
		}
		
		
		AN_GMSGiftsProxy.acceptRequests(string.Join(AndroidNative.DATA_SPLITTER, ids));
	}


	public void DismissRequest(params string[] ids) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		
		if(ids.Length == 0) {
			return;
		}
		
		
		AN_GMSGiftsProxy.dismissRequest(string.Join(AndroidNative.DATA_SPLITTER, ids));
	}

	public void DispatchLeaderboardUpdateEvent(GP_LeaderboardResult result) {
		ActionScoreSubmited(result);
	}

	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	public GPLeaderBoard GetLeaderBoard(string leaderboardId) {
		foreach(GPLeaderBoard lb in LeaderBoards) {
			if (lb.Id.Equals(leaderboardId)) {
				return lb;
			}
		}

		GPLeaderBoard leaderboard = new GPLeaderBoard(leaderboardId, string.Empty);
		LeaderBoards.Add(leaderboard);
		return leaderboard;
	}
	

	public GPAchievement GetAchievement(string achievementId) {
		foreach (GPAchievement achievement in Achievements) {
			if (achievement.Id.Equals(achievementId)) {
				return achievement;
			}
		}
		return null;
	}


	public GooglePlayerTemplate GetPlayerById(string playerId) {
		if(players.ContainsKey(playerId)) {
			return players[playerId];
		} else {
			return null;
		}
	}

	public GPGameRequest GetGameRequestById(string id) {
		foreach(GPGameRequest r in _gameRequests) {
			if(r.id.Equals(id)) {
				return r;
			} 
		}

		return null;
	} 


	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public GooglePlayerTemplate player {
		get {
			return _player;
		}
	}

	public Dictionary<string, GooglePlayerTemplate> players {
		get {
			return _players;
		}
	}

	public List<GPLeaderBoard> LeaderBoards {
		get {
			return AndroidNativeSettings.Instance.Leaderboards;
		}
	}
	
	public List<GPAchievement> Achievements {
		get {
			return AndroidNativeSettings.Instance.Achievements;
		}
	}

	public List<string> friendsList {
		get {
			return _friendsList;
		}
	}


	public List<GPGameRequest> gameRequests {
		get {
			return _gameRequests;
		}
	}

	public List<string> deviceGoogleAccountList {
		get {
			return _deviceGoogleAccountList;
		}
	}

	public string loadedAuthToken {
		get {
			return _loadedAuthToken;
		}
	}

	public static bool IsLeaderboardsDataLoaded {
		get {
			return _IsLeaderboardsDataLoaded;
		}
	}

	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void OnGiftSendResult(string data) {

		Debug.Log("OnGiftSendResult");

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GooglePlayGiftRequestResult result =  new GooglePlayGiftRequestResult(storeData [0]);

		ActionSendGiftResultReceived(result);
	}

	private void OnRequestsInboxDialogDismissed(string data) {
		ActionRequestsInboxDialogDismissed();
	}


	private void OnAchievementsLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.IsSucceeded) {

			Achievements.Clear ();

			for(int i = 1; i < storeData.Length; i+=7) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}

				GPAchievement ach = new GPAchievement (storeData[i], 
				                                       storeData[i + 1],
				                                       storeData[i + 2],
				                                       storeData[i + 3],
				                                       storeData[i + 4],
				                                       storeData[i + 5],
				                                       storeData[i + 6]
				                                       );

				Debug.Log (ach.Name);
				Debug.Log (ach.Type);


				Achievements.Add (ach);

			}

			Debug.Log ("Loaded: " + Achievements.Count + " Achievements");
		}

		ActionAchievementsLoaded(result);

	}

	private void OnAchievementUpdated(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GP_AchievementResult result = new GP_AchievementResult (storeData [0]);
		result.achievementId = storeData [1];

		ActionAchievementUpdated(result);

	}

	private void OnScoreDataRecevied(string data) {
		Debug.Log("OnScoreDataRecevide");
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GP_LeaderboardResult result = new GP_LeaderboardResult(null, storeData[0]);
		if(result.IsSucceeded) {

			GPBoardTimeSpan 	timeSpan 		= (GPBoardTimeSpan)  System.Convert.ToInt32(storeData[1]);
			GPCollectionType 	collection  	= (GPCollectionType) System.Convert.ToInt32(storeData[2]);
			string leaderboardId 				= storeData[3];
			string leaderboardName 				= storeData[4];

			GPLeaderBoard lb = GetLeaderBoard(leaderboardId);
			lb.UpdateName(leaderboardName);
			result = new GP_LeaderboardResult(lb, storeData [0]);

			for(int i = 5; i < storeData.Length; i+=8) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}
				
			 	long score = System.Convert.ToInt64(storeData[i]);
				int rank = System.Convert.ToInt32(storeData[i + 1]);

				string playerId = storeData[i + 2];
				if(!players.ContainsKey(playerId)) {
					GooglePlayerTemplate p = new GooglePlayerTemplate (playerId, storeData[i + 3], storeData[i + 4], storeData[i + 5], storeData[i + 6], storeData[i + 7]);
					AddPlayer(p);
				}

				GPScore s =  new GPScore(score, rank, timeSpan, collection, lb.Id, playerId);
				lb.UpdateScore(s);

				if(playerId.Equals(player.playerId)) {
					lb.UpdateCurrentPlayerScore(s);
				}
			}
		}

		ActionScoresListLoaded(result);
	}

	private void OnLeaderboardDataLoaded(string data) {
		Debug.Log("OnLeaderboardDataLoaded");
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);


		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.IsSucceeded) {

			for(int i = 1; i < storeData.Length; i+=26) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}

				string leaderboardId = storeData[i];
				string leaderboardName = storeData [i + 1];
			
				GPLeaderBoard lb = GetLeaderBoard(leaderboardId);
				lb.UpdateName(leaderboardName);

				int start = i + 2;
				for(int j = 0; j < 6; j++) {

					long score = System.Convert.ToInt64(storeData[start]);
					int rank = System.Convert.ToInt32(storeData[start + 1]);

					GPBoardTimeSpan 	timeSpan 		= (GPBoardTimeSpan)  System.Convert.ToInt32(storeData[start + 2]);
					GPCollectionType 	collection  	= (GPCollectionType) System.Convert.ToInt32(storeData[start + 3]);

					//Debug.Log("timeSpan: " + timeSpan +   " collection: " + collection + " score:" + score + " rank:" + rank);

					GPScore s =  new GPScore(score, rank, timeSpan, collection, lb.Id, player.playerId);
					start = start + 4;
					lb.UpdateScore(s);
					lb.UpdateCurrentPlayerScore(s);
				}
			}

			Debug.Log ("Loaded: " + LeaderBoards.Count + " Leaderboards");
		}

		_IsLeaderboardsDataLoaded = true;
		ActionLeaderboardsLoaded(result);
	}


	private void OnPlayerScoreUpdated(string data) {

		if(data.Equals(string.Empty)) {
			Debug.Log("GooglePlayManager OnPlayerScoreUpdated, no data avaiable");
			return;
		}


		Debug.Log("OnPlayerScoreUpdated " + data);


		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		GP_ScoreResult result = new GP_ScoreResult (storeData [0]);

		string leaderboardId = storeData[1];
		int requestId = System.Convert.ToInt32(storeData[2]);

		GPLeaderBoard lb = GetLeaderBoard(leaderboardId);

		if(result.IsSucceeded) {
			GPBoardTimeSpan 	timeSpan 		= (GPBoardTimeSpan)  System.Convert.ToInt32(storeData[3]);
			GPCollectionType 	collection  	= (GPCollectionType) System.Convert.ToInt32(storeData[4]);

			long score = System.Convert.ToInt64(storeData[5]);
			int rank = System.Convert.ToInt32(storeData[6]);

			GPScore s =  new GPScore(score, rank, timeSpan, collection, lb.Id, player.playerId);
			result.score= s;

			lb.ReportLocalPlayerScoreUpdate(s, requestId);
		} else {
			lb.ReportLocalPlayerScoreUpdateFail(storeData[0], requestId);
		}
	}

	private void OnScoreSubmitted(string data) {
		Debug.Log("OnScoreSubmitted " + data);

		if(data.Equals(string.Empty)) {
			Debug.Log("GooglePlayManager OnScoreSubmitted, no data avaiable");
			return;
		}

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GPLeaderBoard lb = GetLeaderBoard(storeData [1]);
		GP_LeaderboardResult result = new GP_LeaderboardResult (lb, storeData [0]);
		if (result.IsSucceeded) {
			Debug.Log("Score was submitted to leaderboard -> " + lb);

			UpdatePlayerScoreLocal(lb);
		} else {
			ActionScoreSubmited(result);
		}
	}

	private void OnPlayerDataLoaded(string data) {

		Debug.Log("OnPlayerDataLoaded");
		if(data.Equals(string.Empty)) {
			Debug.Log("GooglePlayManager OnPlayerLoaded, no data avaiable");
			return;
		}

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		_player = new GooglePlayerTemplate (storeData [0], storeData [1], storeData [2], storeData [3], storeData [4], storeData [5]);
		AddPlayer(_player);

		_currentAccount = storeData [6];
	}

	private void OnPlayersLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.IsSucceeded) {

			for(int i = 1; i < storeData.Length; i+=6) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}
				

				GooglePlayerTemplate p = new GooglePlayerTemplate (storeData[i], storeData[i + 1], storeData[i + 2], storeData[i + 3], storeData[i + 4], storeData[i + 5]);
				AddPlayer(p);
				if(!_friendsList.Contains(p.playerId)) {
					_friendsList.Add(p.playerId);
				}

			}
		}
		
		
		
		Debug.Log ("OnPlayersLoaded, total:" + players.Count.ToString());
		ActionFriendsListLoaded(result);
	}

	private void OnGameRequestsLoaded(string data) {
		_gameRequests = new List<GPGameRequest>();
		if(data.Length == 0) {
			return;
		}


		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		for(int i = 0; i < storeData.Length; i+=6) {
			if(storeData[i] == AndroidNative.DATA_EOF) {
				break;
			}

			GPGameRequest r = new GPGameRequest();
			r.id = storeData[i];
			r.playload = storeData[i +1];

			r.expirationTimestamp 	 = System.Convert.ToInt64(storeData[i + 2]);
			r.creationTimestamp		 = System.Convert.ToInt64(storeData[i + 3]);

			r.sender = storeData[i +4];
			r.type = (GPGameRequestType) System.Convert.ToInt32(storeData[i + 5]);
			_gameRequests.Add(r);


		}

		ActionPendingGameRequestsDetected(_gameRequests);
	}

	private void OnGameRequestsAccepted(string data) {
		List<GPGameRequest> acceptedList =  new List<GPGameRequest>();

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		for(int i = 0; i < storeData.Length; i+=6) {
			if(storeData[i] == AndroidNative.DATA_EOF) {
				break;
			}
			
			GPGameRequest r = new GPGameRequest();
			r.id = storeData[i];
			r.playload = storeData[i +1];
			
			r.expirationTimestamp 	 = System.Convert.ToInt64(storeData[i + 2]);
			r.creationTimestamp		 = System.Convert.ToInt64(storeData[i + 3]);
			
			r.sender = storeData[i + 4];
			r.type = (GPGameRequestType) System.Convert.ToInt32(storeData[i + 5]);

			acceptedList.Add(r);
			
		}

		ActionGameRequestsAccepted(acceptedList);
	}

	private void OnAccountsLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		_deviceGoogleAccountList.Clear();

		foreach(string acc in storeData) {
			if(acc != AndroidNative.DATA_EOF) {
				_deviceGoogleAccountList.Add(acc);
			}
		}

		ActionAvailableDeviceAccountsLoaded(_deviceGoogleAccountList);
	}

	private void OnTokenLoaded(string token) {
		_loadedAuthToken = token;

		ActionOAuthTokenLoaded(_loadedAuthToken);
	}

	private void OnGamesServerAuthCodeLoaded(string data) {
		string[] raw = data.Split(new string[] {AndroidNative.DATA_SPLITTER}, StringSplitOptions.None);
		GooglePlayResult result = new GooglePlayResult(raw[0]);

		ActionServerAuthCodeLoaded(result, raw[1]);
	}

	//--------------------------------------
	// UTILS
	//--------------------------------------
	
	public static GP_Participant ParseParticipanData(string[] data, int index ) {
		GP_Participant participant =  new GP_Participant(data[index], data[index + 1], data[index + 2], data[index + 3], data[index + 4], data[index + 5]);

		bool hasResult = Convert.ToBoolean(data[index + 6]);
		if(hasResult) {
			GP_ParticipantResult r =  new GP_ParticipantResult(data, index + 7);
			participant.SetResult(r);
		}

		return participant;
	}



	public static List<GP_Participant>  ParseParticipantsData(string[] data, int index ) {

		List<GP_Participant> Participants =  new List<GP_Participant>();
	
		for(int i = index; i < data.Length; i += 11) {
			if(data[i] == AndroidNative.DATA_EOF) {
				break;
			}

			GP_Participant p = ParseParticipanData(data, i);
			Participants.Add(p);

		}

		return Participants;
	}


	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------

	private void AddPlayer(GooglePlayerTemplate p) {
		if(!_players.ContainsKey(p.playerId)) {
			_players.Add(p.playerId, p);
		}
	}





}
