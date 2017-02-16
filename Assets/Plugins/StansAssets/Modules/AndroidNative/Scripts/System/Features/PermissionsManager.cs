using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PermissionsManager : SA.Common.Pattern.Singleton<PermissionsManager> {
	
	private const string PM_CLASS_NAME = "com.androidnative.features.permissions.PermissionsManager";
	
	public static event Action<AN_GrantPermissionsResult>  ActionPermissionsRequestCompleted = delegate {} ;


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public static bool IsPermissionGranted(AN_Permission permission) {
		return IsPermissionGranted(permission.GetFullName());
	}

	public static bool IsPermissionGranted(string permission) {
		#if UNITY_ANDROID
		bool val =  AN_ProxyPool.CallStatic<bool>(PM_CLASS_NAME, "checkForPermission", permission);
		return val;
		#else
		return false;
		#endif
	}

    public static bool ShouldShowRequestPermission(AN_Permission permission)
    {
#if UNITY_ANDROID
        bool val = AN_ProxyPool.CallStatic<bool>(PM_CLASS_NAME, "shouldShowRequestPermissionRationale", permission.GetFullName());
        return val;
#else
		return false;
#endif
    }

    public void RequestPermissions(params AN_Permission[] permissions) {

		List<string> serializedPerms =  new List<string>();

		foreach(AN_Permission p  in permissions) {
			serializedPerms.Add(p.GetFullName());
		}

		RequestPermissions(serializedPerms.ToArray());
	}


	public void RequestPermissions(params string[] permissions) {
		AN_ProxyPool.CallStatic(PM_CLASS_NAME, "requestPermissions", AndroidNative.ArrayToString(permissions));
	}



	private void OnPermissionsResult(string data) {

		Debug.Log("OnPermissionsResult:" + data);


		string[] dataArray = data.Split(new string[] { AndroidNative.DATA_SPLITTER2 }, StringSplitOptions.None); 

		string[] permissionsList = AndroidNative.StringToArray(dataArray[0]);
		string[] resultsList = AndroidNative.StringToArray(dataArray[1]);

		foreach(string s in permissionsList)  {
			Debug.Log(s);
		}

		foreach(string s in resultsList)  {
			Debug.Log(s);
		}

		AN_GrantPermissionsResult result =  new AN_GrantPermissionsResult(permissionsList, resultsList);

		ActionPermissionsRequestCompleted(result);

	}


	public static AN_Permission GetPermissionByName(string fullName) {
		
		foreach( AN_Permission val in System.Enum.GetValues(typeof(AN_Permission)) ) {
			
			if(val.GetFullName().Equals(fullName)) {
				return val;
			}
		}
		
		return AN_Permission.UNDEFINED;
	}









	
}
