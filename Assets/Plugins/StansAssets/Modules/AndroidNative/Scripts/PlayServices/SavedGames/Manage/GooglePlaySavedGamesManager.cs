using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GooglePlaySavedGamesManager :  SA.Common.Pattern.Singleton<GooglePlaySavedGamesManager> {



	//Actions
	public static event Action ActionGameSaveUIClosed = delegate{};
	public static event Action ActionNewGameSaveRequest 	= delegate {};
	public static event Action<GooglePlayResult> ActionAvailableGameSavesLoaded 	= delegate {};

	public static event Action<GP_SpanshotLoadResult> ActionGameSaveLoaded 	= delegate {};
	public static event Action<GP_SpanshotLoadResult> ActionGameSaveResult 	= delegate {};
	public static event Action<GP_SnapshotConflict> ActionConflict 	= delegate {};
	public static event Action<GP_DeleteSnapshotResult> ActionGameSaveRemoved 	= delegate {};

	private List<GP_SnapshotMeta> _AvailableGameSaves = new List<GP_SnapshotMeta>();



	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	//--------------------------------------
	// PUBLIC API CALL METHODS
	//--------------------------------------

	public void ShowSavedGamesUI(string title, int maxNumberOfSavedGamesToShow, bool allowAddButton = true, bool allowDelete = true)  {
		if (!GooglePlayConnection.CheckState ()) { return; }

		AN_GMSGeneralProxy.ShowSavedGamesUI_Bridge(title, maxNumberOfSavedGamesToShow, allowAddButton, allowDelete);
	}


	public void CreateNewSnapshot(string name, string description, Texture2D coverImage, string spanshotData, long PlayedTime)  {
		CreateNewSnapshot(name, description, coverImage, GetBytes(spanshotData), PlayedTime);
	}
	

	public void CreateNewSnapshot(string name, string description, Texture2D coverImage, byte[] spanshotData, long PlayedTime)  {
		string mdeia = string.Empty;

		if(coverImage != null) {
			byte[] val = coverImage.EncodeToPNG();
			mdeia = System.Convert.ToBase64String (val);
		}  else {
			Debug.LogWarning("GooglePlaySavedGmaesManager::CreateNewSnapshot:  coverImage is null");
		}

		string data = System.Convert.ToBase64String (spanshotData);

		AN_GMSGeneralProxy.CreateNewSpanshot_Bridge(name, description, mdeia, data, PlayedTime);
	}


	public void LoadSpanshotByName(string name) {
		AN_GMSGeneralProxy.OpenSpanshotByName_Bridge(name);
	}

	public void DeleteSpanshotByName(string name) {
		AN_GMSGeneralProxy.DeleteSpanshotByName_Bridge(name);
	}

	public void LoadAvailableSavedGames() {
		AN_GMSGeneralProxy.LoadSpanshots_Bridge();
	}


	
	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public List<GP_SnapshotMeta> AvailableGameSaves {
		get {
			return _AvailableGameSaves;
		}
	}


	//--------------------------------------
	// PRIVATE  METHODS
	//--------------------------------------

	private static byte[] GetBytes(string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}


	private static string GetString(byte[] bytes) {
		char[] chars;
		if (bytes.Length % 2 != 0) {
			chars = new char[(bytes.Length / sizeof(char)) + 1];
		}
		else {
			chars = new char[bytes.Length / sizeof(char)];
		}

		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}

	
	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void OnLoadSnapshotsResult(string data) {
		Debug.Log("SavedGamesManager: OnLoadSnapshotsResult");

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.IsSucceeded) {
			
			_AvailableGameSaves.Clear ();
			
			for(int i = 1; i < storeData.Length; i+=5) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}

				GP_SnapshotMeta meta = new GP_SnapshotMeta();
				meta.Title = storeData[i];
				meta.LastModifiedTimestamp = System.Convert.ToInt64(storeData [i + 1]);
				meta.Description = storeData[i + 2];
				meta.CoverImageUrl = storeData[i + 3];
				meta.TotalPlayedTime = System.Convert.ToInt64(storeData[i + 4]);

				_AvailableGameSaves.Add(meta);
				
			}
			
			Debug.Log ("Loaded: " + _AvailableGameSaves.Count + " Snapshots");
		}
		
		ActionAvailableGameSavesLoaded(result);
	}



	private void OnSavedGamePicked(string data) {
		Debug.Log("SavedGamesManager: OnSavedGamePicked");

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);


		GP_SpanshotLoadResult result = new GP_SpanshotLoadResult (storeData [0]);
		if(result.IsSucceeded) {
			string Title = storeData [1];
			long LastModifiedTimestamp = System.Convert.ToInt64(storeData [2]) ;
			string Description = storeData [3];
			string CoverImageUrl = storeData [4];
			long TotalPlayedTime = System.Convert.ToInt64(storeData [5]) ;
			byte[] decodedFromBase64 = System.Convert.FromBase64String(storeData [6]);
			
			
			GP_Snapshot  Snapshot =  new GP_Snapshot();
			Snapshot.meta.Title 					= Title;
			Snapshot.meta.Description 				= Description;
			Snapshot.meta.CoverImageUrl 			= CoverImageUrl;
			Snapshot.meta.LastModifiedTimestamp 	= LastModifiedTimestamp;
			Snapshot.meta.TotalPlayedTime			= TotalPlayedTime;

			Snapshot.bytes 							= decodedFromBase64;
			Snapshot.stringData 					= GetString(decodedFromBase64);

			result.SetSnapShot(Snapshot);
		
		}

		ActionGameSaveLoaded(result);

	}

	private void OnSavedGameSaveResult(string data) {
		Debug.Log("SavedGamesManager: OnSavedGameSaveResult");
		
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		
		GP_SpanshotLoadResult result = new GP_SpanshotLoadResult (storeData [0]);
		if(result.IsSucceeded) {
			string Title = storeData [1];
			long LastModifiedTimestamp = System.Convert.ToInt64(storeData [2]) ;
			string Description = storeData [3];
			string CoverImageUrl = storeData [4];
			long TotalPlayedTime = System.Convert.ToInt64(storeData [5]) ;
			byte[] decodedFromBase64 = System.Convert.FromBase64String(storeData [6]);
			
			
			GP_Snapshot  Snapshot =  new GP_Snapshot();
			Snapshot.meta.Title 					= Title;
			Snapshot.meta.Description 				= Description;
			Snapshot.meta.CoverImageUrl 			= CoverImageUrl;
			Snapshot.meta.LastModifiedTimestamp 	= LastModifiedTimestamp;
			Snapshot.meta.TotalPlayedTime			= TotalPlayedTime;
			
			Snapshot.bytes 							= decodedFromBase64;
			Snapshot.stringData 					= GetString(decodedFromBase64);
			
			result.SetSnapShot(Snapshot);
		}

		ActionGameSaveResult(result);
		
	}


	private void OnConflict(string data)  {
		Debug.Log("SavedGamesManager: OnConflict");
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		string Title = storeData [0];
		long LastModifiedTimestamp = System.Convert.ToInt64(storeData [1]) ;
		string Description = storeData [2];
		string CoverImageUrl = storeData [3];
		long TotalPlayedTime = System.Convert.ToInt64(storeData [4]) ;
		byte[] decodedFromBase64 = System.Convert.FromBase64String(storeData [5]);

		GP_Snapshot  Snapshot1 =  new GP_Snapshot();
		Snapshot1.meta.Title 					= Title;
		Snapshot1.meta.Description 			= Description;
		Snapshot1.meta.CoverImageUrl 			= CoverImageUrl;
		Snapshot1.meta.LastModifiedTimestamp 	= LastModifiedTimestamp;
		Snapshot1.meta.TotalPlayedTime			= TotalPlayedTime;

		Snapshot1.bytes 					= decodedFromBase64;
		Snapshot1.stringData 			= GetString(decodedFromBase64);


		Title = storeData [6];
		LastModifiedTimestamp = System.Convert.ToInt64(storeData [7]) ;
		Description = storeData [8];
		CoverImageUrl = storeData [9];
		TotalPlayedTime = System.Convert.ToInt64(storeData [10]);
		decodedFromBase64 = System.Convert.FromBase64String(storeData [11]);

		GP_Snapshot  Snapshot2 =  new GP_Snapshot();
		Snapshot2.meta.Title 					= Title;
		Snapshot2.meta.Description 			= Description;
		Snapshot2.meta.CoverImageUrl 			= CoverImageUrl;
		Snapshot2.meta.LastModifiedTimestamp 	= LastModifiedTimestamp;
		Snapshot2.meta.TotalPlayedTime			= TotalPlayedTime;

		Snapshot2.bytes 					= decodedFromBase64;
		Snapshot2.stringData 			= GetString(decodedFromBase64);


		GP_SnapshotConflict result =  new GP_SnapshotConflict(Snapshot1, Snapshot2);

		ActionConflict(result);

	}



	private void OnNewGameSaveRequest(string data) {
		Debug.Log("SavedGamesManager: OnNewGameSaveRequest");
		ActionNewGameSaveRequest();

	}

	private void OnSavedGamesUIClosed(string data) {
		Debug.Log ("OnSavedGamesUIClosed");
		ActionGameSaveUIClosed ();
	}

	private void OnDeleteResult(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		
		GP_DeleteSnapshotResult result = new GP_DeleteSnapshotResult (storeData [0]);
		if(result.IsSucceeded) {
			result.SetId(storeData [1]);
		}


		ActionGameSaveRemoved(result);
	}










}
