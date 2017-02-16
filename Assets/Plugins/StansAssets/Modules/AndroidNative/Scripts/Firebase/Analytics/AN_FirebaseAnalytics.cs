using UnityEngine;
using System.Text;
using System.Collections.Generic;

public static class FirebaseAnalytics {

	private const string SEPARATOR1 = "|%|";
	private const string SEPARATOR2 = "|";

	public static void Init () {
		FirebaseProxy.Init ();
	}

	public static void SetEnabled (bool enabled) {
		FirebaseProxy.SetEnabled (enabled);
	}

	public static void SetMinimumSessionDuration(long milliseconds) {
		FirebaseProxy.SetMinimumSessionDuration (milliseconds);
	}

	public static void SetSessionTimeoutDuration(long milliseconds) {
		FirebaseProxy.SetSessionTimeoutDuration (milliseconds);
	}

	public static void SetUserId(string userId) {
		FirebaseProxy.SetUserId (userId);
	}

	public static void SetUserProperty(string name, string value) {
		FirebaseProxy.SetUserProperty (name, value);
	}

	public static void LogEvent(string name, Dictionary<string, object> data = null) {
		if (data == null || data.Count == 0) {
			FirebaseProxy.LogEvent (name, null);
		} else {			
			Dictionary<string, object>.Enumerator enumerator = data.GetEnumerator ();
			enumerator.MoveNext ();

			StringBuilder builder = new StringBuilder ();
			builder.Append (enumerator.Current.Key);
			builder.Append (SEPARATOR2);
			builder.Append (enumerator.Current.Value.ToString ());

			while (enumerator.MoveNext ()) {
				builder.Append (SEPARATOR1);
				builder.Append (enumerator.Current.Key);
				builder.Append (SEPARATOR2);
				builder.Append (enumerator.Current.Value.ToString ());
			}
			FirebaseProxy.LogEvent (name, builder.ToString ());
		}
	}
}
