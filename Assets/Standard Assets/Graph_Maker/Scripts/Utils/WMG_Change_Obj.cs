using UnityEngine;
using System.Collections;

public class WMG_Change_Obj {

	public bool changesPaused;
	public bool changePaused;
	public delegate void ObjChangedHandler();
	public event ObjChangedHandler OnChange;

	public void Changed() {
		ObjChangedHandler handler = OnChange;
		if (handler != null) {
			if (changeOk()) {
//				System.Diagnostics.StackFrame callingFrame = new System.Diagnostics.StackTrace(true).GetFrame(1);
//				Debug.Log(handler.Method + " - " + callingFrame.GetMethod() + " - " + callingFrame.GetMethod().ReflectedType + ": " + callingFrame.GetFileLineNumber());
				handler();
			}
		}
	}
	
	bool changeOk() {
		if (!Application.isPlaying) return false;
		if (changesPaused) {
			changePaused = true;
			return false;
		}
		return true;
	}

	public void UnsubscribeAllHandlers()
	{
		if(OnChange != null)
		{
			foreach(System.Delegate d in OnChange.GetInvocationList())
			{
				OnChange -= (d as ObjChangedHandler);
			}
		}
	} //end method

}
