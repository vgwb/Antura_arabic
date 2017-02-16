using UnityEngine;
using System.Collections;

public class GP_RTM_ReliableMessageDeliveredResult : GP_RTM_Result {

	private int _MessageTokenId = 0;
	private byte[] _Data = null;

	public GP_RTM_ReliableMessageDeliveredResult (string status, string roomId, int messageTokedId, byte[] data) : base (status, roomId) {
		_MessageTokenId = messageTokedId;
		_Data = data;
	}


	public int MessageTokenId {
		get {
			return _MessageTokenId;
		}
	}
	
	public byte[] Data {
		get {
			return _Data;
		}
	}

}
