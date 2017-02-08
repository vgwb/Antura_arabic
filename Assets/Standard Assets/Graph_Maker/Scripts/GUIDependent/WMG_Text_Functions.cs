using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WMG_Text_Functions : MonoBehaviour {

	public enum WMGpivotTypes {Bottom, BottomLeft, BottomRight, Center, Left, Right, Top, TopLeft, TopRight};

	public void changeLabelText(GameObject obj, string aText) {
		Text theLabel = obj.GetComponent<Text>();
		theLabel.text = aText;
	}
	
	public void changeLabelFontSize(GameObject obj, int newFontSize) {
		Text theLabel = obj.GetComponent<Text>();
		theLabel.fontSize = newFontSize;
	}
	
	public Vector2 getTextSize (GameObject obj) {
		Text text = obj.GetComponent<Text> ();
		return new Vector2 (text.preferredWidth, text.preferredHeight);
	}

	public void changeSpritePivot(GameObject obj, WMGpivotTypes theType) {
		RectTransform theSprite = obj.GetComponent<RectTransform>();
		Text theText = obj.GetComponent<Text>();
		if (theSprite == null) return;
		if (theType == WMGpivotTypes.Bottom) {
			theSprite.pivot = new Vector2(0.5f, 0f);
			if (theText != null) theText.alignment = TextAnchor.LowerCenter;
		}
		else if (theType == WMGpivotTypes.BottomLeft) {
			theSprite.pivot = new Vector2(0f, 0f);
			if (theText != null) theText.alignment = TextAnchor.LowerLeft;
		}
		else if (theType == WMGpivotTypes.BottomRight) {
			theSprite.pivot = new Vector2(1f, 0f);
			if (theText != null) theText.alignment = TextAnchor.LowerRight;
		}
		else if (theType == WMGpivotTypes.Center) {
			theSprite.pivot = new Vector2(0.5f, 0.5f);
			if (theText != null) theText.alignment = TextAnchor.MiddleCenter;
		}
		else if (theType == WMGpivotTypes.Left) {
			theSprite.pivot = new Vector2(0f, 0.5f);
			if (theText != null) theText.alignment = TextAnchor.MiddleLeft;
		}
		else if (theType == WMGpivotTypes.Right) {
			theSprite.pivot = new Vector2(1f, 0.5f);
			if (theText != null) theText.alignment = TextAnchor.MiddleRight;
		}
		else if (theType == WMGpivotTypes.Top) {
			theSprite.pivot = new Vector2(0.5f, 1f);
			if (theText != null) theText.alignment = TextAnchor.UpperCenter;
		}
		else if (theType == WMGpivotTypes.TopLeft) {
			theSprite.pivot = new Vector2(0f, 1f);
			if (theText != null) theText.alignment = TextAnchor.UpperLeft;
		}
		else if (theType == WMGpivotTypes.TopRight) {
			theSprite.pivot = new Vector2(1f, 1f);
			if (theText != null) theText.alignment = TextAnchor.UpperRight;
		}
	}

	public void changeLabelColor(GameObject obj, Color newColor) {
		Text theLabel = obj.GetComponent<Text>();
		theLabel.color = newColor;
	}

	public void changeLabelFontStyle(GameObject obj, FontStyle newFontStyle) {
		Text theLabel = obj.GetComponent<Text>();
		theLabel.fontStyle = newFontStyle;
	}

	public void changeLabelFont(GameObject obj, Font newFont) {
		Text theLabel = obj.GetComponent<Text>();
		theLabel.font = newFont;
	}

}
