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

	public void changeSpriteAlpha(GameObject obj, float alpha) {
		Graphic theSprite = obj.GetComponent<Graphic>();
		theSprite.color = new Color (theSprite.color.r, theSprite.color.g, theSprite.color.b, alpha);
	}

	public float getSpriteAlpha(GameObject obj) {
		Graphic theSprite = obj.GetComponent<Graphic>();
		return theSprite.color.a;
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
//		bool isInvisiblePoint = (aWidth == 0 || aHeight == 0);
//		if (isInvisiblePoint) {
//			theSprite.sizeDelta = new Vector2 (1, 1);
//			SetActive (obj, false);
//		} else {
		theSprite.sizeDelta = new Vector2 (aWidth, aHeight);
//		}
	}
	
	public float getSpriteWidth(GameObject obj) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		return theSprite.rect.width;
	}

	public float getSpriteHeight(GameObject obj) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		return theSprite.rect.height;
	}

	//http://answers.unity3d.com/questions/921726/how-to-get-the-size-of-a-unityengineuitext-for-whi.html
	public float getTextWidth(GameObject obj) {
		Text textComp = obj.GetComponent<Text> ();
		return textComp.cachedTextGeneratorForLayout.GetPreferredWidth (
			textComp.text, textComp.GetGenerationSettings (textComp.GetComponent<RectTransform> ().rect.size));
	}
	//http://answers.unity3d.com/questions/921726/how-to-get-the-size-of-a-unityengineuitext-for-whi.html
	public float getTextHeight(GameObject obj) {
		Text textComp = obj.GetComponent<Text> ();
		return textComp.cachedTextGeneratorForLayout.GetPreferredHeight (
			textComp.text, textComp.GetGenerationSettings (textComp.GetComponent<RectTransform> ().rect.size));
	}

	public void forceUpdateText(GameObject obj) {
		changeSpriteSizeFloat (obj, getTextWidth (obj), getTextHeight (obj));
	}

	public void setAnchor(GameObject go, Vector2 anchor, Vector2 pivot, Vector2 anchoredPosition) {
		RectTransform rt = go.GetComponent<RectTransform> ();
		rt.pivot = pivot;
		rt.anchorMin = anchor;
		rt.anchorMax = anchor;
		rt.anchoredPosition = anchoredPosition;
	}

	public void setAnchoredPosition(GameObject go, Vector2 anchoredPosition) {
		RectTransform rt = go.GetComponent<RectTransform> ();
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
		
		Graphic graphic = rtChild.GetComponent<Graphic> ();
		Canvas canvasRt = graphic.canvas;
		
		if (canvasRt != null) {
			// transform to canvas space
			for (int i = 0; i < 4; i++) {
				childCorners [i] = canvasRt.transform.InverseTransformPoint (childCorners [i]);
				contCorners [i] = canvasRt.transform.InverseTransformPoint (contCorners [i]);
			}
		}
		
		Vector2 minChild = new Vector2 (Mathf.Infinity, Mathf.Infinity);
		Vector2 maxChild = new Vector2 (Mathf.NegativeInfinity, Mathf.NegativeInfinity);
		Vector2 minCont = new Vector2 (Mathf.Infinity, Mathf.Infinity);
		Vector2 maxCont = new Vector2 (Mathf.NegativeInfinity, Mathf.NegativeInfinity);
		
		getMinMaxFromCorners (ref minChild, ref maxChild, childCorners);
		getMinMaxFromCorners (ref minCont, ref maxCont, contCorners);
		
		xDif = new Vector2 ((minChild.x - minCont.x), (maxCont.x - maxChild.x));
		yDif = new Vector2 ((minChild.y - minCont.y), (maxCont.y - maxChild.y));
	}
	
	void getMinMaxFromCorners(ref Vector2 min, ref Vector2 max, Vector3[] corners) {
		for (int i = 0; i < 4; i++) {
			if (corners[i].x < min.x) {
				min = new Vector2 (corners[i].x, min.y);
			}
			if (corners[i].y < min.y) {
				min = new Vector2 (min.x, corners[i].y);
			}
			if (corners[i].x > max.x) {
				max = new Vector2 (corners[i].x, max.y);
			}
			if (corners[i].y > max.y) {
				max = new Vector2 (max.x, corners[i].y);
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

	public float getSpritePivotTopToBot(GameObject obj) {
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
}
