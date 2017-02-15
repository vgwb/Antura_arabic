using UnityEngine;
using System.Collections;

public class GallerySaveResult : SA.Common.Models.Result {

	private string _imagePath;

	public GallerySaveResult(string path):base() {
		_imagePath = path;
	}

	public GallerySaveResult(): base(new SA.Common.Models.Error()) {}


	public string imagePath {
		get {
			return _imagePath;
		}
	}
}
