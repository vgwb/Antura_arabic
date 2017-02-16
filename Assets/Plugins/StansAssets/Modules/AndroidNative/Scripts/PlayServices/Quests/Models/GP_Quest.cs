using UnityEngine;
using System.Collections;

public class GP_Quest {

	public string Id;
	public string Name;
	public string Description;

	public string IconImageUrl;
	public string BannerImageUrl;

	public GP_QuestState state;

	public long LastUpdatedTimestamp;
	public long AcceptedTimestamp;
	public long EndTimestamp;

	public byte[] RewardData;

	public long CurrentProgress;
	public long TargetProgress;

	private Texture2D _icon = null;
	private Texture2D _banner = null;


	public void LoadIcon() {
		if(icon != null) {
			return;
		}
		
		SA.Common.Util.Loader.LoadWebTexture(IconImageUrl, OnIconLoaded);
	}

	public void LoadBanner() {
		if(icon != null) {
			return;
		}
		
		SA.Common.Util.Loader.LoadWebTexture(BannerImageUrl, OnBannerLoaded);
	}
	
	
	
	public Texture2D icon {
		get {
			return _icon;
		}
	}
	

	
	public Texture2D banner {
		get {
			return _banner;
		}
	}
	
	

	private void OnBannerLoaded (Texture2D tex) {
		if(this == null) {return;}
		_banner = tex;
	}
	


	private void OnIconLoaded (Texture2D tex) {
		if(this == null) {return;}
		_icon = tex;
	}
}
