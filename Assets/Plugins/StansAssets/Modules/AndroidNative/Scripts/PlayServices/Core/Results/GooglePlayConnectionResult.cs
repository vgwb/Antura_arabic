using UnityEngine;
using System.Collections;

public class GooglePlayConnectionResult  {


	//result code
	public GP_ConnectionResultCode code;

	//if result has resolution connection resolution intent wil be started
	public bool HasResolution;

	//true if connection succsesful
	public bool IsSuccess {
		get {
			if(code == GP_ConnectionResultCode.SUCCESS) {
				return true;
			} else {
				return false;
			}
		}
	}


}

