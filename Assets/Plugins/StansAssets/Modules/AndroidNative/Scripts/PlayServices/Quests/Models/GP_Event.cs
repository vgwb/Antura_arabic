using UnityEngine;
using System.Collections;

public class GP_Event {


	public string Id;
	public string Description;

	public string IconImageUrl;
	public string FormattedValue;
	
	public long Value;
	
	private Texture2D _icon = null;




	public void LoadIcon() {
		if(icon != null) {
			return;
		}

		SA.Common.Util.Loader.LoadWebTexture(IconImageUrl, OnTextureLoaded);
	}



	public Texture2D icon {
		get {
			return _icon;
		}
	}



	private void OnTextureLoaded (Texture2D tex) {
		if(this == null) {return;}
		_icon = tex;
	}
}
