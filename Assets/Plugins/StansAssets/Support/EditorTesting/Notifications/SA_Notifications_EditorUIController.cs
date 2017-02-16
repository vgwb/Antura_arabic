using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SA_Notifications_EditorUIController : MonoBehaviour {

	public Text Title;
	public Text Message;

	public Image[] Icons;


	public SA_UIHightDependence HightDependence;


	private SA.Common.Animation.ValuesTween _CurrentTween = null;

	void Awake() {
		DontDestroyOnLoad(gameObject);

		SA_EditorTesting.CheckForEventSystem();
		
		Canvas can  = GetComponent<Canvas>();
		can.sortingOrder = SA_EditorTesting.DEFAULT_SORT_ORDER + 1;
	}





	public void ShowNotification(string title, string message, SA_EditorNotificationType type) {

		if(_CurrentTween != null) {
			_CurrentTween.Stop();
		}
		CancelInvoke("NotificationDelayComplete");


		Title.text  = title;
		Message.text = message;


		foreach(Image icon in Icons) {
			icon.gameObject.SetActive(false);
		}

		switch(type) {
		case SA_EditorNotificationType.Achievement:
			Icons[0].gameObject.SetActive(true);
			break;

		case SA_EditorNotificationType.Error:
			Icons[1].gameObject.SetActive(true);
			break;

		case SA_EditorNotificationType.Leaderboards:
			Icons[2].gameObject.SetActive(true);
			break;

		case SA_EditorNotificationType.Message:
			Icons[3].gameObject.SetActive(true);
			break;
		}

		Animate(52f, -52f, SA.Common.Animation.EaseType.easeOutBack);
		_CurrentTween.OnComplete += HandleOnInTweenComplete;
	}

	void HandleOnInTweenComplete (){
		_CurrentTween = null;
		Invoke("NotificationDelayComplete", 2f);
	}


	void NotificationDelayComplete() {
		Animate(-52f, 58f, SA.Common.Animation.EaseType.easeInBack);
		_CurrentTween.OnComplete += HandleOnOutTweenComplete;
	}


	void HandleOnOutTweenComplete (){
		_CurrentTween = null;
	}

	void HandleOnValueChanged (float pos) {
		HightDependence.InitialRect.y = pos;
	}


	private void Animate(float from, float to, SA.Common.Animation.EaseType easeType) {
		_CurrentTween =  SA.Common.Animation.ValuesTween.Create();
		_CurrentTween.OnValueChanged += HandleOnValueChanged;
		_CurrentTween.ValueTo(from, to, 0.5f, easeType);
	}

}
