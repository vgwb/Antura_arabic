using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SA_InApps_EditorUIController : MonoBehaviour {
	public Text Title;
	public Text Describtion;
	public Text Price;
	public UnityEngine.UI.Toggle IsSuccsesPurchase;
	public Image Fader;



	public SA_UIHightDependence HightDependence;
	private SA.Common.Animation.ValuesTween _CurrentTween = null;
	private SA.Common.Animation.ValuesTween _FaderTween = null;


	private Action<bool> _OnComplete =  null;


	//--------------------------------------
	// initialization
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);
		
		Canvas can  = GetComponent<Canvas>();
		can.sortingOrder = SA_EditorTesting.DEFAULT_SORT_ORDER + 3;
	}
	
	
	//--------------------------------------
	// Public methods
	//--------------------------------------
	
	
	public void ShowInAppPopup(string title, string describtion, string price, Action<bool> OnComplete = null) {

		if(_CurrentTween != null) {
			_CurrentTween.Stop();
		}

		if(_FaderTween != null) {
			_FaderTween.Stop();
		}


		_OnComplete = OnComplete;

		Title.text  = title;
		Describtion.text = describtion;
		Price.text = price;
		


		Animate(150, -300f, SA.Common.Animation.EaseType.easeOutBack);
		_CurrentTween.OnComplete += HandleOnInTweenComplete;

		FadeIn();
	}


	public void Close() {
		if(_CurrentTween != null) {
			_CurrentTween.Stop();
		}
		
		if(_FaderTween != null) {
			_FaderTween.Stop();
		}

		Animate(-300f, 150f, SA.Common.Animation.EaseType.easeInBack);
		_CurrentTween.OnComplete += HandleOnOutTweenComplete;

		FadeOut();


		if(_OnComplete != null) {
			_OnComplete(IsSuccsesPurchase.isOn);
			_OnComplete = null;
		}
	}



	//--------------------------------------
	// Handlers
	//--------------------------------------
	
	private void HandleOnInTweenComplete (){
		_CurrentTween = null;
	}

	private void HandleOnOutTweenComplete (){
		_CurrentTween = null;
	}


	
	private void HandleOnValueChanged (float pos) {
		HightDependence.InitialRect.y = pos;
	}

	private void HandleOnFadeValueChanged(float a) {
		Fader.color =  new Color(Fader.color.r, Fader.color.g, Fader.color.b, a);

	}

	private void HandleFadeComplete (){
		Fader.enabled = false;
	}


	//--------------------------------------
	// Private Methods
	//--------------------------------------

	private void FadeIn() {
		Fader.enabled = true;
		_FaderTween =  SA.Common.Animation.ValuesTween.Create();
		_FaderTween.OnValueChanged += HandleOnFadeValueChanged;
		_FaderTween.ValueTo(0, 0.5f, 0.5f, SA.Common.Animation.EaseType.linear);
	}

	private void FadeOut() {
		_FaderTween =  SA.Common.Animation.ValuesTween.Create();
		_FaderTween.OnValueChanged += HandleOnFadeValueChanged;
		_FaderTween.OnComplete += HandleFadeComplete;
		_FaderTween.ValueTo(0.5f, 0f, 0.5f, SA.Common.Animation.EaseType.linear);
	}
	
	private void Animate(float from, float to, SA.Common.Animation.EaseType easeType) {
		_CurrentTween =  SA.Common.Animation.ValuesTween.Create();
		_CurrentTween.OnValueChanged += HandleOnValueChanged;
		_CurrentTween.ValueTo(from, to, 0.5f, easeType);
	}

}
