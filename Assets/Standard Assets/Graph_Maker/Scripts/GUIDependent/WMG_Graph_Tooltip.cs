using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// The tooltip for axis graphs when WMG_Axis_Graph::tooltipEnabled = true.
/// </summary>
public class WMG_Graph_Tooltip : WMG_GUI_Functions {

	public delegate string TooltipLabeler(WMG_Series series, WMG_Node node);
	/// <summary>
	/// Use to override the default labeler for the graph tooltip label to put for example a dollar sign for the label.
	/// @code
	/// graph.theTooltip.tooltipLabeler = customTooltipLabeler;
	/// string customTooltipLabeler(WMG_Series aSeries, WMG_Node aNode) {}
	/// @endcode
	/// </summary>
	public TooltipLabeler tooltipLabeler;

	/// <summary>
	/// The graph for this tooltip.
	/// </summary>
	public WMG_Axis_Graph theGraph;

	Canvas _canvas;
	GameObject currentObj;
	CanvasGroup _cg;

	/// <summary>
	/// Sets tool tip references, should not be called outside of Graph Maker code.
	/// </summary>
	public void SetToolTipRefs() {
		_canvas = theGraph.toolTipPanel.GetComponent<Graphic>().canvas;
		_cg = theGraph.toolTipPanel.GetComponent<CanvasGroup>();
		if (!_cg) {
			_cg = theGraph.toolTipPanel.AddComponent<CanvasGroup>();
		}
		// tooltip should not be interactable / block raycasts in case the tooltip is positioned over the cursor
		_cg.interactable = false;
		_cg.blocksRaycasts = false;
	}

	/// <summary>
	/// Sets a different tooltip object that you specify, see example X_WorldSpace scene code for example usage.
	/// </summary>
	/// <param name="toolTipPanel">Tool tip panel.</param>
	/// <param name="toolTipLabel">Tool tip label.</param>
	public void SetTooltipObject(GameObject toolTipPanel, GameObject toolTipLabel) {
		theGraph.toolTipPanel = toolTipPanel;
		theGraph.toolTipLabel = toolTipLabel;
		SetToolTipRefs ();
	}

	void Update () {
		if (theGraph.tooltipEnabled) {
			if (isTooltipObjectNull()) return;
			if(activeInHierarchy(theGraph.toolTipPanel)) {
				if (currentObj && !activeInHierarchy(currentObj)) { // otherwise if deactivate gameobject under mouse, then the mouse exit event never called
					MouseExitCommon();
					performTooltipAnimation (currentObj.transform, Vector3.one, theGraph.tooltipAnimationsDuration, 0, theGraph.tooltipAnimationsEasetype);
					return;
				}
				repositionTooltip();
			}
		}
	}

	void OnDisable() {
		if (theGraph.tooltipEnabled) {
			if (isTooltipObjectNull()) return;
			if (currentObj) {
				MouseExitCommon();
				currentObj.transform.localScale = Vector3.one;
				return;
			}
		}
	}

	/// <summary>
	/// Subcscribes to click and hover events, should not be used outside of Graph Maker code.
	/// </summary>
	/// <param name="val">If set to <c>true</c> value.</param>
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
	
	bool isTooltipObjectNull() {
		if (theGraph.toolTipPanel == null) return true;
		if (theGraph.toolTipLabel == null) return true;
		return false;
	}
	
	void repositionTooltip() {
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
		// Use special getTextHeight API since text label is set on same frame, and height not updated on RectTransform until end of frame
		theGraph.toolTipPanel.transform.localPosition = theGraph.toolTipPanel.transform.parent.InverseTransformPoint(position) 
			+ new Vector3( offsetX, offsetY, 0);

		EnsureTooltipStaysOnScreen();
	}

