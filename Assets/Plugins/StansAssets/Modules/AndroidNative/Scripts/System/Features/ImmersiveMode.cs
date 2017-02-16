using UnityEngine;
using System.Collections;

public class ImmersiveMode : SA.Common.Pattern.Singleton<ImmersiveMode> {


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	public void EnableImmersiveMode()  {
		AN_ImmersiveModeProxy.enableImmersiveMode();
	}
	

}
