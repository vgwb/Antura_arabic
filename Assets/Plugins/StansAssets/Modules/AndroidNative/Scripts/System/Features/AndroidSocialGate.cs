using UnityEngine;
using System;
using System.Collections;

public class AndroidSocialGate : MonoBehaviour {

	private static AndroidSocialGate _Instance = null;

	public static event Action<bool, string> OnShareIntentCallback = delegate {};

	public static void StartGooglePlusShare(string text, Texture2D texture = null) {
		CheckAndCreateInstance();
		AN_SocialSharingProxy.StartGooglePlusShareIntent(text, texture == null ? string.Empty : System.Convert.ToBase64String(texture.EncodeToPNG()));
	}

	public static void StartShareIntent(string caption, string message, string packageNamePattern = "") {
		CheckAndCreateInstance();
		StartShareIntentWithSubject(caption, message, "", packageNamePattern);
	}

	public static void StartShareIntent(string caption, string message, Texture2D texture,  string packageNamePattern = "") {
		CheckAndCreateInstance();
		StartShareIntentWithSubject(caption, message, "", texture, packageNamePattern);
	}

	public static void StartShareIntentWithSubject(string caption, string message, string subject, string packageNamePattern = "") {
		CheckAndCreateInstance();
		AN_SocialSharingProxy.StartShareIntent(caption, message, subject, packageNamePattern);
	}

	public static void StartShareIntentWithSubject(string caption, string message, string subject, Texture2D texture,  string packageNamePattern = "") {
		CheckAndCreateInstance();

		byte[] val = texture.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);

		AN_SocialSharingProxy.StartShareIntent(caption,
												message,
												subject,
												bytesString,
												packageNamePattern,
												(int)AndroidNativeSettings.Instance.ImageFormat,
												AndroidNativeSettings.Instance.SaveCameraImageToGallery);
	}

	public static void SendMail(string caption, string message,  string subject, string recipients, Texture2D texture = null) {
		CheckAndCreateInstance();
		if(texture != null) {
			byte[] val = texture.EncodeToPNG();
			string mdeia = System.Convert.ToBase64String (val);

			AN_SocialSharingProxy.SendMailWithImage(caption,
													message,
													subject,
													recipients,
													mdeia,
													(int)AndroidNativeSettings.Instance.ImageFormat,
													AndroidNativeSettings.Instance.SaveCameraImageToGallery);
		} else {
			AN_SocialSharingProxy.SendMail(caption, message, subject, recipients);
		}
	}

    public static void ShareTwitterGif(string gifPath, string message) {
        AN_SocialSharingProxy.ShareTwitterGif(gifPath, message);
    }

	private static void CheckAndCreateInstance() {
		if (_Instance == null) {
			_Instance = GameObject.FindObjectOfType(typeof(AndroidSocialGate)) as AndroidSocialGate;
			if (_Instance == null) {
				_Instance = new GameObject ().AddComponent<AndroidSocialGate> ();
				_Instance.gameObject.name = _Instance.GetType ().Name;
			}
		}
	}

	private void ShareCallback(string data) {
		string[] rawData = data.Split(new string[] {"|"}, StringSplitOptions.None);
		bool posted = Int32.Parse(rawData[1]) == -1;
		OnShareIntentCallback(posted, rawData[0]);

		Debug.Log("[AndroidSocialGate]ShareCallback Posted:" + posted + " Package:" + rawData[0]);
	}
}

