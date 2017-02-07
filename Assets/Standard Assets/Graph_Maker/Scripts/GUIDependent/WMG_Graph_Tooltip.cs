using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Contains GUI system dependent functions

public class WMG_Graph_Tooltip : WMG_GUI_Functions {

	public delegate string TooltipLabeler(WMG_Series series, WMG_Node node);
	public TooltipLabeler tooltipLabeler;
	
	public WMG_Axis_Graph theGraph;

	Canvas _canvas;
	GameObject currentObj;
	CanvasGroup _cg;
	
	void Start() {
		_canvas = theGraph.toolTipPanel.GetComponent<Graphic>().canvas;
		_cg = theGraph.toolTipPanel.GetComponent<CanvasGroup>();
		if (!_cg) {
			_cg = theGraph.toolTipPanel.AddComponent<CanvasGroup>();
		}
	}

	void Update () {
		if (theGraph.tooltipEnabled) {
			if (isTooltipObjectNull()) return;
			if(getControlVisibility(theGraph.toolTipPanel)) {
				if (currentObj && !activeInHierarchy(currentObj)) { // otherwise if deactivate gameobject under mouse, then the mouse exit event never called
					MouseExitCommon(currentObj);
					return;
				}
				repositionTooltip();
			}
		}
	}

	public void subscribeToEvents(bool val) {
		if (val) {
			theGraph.WMG_MouseEnter += TooltipNodeMouseEnter;
			theGraph.WMG_MouseEnter_Leg += TooltipLegendNodeMouseEnter;
			theGraph.WMG_Link_MouseEnter_Leg += TooltipLegendLinkMouseEnter;
			tooltipLabeler = defaultTooltipLabeler;
		}
		else {
			theGraph.WMG_MouseEnter -= TooltipNodeMouseEnter;
			theGraph.WMG_MouseEnter_Leg -= TooltipLegendNodeMouseEnter;
			theGraph.WMG_Link_MouseEnter_Leg -= TooltipLegendLinkMouseEnter;
		}
	}
	
	private bool isTooltipObjectNull() {
		if (theGraph.toolTipPanel == null) return true;
		if (theGraph.toolTipLabel == null) return true;
		return false;
	}
	
	private void repositionTooltip() {
		// This is called continuously during update if control is visible, and also once before shown visible so tooltip doesn't appear to jump positions
		// Convert position from "screen coordinates" to "gui coordinates"

		Vector3 position;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(theGraph.toolTipPanel.GetComponent<RectTransform>(), 
		                                                        new Vector2(Input.mousePosition.x, Input.mousePosition.y),
		                                                        (_canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera),
		                                                        out position);
		// Without offset, the tooltip's bottom left corner will be at the cursor position
		float offsetX = theGraph.tooltipOffset.x;
		float offsetY = theGraph.tooltipOffset.y;
		// Center the control on the mouse/touch
		theGraph.toolTipPanel.transform.localPosition = theGraph.toolTipPanel.transform.parent.InverseTransformPoint(position) + new Vector3( offsetX, offsetY + getSpriteHeight(theGraph.toolTipPanel)/2f, 0);

		EnsureTooltipStaysOnScreen(position, offsetX, offsetY);
	}

