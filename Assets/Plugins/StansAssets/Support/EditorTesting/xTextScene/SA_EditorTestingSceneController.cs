using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SA_EditorTestingSceneController : MonoBehaviour {

	public Button ShowInterstitial_Button;
	public Button ShowInterstitial_Video;



	public void LoadInterstitial() {
		SA_EditorAd.Instance.LoadInterstitial();
	}

	public void ShowInterstitial() {
		SA_EditorAd.Instance.ShowInterstitial();
	}

	
	public void LoadVideo() {
		SA_EditorAd.Instance.LoadVideo();
	}
	
	public void ShowVideo() {
		SA_EditorAd.Instance.ShowVideo();
	}



	public void Show_Notifications() {
		SA_EditorNotifications.ShowNotification("Test", "Test Notification Body", SA_EditorNotificationType.Message);
	}

	public void Show_A_Notifications() {
		SA_EditorNotifications.ShowNotification("Achievement", "Test Notification Body", SA_EditorNotificationType.Achievement);
	}

	public void Show_L_Notifications() {
		SA_EditorNotifications.ShowNotification("Leaderboard", "Test Notification Body", SA_EditorNotificationType.Leaderboards);
	}

	public void Show_E_Notifications() {
		SA_EditorNotifications.ShowNotification("Error", "Test Notification Body", SA_EditorNotificationType.Error);
	}


	public void Show_InApp_Popup() {
		SA_EditorInApps.ShowInAppPopup("Product Title", "Product Describtion", "2.99$");

	}



	void FixedUpdate() {
		if(SA_EditorAd.Instance.IsInterstitialReady) {
			ShowInterstitial_Button.interactable = true;
		} else {
			ShowInterstitial_Button.interactable = false;
		}


		if(SA_EditorAd.Instance.IsVideoReady) {
			ShowInterstitial_Video.interactable = true;
		} else {
			ShowInterstitial_Video.interactable = false;
		}
	}

}
