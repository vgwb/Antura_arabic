using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GooglePlayRTM : SA.Common.Pattern.Singleton<GooglePlayRTM>  {

	//Actions
	public static Action<GP_RTM_Network_Package> 					ActionDataRecieved		      	= delegate{};
	public static Action<GP_RTM_Room> 								ActionRoomUpdated            	= delegate{};
	public static Action<GP_RTM_ReliableMessageSentResult> 			ActionReliableMessageSent 		= delegate{};
	public static Action<GP_RTM_ReliableMessageDeliveredResult> 	ActionReliableMessageDelivered 	= delegate{};
	
	public static Action ActionConnectedToRoom        	=  delegate{};
	public static Action ActionDisconnectedFromRoom 	=  delegate{};

	//contains participant id
	public static Action<string>  ActionP2PConnected			=  delegate{};
	public static Action<string>  ActionP2PDisconnected 		=  delegate{};

	//contains participants ids
	public static Action<string[]> ActionPeerDeclined 			=  delegate{};
	public static Action<string[]> ActionPeerInvitedToRoom 	=  delegate{};
	public static Action<string[]> ActionPeerJoined 				=  delegate{};
	public static Action<string[]> ActionPeerLeft 				=  delegate{};
	public static Action<string[]> ActionPeersConnected 			=  delegate{};
	public static Action<string[]> ActionPeersDisconnected 		=  delegate{};
	public static Action ActionRoomAutomatching 		=  delegate{};
	public static Action ActionRoomConnecting 			=  delegate{};
	public static Action<GP_GamesStatusCodes> ActionJoinedRoom 				=  delegate{};
	public static Action<GP_RTM_Result> ActionLeftRoom 				=  delegate{};
	public static Action<GP_GamesStatusCodes> ActionRoomConnected 			=  delegate{};
	public static Action<GP_GamesStatusCodes> ActionRoomCreated 			=  delegate{};
	
	public static Action<AndroidActivityResult> ActionInvitationBoxUIClosed =  delegate{};
	public static Action<AndroidActivityResult> ActionWatingRoomIntentClosed =  delegate{};
	
	//contains invitation id
	public static Action<GP_Invite> ActionInvitationAccepted = delegate{};
	public static Action<GP_Invite> ActionInvitationReceived = delegate{};
	public static Action<string> ActionInvitationRemoved =  delegate{};

	private const int BYTE_LIMIT = 256;
	private GP_RTM_Room _currentRoom = new GP_RTM_Room();
	private List<GP_Invite> _invitations =  new List<GP_Invite>();

	// Cache for reliable messages data
	private Dictionary<int, GP_RTM_ReliableMessageListener> _ReliableMassageListeners = new Dictionary<int, GP_RTM_ReliableMessageListener>();
	
	//--------------------------------------
	// INITIALIZATION
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
		_currentRoom = new GP_RTM_Room();

		GooglePlayInvitationManager.ActionInvitationReceived += OnInvitationReceived;
		GooglePlayInvitationManager.ActionInvitationRemoved += OnInvitationRemoved;
		GooglePlayInvitationManager.ActionInvitationAccepted += OnInvitationAccepted;

		GooglePlayInvitationManager.Instance.Init();
		Debug.Log("GooglePlayRTM Created");
	}

	//--------------------------------------
	// API METHODS
	//--------------------------------------

	public void FindMatch(int minPlayers, int maxPlayers) {
		FindMatch(minPlayers, maxPlayers, new string[0] {});
	}


	public void FindMatch(int minPlayers, int maxPlayers, params GooglePlayerTemplate[] playersToInvite) {

		List<string> ids =  new List<string>();
		foreach(GooglePlayerTemplate p in playersToInvite) {
			ids.Add(p.playerId);
		}

		AN_GMSRTMProxy.RTMFindMatch(minPlayers, maxPlayers, ids.ToArray());
	}

	public void FindMatch(int minPlayers, int maxPlayers, params string[] playersToInvite) {
		AN_GMSRTMProxy.RTMFindMatch(minPlayers, maxPlayers, playersToInvite);
	}

	public void FindMatch(GooglePlayerTemplate[] playersToInvite) {
		
		List<string> ids =  new List<string>();
		foreach(GooglePlayerTemplate p in playersToInvite) {
			ids.Add(p.playerId);
		}
		
		AN_GMSRTMProxy.RTMFindMatch(ids.ToArray());
	}
	
	public void FindMatch(string[] playersToInvite) {
		AN_GMSRTMProxy.RTMFindMatch(playersToInvite);
	}

	public void SendDataToAll(byte[] data, GP_RTM_PackageType sendType) {
		string dataString = ConvertByteDataToString(data);
		switch (sendType) {
		case GP_RTM_PackageType.RELIABLE:

			GP_RTM_ReliableMessageListener listener = new GP_RTM_ReliableMessageListener(SA.Common.Util.IdFactory.NextId, data);
			_ReliableMassageListeners.Add(listener.DataTokenId, listener);

			AN_GMSRTMProxy.sendDataToAll(dataString, (int) sendType);
			break;
		case GP_RTM_PackageType.UNRELIABLE:
			AN_GMSRTMProxy.sendDataToAll(dataString, (int) sendType);
			break;
		}
	}
	
	public void sendDataToPlayers(byte[] data, GP_RTM_PackageType sendType, params string[] players) {
		string dataString = ConvertByteDataToString(data);
		string playersString = string.Join(AndroidNative.DATA_SPLITTER, players);
		switch (sendType) {
		case GP_RTM_PackageType.RELIABLE:

			GP_RTM_ReliableMessageListener listener = new GP_RTM_ReliableMessageListener(SA.Common.Util.IdFactory.NextId, data);
			_ReliableMassageListeners.Add(listener.DataTokenId, listener);

			AN_GMSRTMProxy.sendDataToPlayers(dataString, playersString, (int) sendType);
			break;
		case GP_RTM_PackageType.UNRELIABLE:
			AN_GMSRTMProxy.sendDataToPlayers(dataString, playersString, (int) sendType);
			break;
		}
	}

	public void ShowWaitingRoomIntent() {
		AN_GMSRTMProxy.ShowWaitingRoomIntent();
	}

	public void OpenInvitationBoxUI(int minPlayers, int maxPlayers) {
		AN_GMSRTMProxy.InvitePlayers(minPlayers, maxPlayers);
	}

	public void LeaveRoom() {
		AN_GMSGiftsProxy.leaveRoom();
	}


	public void AcceptInvitation(string invitationId)  {
		AN_GMSRTMProxy.RTM_AcceptInvitation (invitationId);
		
	}
	
	public void DeclineInvitation(string invitationId)  {
		AN_GMSRTMProxy.RTM_DeclineInvitation (invitationId);
	}
	
	public void DismissInvitation(string invitationId)  {
		AN_GMSRTMProxy.RTM_DismissInvitation (invitationId);
	}

	
	public void OpenInvitationInBoxUI()  {
		AN_GMSGiftsProxy.showInvitationBox();
	}



	public void SetVariant(int val) {
		AN_GMSRTMProxy.RTM_SetVariant (val);
	}
	

	public void SetExclusiveBitMask(int val) {
		AN_GMSRTMProxy.RTM_SetExclusiveBitMask (val);
	}

	public void ClearReliableMessageListener(int dataTokenId) {
		if (_ReliableMassageListeners.ContainsKey(dataTokenId)) {
			_ReliableMassageListeners.Remove(dataTokenId);
			Debug.Log("[ClearReliableMessageListener] Remove data with token " + dataTokenId);
		}
	}

	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public GP_RTM_Room currentRoom {
		get {
			return _currentRoom;
		}
	}

	public List<GP_Invite> invitations {
		get {
			return _invitations;
		}
	}

	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void OnWatingRoomIntentClosed(string data) {
		Debug.Log("[OnWatingRoomIntentClosed] data " + data);
		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		AndroidActivityResult result =  new AndroidActivityResult(storeData[0], storeData[1]);

		ActionWatingRoomIntentClosed(result);
	}

	private void OnRoomUpdate(string data) {


		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		_currentRoom =  new GP_RTM_Room();
		_currentRoom.id = storeData[0];
		_currentRoom.creatorId = storeData[1];

		string[] ParticipantsInfo = storeData[2].Split(","[0]);

		for(int i = 0; i < ParticipantsInfo.Length; i += 6) {
			if(ParticipantsInfo[i] == AndroidNative.DATA_EOF) {
				break;
			}

			GP_Participant p =  new GP_Participant(ParticipantsInfo[i], ParticipantsInfo[i + 1], ParticipantsInfo[i + 2], ParticipantsInfo[i + 3], ParticipantsInfo[i + 4], ParticipantsInfo[i + 5]);
			_currentRoom.AddParticipant(p);
		}




		_currentRoom.status =  (GP_RTM_RoomStatus) System.Convert.ToInt32(storeData[3]);
		_currentRoom.creationTimestamp = System.Convert.ToInt64(storeData[4]);

		Debug.Log("GooglePlayRTM OnRoomUpdate Room State: " + _currentRoom.status.ToString());

		ActionRoomUpdated(_currentRoom);

	}

	private void OnReliableMessageSent(string data) {
		Debug.Log ("[OnReliableMessageSent] " + data);

		string[] resultData = data.Split(AndroidNative.DATA_SPLITTER[0]);
		int messageTokedId = Int32.Parse(resultData[2]);
		int dataTokenId = Int32.Parse(resultData[3]);

		if (_ReliableMassageListeners.ContainsKey(dataTokenId)) {
			GP_RTM_ReliableMessageSentResult result =
				new GP_RTM_ReliableMessageSentResult(resultData[0], resultData[1], messageTokedId, _ReliableMassageListeners[dataTokenId].Data);
			ActionReliableMessageSent(result);

			_ReliableMassageListeners[dataTokenId].ReportSentMessage();
		} else {
			GP_RTM_ReliableMessageSentResult result = new GP_RTM_ReliableMessageSentResult(resultData[0], resultData[1], messageTokedId, null);
			ActionReliableMessageSent(result);
		}
	}

	private void OnReliableMessageDelivered(string data) {
		Debug.Log("[OnReliableMessageDelivered] " + data);

		string[] resultData = data.Split(AndroidNative.DATA_SPLITTER[0]);
		int messageTokedId = Int32.Parse(resultData[2]);
		int dataTokenId = Int32.Parse(resultData[3]);

		if (_ReliableMassageListeners.ContainsKey(dataTokenId)) {
			GP_RTM_ReliableMessageDeliveredResult result =
				new GP_RTM_ReliableMessageDeliveredResult(resultData[0], resultData[1], messageTokedId, _ReliableMassageListeners[dataTokenId].Data);
			ActionReliableMessageDelivered(result);

			_ReliableMassageListeners[dataTokenId].ReportDeliveredMessage();
		} else {
			GP_RTM_ReliableMessageDeliveredResult result = new GP_RTM_ReliableMessageDeliveredResult(resultData[0], resultData[1], messageTokedId, null);
			ActionReliableMessageDelivered(result);
		}
	}

	private void OnMatchDataRecieved(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("OnMatchDataRecieved, no data avaiable");
			return;
		}
		
		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		GP_RTM_Network_Package package = new GP_RTM_Network_Package (storeData[0], storeData [1]);


		ActionDataRecieved(package);
		Debug.Log ("GooglePlayManager -> DATA_RECEIVED");
	}
	
	private void OnConnectedToRoom(string data) {
		Debug.Log("[OnConnectedToRoom] data " + data);
		ActionConnectedToRoom();
	}
	
	private void OnDisconnectedFromRoom(string data) {
		Debug.Log("[OnDisconnectedFromRoom] data " + data);
		ActionDisconnectedFromRoom();
	}
	
	private void OnP2PConnected(string participantId) {
		Debug.Log("[OnP2PConnected] participantId " + participantId);
		ActionP2PConnected(participantId);
	}
	
	private void OnP2PDisconnected(string participantId) {
		Debug.Log("[OnP2PDisconnected] participantId " + participantId);
		ActionP2PDisconnected(participantId);
	}

	private void OnPeerDeclined(string data) {
		Debug.Log("[OnPeerDeclined] data " + data);
		string[] participantsids = data.Split(","[0]);
		ActionPeerDeclined(participantsids);
	}
	
	private void OnPeerInvitedToRoom(string data) {
		Debug.Log("[OnPeerInvitedToRoom] data " + data);
		string[] participantsids = data.Split(","[0]);
		ActionPeerInvitedToRoom(participantsids);
	}
	
	private void OnPeerJoined(string data) {
		Debug.Log("[OnPeerJoined] data " + data);
		string[] participantsids = data.Split(","[0]);
		ActionPeerJoined(participantsids);
	}
	
	private void OnPeerLeft(string data) {
		Debug.Log("[OnPeerLeft] data " + data);
		string[] participantsids = data.Split(","[0]);
		ActionPeerLeft(participantsids);
	}
	
	private void OnPeersConnected(string data) {
		Debug.Log("[OnPeersConnected] data " + data);
		string[] participantsids = data.Split(","[0]);
		ActionPeersConnected(participantsids);
	}
	
	private void OnPeersDisconnected(string data) {
		Debug.Log("[OnPeersDisconnected] data " + data);
		string[] participantsids = data.Split(","[0]);
		ActionPeersDisconnected(participantsids);
	}
	
	private void OnRoomAutoMatching(string data) {
		Debug.Log("[OnRoomAutoMatching] data " + data);
		ActionRoomAutomatching();
	}
	
	private void OnRoomConnecting(string data) {
		Debug.Log("[OnRoomConnecting] data " + data);
		ActionRoomConnecting();
	}
		
	private void OnJoinedRoom(string data) {
		Debug.Log("[OnJoinedRoom] data " + data);
		GP_GamesStatusCodes code = (GP_GamesStatusCodes)Convert.ToInt32(data);
		ActionJoinedRoom(code);
	}
	
	private void OnLeftRoom(string data) {
		Debug.Log("[OnLeftRoom] Created OnRoomUpdate data " + data);
		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		GP_RTM_Result package = new GP_RTM_Result (storeData[0], storeData [1]);


		_currentRoom =  new GP_RTM_Room();
		ActionRoomUpdated(_currentRoom);

		ActionLeftRoom(package);
	}
	
	private void OnRoomConnected(string data) {
		Debug.Log("[OnRoomConnected] data " + data);
		GP_GamesStatusCodes code = (GP_GamesStatusCodes)Convert.ToInt32(data);
		ActionRoomConnected(code);
	}
	
	private void OnRoomCreated(string data) {
		Debug.Log("[OnRoomCreated] data " + data);
		GP_GamesStatusCodes code = (GP_GamesStatusCodes)Convert.ToInt32(data);
		ActionRoomCreated(code);
	}

	private void OnInvitationBoxUiClosed(string data) {
		Debug.Log("[OnInvitationBoxUiClosed] data " + data);
		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		AndroidActivityResult result =  new AndroidActivityResult(storeData[0], storeData[1]);
	
		ActionInvitationBoxUIClosed(result);
	}

	private void OnInvitationReceived(GP_Invite inv) {
		if (inv.InvitationType == GP_InvitationType.INVITATION_TYPE_REAL_TIME) {
			_invitations.Add(inv);
			ActionInvitationReceived(inv);
		}
	}

	private void OnInvitationRemoved(string invitationId) {
		Debug.Log("[OnInvitationRemoved] invitationId " + invitationId);
		foreach(GP_Invite inv in _invitations) {
			if(inv.Id.Equals(invitationId)) {
				_invitations.Remove(inv);
				return;
			}
		}
		ActionInvitationRemoved(invitationId);
	}

	private void OnInvitationAccepted (GP_Invite inv)
	{
		ActionInvitationAccepted(inv);
	}

	//--------------------------------------
	// STATIC
	//--------------------------------------

	public static byte[] ConvertStringToByteData(string data) {

	#if UNITY_ANDROID
		if(data == null) {
			return null;
		}
		
		data = data.Replace(AndroidNative.DATA_EOF, string.Empty);
		if(data.Equals(string.Empty)) {
			return null;
		}
		
		string[] array = data.Split("," [0]);
		
		List<byte> listOfBytes = new List<byte> ();
		foreach(string str in array) {
			int param = System.Convert.ToInt32(str);
			int temp_param = param < 0 ? BYTE_LIMIT + param : param;
			listOfBytes.Add (System.Convert.ToByte(temp_param));
		}
		
		return listOfBytes.ToArray ();


	#else
		return new byte[]{};
	#endif

	}

	public static string ConvertByteDataToString(byte[] data) {
		System.Text.StringBuilder b = new System.Text.StringBuilder("");
		for(int i = 0; i < data.Length; i++) {
			if(i != 0) {
				b.Append(",");
			}
			
			b.Append(data[i]);
		}
		return b.ToString();		
	}






}
