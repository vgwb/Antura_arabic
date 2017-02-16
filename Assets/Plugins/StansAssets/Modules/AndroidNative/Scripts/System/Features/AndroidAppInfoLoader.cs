using UnityEngine;
using System;
using System.Collections;

public class AndroidAppInfoLoader : SA.Common.Pattern.Singleton<AndroidAppInfoLoader> {
	
	public static event Action<PackageAppInfo> ActionPacakgeInfoLoaded = delegate{};
	
	public PackageAppInfo PacakgeInfo =  new PackageAppInfo();


	public void LoadPackageInfo() {
		AndroidNative.LoadPackageInfo();
	}


	private void OnPackageInfoLoaded(string data) {
		string[] appData;
		appData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		PacakgeInfo.versionName			 = appData[0];
		PacakgeInfo.versionCode 		 = appData[1];
		PacakgeInfo.packageName 		 = appData[2];
		PacakgeInfo.lastUpdateTime 		 = System.Convert.ToInt64(appData[3]);
		PacakgeInfo.sharedUserId 		 = appData[3];
		PacakgeInfo.sharedUserLabel 	 = appData[4];

		ActionPacakgeInfoLoaded(PacakgeInfo);

	}

}
