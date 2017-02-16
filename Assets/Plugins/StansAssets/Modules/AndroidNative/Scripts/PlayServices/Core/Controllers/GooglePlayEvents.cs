using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GooglePlayEvents : SA.Common.Pattern.Singleton<GooglePlayEvents> {

	
	//actions
	public Action<GooglePlayResult> OnEventsLoaded =  delegate{};


	private List<GP_Event> _Events =  new List<GP_Event>() ;

	//--------------------------------------
	// INITIALIZATION
	//--------------------------------------
	
	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	public void LoadEvents() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSQuestsEventsProxy.loadEvents (); 
	}

	public void SumbitEvent(string eventId) {
		SumbitEvent (eventId, 1);
	}

	public void SumbitEvent(string eventId, int count) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AN_GMSQuestsEventsProxy.sumbitEvent (eventId, count);
	}


	//--------------------------------------
	// GET / SET
	//--------------------------------------


	public List<GP_Event> Events {
		get {
			return _Events;
		}
	}



	//--------------------------------------
	// LISTNERS
	//--------------------------------------

	private void OnGPEventsLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.IsSucceeded) {
			
			for(int i = 1; i < storeData.Length; i+=5) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}
				
				GP_Event e = new GP_Event();
				e.Id = storeData[i];
				e.Description = storeData[i + 1];
				e.IconImageUrl = storeData[i + 2];
				e.FormattedValue = storeData[i + 3];
				e.Value = System.Convert.ToInt64( storeData[i + 4]);

				if(AndroidNativeSettings.Instance.LoadEventsIcons) {
					e.LoadIcon();
				}

				_Events.Add(e);
				
			}
		}


		OnEventsLoaded(result);
		Debug.Log ("OnGPEventsLoaded, total:" + _Events.Count.ToString());

	}

}
