//using UnityEditor;

using UnityEngine;
using System.Collections;



[ExecuteInEditMode]
[RequireComponent (typeof (RectTransform))]
public class SA_UIHightDependence : MonoBehaviour {

	private RectTransform _rect = null;

	public bool KeepRatioInEdiotr = false;
	public bool CaclulcateOnlyOntStart = false ;

	//[HideInInspector]
	public Rect InitialRect 	=  new Rect();

	[HideInInspector]
	public Rect InitialScreen   =  new Rect();




	void Awake() {
		if(Application.isPlaying) {
			ApplyTransformation();
		}
	}

	void Update() {
		if(!Application.isPlaying) {

			if(!KeepRatioInEdiotr) {
				InitialRect = new Rect(rect.anchoredPosition.x, rect.anchoredPosition.y, rect.rect.width, rect.rect.height);
				InitialScreen = new Rect(0, 0, Screen.width, Screen.height);
				
				rect.hideFlags = HideFlags.None;
			} else {
				ApplyTransformation();
				rect.hideFlags =  HideFlags.NotEditable;
			}

		}  else {
			if(!CaclulcateOnlyOntStart) {
				ApplyTransformation();
			}
		}

	}

	public void ApplyTransformation() {
		float ration = InitialScreen.height / InitialRect.height;
		float rectRatio = InitialRect.height / InitialRect.width;


		float h = Screen.height / ration;
		float w = h / rectRatio;


		float HRatio = InitialRect.y / InitialRect.height;
		float WRatio = InitialRect.x / InitialRect.width;

		float y = h * HRatio;
		float x = w * WRatio;


		rect.anchoredPosition =  new Vector2(x, y);


		rect.sizeDelta = new Vector2(w, h);
		
	}



	public RectTransform rect {
		get {
			if(_rect == null) {
				_rect = GetComponent<RectTransform>();
			}
			return _rect;
		}
	}

	void OnDetroy() {
		rect.hideFlags = HideFlags.None;
	}
}
