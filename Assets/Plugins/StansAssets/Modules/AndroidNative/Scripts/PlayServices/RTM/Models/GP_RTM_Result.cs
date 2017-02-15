using UnityEngine;
using System.Collections;

public class GP_RTM_Result  {

	public GP_GamesStatusCodes _status;
	public string _roomId;

	public GP_RTM_Result(string r_status, string r_roomId) {
		this._status = (GP_GamesStatusCodes)System.Convert.ToInt32(r_status);
		this._roomId = r_roomId;
	}

	public GP_GamesStatusCodes status {
		get {
			return _status;
		}
	}

	public string roomId {
		get {
			return _roomId;
		}
	}
}
