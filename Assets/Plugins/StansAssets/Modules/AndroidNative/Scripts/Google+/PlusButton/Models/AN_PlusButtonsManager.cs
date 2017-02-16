using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AN_PlusButtonsManager : SA.Common.Pattern.Singleton<AN_PlusButtonsManager> {
	
	public List<AN_PlusButton> buttons =  new List<AN_PlusButton>();



	void Awake() {
		buttons =  new List<AN_PlusButton>();
		DontDestroyOnLoad(gameObject);
	}


	public void RegisterButton(AN_PlusButton b) {
		buttons.Add(b);
	}

	void OnApplicationPause(bool IsPaused) {
		if(!IsPaused) {
			foreach(AN_PlusButton b in buttons) {
				if (b != null && b.IsShowed) {
					b.Refresh();
				}
			}

			Debug.Log("+1 buttons refreshed");
		}

	}

	private void OnPlusClicked(string data) {
		int id = System.Convert.ToInt32(data);

		foreach(AN_PlusButton b in buttons) {
			if(b != null) {
				if(b.ButtonId.Equals(id)) {
					b.FireClickAction();
				}
			}
		}
	}

}
