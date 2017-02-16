using UnityEngine;
using System;
using System.Collections;

public class AndroidCamera : SA.Common.Pattern.Singleton<AndroidCamera>  {

	//Actions
	public event Action<AndroidImagePickResult> OnImagePicked = delegate{};
	public event Action<AndroidImagesPickResult> OnImagesPicked = delegate {};
	public event Action<GallerySaveResult> OnImageSaved = delegate{};

	private static string _lastImageName = string.Empty;

	void Awake() {
		DontDestroyOnLoad(gameObject);

		AndroidNative.InitCameraAPI(AndroidNativeSettings.Instance.GalleryFolderName,
		                            AndroidNativeSettings.Instance.MaxImageLoadSize,
		                            (int) AndroidNativeSettings.Instance.CameraCaptureMode,
		                            (int) AndroidNativeSettings.Instance.ImageFormat);
	}


	[Obsolete("SaveImageToGalalry is deprecated, please use SaveImageToGallery instead.")]
	public void SaveImageToGalalry(Texture2D image, String name = "Screenshot") {
		SaveImageToGallery(image, name);
	}

	public void SaveImageToGallery(Texture2D image, String name = "Screenshot") {
		if(image != null) {
			byte[] val = image.EncodeToPNG();
			string mdeia = System.Convert.ToBase64String (val);
			AndroidNative.SaveToGalalry(mdeia, name);
		}  else {
			Debug.LogWarning("AndroidCamera::SaveToGalalry:  image is null");
		}
	}




	public void SaveScreenshotToGallery(String name = "Screenshot") {
		_lastImageName = name;

		SA.Common.Util.Screen.TakeScreenshot(OnScreenshotReady);
	}


	public void GetImageFromGallery() {
		AndroidNative.GetImageFromGallery();
	}
	
	public void GetImagesFromGallery() {
		AndroidNative.GetImagesFromGallery();
	}
	
	public void GetImageFromCamera() {
		AndroidNative.GetImageFromCamera(AndroidNativeSettings.Instance.SaveCameraImageToGallery);
	}

	private void OnImagePickedEvent(string data) {

		Debug.Log("OnImagePickedEvent");
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		string codeString = storeData[0];
		string ImagePathInfo = storeData[1];
		string ImageData = storeData[2];

		AndroidImagePickResult result =  new AndroidImagePickResult(codeString, ImageData, ImagePathInfo);

		OnImagePicked(result);

	}

	private void ImagesPickedCallback (string data) {
		Debug.Log("[OnImagesPickedEvent]");

		string[] rawData = data.Split(new string[] {AndroidNative.DATA_SPLITTER2}, StringSplitOptions.None);
		string codeString = rawData[0];
		string imagesData = rawData[1];

		AndroidImagesPickResult result = new AndroidImagesPickResult(codeString, imagesData);
		OnImagesPicked(result);
	}

	private void OnImageSavedEvent(string data) {
		GallerySaveResult res =  new GallerySaveResult(data);
		OnImageSaved(res);
	}

	private void OnImageSaveFailedEvent(string data) {
		GallerySaveResult res =  new GallerySaveResult();
		OnImageSaved(res);
	}

	private void OnScreenshotReady(Texture2D tex) {
		SaveImageToGallery(tex, _lastImageName);

	}

	public static string GetRandomString() {
		System.Guid g = System.Guid.NewGuid();

		string GuidString = System.Convert.ToBase64String(g.ToByteArray());
		GuidString = GuidString.Replace("=","");
		GuidString = GuidString.Replace("+","");
		GuidString = GuidString.Replace("/","");

		return GuidString;
	}
}
