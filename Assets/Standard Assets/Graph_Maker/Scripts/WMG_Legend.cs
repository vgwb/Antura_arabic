using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WMG_Legend : WMG_GUI_Functions {

	public enum legendTypes {Bottom, Right};

	// public properties
	public bool hideLegend { get {return _hideLegend;} 
		set {
			if (_hideLegend != value) {
				_hideLegend = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public legendTypes legendType { get {return _legendType;} 
		set {
			if (_legendType != value) {
				_legendType = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public WMG_Enums.labelTypes labelType { get {return _labelType;} 
		set {
			if (_labelType != value) {
				_labelType = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public bool showBackground { get {return _showBackground;} 
		set {
			if (_showBackground != value) {
				_showBackground = value;
				legendC.Changed();
			}
		}
	}
	public bool oppositeSideLegend { get {return _oppositeSideLegend;} 
		set {
			if (_oppositeSideLegend != value) {
				_oppositeSideLegend = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public float offset { get {return _offset;} 
		set {
			if (_offset != value) {
				_offset = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public float legendEntryWidth { get {return _legendEntryWidth;} 
		set {
			if (_legendEntryWidth != value) {
				_legendEntryWidth = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public bool setWidthFromLabels { get {return _setWidthFromLabels;} 
		set {
			if (_setWidthFromLabels != value) {
				_setWidthFromLabels = value;
				legendC.Changed();
			}
		}
	}
	public float legendEntryHeight { get {return _legendEntryHeight;} 
		set {
			if (_legendEntryHeight != value) {
				_legendEntryHeight = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public int numRowsOrColumns { get {return _numRowsOrColumns;} 
		set {
			if (_numRowsOrColumns != value) {
				_numRowsOrColumns = value;
//				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public int numDecimals { get {return _numDecimals;} 
		set {
			if (_numDecimals != value) {
				_numDecimals = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public float legendEntryLinkSpacing { get {return _legendEntryLinkSpacing;} 
		set {
			if (_legendEntryLinkSpacing != value) {
				_legendEntryLinkSpacing = value;
				legendC.Changed();
			}
		}
	}
	public int legendEntryFontSize { get {return _legendEntryFontSize;} 
		set {
			if (_legendEntryFontSize != value) {
				_legendEntryFontSize = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public float legendEntrySpacing { get {return _legendEntrySpacing;} 
		set {
			if (_legendEntrySpacing != value) {
				_legendEntrySpacing = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public float pieSwatchSize { get {return _pieSwatchSize;} 
		set {
			if (_pieSwatchSize != value) {
				_pieSwatchSize = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public float backgroundPadding { get {return _backgroundPadding;} 
		set {
			if (_backgroundPadding != value) {
				_backgroundPadding = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public bool autofitEnabled { get {return _autofitEnabled;} 
		set {
			if (_autofitEnabled != value) {
				_autofitEnabled = value;
				setGraphCallback();
				legendC.Changed();
			}
		}
	}
	public Color labelColor { get {return _labelColor;} 
		set {
			if (_labelColor != value) {
				_labelColor = value;
				legendC.Changed();
			}
		}
	}
	public FontStyle legendEntryFontStyle { get {return _legendEntryFontStyle;} 
		set {
			if (_legendEntryFontStyle != value) {
				_legendEntryFontStyle = value;
				legendC.Changed();
			}
		}
	}
	public Font legendEntryFont { get {return _legendEntryFont;} 
		set {
			if (_legendEntryFont != value) {
				_legendEntryFont = value;
				legendC.Changed();
			}
		}
	}
	
	public WMG_Graph_Manager theGraph;
	public GameObject background;
	public GameObject entriesParent;
	public Object emptyPrefab;
	public List<WMG_Legend_Entry> legendEntries;

	private WMG_Pie_Graph pieGraph;
	private WMG_Axis_Graph axisGraph;

	// Private backing variables
	[SerializeField] private bool _hideLegend;
	[SerializeField] private legendTypes _legendType;
	[SerializeField] private WMG_Enums.labelTypes _labelType;
	[SerializeField] private bool _showBackground;
	[SerializeField] private bool _oppositeSideLegend;
	[SerializeField] private float _offset;
	[SerializeField] private float _legendEntryWidth;
	[SerializeField] private bool _setWidthFromLabels;
	[SerializeField] private float _legendEntryHeight;
	[SerializeField] private int _numRowsOrColumns;
	[SerializeField] private int _numDecimals;
	[SerializeField] private float _legendEntryLinkSpacing;
	[SerializeField] private int _legendEntryFontSize;
	[SerializeField] private float _legendEntrySpacing;
	[SerializeField] private float _pieSwatchSize;
	[SerializeField] private float _backgroundPadding;
	[SerializeField] private bool _autofitEnabled;
	[SerializeField] private Color _labelColor;
	[SerializeField] private FontStyle _legendEntryFontStyle = FontStyle.Normal;
	[SerializeField] private Font _legendEntryFont; 

	// Useful property getters
	public int LegendWidth { 
		get {
			return Mathf.RoundToInt(2*backgroundPadding + legendEntryLinkSpacing + legendEntryWidth * 
			                        (legendType == legendTypes.Right ? numRowsOrColumns : MaxInRowOrColumn));
		}
	}
	
	public int LegendHeight { 
		get {
			return Mathf.RoundToInt(2*backgroundPadding + legendEntryHeight * 
			                        (legendType == legendTypes.Bottom ? numRowsOrColumns : MaxInRowOrColumn));
		}
	}
	
	public int NumEntries {
		get {
			int numEntries = legendEntries.Count;
			for (int j = 0; j < legendEntries.Count; j++) {
				if (!activeInHierarchy(legendEntries[j].gameObject)) numEntries--;
			}
			return numEntries;
		}
	}
	
	public int MaxInRowOrColumn {
		get {
			return Mathf.CeilToInt(1f * NumEntries / numRowsOrColumns);
		}
	}


	// Original property values for use with dynamic resizing
	public float origLegendEntryWidth { get; private set; }
	public float origLegendEntryHeight { get; private set; }
	public float origLegendEntryLinkSpacing { get; private set; }
	public int origLegendEntryFontSize { get; private set; }
	public float origLegendEntrySpacing { get; private set; }
	public float origPieSwatchSize { get; private set; }
	public float origOffset { get; private set; }
	public float origBackgroundPadding { get; private set; }

	private bool hasInit;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	public WMG_Change_Obj legendC = new WMG_Change_Obj();

	public void Init() {
		if (hasInit) return;
		hasInit = true;

		pieGraph = theGraph.GetComponent<WMG_Pie_Graph>();
		axisGraph = theGraph.GetComponent<WMG_Axis_Graph>();
		
		changeObjs.Add(legendC);
		
		setOriginalPropertyValues();
		
		legendC.OnChange += LegendChanged;

		PauseCallbacks();
	}

	public void PauseCallbacks() {
		for (int i = 0; i < changeObjs.Count; i++) {
			changeObjs[i].changesPaused = true;
			changeObjs[i].changePaused = false;
		}
	}
	
	public void ResumeCallbacks() {
		for (int i = 0; i < changeObjs.Count; i++) {
			changeObjs[i].changesPaused = false;
			if (changeObjs[i].changePaused) changeObjs[i].Changed();
		}
	}

	public void LegendChanged() {
		updateLegend();
	}

	void setGraphCallback() {
		if (pieGraph != null) {
			pieGraph.graphC.Changed();
		}
	}
	
	public void setOriginalPropertyValues() {
		origLegendEntryWidth = legendEntryWidth;
		origLegendEntryHeight = legendEntryHeight;
		origLegendEntryLinkSpacing = legendEntryLinkSpacing;
		origLegendEntryFontSize = legendEntryFontSize;
		origLegendEntrySpacing = legendEntrySpacing;
		origPieSwatchSize = pieSwatchSize;
		origOffset = offset;
		origBackgroundPadding = backgroundPadding;
	}

	// Used for pie graphs
	public WMG_Legend_Entry createLegendEntry(Object prefab) {
		GameObject obj = Instantiate(prefab) as GameObject;
		theGraph.changeSpriteParent(obj, entriesParent);
		WMG_Legend_Entry entry = obj.GetComponent<WMG_Legend_Entry>();
		entry.legend = this;
		legendEntries.Add(entry);
		return entry;
	}

	// Used for other graphs
	public WMG_Legend_Entry createLegendEntry(Object prefab, WMG_Series series, int index) {
		GameObject obj = Instantiate(prefab) as GameObject;
		theGraph.changeSpriteParent(obj, entriesParent);
		WMG_Legend_Entry entry = obj.GetComponent<WMG_Legend_Entry>();
		entry.seriesRef = series;
		entry.legend = this;
		entry.nodeLeft = theGraph.CreateNode(emptyPrefab, obj);
		entry.nodeRight = theGraph.CreateNode(emptyPrefab, obj);
//		_legendEntrySpacing = theGraph.getSpritePositionX(entry.label);
		legendEntries.Insert(index, entry);
		return entry;
	}

	public void deleteLegendEntry(int index) {
		DestroyImmediate(legendEntries[index].gameObject);
		legendEntries.RemoveAt(index);
	}

	bool backgroundEnabled() {
		int numSeries = 1;
		if (axisGraph != null) {
			numSeries = axisGraph.lineSeries.Count;
		}
		if (pieGraph != null) {
			numSeries = pieGraph.sliceValues.Count;
		}
		if (!hideLegend && showBackground && numSeries != 0) return true;
		return false;
	}

	float getMaxLabelWidth() {
		float maxLabelWidth = 0;
		foreach (WMG_Legend_Entry entry in legendEntries) {
			float labelWidth = getTextSize(entry.label).x * entry.label.transform.localScale.x;
			if (labelWidth > maxLabelWidth) maxLabelWidth = labelWidth;
		}
		return maxLabelWidth;
	}

	public void updateLegend() {
		if (backgroundEnabled() && !theGraph.activeInHierarchy(background)) theGraph.SetActive(background,true);
		if (!backgroundEnabled() && theGraph.activeInHierarchy(background)) theGraph.SetActive(background,false);
		if (!hideLegend && !theGraph.activeInHierarchy(entriesParent)) theGraph.SetActive(entriesParent,true);
		if (hideLegend && theGraph.activeInHierarchy(entriesParent)) theGraph.SetActive(entriesParent,false);
		if (hideLegend) return;

		float maxPointSize = 0;

		Vector2 entriesAnchor = Vector2.zero;
		Vector2 entriesPivot = Vector2.zero;
		Vector2 entriesOffset = Vector2.zero;

		if (axisGraph != null) {
			maxPointSize = axisGraph.getMaxPointSize();
		}
		if (pieGraph != null) {
			maxPointSize = pieSwatchSize;
		}

		if (legendType == legendTypes.Bottom) {
			if (oppositeSideLegend) {
				entriesAnchor = new Vector2 (0.5f, 1);
				entriesPivot = entriesAnchor;
				entriesOffset = new Vector2 (0, -offset);
			}
			else {
				entriesAnchor = new Vector2 (0.5f, 0);
				entriesPivot = entriesAnchor;
				entriesOffset = new Vector2 (0, offset);
			}
		}
		else if (legendType == legendTypes.Right) {
			if (oppositeSideLegend) {
				entriesAnchor = new Vector2 (0, 0.5f);
				entriesPivot = entriesAnchor;
				entriesOffset = new Vector2 (offset, 0);
			}
			else {
				entriesAnchor = new Vector2 (1, 0.5f);
				entriesPivot = entriesAnchor;
				entriesOffset = new Vector2 (-offset, 0);
			}
		}

		// For pie graphs anchor the legend to the edge of the pie
		if (pieGraph != null) {
			entriesOffset = new Vector2(-1 * entriesOffset.x, -1 * entriesOffset.y);
			if (legendType == legendTypes.Bottom) {
				entriesPivot = new Vector2(entriesPivot.x, 1 - entriesPivot.y);
			}
			else {
				entriesPivot = new Vector2(1 - entriesPivot.x, entriesPivot.y);
			}
		}

		changeSpriteWidth(this.gameObject, LegendWidth);
		changeSpriteHeight(this.gameObject, LegendHeight);

		setAnchor(this.gameObject, entriesAnchor, entriesPivot, entriesOffset);

		Vector2 entriesParentOffset = new Vector2 (legendEntryLinkSpacing + backgroundPadding + maxPointSize/2f,
		                             		-legendEntryHeight/2f + LegendHeight/2f - backgroundPadding);

		setAnchor(entriesParent, new Vector2 (0, 0.5f), new Vector2 (0, 0.5f), entriesParentOffset);



		int numEntries = NumEntries;
		int maxInRowOrColumn = MaxInRowOrColumn; // Max elements in a row for horizontal legends

		if (numRowsOrColumns < 1) _numRowsOrColumns = 1; // Ensure not less than 1
		if (numRowsOrColumns > numEntries) _numRowsOrColumns = numEntries; // Ensure cannot exceed number series 
		
		int extras = 0;
		if (numEntries > 0) {
			extras = numEntries % numRowsOrColumns; // When the number series does not divide evenly by the num rows setting, then this is the number of extras
		}
		int origExtras = extras; // Save the original extras, since we will need to decrement extras in the loop
		int cumulativeOffset = 0; // Used to offset the other dimension, for example, elements moved to a lower row (y), need to also move certain distance (x) left 
		int previousI = 0; // Used to determine when the i (row for horizontal) has changed from the previous i, which is used to increment the cumulative offset
		bool useSmaller = false; // Used to determine whether we need to subtract 1 from maxInRowOrColumn when calculating the cumulative offset 

		if (maxInRowOrColumn == 0) return; // Legend hidden / all entries deactivated

		// Calculate the position of the legend entry for each line series
		for (int j = 0; j < legendEntries.Count; j++) {
			WMG_Legend_Entry legendEntry = legendEntries[j];

			if (axisGraph != null) {

				if (legendEntry.swatchNode == null) {
					foreach(GameObject seriesGO in axisGraph.lineSeries) {
						seriesGO.GetComponent<WMG_Series>().CreateOrDeleteSpritesBasedOnPointValues();
					}
				}

				theGraph.changeSpritePositionRelativeToObjBy(legendEntry.nodeLeft, legendEntry.swatchNode, new Vector3(-legendEntryLinkSpacing, 0, 0));
				theGraph.changeSpritePositionRelativeToObjBy(legendEntry.nodeRight, legendEntry.swatchNode, new Vector3(legendEntryLinkSpacing, 0, 0));
				
				WMG_Link theLine = legendEntry.line.GetComponent<WMG_Link>();
				theLine.Reposition();
			}
			else {
				changeSpriteWidth(legendEntry.swatchNode, Mathf.RoundToInt(pieSwatchSize));
				changeSpriteHeight(legendEntry.swatchNode, Mathf.RoundToInt(pieSwatchSize));
			}

			if (axisGraph != null) {
				theGraph.changeSpritePositionToX(legendEntry.label, legendEntrySpacing);
			}
			else {
				theGraph.changeSpritePositionToX(legendEntry.label, legendEntrySpacing + pieSwatchSize/2);
			}

			// Legend text
			if (axisGraph != null) {
				string theText = legendEntry.seriesRef.seriesName;
				
				if (labelType == WMG_Enums.labelTypes.None) {
					theText = "";
				}
				changeLabelText(legendEntry.label, theText);
			}
			changeLabelFontSize(legendEntry.label, legendEntryFontSize);
			changeSpriteColor(legendEntry.label, labelColor);
			// Font Style
			changeLabelFontStyle(legendEntry.label, legendEntryFontStyle);
			// Font
			if (legendEntryFont != null) {
				changeLabelFont(legendEntry.label, legendEntryFont);
			}

			// i is the row for horizontal legends, and the column for vertical
			int i = Mathf.FloorToInt(j / maxInRowOrColumn);
			if (origExtras > 0) {
				i = Mathf.FloorToInt((j + 1) / maxInRowOrColumn);
			}
			
			// If there were extras, but no longer any more extras, then need to subtract 1 from the maxInRowOrColumn, and recalculate i
			if (extras == 0 && origExtras > 0) {
				i = origExtras + Mathf.FloorToInt((j - origExtras * maxInRowOrColumn)/ (maxInRowOrColumn - 1));
				if ((j - origExtras * maxInRowOrColumn) > 0) useSmaller = true;
			}
			
			// When there are extras decrease i for the last element in the row
			if (extras > 0) {
				if ((j + 1) % maxInRowOrColumn == 0) {
					extras--;
					i--;
				}
			}
			
			// Increment cumulative offset when i changes, use offset to position other dimension correctly.
			if (previousI != i) {
				previousI = i;
				if (useSmaller) {
					cumulativeOffset += (maxInRowOrColumn - 1);
				}
				else {
					cumulativeOffset += maxInRowOrColumn;
				}
			}
			
			// Set the position based on the series index (j), i (row index for horizontal), and cumulative offset
			if (legendType == legendTypes.Bottom) {
				theGraph.changeSpritePositionTo(legendEntry.gameObject, new Vector3(j * legendEntryWidth - legendEntryWidth * cumulativeOffset, -i * legendEntryHeight, 0));
			}
			else if (legendType == legendTypes.Right) {
				theGraph.changeSpritePositionTo(legendEntry.gameObject, new Vector3(i * legendEntryWidth, -j * legendEntryHeight + legendEntryHeight * cumulativeOffset, 0));
			}
		}

		// This needs to be called after label text is set
		if (setWidthFromLabels) {
			if (axisGraph != null && (axisGraph.graphType == WMG_Axis_Graph.graphTypes.line || axisGraph.graphType == WMG_Axis_Graph.graphTypes.line_stacked)) {
				legendEntryWidth = Mathf.Max(legendEntryLinkSpacing, maxPointSize/2) + legendEntrySpacing + getMaxLabelWidth() + 5;
			}
			else {
				legendEntryWidth = maxPointSize + legendEntrySpacing + getMaxLabelWidth() + 5;
			}
		}

		if (autofitEnabled) {
			if (legendType == legendTypes.Bottom) {
				if (LegendWidth > getSpriteWidth(theGraph.gameObject)) {
					if (numRowsOrColumns < NumEntries) {
						numRowsOrColumns++;
					}
				}
				else {
					if (numRowsOrColumns > 1) {
						_numRowsOrColumns--; // temporarily decrease without callback
						if (LegendWidth > getSpriteWidth(theGraph.gameObject)) { // if new temporary width exceeds border, then dont do anything
							_numRowsOrColumns++;
						}
						else { // new width doesn't exceed the border, actually decrease
							_numRowsOrColumns++;
							numRowsOrColumns--;
						}
					}
				}
			}
			else {
				if (LegendHeight > getSpriteHeight(theGraph.gameObject)) {
					if (numRowsOrColumns < NumEntries) {
						numRowsOrColumns++;
					}
				}
				else {
					if (numRowsOrColumns > 1) {
						_numRowsOrColumns--; // temporarily decrease without callback
						if (LegendHeight > getSpriteHeight(theGraph.gameObject)) { // if new temporary width exceeds border, then dont do anything
							_numRowsOrColumns++;
						}
						else { // new width doesn't exceed the border, actually decrease
							_numRowsOrColumns++;
							numRowsOrColumns--;
						}
					}
				}
			}
		}
	}

	public void setLabelScales(float newScale) {
		foreach (WMG_Legend_Entry entry in legendEntries) {
			entry.label.transform.localScale = new Vector3(newScale, newScale, 1);
		}
	}

}
