////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GPAchievement  {

	//Editor Only
	public bool IsOpen = true;

	[SerializeField]
	private string _id = string.Empty;

	[SerializeField]
	private string _name = string.Empty;

	[SerializeField]
	private string _description = string.Empty;

	[SerializeField]
	private Texture2D _Texture;

	private int _currentSteps;
	private int _totalSteps;

	private GPAchievementType _type;
	private GPAchievementState _state;

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public GPAchievement(string id, string name){
		_id = id;
		_name = name;
	}

	public GPAchievement(string aId, string aName, string aDescr, string aCurentSteps, string aTotalSteps, string aState, string aType) {
		_id = aId;
		_name = aName;
		_description = aDescr;

		_currentSteps = System.Convert.ToInt32 (aCurentSteps);
		_totalSteps = System.Convert.ToInt32 (aTotalSteps);


		_type  = PlayServiceUtil.GetAchievementTypeById (System.Convert.ToInt32 (aType));
		_state = PlayServiceUtil.GetAchievementStateById (System.Convert.ToInt32 (aState));
	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------




	//--------------------------------------
	// GET / SET
	//--------------------------------------

	[System.Obsolete("id is deprectaed, please use Id instead")]
	public string id {
		get {
			return Id;
		}
	}

	public string Id {
		get {
			return _id;
		}
		set {
			_id = value;
		}
	}

	[System.Obsolete("name is deprectaed, please use Name instead")]
	public string name {
		get {
			return Name;
		}
	}

	public string Name {
		get {
			return _name;
		}
		set {
			_name = value;
		}
	}

	[System.Obsolete("description is deprectaed, please use Description instead")]
	public string description {
		get {
			return Description;
		}
	}

	public string Description {
		get {
			return _description;
		}
		set {
			_description = value;
		}
	}

	[System.Obsolete("currentSteps is deprectaed, please use CurrentSteps instead")]
	public int currentSteps {
		get {
			return CurrentSteps;
		}
	}

	public int CurrentSteps {
		get {
			return _currentSteps;
		}
	} 

	[System.Obsolete("totalSteps is deprectaed, please use TotalSteps instead")]
	public int totalSteps {
		get {
			return TotalSteps;
		}
	}

	public int TotalSteps {
		get {
			return _totalSteps;
		}
	} 

	[System.Obsolete("type is deprectaed, please use Type instead")]
	public GPAchievementType type {
		get {
			return Type;
		}
	}

	public GPAchievementType Type {
		get {
			return _type;
		}
	}

	[System.Obsolete("state is deprectaed, please use State instead")]
	public GPAchievementState state {
		get {
			return State;
		}
	}

	public GPAchievementState State {
		get {
			return _state;
		}
	}

	public Texture2D Texture {
		get {
			return _Texture;
		}
		set {
			_Texture = value;
		}
	}

}
