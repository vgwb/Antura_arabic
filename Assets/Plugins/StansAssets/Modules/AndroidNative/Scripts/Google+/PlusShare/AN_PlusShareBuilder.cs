using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public class AN_PlusShareBuilder {
	public event Action<SA.Common.Models.Result> OnPlusShareResult;

	private const string LISTENER_OBJECT_NAME = "AN_PlusShareListener";

	private GameObject listenerObject;
	private string message;
	private List<Texture2D> images = new List<Texture2D>(); 

	public AN_PlusShareBuilder(string text) {
		message = text;
	}

	public void AddImage(Texture2D image) {
		images.Add(image);
	}

	public void Share() {
		listenerObject = new GameObject(LISTENER_OBJECT_NAME);
		AN_PlusShareListener listener = listenerObject.AddComponent<AN_PlusShareListener>();
		listener.AttachBuilderCallback(PlusShareCallback);

		List<string> strImgs = new List<string>();
		foreach (Texture2D image in images) {
			byte[] val = image.EncodeToPNG();
			strImgs.Add(System.Convert.ToBase64String (val));
		}
		images.Clear();

		AN_SocialSharingProxy.GooglePlusShare(message, strImgs.ToArray());
	}

	private void PlusShareCallback(SA.Common.Models.Result result) {
		OnPlusShareResult(result);
		GameObject.Destroy(listenerObject);

		Debug.Log("AN_PlusShareListener was destroyed object reference equals " + listenerObject);
	} 
}
