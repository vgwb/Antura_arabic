using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GooglePlayTBM : SA.Common.Pattern.Singleton<GooglePlayTBM>
{
	
	public static event Action<GP_TBM_LoadMatchesResult> ActionMatchesResultLoaded = delegate { };
	public static event Action<GP_TBM_MatchInitiatedResult> ActionMatchInitiated = delegate { };
	public static event Action<GP_TBM_CancelMatchResult> ActionMatchCanceled = delegate { };
	public static event Action<GP_TBM_LeaveMatchResult> ActionMatchLeaved = delegate { };
	public static event Action<GP_TBM_LoadMatchResult> ActionMatchDataLoaded = delegate { };
	
	public static event Action<GP_TBM_UpdateMatchResult> ActionMatchUpdated = delegate { };
	public static event Action<GP_TBM_UpdateMatchResult> ActionMatchTurnFinished = delegate { };
	
	
	public static event Action<GP_TBM_UpdateMatchResult> ActionMatchReceived = delegate { };
	public static event Action<GP_TBM_MatchRemovedResult> ActionMatchRemoved = delegate { };
	
	public static event Action<AndroidActivityResult> ActionMatchCreationCanceled = delegate { };
	
	
	public static event Action<string, GP_TBM_MatchInitiatedResult> ActionMatchInvitationAccepted = delegate {};
	public static event Action<string> ActionMatchInvitationDeclined = delegate {};
	
	
	
	
	
	
	private Dictionary<string, GP_TBM_Match> _Matches = new Dictionary<string, GP_TBM_Match>();
	
	
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		GooglePlayInvitationManager.Instance.Init();
	}
	
	public void Init()
	{
		//Empty init.
	}
	
	
	// --------------------------------------
	// API Methods
	// --------------------------------------
	
	
	public void AcceptInvitation(string invitationId)
	{
		AN_GMSRTMProxy.TBM_AcceptInvitation(invitationId);
		
	}
	
	public void DeclineInvitation(string invitationId)
	{
		AN_GMSRTMProxy.TBM_DeclineInvitation(invitationId);
		ActionMatchInvitationDeclined(invitationId);
		
	}
	
	public void DismissInvitation(string invitationId)
	{
		AN_GMSRTMProxy.TBM_DismissInvitation(invitationId);
	}
	
	public void CreateMatch(int minPlayers, int maxPlayers, string[] playersIds = null)
	{
		AN_GMSRTMProxy.TBM_CreateMatch(minPlayers, maxPlayers, playersIds);
	}
	
	public void CancelMatch(string matchId)
	{
		AN_GMSRTMProxy.CancelMatch(matchId);
	}
	
	public void DismissMatch(string matchId)
	{
		AN_GMSRTMProxy.DismissMatch(matchId);
	}

	public void ConfirmMatchFinish(string matchId) {
		AN_GMSRTMProxy.TBM_FinishMatchWithId(matchId);
	}

	[System.Obsolete("This method is deprecated. Use ConfirmMatchFinish method instead")]
	public void ConfirmhMatchFinis(string matchId) {
		ConfirmMatchFinish(matchId);
	}
	
	public void FinishMatch(string matchId, byte[] matchData, params GP_ParticipantResult[] results)   {
		
		string data = System.Convert.ToBase64String(matchData);
		
		List<String> pIds = new List<string>();
		List<int> pCodes = new List<int>();
		List<int> pResults = new List<int>();
		List<int> pPlacing = new List<int>();
		
		foreach (GP_ParticipantResult r in results) {
			pIds.Add(r.ParticipantId);
			pCodes.Add(r.VersionCode);
			pResults.Add((int)r.Result);
			pPlacing.Add(r.Placing);
		}
		
		
		AN_GMSRTMProxy.TBM_FinishMatch(matchId, data, pIds.ToArray(), pResults.ToArray(), pPlacing.ToArray(), pCodes.ToArray());
		
	}
	
	public void LeaveMatch(string matchId)
	{
		AN_GMSRTMProxy.TBM_LeaveMatch(matchId);
	}
	
	public void LeaveMatchDuringTurn(string matchId, string pendingParticipantId)
	{
		AN_GMSRTMProxy.TBM_LeaveMatchDuringTurn(matchId, pendingParticipantId);
	}
	
	
	public void LoadMatchInfo(string matchId)
	{
		AN_GMSRTMProxy.TBM_LoadMatchInfo(matchId);
	}
	
	
	public void LoadMatchesInfo(GP_TBM_MatchesSortOrder sortOreder, params GP_TBM_MatchTurnStatus[] matchTurnStatuses)
	{
		
		List<int> mStates = new List<int>();
		foreach (GP_TBM_MatchTurnStatus state in matchTurnStatuses)
		{
			mStates.Add((int)state);
		}
		
		AN_GMSRTMProxy.TBM_LoadMatchesInfo((int)sortOreder, mStates.ToArray());
	}
	
	public void LoadAllMatchesInfo(GP_TBM_MatchesSortOrder sortOreder)
	{
		AN_GMSRTMProxy.TBM_LoadAllMatchesInfo((int)sortOreder);
	}
	
	
	public void Rematch(string matchId)
	{
		AN_GMSRTMProxy.TBM_Rematch(matchId);
	}
	
	public void RegisterMatchUpdateListener()
	{
		AN_GMSRTMProxy.TBM_RegisterMatchUpdateListener();
	}
	
	public void UnregisterMatchUpdateListener()
	{
		AN_GMSRTMProxy.TBM_UnregisterMatchUpdateListener();
	}
	
	
	public void TakeTrun(string matchId, byte[] matchData, string pendingParticipantId, params GP_ParticipantResult[] results)
	{
		
		List<String> pIds = new List<string>();
		List<int> pCodes = new List<int>();
		List<int> pResults = new List<int>();
		List<int> pPlacing = new List<int>();
		
		foreach (GP_ParticipantResult r in results)
		{
			pIds.Add(r.ParticipantId);
			pCodes.Add(r.VersionCode);
			pResults.Add((int)r.Result);
			pPlacing.Add(r.Placing);
		}
		
		string data = System.Convert.ToBase64String(matchData);
		
		
		
		AN_GMSRTMProxy.TBM_TakeTrun(matchId, data, pendingParticipantId, pIds.ToArray(), pResults.ToArray(), pPlacing.ToArray(), pCodes.ToArray());
	}
	
	
	
	public void StartSelectOpponentsView(int minPlayers, int maxPlayers, bool allowAutomatch)
	{
		AN_GMSRTMProxy.StartSelectOpponentsView(minPlayers, maxPlayers, allowAutomatch);
	}
	
	public void ShowInbox()
	{
		AN_GMSRTMProxy.TBM_ShowInbox();
	}
	
	
	
	public void SetVariant(int val)
	{
		AN_GMSRTMProxy.TBM_SetVariant(val);
	}
	
	
	//will work only if StartSelectOpponentsView minPlayers > 0
	public void SetExclusiveBitMask(int val)
	{
		AN_GMSRTMProxy.TBM_SetExclusiveBitMask(val);
	}
	
	
	
	
	// --------------------------------------
	// Get / Set
	// --------------------------------------
	
	
	public Dictionary<string, GP_TBM_Match> Matches
	{
		get
		{
			return _Matches;
		}
	}
	
	// --------------------------------------
	// Native Events
	// --------------------------------------
	
	private void OnLoadMatchesResult(string data)
	{
		string[] DataArray = data.Split(new string[] { AndroidNative.DATA_SPLITTER2 }, StringSplitOptions.None);
		GP_TBM_LoadMatchesResult result = new GP_TBM_LoadMatchesResult(DataArray[0]);
		
		if (DataArray.Length > 1)
		{
			result.LoadedMatches = new Dictionary<string, GP_TBM_Match>();
			for (int i = 1; i < DataArray.Length; i++)
			{
				if (DataArray[i] == AndroidNative.DATA_EOF)
				{
					break;
				}
				
				GP_TBM_Match match = ParceMatchInfo(DataArray[i]);
				UpdateMatchInfo(match);
				result.LoadedMatches.Add(match.Id, match);
			}
		}
		
		
		ActionMatchesResultLoaded(result);
	}
	
	private void OnMatchInitiatedCallback(string data)
	{
		
		string[] DataArray = data.Split(AndroidNative.DATA_SPLITTER[0]);
		
		GP_TBM_MatchInitiatedResult result = new GP_TBM_MatchInitiatedResult(DataArray[0]);
		if (DataArray.Length > 1)
		{
			GP_TBM_Match match = ParceMatchInfo(DataArray, 1);
			UpdateMatchInfo(match);
			result.Match = match;
		}
		
		ActionMatchInitiated(result);
	}
	
	
	private void OnInvitationAcceptedCallback(string data) {
		
		Debug.Log("OnInvitationAcceptedCallback received");
		
		string[] DataArray = data.Split(AndroidNative.DATA_SPLITTER[0]);
		
		
		string invitationId = "";
		GP_TBM_MatchInitiatedResult result = new GP_TBM_MatchInitiatedResult(DataArray[0]);
		
		
		
		if (DataArray.Length > 1){
			invitationId = DataArray[1];
			
			GP_TBM_Match match = ParceMatchInfo(DataArray, 2);
			UpdateMatchInfo(match);
			result.Match = match;
		}
		
		Debug.Log("OnInvitationAcceptedCallback fired " + result.IsSucceeded);
		
		ActionMatchInvitationAccepted(invitationId, result);
	}
	
	
	
	
	private void OnCancelMatchResult(string data)
	{
		string[] DataArray = data.Split(AndroidNative.DATA_SPLITTER[0]);
		
		GP_TBM_CancelMatchResult result = new GP_TBM_CancelMatchResult(DataArray[0]);
		if (DataArray.Length > 1)
		{
			result.MatchId = DataArray[1];
			
			if (Matches.ContainsKey(result.MatchId))
			{
				Matches[result.MatchId].Status = GP_TBM_MatchStatus.MATCH_STATUS_CANCELED;
			}
			
		}
		
		ActionMatchCanceled(result);
	}
	
	private void OnLeaveMatchResult(string data)
	{
		
		string[] DataArray = data.Split(AndroidNative.DATA_SPLITTER[0]);
		
		GP_TBM_LeaveMatchResult result = new GP_TBM_LeaveMatchResult(DataArray[0]);
		result.MatchId = DataArray[1];
		
		
		ActionMatchLeaved(result);
	}
	
	private void OnLoadMatchResult(string data)
	{
		string[] DataArray = data.Split(AndroidNative.DATA_SPLITTER[0]);
		
		GP_TBM_LoadMatchResult result = new GP_TBM_LoadMatchResult(DataArray[0]);
		if (DataArray.Length > 1)
		{
			GP_TBM_Match match = ParceMatchInfo(DataArray, 1);
			UpdateMatchInfo(match);
			result.Match = match;
		}
		
		ActionMatchDataLoaded(result);
	}
	
	private void OnUpdateMatchResult(string data)
	{
		string[] DataArray = data.Split(AndroidNative.DATA_SPLITTER[0]);
		
		GP_TBM_UpdateMatchResult result = new GP_TBM_UpdateMatchResult(DataArray[0]);
		if (DataArray.Length > 1)
		{
			GP_TBM_Match match = ParceMatchInfo(DataArray, 1);
			UpdateMatchInfo(match);
			result.Match = match;
		}
		
		ActionMatchUpdated(result);
	}
	
	private void AN_OnTurnResult(string data) {
		Debug.Log("AN_OnTurnResult");
		
		string[] DataArray = data.Split(AndroidNative.DATA_SPLITTER[0]);
		
		GP_TBM_UpdateMatchResult result = new GP_TBM_UpdateMatchResult(DataArray[0]);
		if (DataArray.Length > 1){
			GP_TBM_Match match = ParceMatchInfo(DataArray, 1);
			UpdateMatchInfo(match);
			result.Match = match;
		}
		Debug.Log("ActionMatchTurnFinished fired");
		ActionMatchTurnFinished(result);
	}
	
	
	
	
	private void OnTurnBasedMatchRemoved(string matchId)
	{
		if (Matches.ContainsKey(matchId))
		{
			Matches.Remove(matchId);
		}
		
		ActionMatchRemoved(new GP_TBM_MatchRemovedResult(matchId));
	}
	
	private void OnTurnBasedMatchReceived(string data) {
		string[] DataArray = data.Split(AndroidNative.DATA_SPLITTER[0]);
		GP_TBM_UpdateMatchResult result = new GP_TBM_UpdateMatchResult("0");
		
		GP_TBM_Match match = ParceMatchInfo(DataArray, 0);
		UpdateMatchInfo(match);
		result.Match = match;
		
		ActionMatchUpdated(result);
		ActionMatchReceived (result);
		
		//ActionMatchReceived(new GP_TBM_MatchReceivedResult(ParceMatchInfo(data)));
	}
	
	private void OnMatchCreationCanceled(string data)
	{
		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER[0]);
		AndroidActivityResult result = new AndroidActivityResult(storeData[0], storeData[1]);
		
		
		ActionMatchCreationCanceled(result);
	}
	
	
	// --------------------------------------
	// Utils
	// --------------------------------------
	
	public static GP_TBM_Match ParceMatchInfo(string data)
	{
		string[] MatchData = data.Split(AndroidNative.DATA_SPLITTER[0]);
		return ParceMatchInfo(MatchData, 0);
	}
	
	public static GP_TBM_Match ParceMatchInfo(string[] MatchData, int index)
	{
		
		
		GP_TBM_Match mtach = new GP_TBM_Match();
		
		mtach.Id = MatchData[index];
		mtach.RematchId = MatchData[index + 1];
		mtach.Description = MatchData[index + 2];
		mtach.AvailableAutoMatchSlots = Convert.ToInt32(MatchData[index + 3]);
		mtach.CreationTimestamp = Convert.ToInt64(MatchData[index + 4]);
		mtach.CreatorId = MatchData[index + 5];
		mtach.LastUpdatedTimestamp = Convert.ToInt64(MatchData[index + 6]);
		mtach.LastUpdaterId = MatchData[index + 7];
		mtach.MatchNumber = Convert.ToInt32(MatchData[index + 8]);
		mtach.Status = (GP_TBM_MatchStatus)Convert.ToInt32(MatchData[index + 9]);
		mtach.TurnStatus = (GP_TBM_MatchTurnStatus)Convert.ToInt32(MatchData[index + 10]);
		mtach.CanRematch = Convert.ToBoolean(MatchData[index + 11]);
		mtach.Variant = Convert.ToInt32(MatchData[index + 12]);
		mtach.Version = Convert.ToInt32(MatchData[index + 13]);
		
		mtach.SetData(MatchData[index + 14]);
		mtach.SetPreviousMatchData(MatchData[index + 15]);
		mtach.PendingParticipantId = MatchData[index + 16];
		mtach.Participants = GooglePlayManager.ParseParticipantsData(MatchData, index + 17);
		
		return mtach;
	}
	
	private void UpdateMatchInfo(GP_TBM_Match match)
	{
		if (Matches.ContainsKey(match.Id))
		{
			Matches[match.Id] = match;
		}
		else
		{
			Matches.Add(match.Id, match);
		}
	}
	
	
	
	
}
