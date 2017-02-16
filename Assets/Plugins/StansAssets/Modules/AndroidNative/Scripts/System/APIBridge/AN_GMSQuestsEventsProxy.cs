using UnityEngine;
using System.Collections;

public class AN_GMSQuestsEventsProxy : MonoBehaviour {

	private const string CLASS_NAME = "com.androidnative.gms.core.GameClientBridge";
	
	private static void CallActivityFunction(string methodName, params object[] args) {
		AN_ProxyPool.CallStatic(CLASS_NAME, methodName, args);
	}

	// --------------------------------------
	// QUESTS And Events
	// --------------------------------------
	
	public static void sumbitEvent(string eventId, int count) {
		CallActivityFunction("sumbitEvent", eventId, count.ToString());
	}
	
	public static void loadEvents() {
		CallActivityFunction("loadEvents");
	}
	
	
	public static void showSelectedQuests(string questSelectors) {
		CallActivityFunction("showSelectedQuests", questSelectors);
	}
	
	public static void acceptQuest(string questId) {
		CallActivityFunction("acceptQuest", questId);
	}
	
	public static void loadQuests(string questSelectors, int sortOrder) {
		CallActivityFunction("loadQuests", questSelectors, sortOrder.ToString());
	}
}
