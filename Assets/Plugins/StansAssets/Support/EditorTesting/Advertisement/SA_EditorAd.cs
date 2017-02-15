using UnityEngine;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SA_EditorAd : SA.Common.Pattern.Singleton<SA_EditorAd> {


	public const float  MIN_LOAD_TIME = 1f;
	public const float  MAX_LOAD_TIME = 3f;


	private bool _IsInterstitialLoading = false;
	private bool _IsVideoLoading = false;

	private bool _IsInterstitialReady = false;
	private bool _IsVideoReady = false;

	private int _FillRate = 100;


	public static event Action<bool> OnInterstitialFinished 		= delegate {};
	public static event Action<bool> OnInterstitialLoadComplete		= delegate {};
	public static event Action		 OnInterstitialLeftApplication 	= delegate {};

	public static event Action<bool> OnVideoFinished 		= delegate {};
	public static event Action<bool> OnVideoLoadComplete 	= delegate {};
	public static event Action		 OnVideoLeftApplication = delegate {};

	private SA_Ad_EditorUIController _EditorUI = null;

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	
	//Fill rate should be from 0 to 100, where 0 always error, 100 always success
	public void SetFillRate(int fillRate) {
		_FillRate = fillRate;
	}


	public void LoadInterstitial() {
		if(!_IsInterstitialLoading && !IsInterstitialReady) {
			_IsInterstitialLoading = true;
			float time = UnityEngine.Random.Range(MIN_LOAD_TIME, MAX_LOAD_TIME);
			Invoke("OnInterstitialRequestComplete", time);
		}
	}

	public void ShowInterstitial() {
		if(IsInterstitialReady) {
#if UNITY_EDITOR
			EditorUI.OnInterstitialLeftApplication += OnInterstitialLeftApplication_UIEvent;
			EditorUI.OnCloseInterstitial += OnInterstitialFinished_UIEvent;
			EditorUI.ShowInterstitialAd();
#endif
		}
	}	

	public void LoadVideo() {
		if(!_IsVideoLoading && !IsVideoReady) {
			_IsVideoLoading = true;
			float time = UnityEngine.Random.Range(MIN_LOAD_TIME, MAX_LOAD_TIME);
			Invoke("OnVideoRequestComplete", time);
		}
	}

	public void ShowVideo() {
		if(IsVideoReady) {
#if UNITY_EDITOR
			EditorUI.OnVideoLeftApplication += OnVideoLeftApplication_UIEvent;
			EditorUI.OnCloseVideo += OnVideoFinished_UIEvent;
			EditorUI.ShowVideoAd();
#endif
		}
	}

	//--------------------------------------
	// Get / Set 
	//--------------------------------------
	

	public bool IsVideoReady {
		get {
			return _IsVideoReady;
		}
	}

	public bool IsVideoLoading {
		get {
			return _IsVideoLoading;
		}
	}

	public bool IsInterstitialReady {
		get {
			return _IsInterstitialReady;
		}
	}

	public bool IsInterstitialLoading {
		get {
			return _IsInterstitialLoading;
		}
	}

	public bool HasFill {
		get {
			int probability = UnityEngine.Random.Range(1, 100);
			if(probability <= _FillRate) {
				return true;
			} else {
				return false;
			}
		}
	}

	public int FillRate {
		get {
			return _FillRate;
		}
	}

	private SA_Ad_EditorUIController EditorUI {
		get {
#if UNITY_EDITOR
			if (_EditorUI == null) {
				GameObject o = AssetDatabase.LoadAssetAtPath("Assets/Plugins/StansAssets/Support/EditorTesting/UI/Prefabs/AdsEditorTestingUI.prefab", typeof(GameObject)) as GameObject;
				GameObject go = Instantiate(o) as GameObject;
				_EditorUI = go.GetComponent<SA_Ad_EditorUIController>();
			}
#endif
			return _EditorUI;
		}
	}

	//--------------------------------------
	// Internal Events
	//--------------------------------------

	private void OnVideoRequestComplete() {
		_IsVideoLoading = false;
		_IsVideoReady = HasFill;
		OnVideoLoadComplete(_IsVideoReady);

	}

	private void OnInterstitialRequestComplete() {
		_IsInterstitialLoading = false;
		_IsInterstitialReady = HasFill;
		OnInterstitialLoadComplete(_IsInterstitialReady);
	}


	private void OnInterstitialFinished_UIEvent(bool IsRewarded) {
#if UNITY_EDITOR
		EditorUI.OnInterstitialLeftApplication -= OnInterstitialLeftApplication_UIEvent;
		EditorUI.OnCloseInterstitial -= OnInterstitialFinished_UIEvent;
#endif
		_IsInterstitialReady = false;
		OnInterstitialFinished(IsRewarded);
	}

	private void OnVideoFinished_UIEvent(bool IsRewarded) {
#if UNITY_EDITOR
		EditorUI.OnVideoLeftApplication -= OnVideoLeftApplication_UIEvent;
		EditorUI.OnCloseVideo -= OnVideoFinished_UIEvent;
#endif
		_IsVideoReady = false;
		OnVideoFinished(IsRewarded);
	}

	private void OnInterstitialLeftApplication_UIEvent() {
		OnInterstitialLeftApplication();
	}
	
	private void OnVideoLeftApplication_UIEvent() {
		OnVideoLeftApplication();
	}
	
}
