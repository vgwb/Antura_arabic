using UnityEngine;
using System;
using System.Collections;

public class GP_Participant  {

	private string _id;
	private string _playerid;
	private string _HiResImageUrl;
	private string _IconImageUrl;
	private string _DisplayName;

	private GP_ParticipantResult _result = null;
	private GP_RTM_ParticipantStatus _status = GP_RTM_ParticipantStatus.STATUS_UNRESPONSIVE;


	public event Action<Texture2D> BigPhotoLoaded =  delegate {};
	public event Action<Texture2D> SmallPhotoLoaded =  delegate {};

	private Texture2D _SmallPhoto = null;
	private Texture2D _BigPhoto = null;



	//--------------------------------------
	// Initialization
	//--------------------------------------


	public GP_Participant(string uid, string playerUid, string stat, string hiResImg, string IconImg, string Name) {
		_id = uid;
		_playerid = playerUid;
		_status = (GP_RTM_ParticipantStatus) System.Convert.ToInt32(stat);
		_HiResImageUrl = hiResImg;
		_IconImageUrl = IconImg;
		_DisplayName = Name;
	}


	//--------------------------------------
	// Public Methods
	//--------------------------------------

	public void SetResult(GP_ParticipantResult r) {
		_result = r;
	}

	public void LoadBigPhoto() {
		SA.Common.Util.Loader.LoadWebTexture(_HiResImageUrl, HandheBigPhotoLoaed);
	}
	
	
	public void LoadSmallPhoto() {
		SA.Common.Util.Loader.LoadWebTexture(_IconImageUrl, HandheSmallPhotoLoaed);
	}
	

	public Texture2D SmallPhoto {
		get {
			return _SmallPhoto;
		}
	}

	public Texture2D BigPhoto {
		get {
			return _BigPhoto;
		}
	}

	//--------------------------------------
	// Get / Set
	//--------------------------------------


	public string id {
		get {
			return _id;
		}
	}

	public string playerId {
		get {
			return _playerid;
		}
	}

	public string HiResImageUrl {
		get {
			return _HiResImageUrl;
		}
	}

	public string IconImageUrl {
		get {
			return _IconImageUrl;
		}
	}

	public string DisplayName {
		get {
			return _DisplayName;
		}
	}

	public GP_RTM_ParticipantStatus Status {
		get {
			return _status;
		}
	}

	public GP_ParticipantResult Result {
		get {
			return _result;
		}
	}

	//--------------------------------------
	// Handlers
	//--------------------------------------

	private void HandheBigPhotoLoaed(Texture2D tex) {

		if(this == null) {return;}

		_BigPhoto = tex;
		BigPhotoLoaded(_BigPhoto);
	}
	
	private void HandheSmallPhotoLoaed(Texture2D tex) {

		if(this == null) {return;}

		_SmallPhoto = tex;
		SmallPhotoLoaded(_SmallPhoto);
		
	}

}
