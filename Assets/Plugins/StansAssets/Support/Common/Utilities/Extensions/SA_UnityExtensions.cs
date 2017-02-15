////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;


public static class SA_UnityExtensions  {

	//--------------------------------------
	// GameObject
	//--------------------------------------


	public static void MoveTo(this GameObject go, Vector3 position, float time, SA.Common.Animation.EaseType easeType = SA.Common.Animation.EaseType.linear, System.Action OnCompleteAction = null ) {
		SA.Common.Animation.ValuesTween tw = go.AddComponent<SA.Common.Animation.ValuesTween>();

		tw.DestoryGameObjectOnComplete = false;
		tw.VectorTo(go.transform.position, position, time, easeType);

		tw.OnComplete += OnCompleteAction;
	}


	public static void ScaleTo(this GameObject go, Vector3 scale, float time, SA.Common.Animation.EaseType easeType = SA.Common.Animation.EaseType.linear, System.Action OnCompleteAction = null ) {
		SA.Common.Animation.ValuesTween tw = go.AddComponent<SA.Common.Animation.ValuesTween>();

		tw.DestoryGameObjectOnComplete = false;
		tw.ScaleTo(go.transform.localScale, scale, time, easeType);

		tw.OnComplete += OnCompleteAction;
	}

	//--------------------------------------
	// Texture2D
	//--------------------------------------

	public static Sprite ToSprite(this Texture2D texture) {
		return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f)); 
	}

	//--------------------------------------
	// Vector3
	//--------------------------------------

	public static Vector3 ResetXCoord(this Vector3 vec) {
		Vector3 newVec = vec;
		newVec.x = 0f;

		return newVec;
	}

	public static Vector3 ResetYCoord(this Vector3 vec) {
		Vector3 newVec = vec;
		newVec.y = 0f;

		return newVec;
	}

	public static Vector3 ResetZCoord(this Vector3 vec) {
		Vector3 newVec = vec;
		newVec.z = 0f;

		return newVec;
	}



		

}