	void EnsureTooltipStaysOnScreen(Vector3 position, float offsetX, float offsetY) {
		Vector3 newPos = theGraph.toolTipPanel.transform.position;
		
		offsetX *= _canvas.transform.localScale.x;
		offsetY *= _canvas.transform.localScale.y;
		
		Vector3[] corners = new Vector3[4];
		((RectTransform) theGraph.toolTipPanel.transform).GetWorldCorners(corners);
		var width = corners[2].x - corners[0].x;
		var height = corners[1].y - corners[0].y;
		
		Vector3 screenBotLeft;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvas.GetComponent<RectTransform>(), 
		                                                        new Vector2(0, 0),
		                                                        (_canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera),
		                                                        out screenBotLeft);
		Vector3 screenTopRight;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvas.GetComponent<RectTransform>(), 
		                                                        new Vector2(Screen.width, Screen.height),
		                                                        (_canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera),
		                                                        out screenTopRight);
		
		float distPastX = position.x + offsetX + width - (screenTopRight.x - screenBotLeft.x);
		if (distPastX > screenBotLeft.x) { // passed right edge
			newPos = new Vector3(position.x - distPastX + screenBotLeft.x + offsetX, newPos.y, newPos.z);
		}
		else {
			distPastX = position.x + offsetX;
			if (distPastX < screenBotLeft.x) { // passed left edge
				newPos = new Vector3(position.x - distPastX + screenBotLeft.x + offsetX, newPos.y, newPos.z);
			}
		}
		float distPastY = position.y + offsetY + height - (screenTopRight.y - screenBotLeft.y);
		if (distPastY > screenBotLeft.y) { // passed top edge
			newPos = new Vector3(newPos.x, position.y - distPastY + screenBotLeft.y + offsetY + height/2f, newPos.z);
		}
		else {
			distPastY = position.y + offsetY;
			if (distPastY < screenBotLeft.y) { // passed bottom edge
				newPos = new Vector3(newPos.x, position.y - distPastY + screenBotLeft.y + offsetY + height/2f, newPos.z);
			}
		}
		
		theGraph.toolTipPanel.transform.position = newPos;
	}

	private string defaultTooltipLabeler(WMG_Series aSeries, WMG_Node aNode) {
		// Find out the point value data for this node
		Vector2 nodeData = aSeries.getNodeValue(aNode);
		float numberToMult = Mathf.Pow(10f, aSeries.theGraph.tooltipNumberDecimals);
		string nodeX = (Mathf.Round(nodeData.x*numberToMult)/numberToMult).ToString();
		string nodeY = (Mathf.Round(nodeData.y*numberToMult)/numberToMult).ToString();
		
		// Determine the tooltip text to display
		string textToSet;
		if (aSeries.seriesIsLine) {
			textToSet = "(" + nodeX + ", " + nodeY + ")";
		}
		else {
			textToSet = nodeY;
		}
		if (aSeries.theGraph.tooltipDisplaySeriesName) {
			textToSet = aSeries.seriesName + ": " + textToSet;
		}
		return textToSet;
	}

	IEnumerator delayedTooltipActive() { // otherwise tooltip dimensions not updated till frame after, toggle alpha using canvas group
		showControl(theGraph.toolTipPanel);
		bringSpriteToFront(theGraph.toolTipPanel);
		_cg.alpha = 0;
		yield return new WaitForEndOfFrame();
		_cg.alpha = 1;
	}

	private void MouseEnterCommon(string textToSet, GameObject objToAnimate, Vector3 animTo) {
		currentObj = objToAnimate;

		// Set the text
		changeLabelText(theGraph.toolTipLabel, textToSet);
		
		// Resize this control to match the size of the contents
		changeSpriteWidth(theGraph.toolTipPanel, Mathf.RoundToInt(getSpriteWidth(theGraph.toolTipLabel)) + 24);
		
		// Ensure tooltip is in position before showing it so it doesn't appear to jump
		repositionTooltip();

		StartCoroutine(delayedTooltipActive());
		
		performTooltipAnimation(objToAnimate.transform, animTo);
	}

	private void MouseExitCommon(GameObject objToAnimate) {
		hideControl(theGraph.toolTipPanel);
		sendSpriteToBack(theGraph.toolTipPanel);
		
		performTooltipAnimation(objToAnimate.transform, Vector3.one);
	}

	private void TooltipNodeMouseEnter(WMG_Series aSeries, WMG_Node aNode, bool state) {
		if (isTooltipObjectNull()) return;
		if (state) {
			Vector3 newVec = new Vector3(2,2,1);
			if (!aSeries.seriesIsLine) {
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
					newVec = new Vector3(1,1.1f,1);
				}
				else {
					newVec = new Vector3(1.1f,1,1);
				}
			}

			MouseEnterCommon(tooltipLabeler(aSeries, aNode), aNode.gameObject, newVec);
		}
		else {
			MouseExitCommon(aNode.gameObject);
		}
	}
	
	private void TooltipLegendNodeMouseEnter(WMG_Series aSeries, WMG_Node aNode, bool state) {
		if (isTooltipObjectNull()) return;
		if (state) {
			MouseEnterCommon(aSeries.seriesName, aNode.gameObject, new Vector3(2,2,1));
		}
		else {
			MouseExitCommon(aNode.gameObject);
		}
	}
	
	private void TooltipLegendLinkMouseEnter(WMG_Series aSeries, WMG_Link aLink, bool state) {
		if (isTooltipObjectNull()) return;
		if (!aSeries.hidePoints) return;
		if (state) {
			MouseEnterCommon(aSeries.seriesName, aLink.gameObject, new Vector3(2,1.05f,1));
		}
		else {
			MouseExitCommon(aLink.gameObject);
		}
	}
	
	private void performTooltipAnimation (Transform trans, Vector3 newScale) {
		if (theGraph.tooltipAnimationsEnabled) {
			WMG_Anim.animScale(trans.gameObject, theGraph.tooltipAnimationsDuration, theGraph.tooltipAnimationsEasetype, newScale, 0);
		}
	}
}
