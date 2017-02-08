using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Contains GUI system dependent functions

public class WMG_GUI_Functions : WMG_Text_Functions {
	
	public void SetActive(GameObject obj, bool state) {
		obj.SetActive(state);
	}
	
	public bool activeInHierarchy(GameObject obj) {
		return obj.activeInHierarchy;
	}
	
	public void SetActiveAnchoredSprite(GameObject obj, bool state) {
		SetActive(obj, state);
	}
	
	public void SetActiveImage(GameObject obj, bool state) {
		obj.GetComponent<Image> ().enabled = state;
	}

	public Texture2D getTexture(GameObject obj) {
		return (Texture2D)obj.GetComponent<Image>().mainTexture;
	}

	public void setTexture(GameObject obj, Sprite sprite) {
		obj.GetComponent<Image>().sprite = sprite;
	}
	
	public void changeSpriteFill(GameObject obj, float fill) {
		Image theSprite = obj.GetComponent<Image>();
		theSprite.fillAmount = fill;
	}

	public void changeRadialSpriteRotation(GameObject obj, Vector3 newRot) {
		obj.transform.localEulerAngles = newRot;
	}
	
	public void changeSpriteColor(GameObject obj, Color aColor) {
		Graphic theSprite = obj.GetComponent<Graphic>();
		theSprite.color = aColor;
	}
	
