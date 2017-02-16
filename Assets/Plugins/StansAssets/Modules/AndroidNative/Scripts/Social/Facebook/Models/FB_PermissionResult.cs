using UnityEngine;
using System.Collections.Generic;

public class FB_PermissionResult : FB_Result {

	private Dictionary<string, FB_Permission> _Permissions = null;

	public FB_PermissionResult(string RawData, string Error) : base(RawData, Error) {}

	public void SetPermissions(Dictionary<string, FB_Permission> permissions) {
		_Permissions = permissions;
	}

	public Dictionary<string, FB_Permission> Permissions {
		get {
			if (_Permissions == null) {
				return new Dictionary<string, FB_Permission>();
			}
			return _Permissions;
		}
	}
}
