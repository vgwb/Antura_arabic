////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;


public class GooglePlayConnection : SA.Common.Pattern.Singleton<GooglePlayConnection> {
	

	

	//Actions
	public static event Action<GooglePlayConnectionResult> ActionConnectionResultReceived =  delegate {};

	public static event Action<GPConnectionState> ActionConnectionStateChanged =  delegate {};
	public static event Action ActionPlayerConnected =  delegate {};
	public static event Action ActionPlayerDisconnected =  delegate {};


	private bool _IsInitialized = false;
	private static GPConnectionState _State = GPConnectionState.STATE_UNCONFIGURED;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);

		GooglePlayManager.Instance.Create();
		Init();
	}


	//--------------------------------------
	// PUBLIC API CALL METHODS
	//--------------------------------------




	private void Init() {
		string connectionString = "";
		if(AndroidNativeSettings.Instance.EnableGamesAPI) {
			connectionString += "GamesAPI";
		}

		if(AndroidNativeSettings.Instance.EnablePlusAPI) {
			connectionString += "PlusAPI";
		}

		if(AndroidNativeSettings.Instance.EnableDriveAPI) {
			connectionString += "DriveAPI";
		}

		if(AndroidNativeSettings.Instance.EnableAppInviteAPI) {
			connectionString += "AppInvite";
		}


		AN_GMSGeneralProxy.setConnectionParams (AndroidNativeSettings.Instance.ShowConnectingPopup);
		AN_GMSGeneralProxy.playServiceInit(connectionString);
	}


	[Obsolete("connect is deprecated, please use Connect instead.")]
	public void connect() {
		Connect();
	}

	public void Connect()  {
		Connect(null);
	}

	[Obsolete("connect is deprecated, please use Connect instead.")]
	public void connect(string accountName) {
		Connect(accountName);
	}


	public void Connect(string accountName) {

		if(_State == GPConnectionState.STATE_CONNECTED || _State == GPConnectionState.STATE_CONNECTING) {
			return;
		}

		OnStateChange(GPConnectionState.STATE_CONNECTING);

		if(accountName != null) {
			AN_GMSGeneralProxy.playServiceConnect (accountName);
		} else {
			AN_GMSGeneralProxy.playServiceConnect ();
		}

	}

	[Obsolete("disconnect is deprecated, please use Disconnect instead.")]
	public void disconnect() {
		Disconnect();
	}

	public void Disconnect() {

		if(_State == GPConnectionState.STATE_DISCONNECTED || _State == GPConnectionState.STATE_CONNECTING) {
			return;
		}

		OnStateChange(GPConnectionState.STATE_DISCONNECTED);
		AN_GMSGeneralProxy.playServiceDisconnect ();

	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------


	public static bool CheckState() {
		switch(_State) {
			case GPConnectionState.STATE_CONNECTED:
			return true;
			default:
			return false;
		}
	}



	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public bool IsConnected {
		get {
			return State == GPConnectionState.STATE_CONNECTED;
		}
	}

	[Obsolete("state is deprecated, please use State instead.")]
	public static GPConnectionState state {
		get {
			return State;
		}
	}

	public static GPConnectionState State {
		get {
			return _State;
		}
	}


	[Obsolete("isInitialized is deprecated, please use IsInitialized instead.")]
	public bool isInitialized {
		get {
			return IsInitialized;
		}
	}

	public  bool IsInitialized {
		get {
			return _IsInitialized;
		}
	}




	//--------------------------------------
	// EVENTS
	//--------------------------------------

	void OnApplicationPause(bool pauseStatus) {
		AN_GMSGeneralProxy.OnApplicationPause(pauseStatus);
	}
	

	private void OnPlayServiceDisconnected(string data) {
		OnStateChange(GPConnectionState.STATE_DISCONNECTED);
	}


	private void OnConnectionResult(string resultCode) {
		Debug.Log("[OnConnectionResult] resultCode " + resultCode);
		GooglePlayConnectionResult result = new GooglePlayConnectionResult();
		result.code = (GP_ConnectionResultCode) System.Convert.ToInt32(resultCode);


		if(result.IsSuccess) {
			OnStateChange(GPConnectionState.STATE_CONNECTED);
		} else {
			OnStateChange(GPConnectionState.STATE_DISCONNECTED);
		}

		ActionConnectionResultReceived(result);

	}


	private void OnStateChange(GPConnectionState connectionState) {

		_State = connectionState;
		switch(_State) {
			case GPConnectionState.STATE_CONNECTED:
				ActionPlayerConnected();
				break;
			case GPConnectionState.STATE_DISCONNECTED:
				ActionPlayerDisconnected();
				break; 
		}

		ActionConnectionStateChanged(_State);

		Debug.Log("Play Serice Connection State -> " + _State.ToString());
	}

	


}
