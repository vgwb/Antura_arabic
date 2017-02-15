////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


 


public class GooglePlayGiftRequestResult {


	private GP_GamesActivityResultCodes _code;



	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	public GooglePlayGiftRequestResult (string r_code) {
		_code = (GP_GamesActivityResultCodes) System.Convert.ToInt32(r_code);

	}

	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public GP_GamesActivityResultCodes code {
		get {
			return _code;
		}
	}
	
	public bool isSuccess  {
		get {
			return _code == GP_GamesActivityResultCodes.RESULT_OK;
		}
	}

	public bool isFailure {
		get {
			return !isSuccess;
		}
	}


		 
}