	void EnsureTooltipStaysOnScreen() {
		// get world space corners of tooltip
		Vector3[] corners = new Vector3[4];
		((RectTransform) theGraph.toolTipPanel.transform).GetWorldCorners(corners);
		// convert to screen space
		Vector2[] ssCorners = new Vector2[4];
		ssCorners[0] = RectTransformUtility.WorldToScreenPoint(_canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera, corners[0]);
		ssCorners[1] = RectTransformUtility.WorldToScreenPoint(_canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera, corners[1]);
		ssCorners[2] = RectTransformUtility.WorldToScreenPoint(_canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera, corners[2]);
		ssCorners[3] = RectTransformUtility.WorldToScreenPoint(_canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera, corners[3]);

		// calculate min and max of screen space corners so we can measure how far (if at all) it exceeds screen. 
		// Can be rotated in 3d space, so need to use all 4 corners
		float minX = Mathf.Infinity;
		float minY = Mathf.Infinity;
		float maxX = Mathf.NegativeInfinity;
		float maxY = Mathf.NegativeInfinity;
		for (int i = 0; i < ssCorners.Length; i++) {
			if (ssCorners [i].x < minX) {
				minX = ssCorners [i].x;
			}
			if (ssCorners [i].x > maxX) {
				maxX = ssCorners [i].x;
			}
			if (ssCorners [i].y < minY) {
				minY = ssCorners [i].y;
			}
			if (ssCorners [i].y > maxY) {
				maxY = ssCorners [i].y;
			}
		}

		// calculate screen space positional offsets to apply
		float offsetX = 0;
		float offsetY = 0;

		if (maxX > Screen.width) { // exceeded screen right edge
			offsetX = maxX - Screen.width;
		} else if (minX < 0) { // exceeded screen left edge
			offsetX = minX;
		}

		if (maxY > Screen.height) { // exceeded screen top edge
			offsetY = maxY - Screen.height;
		} else if (minY < 0) { // exceeded screen bottom edge
			offsetY = minY;
		}

		// one of the offsets is not zero, so change the tooltip position
		if (offsetX != 0 || offsetY != 0) {
			Vector2 newScreenPos = ssCorners[0] - new Vector2 (offsetX, offsetY);

			Vector3 newWorldPos;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvas.GetComponent<RectTransform>(), 
				newScreenPos,
				(_canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera),
				out newWorldPos);

			theGraph.toolTipPanel.transform.position = newWorldPos;
		}
	}

	string defaultTooltipLabeler(WMG_Series aSeries, WMG_Node aNode) {
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

	void MouseEnterCommon(string textToSet, GameObject objUnderMouse) {
		currentObj = objUnderMouse;

		// Set the text
		changeLabelText(theGraph.toolTipLabel, textToSet);

		// Ensure tooltip is in position before showing it so it doesn't appear to jump
		repositionTooltip();

		SetActive(theGraph.toolTipPanel, true);
		bringSpriteToFront(theGraph.toolTipPanel);
	}

	void MouseExitCommon() {
		SetActive(theGraph.toolTipPanel, false);
		sendSpriteToBack(theGraph.toolTipPanel);
	}

	void TooltipNodeMouseEnter(WMG_Series aSeries, WMG_Node aNode, bool state) {
		if (isTooltipObjectNull()) return;
		if (state) {
			MouseEnterCommon(tooltipLabeler(aSeries, aNode), aNode.gameObject);
		}
		else {
			MouseExitCommon();
		}
		aSeries.tooltipPointAnimator (aSeries, aNode, state);
	}
	
	void TooltipLegendNodeMouseEnter(WMG_Series aSeries, WMG_Node aNode, bool state) {
		if (isTooltipObjectNull()) return;
		if (state) {
			MouseEnterCommon(aSeries.seriesName, aNode.gameObject);
			performTooltipAnimation(aNode.transform, new Vector3(2,2,1), theGraph.tooltipAnimationsDuration, 0, theGraph.tooltipAnimationsEasetype);
		}
		else {
			MouseExitCommon();
			performTooltipAnimation (aNode.transform, Vector3.one, theGraph.tooltipAnimationsDuration, 0, theGraph.tooltipAnimationsEasetype);
		}
	}
	
	void TooltipLegendLinkMouseEnter(WMG_Series aSeries, WMG_Link aLink, bool state) {
		if (isTooltipObjectNull()) return;
		if (!aSeries.hidePoints) return;
		if (state) {
			MouseEnterCommon(aSeries.seriesName, aLink.gameObject);
			performTooltipAnimation(aLink.transform, new Vector3(2,1.05f,1), theGraph.tooltipAnimationsDuration, 0, theGraph.tooltipAnimationsEasetype);
		}
		else {
			MouseExitCommon();
			performTooltipAnimation (aLink.transform, Vector3.one, theGraph.tooltipAnimationsDuration, 0, theGraph.tooltipAnimationsEasetype);
		}
	}
	
	void performTooltipAnimation (Transform trans, Vector3 newScale, float animDuration, float animDelay, DG.Tweening.Ease easeType) {
		if (theGraph.tooltipAnimationsEnabled) {
			WMG_Anim.animScale(trans.gameObject, animDuration, easeType, newScale, animDelay);
		}
	}
}
