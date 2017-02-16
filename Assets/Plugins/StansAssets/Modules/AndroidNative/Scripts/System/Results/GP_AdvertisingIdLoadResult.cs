using UnityEngine;
using System.Collections;

public class GP_AdvertisingIdLoadResult : SA.Common.Models.Result {

	public string id = string.Empty;
	public bool isLimitAdTrackingEnabled = false;


	public GP_AdvertisingIdLoadResult():base() {

	}

	public GP_AdvertisingIdLoadResult(SA.Common.Models.Error error):base(error) {
	
	} 

}
