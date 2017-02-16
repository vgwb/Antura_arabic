using UnityEngine;
using System;
using System.Collections.Generic;

public class AndroidImagesPickResult : AndroidActivityResult {

	private Dictionary<string, Texture2D> _Images = new Dictionary<string, Texture2D>();

	public AndroidImagesPickResult(string resultCode, string imagesData) : base("0", resultCode) {
		string[] rawData = imagesData.Split(new string[] {AndroidNative.DATA_SPLITTER}, StringSplitOptions.None);

		for (int i = 0; i < rawData.Length; i += 2) {
			if (!rawData[i].Equals(AndroidNative.DATA_EOF)) {
				string imagePath = rawData[i];
				string image = rawData[i + 1];

				byte[] decodedFromBase64 = System.Convert.FromBase64String(image);
				Texture2D img = new Texture2D(1, 1, TextureFormat.DXT5, false);
				img.LoadImage(decodedFromBase64);

				_Images.Add(imagePath, img);
			} else {
				break;
			}
		}
	}

	public Dictionary<string, Texture2D> Images {
		get {
			return _Images;
		}
	}
}
