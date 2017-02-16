using UnityEngine;
using System.Collections;

public class AN_GMSRTMProxy : MonoBehaviour
{
	
	private const string CLASS_NAME = "com.androidnative.gms.core.GameClientBridge";
	
	private static void CallActivityFunction(string methodName, params object[] args)
	{
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}
	
	// --------------------------------------
	// RTM
	// --------------------------------------
	
	public static void RTMFindMatch(int minPlayers, int maxPlayers, string[] pIds)
	{
		CallActivityFunction("RTMFindMatch", minPlayers, maxPlayers, pIds);
	}
	
	public static void RTMFindMatch(string[] pIds)
	{
		CallActivityFunction("RTMFindMatch", pIds);
	}
	
	public static void sendDataToAll(string data, int sendType, int dataId = 0)
	{
		CallActivityFunction("sendDataToAll", data, sendType.ToString(), dataId);
	}
	
	public static void sendDataToPlayers(string data, string players, int sendType, int dataId = 0)
	{
		CallActivityFunction("sendDataToPlayers", data, players, sendType.ToString(), dataId);
	}
	
	public static void ShowWaitingRoomIntent()
	{
		CallActivityFunction("showWaitingRoomIntent");
	}
	
	public static void InvitePlayers(int minPlayers, int maxPlayers)
	{
		CallActivityFunction("invitePlayers", minPlayers.ToString(), maxPlayers.ToString());
	}
	
	public static void RTM_SetVariant(int val)
	{
		CallActivityFunction("RTM_SetVariant", val);
	}
	
	
	//will work only if StartSelectOpponentsView minPlayers > 0
	public static void RTM_SetExclusiveBitMask(int val)
	{
		CallActivityFunction("RTM_SetExclusiveBitMask", val);
	}
	
	
	
	public static void RTM_AcceptInvitation(string invitationId)
	{
		CallActivityFunction("RTM_AcceptInvitation", invitationId);
	}
	
	public static void RTM_DeclineInvitation(string invitationId)
	{
		CallActivityFunction("RTM_DeclineInvitation", invitationId);
	}
	
	public static void RTM_DismissInvitation(string invitationId)
	{
		CallActivityFunction("RTM_DismissInvitation", invitationId);
	}
	
	
	
	
	// --------------------------------------
	// TBM
	// --------------------------------------
	
	
	
	
	public static void TBM_AcceptInvitation(string invitationId)
	{
		CallActivityFunction("TBM_AcceptInvitation", invitationId);
	}
	
	public static void TBM_DeclineInvitation(string invitationId)
	{
		CallActivityFunction("TBM_DeclineInvitation", invitationId);
	}
	
	public static void TBM_DismissInvitation(string invitationId)
	{
		CallActivityFunction("TBM_DismissInvitation", invitationId);
	}
	
	public static void TBM_CreateMatch(int minPlayers, int maxPlayers, string[] playersIds)
	{
		CallActivityFunction("TBM_CreateMatch", minPlayers, maxPlayers, playersIds);
	}
	
	public static void CancelMatch(string matchId)
	{
		CallActivityFunction("CancelMatch", matchId);
	}
	
	public static void DismissMatch(string matchId)
	{
		CallActivityFunction("DismissMatch", matchId);
	}
	
	public static void TBM_FinishMatch(string matchId, string matchData, string[] pIds, int[] results, int[] placing, int[] versions)
	{
		CallActivityFunction("TBM_FinishMatch", matchId, matchData, pIds, results, placing, versions);
	}
	
	public static void TBM_FinishMatchWithId(string matchId)
	{
		CallActivityFunction("TBM_FinishMatchWithId", matchId);
	}
	
	public static void TBM_LeaveMatch(string matchId)
	{
		CallActivityFunction("TBM_LeaveMatch", matchId);
	}
	
	public static void TBM_LeaveMatchDuringTurn(string matchId, string pendingParticipantId)
	{
		CallActivityFunction("TBM_LeaveMatchDuringTurn", matchId, pendingParticipantId);
	}
	
	public static void TBM_LoadMatchInfo(string matchId)
	{
		CallActivityFunction("TBM_LoadMatchInfo", matchId);
	}
	
	public static void TBM_LoadMatchesInfo(int invitationSortOrder, int[] matchTurnStatuses)
	{
		CallActivityFunction("TBM_LoadMatchesInfo", invitationSortOrder, matchTurnStatuses);
	}
	
	public static void TBM_LoadAllMatchesInfo(int invitationSortOrder)
	{
		CallActivityFunction("TBM_LoadAllMatchesInfo", invitationSortOrder);
	}
	
	public static void TBM_Rematch(string matchId)
	{
		CallActivityFunction("TBM_Rematch", matchId);
	}
	
	public static void TBM_RegisterMatchUpdateListener()
	{
		CallActivityFunction("TBM_RegisterMatchUpdateListener");
	}
	
	public static void TBM_UnregisterMatchUpdateListener()
	{
		CallActivityFunction("TBM_UnregisterMatchUpdateListener");
	}
	
	public static void TBM_TakeTrun(string matchId, string matchData, string pendingParticipantId, string[] pIds, int[] results, int[] placing, int[] versions)
	{
		CallActivityFunction("TBM_TakeTrun", matchId, matchData, pendingParticipantId, pIds, results, placing, versions);
	}
	
	
	public static void StartSelectOpponentsView(int minPlayers, int maxPlayers, bool allowAutomatch)
	{
		CallActivityFunction("StartSelectOpponentsView", minPlayers, maxPlayers, allowAutomatch);
	}
	
	public static void TBM_ShowInbox()
	{
		CallActivityFunction("TBM_ShowInbox");
	}
	
	public static void TBM_SetVariant(int val)
	{
		CallActivityFunction("TBM_SetVariant", val);
	}
	
	
	//will work only if StartSelectOpponentsView minPlayers > 0
	public static void TBM_SetExclusiveBitMask(int val)
	{
		CallActivityFunction("TBM_SetExclusiveBitMask", val);
	}
	
	
	
	
	
	
}
