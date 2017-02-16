////////////////////////////////////////////////////////////////////////////////
//  
// @module EditorTesting
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SA_Ad_EditorUIController : MonoBehaviour {
	public GameObject VideoPanel;
	public GameObject InterstitialPanel;

	public Image[] AppIcons;
	public Text[] AppNames;

	public event Action<bool>	OnCloseVideo 			= delegate{};
	public event Action 		OnVideoLeftApplication 	= delegate{};

	public event Action<bool>  	OnCloseInterstitial 			= delegate{};
	public event Action 		OnInterstitialLeftApplication 	= delegate{};


	void Awake() {
		DontDestroyOnLoad(gameObject);
		SA_EditorTesting.CheckForEventSystem();

		Canvas can  = GetComponent<Canvas>();
		can.sortingOrder = SA_EditorTesting.DEFAULT_SORT_ORDER + 1;

	}


	void Start() {
#if UNITY_EDITOR
		Texture2D[] icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
		if (icons[0] != null) {
			foreach (Image image in AppIcons) {
				image.sprite = Sprite.Create(icons[0], new Rect(0.0f, 0.0f, icons[0].width, icons[0].height), new Vector2(0.5f, 0.5f));
			}
		}

		foreach (Text name in AppNames) {
			name.text = PlayerSettings.productName;
		}
#endif
	}

	public void InterstitialClick() {
		OnInterstitialLeftApplication();
	}

	public void VideoClick() {
		OnVideoLeftApplication();
	}

	public void ShowInterstitialAd() {
		gameObject.SetActive(true);
		InterstitialPanel.SetActive(true);
	}

	public void ShowVideoAd() {
		gameObject.SetActive(true);
		VideoPanel.SetActive(true);
	}

	public void CloseInterstitial(){
		gameObject.SetActive(false);
		InterstitialPanel.SetActive(false);
		OnCloseInterstitial(true);
	}

	public void CloseVideo(){
		gameObject.SetActive(false);
		VideoPanel.SetActive(false);
		OnCloseVideo(true);
	}
}
