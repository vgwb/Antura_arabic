using UnityEngine;
using System.Collections;

public class AndroidActivityResult {

	protected AdroidActivityResultCodes _code;
	protected int _requestId;


	public AndroidActivityResult(string rId, string codeString) {
		_requestId = System.Convert.ToInt32(rId);
		_code = (AdroidActivityResultCodes) System.Convert.ToInt32(codeString);
	}


	public AdroidActivityResultCodes code {
		get {
			return _code;
		}
	}

	public int requestId {
		get {
			return _requestId;
		}
	}

	public bool IsSucceeded {
		get{
			if(code == AdroidActivityResultCodes.RESULT_OK) {
				return true;
			} else {
				return false;
			}
		}
	}

	public bool IsFailed {
		get{
			return !IsSucceeded;
		}
	}
}