	public void changeSpriteWidth(GameObject obj, int aWidth) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		if (theSprite == null) return;
		theSprite.sizeDelta = new Vector2(aWidth, theSprite.rect.height);
	}
	
	public void changeSpriteHeight(GameObject obj, int aHeight) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		if (theSprite == null) return;
		theSprite.sizeDelta = new Vector2(theSprite.rect.width, aHeight);
	}
	
	public void setTextureMaterial(GameObject obj, Material aMat) {
		Image curTex = obj.GetComponent<Image>();
		curTex.material = new Material(aMat);
	}
	
	public Material getTextureMaterial(GameObject obj) {
		Image curTex = obj.GetComponent<Image>();
		if (curTex == null) return null;
		return curTex.material;
	}
	
	public void changeSpriteSize(GameObject obj, int aWidth, int aHeight) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		if (theSprite == null) return;
		theSprite.sizeDelta = new Vector2(aWidth, aHeight);
	}

	public void changeSpriteSizeFloat(GameObject obj, float aWidth, float aHeight) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		if (theSprite == null) return;
		theSprite.sizeDelta = new Vector2(aWidth, aHeight);
	}

	public Vector2 getSpriteSize(GameObject obj) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		return theSprite.sizeDelta;
	}
	
	public void changeBarWidthHeight(GameObject obj, int aWidth, int aHeight) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		if (theSprite == null) return;
		theSprite.sizeDelta = new Vector2(aWidth, aHeight);
	}
	
	public float getSpriteWidth(GameObject obj) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		return theSprite.rect.width;
	}
	
	public float getSpriteHeight(GameObject obj) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		return theSprite.rect.height;
	}

	public void forceUpdateUI() {
		Canvas.ForceUpdateCanvases ();
	}

	public void setAnchor(GameObject go, Vector2 anchor, Vector2 pivot, Vector2 anchoredPosition) {
		RectTransform rt = go.GetComponent<RectTransform> ();
		rt.pivot = pivot;
		rt.anchorMin = anchor;
		rt.anchorMax = anchor;
		rt.anchoredPosition = anchoredPosition;
	}

	public void stretchToParent(GameObject go) {
		RectTransform rt = go.GetComponent<RectTransform> ();
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.sizeDelta = Vector2.zero;
	}

	public bool rectIntersectRect(GameObject r1, GameObject r2) {
		RectTransform rt1 = r1.GetComponent<RectTransform> ();
		Vector3[] rtCorners1 = new Vector3[4];
		rt1.GetWorldCorners (rtCorners1);
		
		RectTransform rt2 = r2.GetComponent<RectTransform> ();
		Vector3[] rtCorners2 = new Vector3[4];
		rt2.GetWorldCorners (rtCorners2);

		// If one rectangle is on left side of other
		if (rtCorners1[1].x > rtCorners2[3].x || rtCorners2[1].x > rtCorners1[3].x)
			return false;
		
		// If one rectangle is above other
		if (rtCorners1[1].y < rtCorners2[3].y || rtCorners2[1].y < rtCorners1[3].y)
			return false;
		
		return true;
	}

	public void getRectDiffs(GameObject child, GameObject container, ref Vector2 xDif, ref Vector2 yDif) {
		RectTransform rtChild = child.GetComponent<RectTransform> ();
		Vector3[] childCorners = new Vector3[4];
		rtChild.GetWorldCorners (childCorners);
		
		RectTransform rtCont = container.GetComponent<RectTransform> ();
		Vector3[] contCorners = new Vector3[4];
		rtCont.GetWorldCorners (contCorners);
		
		Vector2 minChild = new Vector2 (Mathf.Infinity, Mathf.Infinity);
		Vector2 maxChild = new Vector2 (Mathf.NegativeInfinity, Mathf.NegativeInfinity);
		Vector2 minCont = new Vector2 (Mathf.Infinity, Mathf.Infinity);
		Vector2 maxCont = new Vector2 (Mathf.NegativeInfinity, Mathf.NegativeInfinity);
		
		Graphic graphic = rtChild.GetComponent<Graphic> ();
		
		getMinMaxFromCorners (ref minChild, ref maxChild, childCorners, graphic == null ? null : graphic.canvas);
		getMinMaxFromCorners (ref minCont, ref maxCont, contCorners, graphic == null ? null : graphic.canvas);

		float scaleFactor = graphic == null ? 1 : (graphic.canvas == null ? 1 : graphic.canvas.scaleFactor);

		xDif = new Vector2 ((minChild.x - minCont.x)/scaleFactor, (maxCont.x - maxChild.x)/scaleFactor);
		yDif = new Vector2 ((minChild.y - minCont.y)/scaleFactor, (maxCont.y - maxChild.y)/scaleFactor);
	}
	
	void getMinMaxFromCorners(ref Vector2 min, ref Vector2 max, Vector3[] corners, Canvas canvas) {
		Camera cam = canvas == null ? null : canvas.worldCamera;
		if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay) cam = null;
		for (int i = 0; i < 4; i++) {
			Vector3 screenCoord = RectTransformUtility.WorldToScreenPoint(cam, corners[i]);
			if (screenCoord.x < min.x) {
				min = new Vector2 (screenCoord.x, min.y);
			}
			if (screenCoord.y < min.y) {
				min = new Vector2 (min.x, screenCoord.y);
			}
			if (screenCoord.x > max.x) {
				max = new Vector2 (screenCoord.x, max.y);
			}
			if (screenCoord.y > max.y) {
				max = new Vector2 (max.x, screenCoord.y);
			}
		}
	}
	
	public float getSpritePositionX(GameObject obj) {
		return obj.transform.localPosition.x;
	}
	
	public float getSpritePositionY(GameObject obj) {
		return obj.transform.localPosition.y;
	}

	public Vector2 getSpritePositionXY(GameObject obj) {
		return new Vector2(obj.transform.localPosition.x, obj.transform.localPosition.y);
	}
	
	public float getSpriteFactorY2(GameObject obj) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		return 1 - theSprite.pivot.y; // Top corresponds to pivot of 1, return 1 for bottom
	}

	public Vector3 getPositionRelativeTransform(GameObject obj, GameObject relative) {
		return relative.transform.InverseTransformPoint(obj.transform.TransformPoint(Vector3.zero));
	}

	public void changePositionByRelativeTransform(GameObject obj, GameObject relative, Vector2 delta) {
		obj.transform.position = relative.transform.TransformPoint(getPositionRelativeTransform(obj, relative) + new Vector3 (delta.x, delta.y, 0));
	}
	
	public void changeSpritePositionTo(GameObject obj, Vector3 newPos) {
		obj.transform.localPosition = new Vector3(newPos.x, newPos.y, newPos.z);
	}
	
	public void changeSpritePositionToX(GameObject obj, float newPos) {
		Vector3 thePos = obj.transform.localPosition;
		obj.transform.localPosition = new Vector3(newPos, thePos.y, thePos.z);
	}
	
	public void changeSpritePositionToY(GameObject obj, float newPos) {
		Vector3 thePos = obj.transform.localPosition;
		obj.transform.localPosition = new Vector3(thePos.x, newPos, thePos.z);
	}
	
	public Vector2 getChangeSpritePositionTo(GameObject obj, Vector2 newPos) {
		return new Vector2(newPos.x, newPos.y);
	}

	public void changeSpritePositionRelativeToObjBy(GameObject obj, GameObject relObj, Vector3 changeAmt) {
		Vector3 thePos = relObj.transform.localPosition;
		obj.transform.localPosition = new Vector3(thePos.x + changeAmt.x, thePos.y + changeAmt.y, thePos.z + changeAmt.z);
	}
	
	public void changeSpritePositionRelativeToObjByX(GameObject obj, GameObject relObj, float changeAmt) {
		Vector3 thePos = relObj.transform.localPosition;
		Vector3 curPos = obj.transform.localPosition;
		obj.transform.localPosition = new Vector3(thePos.x + changeAmt, curPos.y, curPos.z);
	}
	
	public void changeSpritePositionRelativeToObjByY(GameObject obj, GameObject relObj, float changeAmt) {
		Vector3 thePos = relObj.transform.localPosition;
		Vector3 curPos = obj.transform.localPosition;
		obj.transform.localPosition = new Vector3(curPos.x, thePos.y + changeAmt, curPos.z);
	}

	public Vector2 getSpritePivot(GameObject obj) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		return theSprite.pivot;
	}
	
	public void changeSpriteParent(GameObject child, GameObject parent) {
		child.transform.SetParent(parent.transform, false);
	}

	public void getFirstCanvasOnSelfOrParent(Transform trans, ref Canvas canv) {
		canv = trans.GetComponent<Canvas>();
		if (canv != null) return;
		if (trans.parent == null) return;
		getFirstCanvasOnSelfOrParent(trans.parent, ref canv);
	}

	public void addRaycaster(GameObject obj) {
		obj.AddComponent<GraphicRaycaster>();
	}

	public void setAsNotInteractible(GameObject obj) {
		CanvasGroup cg = obj.GetComponent<CanvasGroup>();
		if (cg == null) {
			cg = obj.AddComponent<CanvasGroup>();
		}
		cg.interactable = false;
		cg.blocksRaycasts = false;
	}
	
	public void bringSpriteToFront(GameObject obj) {
		obj.transform.SetAsLastSibling();
	}
	
	public void sendSpriteToBack(GameObject obj) {
		obj.transform.SetAsFirstSibling();
	}
	
	public string getDropdownSelection(GameObject obj) {
//		UIPopupList dropdown = obj.GetComponent<UIPopupList>();
//		return dropdown.value;
		return null;
	}
	
	public void setDropdownSelection(GameObject obj, string newval) {
//		UIPopupList dropdown = obj.GetComponent<UIPopupList>();
//		dropdown.value = newval;
	}
	
	public void addDropdownItem(GameObject obj, string item) {
//		UIPopupList dropdown = obj.GetComponent<UIPopupList>();
//		dropdown.items.Add(item);
	}
	
	public void deleteDropdownItem(GameObject obj) {
//		UIPopupList dropdown = obj.GetComponent<UIPopupList>();
//		dropdown.items.RemoveAt(dropdown.items.Count-1);
	}
	
	public void setDropdownIndex(GameObject obj, int index) {
//		UIPopupList dropdown = obj.GetComponent<UIPopupList>();
//		dropdown.value = dropdown.items[index];
	}
	
	public void setButtonColor(Color aColor, GameObject obj) {
//		UILabel aButton = obj.GetComponent<UILabel>();
//		aButton.color = aColor;
	}
	
	public bool getToggle(GameObject obj) {
//		UIToggle theTog = obj.GetComponent<UIToggle>();
//		return theTog.value;
		return false;
	}
	
	public void setToggle(GameObject obj, bool state) {
//		UIToggle theTog = obj.GetComponent<UIToggle>();
//		theTog.value = state;
	}
	
	public float getSliderVal(GameObject obj) {
//		UISlider theSlider = obj.GetComponent<UISlider>();
//		return theSlider.value;
		return 0;
	}
	
	public void setSliderVal(GameObject obj, float val) {
//		UISlider theSlider = obj.GetComponent<UISlider>();
//		theSlider.value = val;
	}
	
	public void showControl(GameObject obj) {
		SetActive(obj, true);
	}
	
	public void hideControl(GameObject obj) {
		SetActive(obj, false);
	}
	
	public bool getControlVisibility(GameObject obj) {
		return activeInHierarchy(obj);
	}
}
