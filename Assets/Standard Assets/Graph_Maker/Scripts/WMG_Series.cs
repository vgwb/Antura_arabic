using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WMG_Series : MonoBehaviour {

	public enum comboTypes {line, bar};
	public enum areaShadingTypes {None, Solid, Gradient};

	[SerializeField] private List<Vector2> _pointValues;
	public WMG_List<Vector2> pointValues = new WMG_List<Vector2>();
	[SerializeField] private List<Color> _pointColors;
	public WMG_List<Color> pointColors = new WMG_List<Color>();

	// public properties
	public comboTypes comboType { get {return _comboType;} 
		set {
			if (_comboType != value) {
				_comboType = value;
				prefabC.Changed();
			}
		}
	}
	public bool useSecondYaxis { get {return _useSecondYaxis;} 
		set {
			if (_useSecondYaxis != value) {
				if (theGraph.axesType != WMG_Axis_Graph.axesTypes.DUAL_Y && value == true) {
					Debug.LogWarning("Cannot set useSecondYaxis to true before setting graph axesType to dual-y");
					return;
				}
				_useSecondYaxis = value;
				pointValuesC.Changed();
			}
		}
	}
	public string seriesName { get {return _seriesName;} 
		set {
			if (_seriesName != value) {
				_seriesName = value;
				seriesNameC.Changed();
			}
		}
	}
	public float pointWidthHeight { get {return _pointWidthHeight;} 
		set {
			if (_pointWidthHeight != value) {
				_pointWidthHeight = value;
				pointWidthHeightC.Changed();
			}
		}
	}
	public float lineScale { get {return _lineScale;} 
		set {
			if (_lineScale != value) {
				_lineScale = value;
				lineScaleC.Changed();
			}
		}
	}
	public Color pointColor { get {return _pointColor;} 
		set {
			if (_pointColor != value) {
				_pointColor = value;
				pointColorC.Changed();
			}
		}
	}
	public bool usePointColors { get {return _usePointColors;} 
		set {
			if (_usePointColors != value) {
				_usePointColors = value;
				pointColorC.Changed();
			}
		}
	}
	public Color lineColor { get {return _lineColor;} 
		set {
			if (_lineColor != value) {
				_lineColor = value;
				lineColorC.Changed();
			}
		}
	}
	public bool UseXDistBetweenToSpace { get {return _UseXDistBetweenToSpace;} 
		set {
			if (_UseXDistBetweenToSpace != value) {
				_UseXDistBetweenToSpace = value;
				pointValuesC.Changed();
			}
		}
	}
	public bool ManuallySetXDistBetween { get {return _ManuallySetXDistBetween;} 
		set {
			if (_ManuallySetXDistBetween != value) {
				_ManuallySetXDistBetween = value;
				pointValuesC.Changed();
			}
		}
	}
	public float xDistBetweenPoints { get {return _xDistBetweenPoints;} 
		set {
			if (_xDistBetweenPoints != value) {
				_xDistBetweenPoints = value;
				pointValuesC.Changed();
			}
		}
	}
	public bool ManuallySetExtraXSpace { get {return _ManuallySetExtraXSpace;} 
		set {
			if (_ManuallySetExtraXSpace != value) {
				_ManuallySetExtraXSpace = value;
				pointValuesC.Changed();
			}
		}
	}
	public float extraXSpace { get {return _extraXSpace;} 
		set {
			if (_extraXSpace != value) {
				_extraXSpace = value;
				pointValuesC.Changed();
			}
		}
	}
	public bool hidePoints { get {return _hidePoints;} 
		set {
			if (_hidePoints != value) {
				_hidePoints = value;
				hidePointC.Changed();
			}
		}
	}
	public bool hideLines { get {return _hideLines;} 
		set {
			if (_hideLines != value) {
				_hideLines = value;
				hideLineC.Changed();
			}
		}
	}
	public bool connectFirstToLast { get {return _connectFirstToLast;} 
		set {
			if (_connectFirstToLast != value) {
				_connectFirstToLast = value;
				connectFirstToLastC.Changed();
				lineScaleC.Changed();
				linePaddingC.Changed();
				hideLineC.Changed();
				lineColorC.Changed();
			}
		}
	}
	public float linePadding { get {return _linePadding;} 
		set {
			if (_linePadding != value) {
				_linePadding = value;
				linePaddingC.Changed();
			}
		}
	}
	public bool dataLabelsEnabled { get {return _dataLabelsEnabled;} 
		set {
			if (_dataLabelsEnabled != value) {
				_dataLabelsEnabled = value;
				dataLabelsC.Changed();
			}
		}
	}
	public int dataLabelsNumDecimals { get {return _dataLabelsNumDecimals;} 
		set {
			if (_dataLabelsNumDecimals != value) {
				_dataLabelsNumDecimals = value;
				dataLabelsC.Changed();
			}
		}
	}
	public int dataLabelsFontSize { get {return _dataLabelsFontSize;} 
		set {
			if (_dataLabelsFontSize != value) {
				_dataLabelsFontSize = value;
				dataLabelsC.Changed();
			}
		}
	}
	public Color dataLabelsColor { get {return _dataLabelsColor;} 
		set {
			if (_dataLabelsColor != value) {
				_dataLabelsColor = value;
				dataLabelsC.Changed();
			}
		}
	}
	public FontStyle dataLabelsFontStyle { get {return _dataLabelsFontStyle;} 
		set {
			if (_dataLabelsFontStyle != value) {
				_dataLabelsFontStyle = value;
				dataLabelsC.Changed();
			}
		}
	}
	public Font dataLabelsFont { get {return _dataLabelsFont;} 
		set {
			if (_dataLabelsFont != value) {
				_dataLabelsFont = value;
				dataLabelsC.Changed();
			}
		}
	}
	public Vector2 dataLabelsOffset { get {return _dataLabelsOffset;} 
		set {
			if (_dataLabelsOffset != value) {
				_dataLabelsOffset = value;
				dataLabelsC.Changed();
			}
		}
	}
	public areaShadingTypes areaShadingType { get {return _areaShadingType;} 
		set {
			if (_areaShadingType != value) {
				_areaShadingType = value;
				areaShadingTypeC.Changed();
			}
		}
	}
	public bool areaShadingUsesComputeShader { get {return _areaShadingUsesComputeShader;} 
		set {
			if (_areaShadingUsesComputeShader != value) {
				_areaShadingUsesComputeShader = value;
				areaShadingTypeC.Changed();
			}
		}
	}
	public Color areaShadingColor { get {return _areaShadingColor;} 
		set {
			if (_areaShadingColor != value) {
				_areaShadingColor = value;
				areaShadingC.Changed();
			}
		}
	}
	public float areaShadingAxisValue { get {return _areaShadingAxisValue;} 
		set {
			if (_areaShadingAxisValue != value) {
				_areaShadingAxisValue = value;
				areaShadingC.Changed();
			}
		}
	}
	public int pointPrefab { get {return _pointPrefab;} 
		set {
			if (_pointPrefab != value) {
				_pointPrefab = value;
				prefabC.Changed();
			}
		}
	}
	public int linkPrefab { get {return _linkPrefab;} 
		set {
			if (_linkPrefab != value) {
				_linkPrefab = value;
				prefabC.Changed();
			}
		}
	}

	[System.Obsolete("This parameter is no longer used. Use ManuallySetXDistBetween if needed.")]
	public bool AutoUpdateXDistBetween;
	
	// Public variables without change tracking
	public UnityEngine.Object dataLabelPrefab;
	public GameObject dataLabelsParent;

	public Material areaShadingMatSolid;
	public Material areaShadingMatGradient;
	public GameObject areaShadingParent;
	public UnityEngine.Object areaShadingPrefab;
	public UnityEngine.Object areaShadingCSPrefab;

	public WMG_Axis_Graph theGraph;
	public WMG_Data_Source realTimeDataSource;
	public WMG_Data_Source pointValuesDataSource;
	public UnityEngine.Object legendEntryPrefab;
	public GameObject linkParent;
	public GameObject nodeParent;

	public WMG_Legend_Entry legendEntry;

	// Private backing variables
	[SerializeField] private comboTypes _comboType;
	[SerializeField] private bool _useSecondYaxis;
	[SerializeField] private string _seriesName;
	[SerializeField] private float _pointWidthHeight;
	[SerializeField] private float _lineScale;
	[SerializeField] private Color _pointColor;
	[SerializeField] private bool _usePointColors;
	[SerializeField] private Color _lineColor;
	[SerializeField] private bool _UseXDistBetweenToSpace;
	[SerializeField] private bool _ManuallySetXDistBetween;
	[SerializeField] private float _xDistBetweenPoints;
	[SerializeField] private bool _ManuallySetExtraXSpace;
	[SerializeField] private float _extraXSpace;
	[SerializeField] private bool _hidePoints;
	[SerializeField] private bool _hideLines;
	[SerializeField] private bool _connectFirstToLast;
	[SerializeField] private float _linePadding;
	[SerializeField] private bool _dataLabelsEnabled;
	[SerializeField] private int _dataLabelsNumDecimals;
	[SerializeField] private int _dataLabelsFontSize;
	[SerializeField] private Color _dataLabelsColor = Color.white;
	[SerializeField] private FontStyle _dataLabelsFontStyle = FontStyle.Normal;
	[SerializeField] private Font _dataLabelsFont; 
	[SerializeField] private Vector2 _dataLabelsOffset;
	[SerializeField] private areaShadingTypes _areaShadingType;
	[SerializeField] private bool _areaShadingUsesComputeShader;
	[SerializeField] private Color _areaShadingColor;
	[SerializeField] private float _areaShadingAxisValue;
	[SerializeField] private int _pointPrefab;
	[SerializeField] private int _linkPrefab;

	// Useful property getters
	public bool seriesIsLine { 
		get {
			return (theGraph.graphType == WMG_Axis_Graph.graphTypes.line ||
			        theGraph.graphType == WMG_Axis_Graph.graphTypes.line_stacked ||
				(theGraph.graphType == WMG_Axis_Graph.graphTypes.combo && comboType == comboTypes.line));
		}
	}
	public bool IsLast {
		get {
			return theGraph.lineSeries[theGraph.lineSeries.Count-1].GetComponent<WMG_Series>() == this;
		}
	}
	public WMG_Axis yAxis { 
		get {
			if (theGraph.axesType == WMG_Axis_Graph.axesTypes.DUAL_Y && useSecondYaxis && theGraph.yAxis2 != null) {
				return theGraph.yAxis2;
			}
			else {
				return theGraph.yAxis;
			}
		}
	}

	// Private vars
	private UnityEngine.Object nodePrefab;
	private List<GameObject> points = new List<GameObject>();
	private List<GameObject> lines = new List<GameObject>();
	private List<GameObject> areaShadingRects = new List<GameObject>();
	private List<GameObject> dataLabels = new List<GameObject>();
	private List<bool> barIsNegative = new List<bool>();
	private List<int> changedValIndices = new List<int>();

	// Original property values for use with dynamic resizing
	public float origPointWidthHeight { get; private set; }
	public float origLineScale { get; private set; }
	public int origDataLabelsFontSize { get; private set; }
	public Vector2 origDataLabelOffset { get; set; }
	
	// Cache
	private WMG_Axis_Graph.graphTypes cachedSeriesType;
	
	// Real time update
	private bool realTimeRunning;
	private float realTimeLoopVar;
	private float realTimeOrigMax;
	
	// Automatic Animation variables and functions
	private bool animatingFromPreviousData;
	public bool currentlyAnimating { get; set; }
	private List<Vector2> afterPositions = new List<Vector2>();
	private List<int> afterWidths = new List<int>();
	private List<int> afterHeights = new List<int>();
	//private List<Vector2> previousPointValues = new List<Vector2>();

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	public WMG_Change_Obj pointValuesC = new WMG_Change_Obj();
	public WMG_Change_Obj pointValuesCountC = new WMG_Change_Obj();
	private WMG_Change_Obj pointValuesValC = new WMG_Change_Obj();
	private WMG_Change_Obj lineScaleC = new WMG_Change_Obj();
	private WMG_Change_Obj pointWidthHeightC = new WMG_Change_Obj();
	private WMG_Change_Obj dataLabelsC = new WMG_Change_Obj();
	private WMG_Change_Obj lineColorC = new WMG_Change_Obj();
	private WMG_Change_Obj pointColorC = new WMG_Change_Obj();
	private WMG_Change_Obj hideLineC = new WMG_Change_Obj ();
	private WMG_Change_Obj hidePointC = new WMG_Change_Obj ();
	private WMG_Change_Obj seriesNameC = new WMG_Change_Obj();
	private WMG_Change_Obj linePaddingC = new WMG_Change_Obj();
	private WMG_Change_Obj areaShadingTypeC = new WMG_Change_Obj ();
	private WMG_Change_Obj areaShadingC = new WMG_Change_Obj ();
	public WMG_Change_Obj prefabC = new WMG_Change_Obj ();
	private WMG_Change_Obj connectFirstToLastC = new WMG_Change_Obj ();

	private bool hasInit;

	public delegate string SeriesDataLabeler(WMG_Series series, float val);
	public SeriesDataLabeler seriesDataLabeler;

	public string formatSeriesDataLabel(WMG_Series series, float val) {
		float numberToMult = Mathf.Pow(10f, series.dataLabelsNumDecimals);
		return (Mathf.Round(val * numberToMult) / numberToMult).ToString();
	}
	
	public delegate void SeriesDataChangedHandler(WMG_Series aSeries);
	public event SeriesDataChangedHandler SeriesDataChanged;
	
	protected virtual void OnSeriesDataChanged() {
		SeriesDataChangedHandler handler = SeriesDataChanged;
		if (handler != null) {
			handler(this);
		}
	}

	public void Init(int index) {
		if (hasInit) return;
		hasInit = true;

		changeObjs.Add (pointValuesCountC);
		changeObjs.Add (pointValuesC);
		changeObjs.Add (pointValuesValC);
		changeObjs.Add (connectFirstToLastC);
		changeObjs.Add (lineScaleC);
		changeObjs.Add (pointWidthHeightC);
		changeObjs.Add (dataLabelsC);
		changeObjs.Add (lineColorC);
		changeObjs.Add (pointColorC);
		changeObjs.Add (hideLineC);
		changeObjs.Add (hidePointC);
		changeObjs.Add (seriesNameC);
		changeObjs.Add (linePaddingC);
		changeObjs.Add (areaShadingTypeC);
		changeObjs.Add (areaShadingC);
		changeObjs.Add (prefabC);
		
		if (seriesIsLine) {
			nodePrefab = theGraph.pointPrefabs[pointPrefab];
		}
		else {
			nodePrefab = theGraph.barPrefab;
		}
		
		legendEntry = theGraph.legend.createLegendEntry(legendEntryPrefab, this, index);
		createLegendSwatch();
		theGraph.legend.updateLegend();
		
		pointValues.SetList (_pointValues);
		pointValues.Changed += pointValuesListChanged;
		
		pointColors.SetList (_pointColors);
		pointColors.Changed += pointColorsListChanged;
		
		pointValuesCountC.OnChange += PointValuesCountChanged;
		pointValuesC.OnChange += PointValuesChanged;
		pointValuesValC.OnChange += PointValuesValChanged;
		lineScaleC.OnChange += LineScaleChanged;
		pointWidthHeightC.OnChange += PointWidthHeightChanged;
		dataLabelsC.OnChange += DataLabelsChanged;
		lineColorC.OnChange += LineColorChanged;
		pointColorC.OnChange += PointColorChanged;
		hideLineC.OnChange += HideLinesChanged;
		hidePointC.OnChange += HidePointsChanged;
		seriesNameC.OnChange += SeriesNameChanged;
		linePaddingC.OnChange += LinePaddingChanged;
		areaShadingTypeC.OnChange += AreaShadingTypeChanged;
		areaShadingC.OnChange += AreaShadingChanged;
		prefabC.OnChange += PrefabChanged;
		connectFirstToLastC.OnChange += ConnectFirstToLastChanged;

		seriesDataLabeler = formatSeriesDataLabel;

		setOriginalPropertyValues();
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

	public void pointColorsListChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref pointColors, ref _pointColors, oneValChanged, index);
		pointColorC.Changed();
	}

	public void pointValuesListChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref pointValues, ref _pointValues, oneValChanged, index);
		if (countChanged) {
			pointValuesCountC.Changed();
		} 
		else {
			setAnimatingFromPreviousData();
			if (oneValChanged) {
				changedValIndices.Add(index);
				pointValuesValC.Changed();
			}
			else {
				pointValuesC.Changed ();
			}
		}
	}

	public void PrefabChanged() {
		UpdatePrefabType();
		pointValuesCountC.Changed ();
	}

	public void pointValuesChanged() {
		theGraph.aSeriesPointsChanged ();
		UpdateNullVisibility();
		UpdateSprites();
	}

	public void pointValuesCountChanged() {
		theGraph.aSeriesPointsChanged ();
		CreateOrDeleteSpritesBasedOnPointValues();
		UpdateLineColor();
		UpdatePointColor();
		UpdateLineScale();
		UpdatePointWidthHeight();
		UpdateHideLines();
		UpdateHidePoints();
		UpdateNullVisibility();
		UpdateLinePadding();
		UpdateSprites();
	}

	public void pointValuesValChanged(int index) {
		theGraph.aSeriesPointsChanged ();
		UpdateNullVisibility();
		UpdateSprites();
	}

	public void PointValuesChanged() {
		if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent || (theGraph.IsStacked && !IsLast)) {
			theGraph.aSeriesPointsChanged ();
			theGraph.SeriesChanged(false, true);
		}
		else {
			pointValuesChanged();
		}
	}

	public void PointValuesCountChanged() {
		if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent || (theGraph.IsStacked && !IsLast)) {
			theGraph.aSeriesPointsChanged ();
			theGraph.SeriesChanged(true, true);
		}
		else {
			pointValuesCountChanged();
		}
	}

	public void PointValuesValChanged() {
		if (changedValIndices.Count != 1) { // multiple single vals changed in a single frame
			PointValuesChanged();
		}
		else {
			if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent || (theGraph.IsStacked && !IsLast)) {
				theGraph.aSeriesPointsChanged ();
				theGraph.SeriesChanged(false, true);
			}
			else {
				pointValuesValChanged(changedValIndices[0]);
			}
			changedValIndices.Clear();
		}
	}

	public void LineColorChanged() {
		UpdateLineColor();
	}

	public void ConnectFirstToLastChanged() {
		createOrDeletePoints(pointValues.Count);
	}

	public void PointColorChanged() {
		UpdatePointColor();
	}

	public void LineScaleChanged() {
		UpdateLineScale();
	}

	public void PointWidthHeightChanged() {
		UpdatePointWidthHeight();
	}

	public void HideLinesChanged() {
		UpdateHideLines();
		UpdateNullVisibility();
	}

	public void HidePointsChanged() {
		UpdateHidePoints();
		UpdateNullVisibility();
	}

	public void SeriesNameChanged() {
		UpdateSeriesName();
	}

	public void LinePaddingChanged() {
		UpdateLinePadding();
	}

	public void AreaShadingTypeChanged() {
		createOrDeleteAreaShading(pointValues.Count);
	}

	public void AreaShadingChanged() {
		if (areaShadingUsesComputeShader) {
			UpdateSprites();
		}
		else {
			updateAreaShading(null);
		}
	}

	public void DataLabelsChanged() {
		createOrDeleteLabels(pointValues.Count);
		updateDataLabels();
	}

	public void UpdateFromDataSource() {
		if (pointValuesDataSource != null) {
			List<Vector2> dataSourceData = pointValuesDataSource.getData<Vector2>();
			if (theGraph.useGroups) {
				dataSourceData = sanitizeGroupData(dataSourceData);
			}
			pointValues.SetList(dataSourceData);
		}
	}

	public void RealTimeUpdate() {
		if (realTimeRunning) {
			DoRealTimeUpdate();
		}
	}
	
	public List<Vector2> AfterPositions() {
		return afterPositions;
	}
	
	public List<int> AfterHeights() {
		return afterHeights;
	}
	
	public List<int> AfterWidths() {
		return afterWidths;
	}
	
	public bool AnimatingFromPreviousData() {
		return animatingFromPreviousData;
	}
	
	public void setAnimatingFromPreviousData() {
		// Automatic animations doesn't work for real time updating or stacked graphs
		if (realTimeRunning) return;
		if (theGraph.IsStacked) return;
		if (theGraph.autoAnimationsEnabled) {
			animatingFromPreviousData = true;
		}
	}

	// Set initial property values for use with percentage based dynamic resizing 
	public void setOriginalPropertyValues() {
		origPointWidthHeight = pointWidthHeight;
		origLineScale = lineScale;
		origDataLabelsFontSize = dataLabelsFontSize;
		origDataLabelOffset = dataLabelsOffset;
	}
	
	public List<GameObject> getPoints() {
		return points;
	}

	public GameObject getLastPoint() {
		return points[points.Count-1];
	}

	public GameObject getFirstPoint() {
		return points[0];
	}
	
	public List<GameObject> getLines() {
		return lines;
	}
	
	public List<GameObject> getDataLabels() {
		return dataLabels;
	}
	
	public bool getBarIsNegative(int i) {
		return barIsNegative[i];
	}
	
	// Get Vector2 associated with a node in this series
	public Vector2 getNodeValue(WMG_Node aNode) {
		for (int i = 0; i < pointValues.Count; i++) {
			if (points[i].GetComponent<WMG_Node>() == aNode) return pointValues[i];
		}
		return Vector2.zero;
	}

	public void UpdateHidePoints() {
		// Series points
		for (int i = 0; i < points.Count; i++) {
			theGraph.SetActive(points[i],!hidePoints);
		}
		// Legend point
		theGraph.SetActive(legendEntry.swatchNode, !hidePoints);
		if (!areaShadingUsesComputeShader) StartCoroutine(SetDelayedAreaShadingChanged ());
	}

	public void UpdateNullVisibility() {
		// For null groups hide the appropriate points
		if (theGraph.useGroups) {
			for (int i = 0; i < points.Count; i++) {
				theGraph.SetActive(points[i], pointValues[i].x > 0);
			}
			// For null groups hide the appropriate lines
			if (seriesIsLine) {
				for (int i = 0; i < lines.Count; i++) {
					theGraph.SetActive(lines[i],true);
				}
				for (int i = 0; i < points.Count; i++) {
					if (pointValues[i].x < 0) {
						WMG_Node thePoint = points[i].GetComponent<WMG_Node>();
						for (int j = 0; j < thePoint.links.Count; j++) {
							theGraph.SetActive(thePoint.links[j], false);
						}
					}
				}
			}
			if (!areaShadingUsesComputeShader) StartCoroutine(SetDelayedAreaShadingChanged ());
		}
		if (hidePoints) {
			// Series points
			for (int i = 0; i < points.Count; i++) {
				theGraph.SetActive(points[i],false);
			}
		}
		if (hideLines || !seriesIsLine) {
			// Series lines
			for (int i = 0; i < lines.Count; i++) {
				theGraph.SetActive (lines [i], false);
			}
		}
	}
	
	public void UpdateHideLines() {
		// Series lines
		for (int i = 0; i < lines.Count; i++) {
			if (hideLines || !seriesIsLine) theGraph.SetActive(lines[i],false);
			else theGraph.SetActive(lines[i],true);
		}
		// Legend lines
		if (hideLines || !seriesIsLine) {
			theGraph.SetActive(legendEntry.line, false);
		}
		else {
			theGraph.SetActive(legendEntry.line, true);
		}
		if (!areaShadingUsesComputeShader) StartCoroutine(SetDelayedAreaShadingChanged ());
	}
	
	public void UpdateLineColor() {
		// Series line colors
		for (int i = 0; i < lines.Count; i++) {
			WMG_Link theLine = lines[i].GetComponent<WMG_Link>();
			theGraph.changeSpriteColor(theLine.objectToColor, lineColor);
		}
		// Legend line colors
		WMG_Link legendLine = legendEntry.line.GetComponent<WMG_Link>();
		theGraph.changeSpriteColor(legendLine.objectToColor, lineColor);
	}
	
	public void UpdatePointColor() {
		// Series point colors
		for (int i = 0; i < points.Count; i++) {
			WMG_Node thePoint = points[i].GetComponent<WMG_Node>();
			if (usePointColors) {
				if (i < pointColors.Count) {
					theGraph.changeSpriteColor(thePoint.objectToColor, pointColors[i]);
				}
			}
			else {
				theGraph.changeSpriteColor(thePoint.objectToColor, pointColor);
			}
		}
		// Legend point color
		WMG_Node legendPoint = legendEntry.swatchNode.GetComponent<WMG_Node>();
		theGraph.changeSpriteColor(legendPoint.objectToColor, pointColor);
	}
	
	public void UpdateLineScale() {
		// Series line widths
		for (int i = 0; i < lines.Count; i++) {
			WMG_Link theLine = lines[i].GetComponent<WMG_Link>();
			theLine.objectToScale.transform.localScale = new Vector3(lineScale, theLine.objectToScale.transform.localScale.y, theLine.objectToScale.transform.localScale.z);
		}
		// Legend line widths
		WMG_Link legendLine = legendEntry.line.GetComponent<WMG_Link>();
		legendLine.objectToScale.transform.localScale = new Vector3(lineScale, legendLine.objectToScale.transform.localScale.y, legendLine.objectToScale.transform.localScale.z);
	}
	
	public void UpdatePointWidthHeight() {
		// Series line point dimensions
		if (seriesIsLine) {
			for (int i = 0; i < points.Count; i++) {
				WMG_Node thePoint = points[i].GetComponent<WMG_Node>();
				theGraph.changeSpriteHeight(thePoint.objectToColor, Mathf.RoundToInt(pointWidthHeight));
				theGraph.changeSpriteWidth(thePoint.objectToColor, Mathf.RoundToInt(pointWidthHeight));
			}
		}
		// Legend point / bar dimensions
		WMG_Node legendPoint = legendEntry.swatchNode.GetComponent<WMG_Node>();
		theGraph.changeSpriteHeight(legendPoint.objectToColor, Mathf.RoundToInt(pointWidthHeight));
		theGraph.changeSpriteWidth(legendPoint.objectToColor, Mathf.RoundToInt(pointWidthHeight));
	}
	
	public void UpdatePrefabType() {
		// Update prefab variable used later in the creating sprites function
		if (seriesIsLine) {
			nodePrefab = theGraph.pointPrefabs[pointPrefab];
		}
		else {
			nodePrefab = theGraph.barPrefab;
		}
		
		// Delete points and lines
		for (int i = points.Count - 1; i >= 0; i--) {
			if (points[i] != null) {
				WMG_Node thePoint = points[i].GetComponent<WMG_Node>();
				foreach (GameObject child in thePoint.links) {
					lines.Remove(child);
				}
				theGraph.DeleteNode(thePoint);
				points.RemoveAt(i);
			}
		}
		// Delete legend
		if (legendEntry.swatchNode != null) {
			theGraph.DeleteNode(legendEntry.swatchNode.GetComponent<WMG_Node>());
			theGraph.DeleteLink(legendEntry.line.GetComponent<WMG_Link>());
		}
	}
	
	public void UpdateSeriesName() {
		theGraph.legend.LegendChanged();
	}
	
	public void UpdateLinePadding() {
		for (int i = 0; i < points.Count; i++) {
			points[i].GetComponent<WMG_Node>().radius = -1 * linePadding;
		}
		RepositionLines();
	}

	public void RepositionLines() {
		for (int i = 0; i < lines.Count; i++) {
			lines[i].GetComponent<WMG_Link>().Reposition();
		}
	}

	public void CreateOrDeleteSpritesBasedOnPointValues() {
		if (theGraph.useGroups) {
			pointValues.SetListNoCb(sanitizeGroupData(pointValues.list), ref _pointValues);
		}

		int pointValuesCount = pointValues.Count;

		createOrDeletePoints(pointValuesCount);
		createOrDeleteLabels(pointValuesCount);
		createOrDeleteAreaShading(pointValuesCount);
	}

	List<Vector2> sanitizeGroupData(List<Vector2> groupData) {
		// Groups are defined at the graph level in the groups variable.
		// If, for example, there are 5 groups defined, then the data in the series must comprise of 5 Vector2's
		// The x value in each Vector2 represents the group, and a negative x value represents a null group.
		// Null groups will not be graphed at all (for example line graph with broken line segments)
		// This function will automatically group together data and insert nulls as needed.
		// For example, for 3 groups, if you supply input of (2,3) (2,5), this will convert it to (-1,0) (2,8) (-3,0)

		// remove values that can't possibly represent groups
		for (int i = groupData.Count - 1; i >= 0; i--) {
			int intVal = Mathf.RoundToInt(groupData[i].x);
			if (intVal - groupData[i].x != 0) {
				groupData.RemoveAt(i); // Not an integer
				continue;
			}
			if (Mathf.Abs(intVal) > theGraph.groups.Count) {
				groupData.RemoveAt(i); // Out of bounds
				continue;
			}
			if (intVal == 0) {
				groupData.RemoveAt(i); // 0, because nulls are represented by negatives and there is no negative 0
				continue;
			}
		}
		// sort values, combine duplicates
		groupData.Sort( (vec1,vec2)=>vec1.x.CompareTo(vec2.x));
		List<Vector2> newPoints = new List<Vector2>();
		bool newPoint = true;
		for (int i = 0; i < groupData.Count; i++) {
			if (newPoint) {
				newPoints.Add(groupData[i]);
				newPoint = false;
			}
			else {
				Vector2 prev = newPoints[newPoints.Count-1];
				newPoints[newPoints.Count-1] = new Vector2(prev.x, prev.y + groupData[i].y);
			}

			if (i < groupData.Count-1) {
				if (groupData[i].x != groupData[i+1].x) {
					newPoint = true;
				}
			}
		}

		// insert nulls
		if (newPoints.Count < theGraph.groups.Count) {
			int numNullsToAdd = theGraph.groups.Count - newPoints.Count;
			for (int i = 0; i < numNullsToAdd; i++) {
				newPoints.Insert(0, new Vector2(-1, 0));
			}
		}

		// this is rare, but there could be extras nulls (negatives), remove them until the counts are equal
		if (newPoints.Count > theGraph.groups.Count) {
			int numNullsToRemove = newPoints.Count - theGraph.groups.Count;
			for (int i = 0; i < numNullsToRemove; i++) {
				newPoints.RemoveAt(0);
			}
		}

		// at this point, we should have for example, if 5 groups, -1, -1, 1, 2, 5
		// now to easily determine which groups are null, need something like 1, 2, -3, -4, 5
		List<int> nullGroups = new List<int>();
		for (int i = 0; i < theGraph.groups.Count; i++) {
			nullGroups.Add(i+1);
		}
		for (int i = newPoints.Count - 1; i >= 0; i--) {
			if (newPoints[i].x > 0) nullGroups.Remove(Mathf.RoundToInt(newPoints[i].x));
		}
		for (int i = 0; i < nullGroups.Count; i++) {
			newPoints[i] = new Vector2(-1*nullGroups[i], 0);
		}

		// sort values, so that negatives treated same as positives
		newPoints.Sort( (vec1,vec2)=>Mathf.Abs(vec1.x).CompareTo(Mathf.Abs(vec2.x)));

		return newPoints;
	}

	void createOrDeletePoints(int pointValuesCount) {
		// Create points based on pointValues data
		for (int i = 0; i < pointValuesCount; i++) {
			if (points.Count <= i) {
				GameObject curObj = theGraph.CreateNode(nodePrefab, nodeParent);
				
				theGraph.addNodeClickEvent(curObj);
				theGraph.addNodeMouseEnterEvent(curObj);
				theGraph.addNodeMouseLeaveEvent(curObj);
				
				curObj.GetComponent<WMG_Node>().radius = -1 * linePadding;
				theGraph.SetActive(curObj,false);
				points.Add(curObj);
				barIsNegative.Add(false);
				if (i > 0) {
					WMG_Node fromNode = points[i-1].GetComponent<WMG_Node>();
					curObj = theGraph.CreateLink(fromNode, curObj, theGraph.linkPrefabs[linkPrefab], linkParent);
					
					theGraph.addLinkClickEvent(curObj);
					theGraph.addLinkMouseEnterEvent(curObj);
					theGraph.addLinkMouseLeaveEvent(curObj);
					
					theGraph.SetActive(curObj,false);
					lines.Add(curObj);
				}
			}
		}
		// If there are more points than pointValues data, delete the extras
		for (int i = points.Count - 1; i >= 0; i--) {
			if (points[i] != null && i >= pointValuesCount) {
				WMG_Node thePoint = points[i].GetComponent<WMG_Node>();
				foreach (GameObject child in thePoint.links) {
					lines.Remove(child);
				}
				theGraph.DeleteNode(thePoint);
				points.RemoveAt(i);
				barIsNegative.RemoveAt(i);
			}
			// Delete existing connect first to last
			if (i > 1 && i < pointValuesCount-1) {
				WMG_Node firstNode = points[0].GetComponent<WMG_Node>();
				WMG_Node toNode = points[i].GetComponent<WMG_Node>();
				WMG_Link delLink = theGraph.GetLink(firstNode,toNode);
				if (delLink != null) {
					lines.Remove(delLink.gameObject);
					theGraph.DeleteLink(delLink);
				}
			}
		}
		// Connect first to last
		if (points.Count > 2) {
			WMG_Node firstNode = points[0].GetComponent<WMG_Node>();
			WMG_Node toNode = points[points.Count-1].GetComponent<WMG_Node>();
			WMG_Link delLink = theGraph.GetLink(firstNode,toNode);
			if (connectFirstToLast && delLink == null) {
				GameObject curObj = theGraph.CreateLink(firstNode, toNode.gameObject, theGraph.linkPrefabs[linkPrefab], linkParent);
				
				theGraph.addLinkClickEvent(curObj);
				theGraph.addLinkMouseEnterEvent(curObj);
				theGraph.addLinkMouseLeaveEvent(curObj);
				
				theGraph.SetActive(curObj,false);
				lines.Add(curObj);
			}
			if (!connectFirstToLast && delLink != null) {
				lines.Remove(delLink.gameObject);
				theGraph.DeleteLink(delLink);
			}
		}
		// Create the legend if it doesn't exist (changing prefab type deletes the legend swatch and lines)
		if (legendEntry.swatchNode == null) {
			createLegendSwatch();
		}
	}

	void createLegendSwatch() {
		legendEntry.swatchNode = theGraph.CreateNode(nodePrefab, legendEntry.gameObject);
		
		theGraph.addNodeClickEvent_Leg(legendEntry.swatchNode);
		theGraph.addNodeMouseEnterEvent_Leg(legendEntry.swatchNode);
		theGraph.addNodeMouseLeaveEvent_Leg(legendEntry.swatchNode);
		
		WMG_Node cNode = legendEntry.swatchNode.GetComponent<WMG_Node>();
		theGraph.changeSpritePivot(cNode.objectToColor, WMG_Graph_Manager.WMGpivotTypes.Center);
		cNode.Reposition(0,0);
		
		legendEntry.line = theGraph.CreateLink(legendEntry.nodeRight.GetComponent<WMG_Node>(), legendEntry.nodeLeft, theGraph.linkPrefabs[linkPrefab], legendEntry.gameObject);
		
		theGraph.addLinkClickEvent_Leg(legendEntry.line);
		theGraph.addLinkMouseEnterEvent_Leg(legendEntry.line);
		theGraph.addLinkMouseLeaveEvent_Leg(legendEntry.line);
		
		theGraph.bringSpriteToFront(legendEntry.swatchNode);
	}
	
	void createOrDeleteLabels(int pointValuesCount) {
		// Create / delete data labels
		if (dataLabelPrefab != null && dataLabelsParent != null) {
			if (dataLabelsEnabled) {
				for (int i = 0; i < pointValuesCount; i++) {
					if (dataLabels.Count <= i) {
						GameObject curObj = Instantiate(dataLabelPrefab) as GameObject;
						theGraph.changeSpriteParent(curObj, dataLabelsParent);
						curObj.transform.localScale = Vector3.one;
						dataLabels.Add(curObj);
						curObj.name = "Data_Label_" + dataLabels.Count;
					}
				}
			}
			int numLabels = pointValuesCount;
			if (!dataLabelsEnabled) {
				numLabels = 0;
			}
			else {
				// Data labels doesn't work for stacked bar or stacked percentage bar
				if (theGraph.IsStacked && theGraph.graphType != WMG_Axis_Graph.graphTypes.line_stacked) {
					numLabels = 0;
					dataLabelsEnabled = false;
				}
			}
			// If there are more data labels than pointValues data, delete the extras
			for (int i = dataLabels.Count - 1; i >= 0; i--) {
				if (dataLabels[i] != null && i >= numLabels) {
					DestroyImmediate(dataLabels[i]);
					dataLabels.RemoveAt(i);
				}
			}
			if (!areaShadingUsesComputeShader) StartCoroutine(SetDelayedAreaShadingChanged ()); // For some reason creating / deleting objects hides area shading
		}
	}

	void createOrDeleteAreaShading(int pointValuesCount) {
		if (areaShadingUsesComputeShader) {
			if (areaShadingCSPrefab == null || areaShadingParent == null) return;

			if (areaShadingType != areaShadingTypes.None && areaShadingRects.Count == 1 && areaShadingRects[0].name == "Area_Shading_CS") {
				UpdateSprites(); // changed from gradient to fill or vice versa
				return;
			}

			for (int i = areaShadingRects.Count - 1; i >= 0; i--) {
				if (areaShadingRects[i] != null && i >= 0) {
					DestroyImmediate(areaShadingRects[i]);
					areaShadingRects.RemoveAt(i);
				}
			}

			if (areaShadingType != areaShadingTypes.None) {
				if (areaShadingRects.Count != 1) {
					GameObject curObj = Instantiate(areaShadingCSPrefab) as GameObject;
					theGraph.changeSpriteParent(curObj, areaShadingParent);
					theGraph.changeSpriteSizeFloat(curObj, theGraph.xAxisLength, theGraph.yAxisLength);
					theGraph.changeSpritePivot(curObj, WMG_GUI_Functions.WMGpivotTypes.BottomLeft);
					theGraph.changeSpritePositionTo(curObj, new Vector3(0, 0, 0));
					curObj.transform.localScale = Vector3.one;
					areaShadingRects.Add(curObj);
					curObj.name = "Area_Shading_CS";
					WMG_Compute_Shader areaShadingCS = curObj.GetComponent<WMG_Compute_Shader>();
					areaShadingCS.Init();
					UpdateSprites();
				}
			}

		}
		else {
			if (areaShadingPrefab == null || areaShadingParent == null) return;

			if (areaShadingRects.Count == 1 && areaShadingRects[0].name == "Area_Shading_CS") {
				DestroyImmediate(areaShadingRects[0]);
				areaShadingRects.RemoveAt(0);
			}

			// Create area shading rectangles based on pointValues data
			if (areaShadingType != areaShadingTypes.None) {
				for (int i = 0; i < pointValuesCount-1; i++) {
					if (areaShadingRects.Count <= i) {
						GameObject curObj = Instantiate(areaShadingPrefab) as GameObject;
						theGraph.changeSpriteParent(curObj, areaShadingParent);
						curObj.transform.localScale = Vector3.one;
						areaShadingRects.Add(curObj);
						curObj.name = "Area_Shading_" + areaShadingRects.Count;
						StartCoroutine(SetDelayedAreaShadingChanged ());
					}
				}
			}
			int numRects = pointValuesCount-1;
			if (areaShadingType == areaShadingTypes.None) {
				numRects = 0;
			}
			// If there are more shading rectangles than pointValues data, delete the extras
			for (int i = areaShadingRects.Count - 1; i >= 0; i--) {
				if (areaShadingRects[i] != null && i >= numRects) {
					DestroyImmediate(areaShadingRects[i]);
					areaShadingRects.RemoveAt(i);
					StartCoroutine(SetDelayedAreaShadingChanged ());
				}
			}
			Material matToUse = areaShadingMatSolid;
			if (areaShadingType == areaShadingTypes.Gradient) {
				matToUse = areaShadingMatGradient;
			}
			for (int i = 0; i < areaShadingRects.Count; i++) {
				theGraph.setTextureMaterial(areaShadingRects[i], matToUse);
				StartCoroutine(SetDelayedAreaShadingChanged ());
			}
		}
	}
	
	IEnumerator SetDelayedAreaShadingChanged() {
		yield return new WaitForEndOfFrame();
		AreaShadingChanged();
		yield return new WaitForEndOfFrame();
		AreaShadingChanged();
	}
	
	public void UpdateSprites() {
		List<GameObject> prevPoints = null;
		if (theGraph.IsStacked) {
			for (int j = 1; j < theGraph.lineSeries.Count; j++) {
				if (!theGraph.activeInHierarchy(theGraph.lineSeries[j])) continue;
				WMG_Series theSeries = theGraph.lineSeries[j].GetComponent<WMG_Series>();
				if (theSeries == this) {
					if (!theGraph.activeInHierarchy(theGraph.lineSeries[j-1])) continue;
					WMG_Series prevSeries = theGraph.lineSeries[j-1].GetComponent<WMG_Series>();
					prevPoints = prevSeries.getPoints();
				}
			}
		}

		List<Vector2> newPositions = new List<Vector2>();
		List<int> newWidths = new List<int>();
		List<int> newHeights = new List<int>();
		bool callUpdateShading = true;
		getNewPointPositionsAndSizes(prevPoints, ref newPositions, ref newWidths, ref newHeights);
		updatePointSprites(newPositions, newWidths, newHeights, ref callUpdateShading);
		updateDataLabels();
		if (callUpdateShading) {
			updateAreaShading(newPositions);
		}
	}

	public void updateXdistBetween() {
		// Auto set xDistBetween based on the axis length and point count
		if (!ManuallySetXDistBetween) {
			_xDistBetweenPoints = theGraph.getDistBetween(points.Count, (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal ? theGraph.yAxisLength : theGraph.xAxisLength));
		}
	}

	public void updateExtraXSpace() {
		// auto update space from axis
		if (!ManuallySetExtraXSpace) {
			if (theGraph.autoUpdateSeriesAxisSpacing) {
				if (theGraph.graphType == WMG_Axis_Graph.graphTypes.line || theGraph.graphType == WMG_Axis_Graph.graphTypes.line_stacked) {
					_extraXSpace = 0;
				}
				else {
					_extraXSpace = xDistBetweenPoints / 2;
				}
			}
		}
	}

	void getNewPointPositionsAndSizes(List<GameObject> prevPoints, ref List<Vector2> newPositions, ref List<int> newWidths, ref List<int> newHeights) {
		if (points.Count == 0) return;
		float xAxisLength = theGraph.xAxisLength;
		float yAxisLength = theGraph.yAxisLength;
		float xAxisMax = theGraph.xAxis.AxisMaxValue;
		float yAxisMax = yAxis.AxisMaxValue;
		float xAxisMin = theGraph.xAxis.AxisMinValue;
		float yAxisMin = yAxis.AxisMinValue;
		
		if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
			theGraph.SwapVals(ref xAxisLength, ref yAxisLength);
			theGraph.SwapVals(ref xAxisMax, ref yAxisMax);
			theGraph.SwapVals(ref xAxisMin, ref yAxisMin);
		}
		
		updateXdistBetween();
		
		updateExtraXSpace();
		
		for (int i = 0; i < points.Count; i++) {
			if (i >= pointValues.Count) break;
			
			float newX = 0;
			float newY = (pointValues[i].y - yAxisMin)/(yAxisMax - yAxisMin) * yAxisLength; // new y always based on the pointValues.y
			
			// If using xDistBetween then point positioning based on previous point point position
			if (!theGraph.useGroups && UseXDistBetweenToSpace) {
				if (i > 0) { // For points greater than 0, use the previous point position plus xDistBetween
					float prevPosX = newPositions[i-1].x;
					float barOffsetX = 0;
					if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
						prevPosX = newPositions[i-1].y;
						barOffsetX = theGraph.barWidth;
					}
					newX = prevPosX + xDistBetweenPoints;
					if (!seriesIsLine) {
						newX += barOffsetX;
					}
				}
				else { // For point 0, one of the positions is just 0
					newX = extraXSpace;
				}
			}
			else if (theGraph.useGroups) { // Using groups, x values represent integer index of group
				newX = extraXSpace + xDistBetweenPoints * (Mathf.Abs(pointValues[i].x) - 1);
			}
			else { // Not using xDistBetween or groups, so use the actual x values in the Vector2 list
				newX = (pointValues[i].x - xAxisMin)/(xAxisMax - xAxisMin) * xAxisLength;
			}
			
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
				theGraph.SwapVals(ref newX, ref newY);
			}
			
			int newWidth = 0;
			int newHeight = 0;
			
			if (seriesIsLine) {
				// Width and height of points for line graphs - needed because autospace functionality requires height and width of previous point
				// And previous point widths and heights are not set in this loop because of automatic animations
				newWidth = Mathf.RoundToInt(pointWidthHeight);
				newHeight = Mathf.RoundToInt(pointWidthHeight);
				
				if (theGraph.graphType == WMG_Axis_Graph.graphTypes.line_stacked) {
					if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
						if (prevPoints != null && i < prevPoints.Count) {
							newY += theGraph.getSpritePositionY(prevPoints[i]);
						}
					}
					else {
						if (prevPoints != null && i < prevPoints.Count) {
							newX += theGraph.getSpritePositionX(prevPoints[i]);
						}
					}
				}
				
			}
			else {
				// For bar graphs, need to update sprites width and height based on positions
				// For stacked percentage, need to set a y position based on the percentage of all series values combined
				if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent && theGraph.TotalPointValues.Count > i) {
					if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
						newY = (pointValues[i].y - yAxisMin) / theGraph.TotalPointValues[i] * yAxisLength;
					}
					else {
						newX = (pointValues[i].y - yAxisMin) / theGraph.TotalPointValues[i] * yAxisLength;
					}
				}
				
				// Update sprite dimensions and increase position using previous point position
				// Previous points is null for side by side bar, but should not be empty for stacked and stacked percentage for series after the first series
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
					newWidth = Mathf.RoundToInt(theGraph.barWidth);
					newHeight = Mathf.RoundToInt(newY);
					// Adjust height based on barAxisValue
					int heightAdjust = 0;
					if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_side || (theGraph.graphType == WMG_Axis_Graph.graphTypes.combo && comboType == comboTypes.bar)) {
						heightAdjust = Mathf.RoundToInt((theGraph.barAxisValue - yAxisMin) / (yAxisMax - yAxisMin) * yAxisLength);
					}
					newHeight -= heightAdjust;
					newY -= newHeight;
					barIsNegative[i] = false;
					if (newHeight < 0) {
						newHeight *= -1;
						newY -= newHeight;
						barIsNegative[i] = true;
					}
					if (prevPoints != null && i < prevPoints.Count) {
						newY += theGraph.getSpritePositionY(prevPoints[i]) + theGraph.getSpriteHeight(prevPoints[i]);
					}
				}
				else {
					newWidth = Mathf.RoundToInt(newX);
					newHeight = Mathf.RoundToInt(theGraph.barWidth);
					// Adjust width based on barAxisValue
					int widthAdjust = 0;
					if (theGraph.graphType == WMG_Axis_Graph.graphTypes.bar_side || (theGraph.graphType == WMG_Axis_Graph.graphTypes.combo && comboType == comboTypes.bar)) {
						widthAdjust = Mathf.RoundToInt((theGraph.barAxisValue - yAxisMin) / (yAxisMax - yAxisMin) * yAxisLength);
					}
					newWidth -= widthAdjust;
					newX = widthAdjust;
					newY -= theGraph.barWidth;
					barIsNegative[i] = false;
					if (newWidth < 0) {
						newWidth *= -1;
						newX -= newWidth;
						barIsNegative[i] = true;
					}
					if (prevPoints != null && i < prevPoints.Count) {
						newX += theGraph.getSpritePositionX(prevPoints[i]) + theGraph.getSpriteWidth(prevPoints[i]);
					}
				}
			}
			newWidths.Add(newWidth);
			newHeights.Add(newHeight);
			newPositions.Add(new Vector2(newX, newY));
		}

	}

	void updatePointSprites(List<Vector2> newPositions, List<int> newWidths, List<int> newHeights, ref bool callUpdateShading) {
		if (points.Count == 0) return;
		if (animatingFromPreviousData) { // For animations, copy over the newly calculated values into lists to be used later in the animation code
			if (seriesIsLine) {
				for (int i = 0; i < points.Count; i++) {
					if (i >= pointValues.Count) break;
					newPositions[i] = theGraph.getChangeSpritePositionTo(points[i], newPositions[i]);
				}
			}
			afterPositions = new List<Vector2>(newPositions);
			afterWidths = new List<int>(newWidths);
			afterHeights = new List<int>(newHeights);
			OnSeriesDataChanged();
			animatingFromPreviousData = false;
			if (areaShadingUsesComputeShader) {
				callUpdateShading = false;
			}
		}
		else { // Otherwise update the visuals now
			for (int i = 0; i < points.Count; i++) {
				if (i >= pointValues.Count) break;
				if (!seriesIsLine) {
					WMG_Node thePoint = points[i].GetComponent<WMG_Node>();
					theGraph.changeBarWidthHeight(thePoint.objectToColor, newWidths[i], newHeights[i]); 
				}
				theGraph.changeSpritePositionTo(points[i], new Vector3(newPositions[i].x, newPositions[i].y, 0));
			}
			RepositionLines();
		}
	}


	void updateDataLabels() {
		if (!dataLabelsEnabled) return;
		for (int i = 0; i < dataLabels.Count; i++) {
			Vector2 currentPointPosition = new Vector2(theGraph.getSpritePositionX(points[i]), theGraph.getSpritePositionY(points[i]));
			// Update font size
			theGraph.changeLabelFontSize(dataLabels[i], dataLabelsFontSize);
			// Font Color
			theGraph.changeLabelColor(dataLabels[i], dataLabelsColor);
			// Font Style
			theGraph.changeLabelFontStyle(dataLabels[i], dataLabelsFontStyle);
			// Font
			if (dataLabelsFont != null) {
				theGraph.changeLabelFont(dataLabels[i], dataLabelsFont);
			}
			// Update text based on y value and number decimals
			theGraph.changeLabelText(dataLabels[i], seriesDataLabeler(this, pointValues[i].y));

			// Update pivot
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
				theGraph.changeSpritePivot(dataLabels[i], WMG_Graph_Manager.WMGpivotTypes.Left);
			}
			else {
				theGraph.changeSpritePivot(dataLabels[i], WMG_Graph_Manager.WMGpivotTypes.Bottom);
			}

			// Update positions
			if (seriesIsLine) {
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
					theGraph.changeSpritePositionTo(dataLabels[i], new Vector3(
						dataLabelsOffset.x + currentPointPosition.x, 
						dataLabelsOffset.y + currentPointPosition.y + pointWidthHeight / 2, 
						0));
				}
				else {
					theGraph.changeSpritePositionTo(dataLabels[i], new Vector3(
						dataLabelsOffset.x + currentPointPosition.x + pointWidthHeight / 2, 
						dataLabelsOffset.y + currentPointPosition.y, 
						0));
				}
			}
			else {
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
					float newY = dataLabelsOffset.y + currentPointPosition.y + theGraph.getSpriteHeight(points[i]);
					if (barIsNegative[i]) {
						newY = -dataLabelsOffset.y - theGraph.getSpriteHeight(points[i]) + Mathf.RoundToInt((theGraph.barAxisValue - yAxis.AxisMinValue) / (yAxis.AxisMaxValue - yAxis.AxisMinValue) * theGraph.yAxisLength);
					}
					theGraph.changeSpritePositionTo(dataLabels[i], new Vector3(
						dataLabelsOffset.x + currentPointPosition.x + theGraph.barWidth / 2, 
						newY, 
						0));
				}
				else {
					float newX = dataLabelsOffset.x + currentPointPosition.x + theGraph.getSpriteWidth(points[i]);
					if (barIsNegative[i]) {
						newX = -dataLabelsOffset.x - theGraph.getSpriteWidth(points[i]) + Mathf.RoundToInt((theGraph.barAxisValue - theGraph.xAxis.AxisMinValue) / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue) * theGraph.xAxisLength);
					}
					theGraph.changeSpritePositionTo(dataLabels[i], new Vector3(
						newX, 
						dataLabelsOffset.y + currentPointPosition.y + theGraph.barWidth / 2, 
						0));
				}
			}
		}
	}


	// Update the position, alpha clipping, and other properties of the area shading rectangles
	public void updateAreaShading(List<Vector2> newPositions) {
		if (areaShadingType == areaShadingTypes.None) return;
		if (areaShadingUsesComputeShader && areaShadingRects.Count == 1) { // all values (min, max, and the points) represent a percentage of graph
			WMG_Compute_Shader areaShadingCS = areaShadingRects[0].GetComponent<WMG_Compute_Shader>();

			areaShadingCS.computeShader.SetFloats ("color", new float[]{ areaShadingColor.r, areaShadingColor.g, areaShadingColor.b, areaShadingColor.a });
			areaShadingCS.computeShader.SetInt("numPoints", pointValues.Count);
			areaShadingCS.computeShader.SetInt("isFill", areaShadingType == areaShadingTypes.Solid ? 1 : 0);
			areaShadingCS.computeShader.SetInt("isHorizontal", theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal ? 1 : 0);
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
				areaShadingCS.computeShader.SetFloat ("minVal", (areaShadingAxisValue - theGraph.xAxis.AxisMinValue) / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue));
			}
			else {
				areaShadingCS.computeShader.SetFloat ("minVal", (areaShadingAxisValue - yAxis.AxisMinValue) / (yAxis.AxisMaxValue - yAxis.AxisMinValue));
			}
			float maxVal = 0;
			for (int i = 0; i < pointValues.Count; i++) {
				Vector2 pointPos = newPositions == null ? theGraph.getSpritePositionXY(points[i]) : newPositions[i];
				pointPos = new Vector2(pointPos.x / theGraph.xAxisLength, pointPos.y / theGraph.yAxisLength); 
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
					pointPos = new Vector2(pointPos.y, pointPos.x);
				}
				areaShadingCS.pointVals[4 * i] = pointPos.x;
				areaShadingCS.pointVals[4 * i + 1] = pointPos.y;
				if (pointPos.y > maxVal) {
					maxVal = pointPos.y;
				}
			}
			areaShadingCS.computeShader.SetFloat ("maxVal", maxVal);
			areaShadingCS.computeShader.SetFloats("pointVals", areaShadingCS.pointVals);

			areaShadingCS.dispatchAndUpdateImage();

		}
		else {
			// Find the maximum area shading height so that we can corectly adjust each sprites transparency based on their height in comparison to the max height
			float maxVal = Mathf.NegativeInfinity;
			for (int i = 0; i < points.Count; i++) {
				if (i >= pointValues.Count) break;
				if (pointValues[i].y > maxVal) {
					maxVal = pointValues[i].y;
				}
			}
			for (int i = 0; i < points.Count - 1; i++) {
				if (i >= pointValues.Count) break;
				
				int rotation = 180;
				Vector2 currentPointPosition = theGraph.getSpritePositionXY(points[i]);
				Vector2 nextPointPosition = theGraph.getSpritePositionXY(points[i+1]);
				float axisMultiplier = theGraph.yAxisLength / (yAxis.AxisMaxValue - yAxis.AxisMinValue);
				float yPosOfAxisVal = (areaShadingAxisValue - yAxis.AxisMinValue) * axisMultiplier;
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
					rotation = 90;
					currentPointPosition = new Vector2(theGraph.getSpritePositionY(points[i]), theGraph.getSpritePositionX(points[i]));
					nextPointPosition = new Vector2(theGraph.getSpritePositionY(points[i+1]), theGraph.getSpritePositionX(points[i+1]));
					axisMultiplier = theGraph.xAxisLength / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue);
					yPosOfAxisVal = (areaShadingAxisValue - theGraph.xAxis.AxisMinValue) * axisMultiplier;
				}
				
				areaShadingRects[i].transform.localEulerAngles = new Vector3(0, 0, rotation);
				float maxY = Mathf.Max(nextPointPosition.y, currentPointPosition.y);
				float minY = Mathf.Min(nextPointPosition.y, currentPointPosition.y);
				int newX = Mathf.RoundToInt(currentPointPosition.x);
				int newWidth = Mathf.RoundToInt(nextPointPosition.x - currentPointPosition.x);
				float newHeight = maxY - minY + ((Mathf.Min(pointValues[i+1].y, pointValues[i].y) - areaShadingAxisValue) * axisMultiplier ) ;
				
				// If areaShading value goes above a line segment, decrease the width appropriately
				if (minY < yPosOfAxisVal) {
					float slope = (nextPointPosition.y - currentPointPosition.y) / (nextPointPosition.x - currentPointPosition.x);
					// Slope increasing
					if (nextPointPosition.y > currentPointPosition.y) {
						float deltaY = yPosOfAxisVal - minY;
						int deltaX = Mathf.RoundToInt(deltaY / slope);
						newWidth -= deltaX;
						// Increase position by delta
						newX += deltaX;
					}
					else {
						float deltaY = yPosOfAxisVal - minY;
						int deltaX = Mathf.RoundToInt(deltaY / slope * -1);
						newWidth -= deltaX;
					}
				}
				
				
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
					theGraph.changeSpritePositionTo(areaShadingRects[i], new Vector3(maxY,
					                                                                 newX + newWidth, 0));
				}
				else {
					theGraph.changeSpritePositionTo(areaShadingRects[i], new Vector3(newX,
					                                                                 maxY, 0));
				}
				theGraph.changeSpriteSizeFloat(areaShadingRects[i], newWidth, newHeight);
				
				// Adjust previous sprite width based on previous width and position so that the sprites do not overlap due to position being a float and width being an integer
				if (i > 0) { // Don't need to adjust the width for the first rectangle
					if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
						int previousSpriteWidthPlusPosition = Mathf.RoundToInt(theGraph.getSpritePositionY(areaShadingRects[i])) 
							- Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[i]));
						int yPrevious = Mathf.RoundToInt(theGraph.getSpritePositionY(areaShadingRects[i-1]));
						// if y previous < y current - width current then increase current width by 1y 1
						if (previousSpriteWidthPlusPosition > yPrevious) {
							theGraph.changeSpriteWidth(areaShadingRects[i], Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[i]) + 1));
						}
						// if y previous > y current - width current then decrease current width by 1y 1
						if (previousSpriteWidthPlusPosition < yPrevious) {
							theGraph.changeSpriteWidth(areaShadingRects[i], Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[i]) - 1));
						}
					}
					else {
						int previousSpriteWidthPlusPosition = Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[i-1])) + Mathf.RoundToInt(theGraph.getSpritePositionX(areaShadingRects[i-1]));
						// If greater then sprites would overlap, subtract width by 1
						if (previousSpriteWidthPlusPosition > Mathf.RoundToInt(theGraph.getSpritePositionX(areaShadingRects[i]))) {
							theGraph.changeSpriteWidth(areaShadingRects[i-1], Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[i-1]) - 1));
						}
						// If lesser then sprites would have gap, increased width by 1
						if (previousSpriteWidthPlusPosition < Mathf.RoundToInt(theGraph.getSpritePositionX(areaShadingRects[i]))) {
							theGraph.changeSpriteWidth(areaShadingRects[i-1], Mathf.RoundToInt(theGraph.getSpriteWidth(areaShadingRects[i-1]) + 1));
						}
					}
				}
				
				// Set custom shader properties to do appropriate alpha clipping, gradient shading, color, etc.
				Material curMat = theGraph.getTextureMaterial(areaShadingRects[i]);
				if (curMat == null) continue;
				
				if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal) {
					curMat.SetFloat("_Slope", -(nextPointPosition.y - currentPointPosition.y) / newHeight);
				}
				else {
					curMat.SetFloat("_Slope", (nextPointPosition.y - currentPointPosition.y) / newHeight);
				}
				
				curMat.SetColor("_Color", areaShadingColor);
				curMat.SetFloat("_Transparency", 1 - areaShadingColor.a );
				// Set the gradient scale based on current sprite height in comparison to maximum sprite height
				curMat.SetFloat("_GradientScale", 
				                (Mathf.Max(pointValues[i+1].y, pointValues[i].y) - areaShadingAxisValue) / (maxVal - areaShadingAxisValue)
				                );
			}
		}
	}
	
	public void StartRealTimeUpdate() {
		if (realTimeRunning) return;
		if (realTimeDataSource != null) {
			realTimeRunning = true;
			pointValues.SetListNoCb(new List<Vector2>(), ref _pointValues);
			pointValues.AddNoCb(new Vector2(0, realTimeDataSource.getDatum<float>()), ref _pointValues);
			realTimeLoopVar = 0;
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
				realTimeOrigMax = theGraph.xAxis.AxisMaxValue;
			}
			else {
				realTimeOrigMax = yAxis.AxisMaxValue;
			}
		}
	}
	
	public void StopRealTimeUpdate() {
		realTimeRunning = false;
	}

	public void ResumeRealTimeUpdate() {
		realTimeRunning = true;
	}
	
	private void DoRealTimeUpdate() {
		/* This "Real Time" update is FPS dependent, so the time axis actually represents a number of frames.
		 * The waitForSeconds for coroutines does not actually wait for the specified number of seconds, and is also FPS dependent.
		 * An FPS independent solution only seems possible with fixedUpdate, which may be added later. */
		
		float waitTime = 0.0166f; // Each x-axis unit is 60 frames. This is 1 second at 60 fps.
		
		realTimeLoopVar += waitTime;
		
		float yval = realTimeDataSource.getDatum<float>();
		int sampleSize = 2;
		
		// Add new point or move the last existing point
		if (pointValues.Count >= 2) {
			float maxStdDev = 0.3f; // a standard deviation above this means generate a new point
			// use graph slope to normalize the calculated slopes.
			float graphSlope = (yAxis.AxisMaxValue - yAxis.AxisMinValue) / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue);
			float[] slopes = new float[sampleSize];
			Vector2 p1 = new Vector2(realTimeLoopVar, yval);

			for (int i = 0; i < slopes.Length; i++) {
				Vector2 p2 = pointValues[pointValues.Count-(i+1)];
				slopes[i] = ((p1.y - p2.y) / (p1.x - p2.x)) / graphSlope;
			}

			// For 2 points this is equivalent to the standard deviation
			if (Mathf.Abs(slopes[0]-slopes[1]) <= maxStdDev) { // if standard deviation less than threshold, then move point
				// Slopes about the same, move the last point
				pointValues[pointValues.Count-1] = new Vector2(realTimeLoopVar,yval);
			}
			else {
				// Slopes significantly different, add a new point
				pointValues.Add(new Vector2(realTimeLoopVar, yval));
			}
		}
		else {
			// Just add the second point
			pointValues.Add(new Vector2(realTimeLoopVar, yval));
		}
		
		// If needed, change graph axis boundary and remove or move the first point to keep the series within the graph boundaries
		if (pointValues.Count > 1 && pointValues[pointValues.Count-1].x > realTimeOrigMax) {
			
			// For the last real time update series update the axis boundaries by the difference
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
				theGraph.xAxis.AxisMinValue = realTimeLoopVar - realTimeOrigMax;
				theGraph.xAxis.AxisMaxValue = realTimeLoopVar;
			}
			else {
				yAxis.AxisMinValue = realTimeLoopVar - realTimeOrigMax;
				yAxis.AxisMaxValue = realTimeLoopVar;
			}
			
			// First and second points used to see if the first point should be moved or deleted after incrementing the minimum axis value
			float x1 = pointValues[0].x;
			float x2 = pointValues[1].x;
			float y1 = pointValues[0].y;
			float y2 = pointValues[1].y;
			
			// Delete or move the very first point to keep the series in the graph boundary when the maximum is increased
			if (Mathf.Approximately(x1 + waitTime, x2)) pointValues.RemoveAt(0);
			else pointValues[0] = new Vector2(x1 + waitTime, y1 + (y2 - y1) / (x2 - x1) * waitTime);
		}
	}
	
	public void deleteAllNodesFromGraphManager() {
		// This should not be called manually, only an internal helper function for dynamically deleting series
		for (int i = points.Count - 1; i >= 0; i--) {
			theGraph.DeleteNode(points[i].GetComponent<WMG_Node>());
		}
		theGraph.DeleteNode(legendEntry.nodeLeft.GetComponent<WMG_Node>());
		theGraph.DeleteNode(legendEntry.nodeRight.GetComponent<WMG_Node>());
		theGraph.DeleteNode(legendEntry.swatchNode.GetComponent<WMG_Node>());
	}
}