using UnityEngine;
using System.Collections;

public class AN_PoupsProxy  {

	private const string CLASS_NAME = "com.androidnative.popups.PopUpsManager";
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}

	//--------------------------------------
	//  MESSAGING
	//--------------------------------------	
	
	public static void showDialog(string title, string message, AndroidDialogTheme theme = AndroidDialogTheme.ThemeDeviceDefaultDark) {
		showDialog (title, message, "Yes", "No", theme);
	}
	
	public static void showDialog(string title, string message, string yes, string no, AndroidDialogTheme theme = AndroidDialogTheme.ThemeDeviceDefaultDark) {
		CallActivityFunction("ShowDialog", title, message, yes, no, (int)theme);
	}
	
	
	public static void showMessage(string title, string message, AndroidDialogTheme theme = AndroidDialogTheme.ThemeDeviceDefaultDark) {
		showMessage (title, message, "Ok", theme);
	}	
	
	public static void showMessage(string title, string message, string ok, AndroidDialogTheme theme = AndroidDialogTheme.ThemeDeviceDefaultDark) {
		CallActivityFunction("ShowMessage", title, message, ok, (int)theme);
	}
	
	public static void OpenAppRatePage(string url) {
		CallActivityFunction("OpenAppRatingPage", url);
	}	
	
	public static void showRateDialog(string title, string message, string yes, string laiter, string no, AndroidDialogTheme theme = AndroidDialogTheme.ThemeDeviceDefaultDark) {
		CallActivityFunction("ShowRateDialog", title, message, yes, laiter, no, (int)theme);
	}
	
	public static void ShowPreloader(string title, string message, AndroidDialogTheme theme = AndroidDialogTheme.ThemeDeviceDefaultDark) {
		CallActivityFunction("ShowPreloader",  title, message, (int)theme);
	}
	
	public static void HidePreloader() {
		CallActivityFunction("HidePreloader");
	}

	public static void HideCurrentPopup() {
		CallActivityFunction("HideCurrentPopup");
	}
}
