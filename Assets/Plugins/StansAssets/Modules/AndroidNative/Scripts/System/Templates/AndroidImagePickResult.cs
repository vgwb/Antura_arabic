using System;
using UnityEngine;
using System.Collections;


public class AndroidImagePickResult : AndroidActivityResult {
	
	private Texture2D _Image = null;
	private string _ImagePath = string.Empty;

	public AndroidImagePickResult(string codeString, string ImageData, string ImagePathInfo) : base("0", codeString) {

		if(ImageData.Length > 0) {
			byte[] decodedFromBase64 = System.Convert.FromBase64String(ImageData);
			_Image = new Texture2D(1, 1, TextureFormat.DXT5, false);
			_Image.LoadImage(decodedFromBase64);
		}
			
		_ImagePath = ImagePathInfo;

	}
	
	[Obsolete("image is deprecated, please use Image instead.")]
	public Texture2D image {
		get {
			return _Image;
		}
	}

	public Texture2D Image {
		get {
			return _Image;
		}
	}

	public string ImagePath {
		get {
			return _ImagePath;
		}
	}
}
