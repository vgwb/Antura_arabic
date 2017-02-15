using UnityEngine;
using System.Collections;

public class GP_RTM_ReliableMessageListener {

	private byte[] _Data = null;

	private int _DataTokenId = 0;
	private int _ReliableMessagesCounter = 0;

	public GP_RTM_ReliableMessageListener(int dataTokenId, byte[] data) {
		_DataTokenId = dataTokenId;
	}

	public void ReportSentMessage() {
		_ReliableMessagesCounter++;
	}

	public void ReportDeliveredMessage() {
		_ReliableMessagesCounter--;

		if (_ReliableMessagesCounter == 0) {
			//Clear messages with this data token id from Google Play RTM controller
			GooglePlayRTM.Instance.ClearReliableMessageListener(_DataTokenId);
		}
	}

	public int DataTokenId {
		get {
			return _DataTokenId;
		}
	}

	public byte[] Data {
		get {
			return _Data;
		}
	}
}
