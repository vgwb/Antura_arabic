using UnityEngine;
using System.Collections;

public enum GADInAppResolution  {

	RESOLUTION_SUCCESS = 0,        // Purchase was a success
	RESOLUTION_FAILURE = 1,         // Error while processing purchase
	RESOLUTION_INVALID_PRODUCT = 2, // Error while looking up product
	RESOLUTION_CANCELLED = 3       // Purchase was cancelled by user
}

