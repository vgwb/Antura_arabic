using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AN_GrantPermissionsResult : SA.Common.Models.Result {


	private Dictionary<AN_Permission, AN_PermissionState> _RequestedPermissionsState =  new Dictionary<AN_Permission, AN_PermissionState>();

	public AN_GrantPermissionsResult(string[] permissionsList, string[] resultsList):base() {

		int index = 0;
		foreach(string permissionName in permissionsList) {
			AN_Permission p = PermissionsManager.GetPermissionByName(permissionName);

			int state = System.Convert.ToInt32(resultsList[index]); 
			_RequestedPermissionsState.Add(p, (AN_PermissionState) state);
			index++;
		}

	}

	public Dictionary<AN_Permission, AN_PermissionState> RequestedPermissionsState {
		get {
			return _RequestedPermissionsState;
		}
	}
}
