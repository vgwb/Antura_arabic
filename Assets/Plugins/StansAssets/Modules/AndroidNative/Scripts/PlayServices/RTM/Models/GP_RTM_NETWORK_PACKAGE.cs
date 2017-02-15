using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GP_RTM_Network_Package  {



	private string _playerId;
	private byte[] _buffer;

	private const int BYTE_LIMIT = 256;


	public GP_RTM_Network_Package(string player, string recievedData) {
		_playerId = player;
		
		Debug.Log ("GOOGLE_PLAY_RESULT -> OnMatchDataRecieved " + recievedData);
		_buffer = ConvertStringToByteData(recievedData);
	}
	


	public string participantId {
		get {
			return _playerId;
		}
	}

	public byte[] buffer {
		get {
			return _buffer;
		}
	}

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
}
