using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WMG_Axis_Graph : WMG_Graph_Manager {

	[SerializeField] public WMG_Axis yAxis;
	[SerializeField] public WMG_Axis xAxis;
	[SerializeField] public WMG_Axis yAxis2;

	public enum graphTypes {line, line_stacked, bar_side, bar_stacked, bar_stacked_percent, combo};
	public enum orientationTypes {vertical, horizontal};
	public enum axesTypes {MANUAL, CENTER, AUTO_ORIGIN, AUTO_ORIGIN_X, AUTO_ORIGIN_Y, I, II, III, IV, I_II, III_IV, II_III, I_IV, DUAL_Y};
	[System.Flags]
	public enum ResizeProperties {
		SeriesPointSize 		= 1 << 0,
		SeriesLineWidth 		= 1 << 1,
		SeriesDataLabelSize 	= 1 << 2,
		SeriesDataLabelOffset	= 1 << 3,
		LegendFontSize			= 1 << 4,
		LegendEntrySize			= 1 << 5,
		LegendOffset			= 1 << 6,
		AxesWidth	 			= 1 << 7,
		AxesLabelSize			= 1 << 8,
		AxesLabelOffset			= 1 << 9,
		AxesTitleSize			= 1 << 10,
		AxesLinePadding			= 1 << 11,
		AxesArrowSize			= 1 << 12,
		AutofitPadding			= 1 << 13,
		BorderPadding			= 1 << 14,
		TickSize				= 1 << 15
	}

	[SerializeField] private List<string> _groups;
	public WMG_List<string> groups = new WMG_List<string>();

	// public properties
	public graphTypes graphType { get {return _graphType;} 
		set {
			if (_graphType != value) {
				_graphType = value;
				graphTypeC.Changed();
				graphC.Changed();
				seriesCountC.Changed();
				legend.legendC.Changed();
			}
		}
	}
	public orientationTypes orientationType { get {return _orientationType;} 
		set {
			if (_orientationType != value) {
				if (axesType == axesTypes.DUAL_Y && value == orientationTypes.horizontal) {
					Debug.LogWarning("Cannot change orientation to horizontal for a dual-y axis chart");
					return;
				}
				_orientationType = value;
				orientationC.Changed();
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	public axesTypes axesType { get {return _axesType;} 
		set {
			if (_axesType != value) {
				if (orientationType == orientationTypes.horizontal && value == axesTypes.DUAL_Y) {
					Debug.LogWarning("Cannot change axes to dual-y for a horizontally oriented chart");
					return;
				}
				if (yAxis2 == null && value == axesTypes.DUAL_Y) {
					Debug.LogWarning("Cannot change axes to dual-y without setting up a second y axis");
					return;
				}
				_axesType = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	public bool resizeEnabled { get {return _resizeEnabled;} 
		set {
			if (_resizeEnabled != value) {
				_resizeEnabled = value;
				resizeC.Changed();
			}
		}
	}
	public ResizeProperties resizeProperties { get {return _resizeProperties;} 
		set {
			if (_resizeProperties != value) {
				_resizeProperties = value;
				resizeC.Changed();
			}
		}
	}
	public bool useGroups { get {return _useGroups;} 
		set {
			if (_useGroups != value) {
				_useGroups = value;
				graphC.Changed();
			}
		}
	}
	public Vector2 paddingLeftRight { get {return _paddingLeftRight;} 
		set {
			if (_paddingLeftRight != value) {
				_paddingLeftRight = value;
				graphC.Changed();
				seriesCountC.Changed();
				legend.legendC.Changed();
			}
		}
	}
	public Vector2 paddingTopBottom { get {return _paddingTopBottom;} 
		set {
			if (_paddingTopBottom != value) {
				_paddingTopBottom = value;
				graphC.Changed();
				seriesCountC.Changed();
				legend.legendC.Changed();
			}
		}
	}
	public Vector2 theOrigin { get {return _theOrigin;} 
		set {
			if (_theOrigin != value) {
				_theOrigin = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	public float barWidth { get {return _barWidth;} 
		set {
			if (_barWidth != value) {
				_barWidth = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	public float barAxisValue { get {return _barAxisValue;} 
		set {
			if (_barAxisValue != value) {
				_barAxisValue = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	public bool autoUpdateOrigin { get {return _autoUpdateOrigin;} 
		set {
			if (_autoUpdateOrigin != value) {
				_autoUpdateOrigin = value;
				graphC.Changed();
			}
		}
	}
	public bool autoUpdateBarWidth { get {return _autoUpdateBarWidth;} 
		set {
			if (_autoUpdateBarWidth != value) {
				_autoUpdateBarWidth = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	public float autoUpdateBarWidthSpacing { get {return _autoUpdateBarWidthSpacing;} 
		set {
			if (_autoUpdateBarWidthSpacing != value) {
				_autoUpdateBarWidthSpacing = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	public bool autoUpdateSeriesAxisSpacing { get {return _autoUpdateSeriesAxisSpacing;} 
		set {
			if (_autoUpdateSeriesAxisSpacing != value) {
				_autoUpdateSeriesAxisSpacing = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	public bool autoUpdateBarAxisValue { get {return _autoUpdateBarAxisValue;} 
		set {
			if (_autoUpdateBarAxisValue != value) {
				_autoUpdateBarAxisValue = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	//[System.Obsolete("This is obsolete")]
	public int axisWidth { get {return _axisWidth;} 
		set {
			if (_axisWidth != value) {
				_axisWidth = value;
				graphC.Changed();
			}
		}
	}
	public float autoShrinkAtPercent { get {return _autoShrinkAtPercent;} 
		set {
			if (_autoShrinkAtPercent != value) {
				_autoShrinkAtPercent = value;
				graphC.Changed();
			}
		}
	}
	public float autoGrowAndShrinkByPercent { get {return _autoGrowAndShrinkByPercent;} 
		set {
			if (_autoGrowAndShrinkByPercent != value) {
				_autoGrowAndShrinkByPercent = value;
				graphC.Changed();
			}
		}
	}
	public bool tooltipEnabled { get {return _tooltipEnabled;} 
		set {
			if (_tooltipEnabled != value) {
				_tooltipEnabled = value;
				tooltipEnabledC.Changed();
			}
		}
	}
	public bool autoAnimationsEnabled { get {return _autoAnimationsEnabled;} 
		set {
			if (_autoAnimationsEnabled != value) {
				_autoAnimationsEnabled = value;
				autoAnimEnabledC.Changed();
			}
		}
	}
	public bool autoFitLabels { get {return _autoFitLabels;} 
		set {
			if (_autoFitLabels != value) {
				_autoFitLabels = value;
				graphC.Changed();
			}
		}
	}
	public float autoFitPadding { get {return _autoFitPadding;} 
		set {
			if (_autoFitPadding != value) {
				_autoFitPadding = value;
				graphC.Changed();
			}
		}
	}
	public Vector2 tickSize { get {return _tickSize;} 
		set {
			if (_tickSize != value) {
				_tickSize = value;
				graphC.Changed();
			}
		}
	}
	public string graphTitleString { get {return _graphTitleString;} 
		set {
			if (_graphTitleString != value) {
				_graphTitleString = value;
				graphC.Changed();
			}
		}
	}
	public Vector2 graphTitleOffset { get {return _graphTitleOffset;} 
		set {
			if (_graphTitleOffset != value) {
				_graphTitleOffset = value;
				graphC.Changed();
			}
		}
	}

	// Public variables without change tracking
	public Vector2 tooltipOffset;
	public int tooltipNumberDecimals;
	public bool tooltipDisplaySeriesName;
	public bool tooltipAnimationsEnabled;
	public Ease tooltipAnimationsEasetype;
	public float tooltipAnimationsDuration;
	public Ease autoAnimationsEasetype;
	public float autoAnimationsDuration;
	public List<GameObject> lineSeries;
	public List<Object> pointPrefabs;
	public List<Object> linkPrefabs;
	public Object barPrefab;
	public Object seriesPrefab;
	public WMG_Legend legend;
	public GameObject graphTitle;
	public GameObject graphBackground;
	public GameObject anchoredParent;
	public GameObject seriesParent;
	public GameObject toolTipPanel;
	public GameObject toolTipLabel;

	// Private backing variables
	[SerializeField] private graphTypes _graphType;
	[SerializeField] private orientationTypes _orientationType;
	[SerializeField] private axesTypes _axesType;
	[SerializeField] private bool _resizeEnabled;
	[WMG_EnumFlagAttribute] [SerializeField] private ResizeProperties _resizeProperties;
	[SerializeField] private bool _useGroups;
	[SerializeField] private Vector2 _paddingLeftRight;
	[SerializeField] private Vector2 _paddingTopBottom;
	[SerializeField] private Vector2 _theOrigin;
	[SerializeField] private float _barWidth;
	[SerializeField] private float _barAxisValue;
	[SerializeField] private bool _autoUpdateOrigin;
	[SerializeField] private bool _autoUpdateBarWidth;
	[SerializeField] private float _autoUpdateBarWidthSpacing;
	[SerializeField] private bool _autoUpdateSeriesAxisSpacing;
	[SerializeField] private bool _autoUpdateBarAxisValue;
	[SerializeField] private int _axisWidth;
	[SerializeField] private float _autoShrinkAtPercent;
	[SerializeField] private float _autoGrowAndShrinkByPercent;
	[SerializeField] private bool _tooltipEnabled;
	[SerializeField] private bool _autoAnimationsEnabled;
	[SerializeField] private bool _autoFitLabels;
	[SerializeField] private float _autoFitPadding;
	[SerializeField] private Vector2 _tickSize;
	[SerializeField] private string _graphTitleString;
	[SerializeField] private Vector2 _graphTitleOffset;

	// Useful property getters
	public float xAxisLength { 
		get {
			return getSpriteWidth(this.gameObject) - paddingLeftRight.x - paddingLeftRight.y;
		}
	}
	
	public float yAxisLength { 
		get {
			return getSpriteHeight(this.gameObject) - paddingTopBottom.x - paddingTopBottom.y;
		}
	}

	// Useful property getters
	public bool IsStacked { 
		get {
			return (graphType == WMG_Axis_Graph.graphTypes.bar_stacked 
			        || graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent
			        || graphType == WMG_Axis_Graph.graphTypes.line_stacked);
		}
	}

	// Private variables
	private List<float> totalPointValues = new List<float>();
	private int maxSeriesPointCount;
	private int maxSeriesBarCount;
	private int numComboBarSeries;

	public int NumComboBarSeries() {
		return numComboBarSeries;
	}

	// Original property values for use with dynamic resizing
	private float origWidth;
	private float origHeight;
	private float origBarWidth;
	private float origAxisWidth;
	private float origAutoFitPadding;
	private Vector2 origTickSize;
	private Vector2 origPaddingLeftRight;
	private Vector2 origPaddingTopBottom;

	// Cache
	private float cachedContainerWidth;
	private float cachedContainerHeight;

	// Other private variables
	public WMG_Graph_Tooltip theTooltip;
	private WMG_Graph_Auto_Anim autoAnim;

	public bool _autoFitting { get; set; }

	private bool hasInit;
	
	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	public WMG_Change_Obj graphC = new WMG_Change_Obj();
	public WMG_Change_Obj resizeC = new WMG_Change_Obj();
	public WMG_Change_Obj seriesCountC = new WMG_Change_Obj();
	public WMG_Change_Obj seriesNoCountC = new WMG_Change_Obj();
	private WMG_Change_Obj tooltipEnabledC = new WMG_Change_Obj();
	private WMG_Change_Obj autoAnimEnabledC = new WMG_Change_Obj();
	private WMG_Change_Obj orientationC = new WMG_Change_Obj();
	private WMG_Change_Obj graphTypeC = new WMG_Change_Obj();

	public delegate void GraphBackgroundChangedHandler(WMG_Axis_Graph aGraph);
	public event GraphBackgroundChangedHandler GraphBackgroundChanged;

	protected virtual void OnGraphBackgroundChanged() {
		GraphBackgroundChangedHandler handler = GraphBackgroundChanged;
		if (handler != null) {
			handler(this);
		}
	}

	void Start() {
		Init ();
		PauseCallbacks();
		AllChanged();
	}
	
	public void Init() {
		if (hasInit) return;
		hasInit = true;

		changeObjs.Add(orientationC);
		changeObjs.Add(graphTypeC);
		changeObjs.Add(graphC);
		changeObjs.Add(resizeC);
		changeObjs.Add(seriesCountC);
		changeObjs.Add(seriesNoCountC);
		changeObjs.Add(tooltipEnabledC);
		changeObjs.Add(autoAnimEnabledC);

		legend.Init ();
		xAxis.Init (yAxis, yAxis2, false, false);
		yAxis.Init (xAxis, null, true, false);
		if (yAxis2 != null) {
			yAxis2.Init (xAxis, null, true, true);
		}

		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
			theSeries.Init(j);
		}
		
		theTooltip = this.gameObject.AddComponent<WMG_Graph_Tooltip>(); // Add tooltip script
		theTooltip.hideFlags = HideFlags.HideInInspector; // Don't show tooltip script
		theTooltip.theGraph = this; // Set tooltip graph
		if (tooltipEnabled) theTooltip.subscribeToEvents(true);
		autoAnim = this.gameObject.AddComponent<WMG_Graph_Auto_Anim>(); // Add automatic animations script
		autoAnim.hideFlags = HideFlags.HideInInspector; // Don't show automatic animations script
		autoAnim.theGraph = this; // Set automatic animations graph
		if (autoAnimationsEnabled) autoAnim.subscribeToEvents(true);
		
		groups.SetList (_groups);
		groups.Changed += groupsChanged;
		
		graphTypeC.OnChange += GraphTypeChanged;
		tooltipEnabledC.OnChange += TooltipEnabledChanged;
		autoAnimEnabledC.OnChange += AutoAnimationsEnabledChanged;
		orientationC.OnChange += OrientationChanged;
		resizeC.OnChange += ResizeChanged;
		graphC.OnChange += GraphChanged;
		seriesCountC.OnChange += SeriesCountChanged;
		seriesNoCountC.OnChange += SeriesNoCountChanged;

		setOriginalPropertyValues();
		PauseCallbacks();
	}

	void Update () {
		updateFromDataSource();
		updateFromResize();

		Refresh();
	}

	public void Refresh() {
		ResumeCallbacks();
		PauseCallbacks();
	}

	public void ManualResize() {
		PauseCallbacks();
		resizeEnabled = true;
		UpdateFromContainer();
		resizeEnabled = false;
		ResumeCallbacks();
	}

	void PauseCallbacks() {
		yAxis.PauseCallbacks();
		if (axesType == axesTypes.DUAL_Y) {
			yAxis2.PauseCallbacks();
		}
		xAxis.PauseCallbacks();
		for (int i = 0; i < changeObjs.Count; i++) {
			changeObjs[i].changesPaused = true;
			changeObjs[i].changePaused = false;
		}
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
			theSeries.PauseCallbacks();
		}
		legend.PauseCallbacks();
	}
	
	void ResumeCallbacks() {
		yAxis.ResumeCallbacks();
		if (axesType == axesTypes.DUAL_Y) {
			yAxis2.ResumeCallbacks();
		}
		xAxis.ResumeCallbacks();
		for (int i = 0; i < changeObjs.Count; i++) {
			changeObjs[i].changesPaused = false;
			if (changeObjs[i].changePaused) changeObjs[i].Changed();
		}
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
			theSeries.ResumeCallbacks();
		}
		legend.ResumeCallbacks();
	}

	void updateFromResize() {
		bool resizeChanged = false;
		updateCacheAndFlag<float>(ref cachedContainerWidth, getSpriteWidth(this.gameObject), ref resizeChanged);
		updateCacheAndFlag<float>(ref cachedContainerHeight, getSpriteHeight(this.gameObject), ref resizeChanged);
		if (resizeChanged) {
			resizeC.Changed();
			graphC.Changed();
			seriesNoCountC.Changed();
			legend.legendC.Changed();
		}
	}

	void updateFromDataSource() {
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
			theSeries.UpdateFromDataSource();
			theSeries.RealTimeUpdate();
		}
	}

	void OrientationChanged() {
		UpdateOrientation();
	}

	void TooltipEnabledChanged() {
		UpdateTooltip();
	}

	void AutoAnimationsEnabledChanged() {
		UpdateAutoAnimEvents();
	}

	void ResizeChanged() {
		UpdateFromContainer();
	}

	void AllChanged() {
		graphC.Changed();
		seriesCountC.Changed();
		legend.legendC.Changed();
	}

	void GraphTypeChanged() {
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
			theSeries.prefabC.Changed();
		}
	}
	
	public void SeriesChanged(bool countChanged, bool instant) {
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
			if (countChanged) {
				if (instant) {
					theSeries.pointValuesCountChanged();
				}
				else {
					theSeries.pointValuesCountC.Changed();
				}
			}
			else {
				if (instant) {
					theSeries.pointValuesChanged();
				}
				else {
					theSeries.pointValuesC.Changed();
				}
			}
		}
	}

	void SeriesCountChanged() {
		SeriesChanged(true, false);
	}

	void SeriesNoCountChanged() {
		SeriesChanged(false, false);
	}

	public void aSeriesPointsChanged() {
		if (!Application.isPlaying) return;
		UpdateTotals ();
		UpdateBarWidth ();
		UpdateAxesMinMaxValues ();
	}

	public void GraphChanged() {

		// Update total point values used in stacked charts, and max series point count
		UpdateTotals();
		
		// Update bar width
		UpdateBarWidth();
		
		// Auto update Axes Min Max values based on grow and shrink booleans
		UpdateAxesMinMaxValues();
		
		// Update axes quadrant and related boolean variables such as which arrows appear
		UpdateAxesType();
		
		// Update visuals of axes, grids, and ticks
		UpdateAxesGridsAndTicks();
		
		// Update position and text of axes labels which might be based off max / min values or percentages for stacked percentage bar
		UpdateAxesLabels();
		
		// Update Line Series Parents
		UpdateSeriesParentPositions();
		
		// Update background sprite
		UpdateBG();
		
		// Update Titles
		UpdateTitles();
	}

	private void groupsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref groups, ref _groups, oneValChanged, index);
		graphC.Changed();
		if (oneValChanged) {
			seriesNoCountC.Changed();
		}
		else {
			seriesCountC.Changed();
		}
	}

	// Set initial property values for use with percentage based dynamic resizing 
	public void setOriginalPropertyValues() {
		cachedContainerWidth = getSpriteWidth(this.gameObject);
		cachedContainerHeight = getSpriteHeight(this.gameObject);
		origWidth = getSpriteWidth(this.gameObject);
		origHeight = getSpriteHeight(this.gameObject);
		origBarWidth = barWidth;
		origAxisWidth = axisWidth;
		origAutoFitPadding = autoFitPadding;
		origTickSize = tickSize;
		origPaddingLeftRight = paddingLeftRight;
		origPaddingTopBottom = paddingTopBottom;
	}

	void UpdateOrientation() {
		yAxis.ChangeOrientation();

		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
			theSeries.origDataLabelOffset = new Vector2(theSeries.origDataLabelOffset.y, theSeries.origDataLabelOffset.x);
			theSeries.dataLabelsOffset = new Vector2(theSeries.dataLabelsOffset.y, theSeries.dataLabelsOffset.x);
			theSeries.setAnimatingFromPreviousData(); // If automatic animations set, then set flag to animate for each series
			if (theSeries.dataLabelsEnabled) {
				if (orientationType == orientationTypes.horizontal) {
					foreach (GameObject labelgo in theSeries.getDataLabels()) {
						changeSpritePivot(labelgo, WMG_Graph_Manager.WMGpivotTypes.Left);
					}
				}
				else {
					foreach (GameObject labelgo in theSeries.getDataLabels()) {
						changeSpritePivot(labelgo, WMG_Graph_Manager.WMGpivotTypes.Bottom);
					}
				}
			}
		}
	}
	
	void UpdateAxesType() {
		 // toggle visibility of all objects related to secondary y axis based on axesType
		if (yAxis2 != null) {
			yAxis2.AxisTitle.SetActive(axesType == axesTypes.DUAL_Y);
			yAxis2.AxisTicks.SetActive(axesType == axesTypes.DUAL_Y);
			yAxis2.AxisLine.SetActive(axesType == axesTypes.DUAL_Y);
			yAxis2.AxisLabelObjs.SetActive(axesType == axesTypes.DUAL_Y);
		}

		if (axesType == axesTypes.MANUAL) {
			// Don't do anything with the axes position related variables
		}
		else if (axesType == axesTypes.AUTO_ORIGIN || axesType == axesTypes.AUTO_ORIGIN_X || axesType == axesTypes.AUTO_ORIGIN_Y) {
			// Automatically position axes relative to the origin
			updateAxesRelativeToOrigin();
		}
		else {
			// Automatically position origin relative to the axes
			updateOriginRelativeToAxes();
			// These are the static axes types (axes dont change based on the min and max values)
			if (axesType == axesTypes.I || axesType == axesTypes.II || axesType == axesTypes.III || axesType == axesTypes.IV || axesType == axesTypes.DUAL_Y) {
				// These axes types should always position based on the edge
				if (axesType == axesTypes.I) {
					setAxesQuadrant1();
				}
				else if (axesType == axesTypes.II) {
					setAxesQuadrant2();
				}
				else if (axesType == axesTypes.III) {
					setAxesQuadrant3();
				}
				else if (axesType == axesTypes.IV) {
					setAxesQuadrant4();
				}
				else if (axesType == axesTypes.DUAL_Y) {
					setAxesDualYaxis();
				}
			}
			else {
				// These axes types may not necessarily have an axis on the edge
				if (axesType == axesTypes.CENTER) {
					setAxesQuadrant1_2_3_4();
				}
				else if (axesType == axesTypes.I_II) {
					setAxesQuadrant1_2();
				}
				else if (axesType == axesTypes.III_IV) {
					setAxesQuadrant3_4();
				}
				else if (axesType == axesTypes.II_III) {
					setAxesQuadrant2_3();
				}
				else if (axesType == axesTypes.I_IV) {
					setAxesQuadrant1_4();
				}

				yAxis.possiblyHideTickBasedOnPercent();
				xAxis.possiblyHideTickBasedOnPercent();

			}
		}
	}

	void updateOriginRelativeToAxes() {
		if (autoUpdateOrigin) {
			if (axesType == axesTypes.I || axesType == axesTypes.DUAL_Y) {
				_theOrigin = new Vector2(xAxis.AxisMinValue, yAxis.AxisMinValue);
			}
			else if (axesType == axesTypes.II) {
				_theOrigin = new Vector2(xAxis.AxisMaxValue, yAxis.AxisMinValue);
			}
			else if (axesType == axesTypes.III) {
				_theOrigin = new Vector2(xAxis.AxisMaxValue, yAxis.AxisMaxValue);
			}
			else if (axesType == axesTypes.IV) {
				_theOrigin = new Vector2(xAxis.AxisMinValue, yAxis.AxisMaxValue);
			}
			else if (axesType == axesTypes.CENTER) {
				_theOrigin = new Vector2((xAxis.AxisMaxValue + xAxis.AxisMinValue) / 2, (yAxis.AxisMaxValue + yAxis.AxisMinValue) / 2);
			}
			else if (axesType == axesTypes.I_II) {
				_theOrigin = new Vector2((xAxis.AxisMaxValue + xAxis.AxisMinValue) / 2, yAxis.AxisMinValue);
			}
			else if (axesType == axesTypes.III_IV) {
				_theOrigin = new Vector2((xAxis.AxisMaxValue + xAxis.AxisMinValue) / 2, yAxis.AxisMaxValue);
			}
			else if (axesType == axesTypes.II_III) {
				_theOrigin = new Vector2(xAxis.AxisMaxValue, (yAxis.AxisMaxValue + yAxis.AxisMinValue) / 2);
			}
			else if (axesType == axesTypes.I_IV) {
				_theOrigin = new Vector2(xAxis.AxisMinValue, (yAxis.AxisMaxValue + yAxis.AxisMinValue) / 2);
			}
		}
		if (autoUpdateBarAxisValue) {
			if (orientationType == orientationTypes.vertical) {
				_barAxisValue = theOrigin.y;
			}
			else {
				_barAxisValue = theOrigin.x;
			}
		}
	}
	
	void updateAxesRelativeToOrigin() {
		yAxis.updateAxesRelativeToOrigin(theOrigin.x);
		xAxis.updateAxesRelativeToOrigin(theOrigin.y);
		if (autoUpdateBarAxisValue) {
			if (orientationType == orientationTypes.vertical) {
				_barAxisValue = theOrigin.y;
			}
			else {
				_barAxisValue = theOrigin.x;
			}
		}
	}
	
	void UpdateAxesMinMaxValues() {
		yAxis.UpdateAxesMinMaxValues();
		if (axesType == axesTypes.DUAL_Y) {
			yAxis2.UpdateAxesMinMaxValues();
		}
		xAxis.UpdateAxesMinMaxValues();
	}

	void UpdateAxesGridsAndTicks() {
		yAxis.UpdateAxesGridsAndTicks();
		if (axesType == axesTypes.DUAL_Y) {
			yAxis2.UpdateAxesGridsAndTicks();
		}
		xAxis.UpdateAxesGridsAndTicks();
	}


	void UpdateAxesLabels() {
		yAxis.UpdateAxesLabels ();
		if (axesType == axesTypes.DUAL_Y) {
			yAxis2.UpdateAxesLabels ();
		}
		xAxis.UpdateAxesLabels ();
		yAxis.AutofitAxesLabels ();
		if (axesType == axesTypes.DUAL_Y) {
			yAxis2.AutofitAxesLabels ();
		}
		xAxis.AutofitAxesLabels ();
	}


	void UpdateSeriesParentPositions () {
		int prevComboBarSeries = -1;
		bool existsComboBar = false;
		int totalNumberComboBars = 0;
		if (graphType == graphTypes.combo) {
			for (int j = 0; j < lineSeries.Count; j++) {
				if (!activeInHierarchy(lineSeries[j])) continue;
				WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
				if (theSeries.comboType == WMG_Series.comboTypes.bar) {
					existsComboBar = true;
					totalNumberComboBars++;
				}
			}
		}

		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
			Vector2 axisWidthOffset = getAxesOffsetFactor();
			axisWidthOffset = new Vector2(-axisWidth/2 * axisWidthOffset.x, -axisWidth/2 * axisWidthOffset.y);

			if (theSeries.seriesIsLine) {
				if (theSeries.ManuallySetExtraXSpace) {
					changeSpritePositionTo(lineSeries[j], new Vector3(theSeries.extraXSpace, 0, 0));
					continue;
				}
				changeSpritePositionTo(lineSeries[j], new Vector3(0, 0, 0));
			}
			else {
				if (orientationType == orientationTypes.vertical) {
					if (theSeries.ManuallySetExtraXSpace) {
						changeSpritePositionTo(lineSeries[j], new Vector3(theSeries.extraXSpace, axisWidthOffset.y, 0));
						continue;
					}
					changeSpritePositionTo(lineSeries[j], new Vector3(axisWidthOffset.x, axisWidthOffset.y, 0));
				}
				else {
					if (theSeries.ManuallySetExtraXSpace) {
						changeSpritePositionTo(lineSeries[j], new Vector3(theSeries.extraXSpace, axisWidthOffset.y + barWidth, 0));
						continue;
					}
					changeSpritePositionTo(lineSeries[j], new Vector3(axisWidthOffset.x, axisWidthOffset.y + barWidth, 0));
				}
			}
			
			// Update spacing between series
			if (graphType == graphTypes.bar_side) {
				if (j > 0) {
					if (orientationType == orientationTypes.vertical) {
						changeSpritePositionRelativeToObjByX(lineSeries[j], lineSeries[j-1], barWidth);
					}
					else {
						changeSpritePositionRelativeToObjByY(lineSeries[j], lineSeries[j-1], barWidth);
					}
				}
			}
			else if (graphType == graphTypes.combo) {
				if (j > 0) {
					if (lineSeries[j-1].GetComponent<WMG_Series>().comboType == WMG_Series.comboTypes.bar) {
						prevComboBarSeries = j-1;
					}
					if (prevComboBarSeries > -1 && lineSeries[j].GetComponent<WMG_Series>().comboType == WMG_Series.comboTypes.bar) {
						if (orientationType == orientationTypes.vertical) {
							changeSpritePositionRelativeToObjByX(lineSeries[j], lineSeries[prevComboBarSeries], barWidth);
						}
						else {
							changeSpritePositionRelativeToObjByY(lineSeries[j], lineSeries[prevComboBarSeries], barWidth);
						}
					}
					if (existsComboBar && lineSeries[j].GetComponent<WMG_Series>().comboType == WMG_Series.comboTypes.line) {
						changeSpritePositionRelativeToObjByX(lineSeries[j], lineSeries[0], barWidth/2 * totalNumberComboBars);
					}
				}
			}
			else {
				if (j > 0) {
					if (orientationType == orientationTypes.vertical) {
						changeSpritePositionRelativeToObjByX(lineSeries[j], lineSeries[0], 0);
					}
					else {
						changeSpritePositionRelativeToObjByY(lineSeries[j], lineSeries[0], 0);
					}
				}
			}
		}
	}

	public void UpdateBG() {
		changeSpriteSize(graphBackground, Mathf.RoundToInt(getSpriteWidth(this.gameObject)), Mathf.RoundToInt(getSpriteHeight(this.gameObject)));
		changeSpritePositionTo (graphBackground, new Vector3 (-paddingLeftRight.x, -paddingTopBottom.y, 0));
		changeSpriteSize(anchoredParent, Mathf.RoundToInt(getSpriteWidth(this.gameObject)), Mathf.RoundToInt(getSpriteHeight(this.gameObject)));
		changeSpritePositionTo (anchoredParent, new Vector3 (-paddingLeftRight.x, -paddingTopBottom.y, 0));
		UpdateBGandSeriesParentPositions(cachedContainerWidth, cachedContainerHeight);
		OnGraphBackgroundChanged();
	}

	public void UpdateBGandSeriesParentPositions (float x, float y) {
		Vector2 pivot = getSpritePivot(this.gameObject);
		Vector3 newChildPos = new Vector3(-x * pivot.x + paddingLeftRight.x, 
		                                  -y * pivot.y + paddingTopBottom.y);
		changeSpritePositionTo(graphBackground.transform.parent.gameObject, newChildPos);
		changeSpritePositionTo(seriesParent, newChildPos);
	}

	void UpdateTotals() {
		// Find max number points
		int maxNumValues = 0;
		int maxNumBars = 0;
		numComboBarSeries = 0;
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
			if (maxNumValues < theSeries.pointValues.Count) maxNumValues = theSeries.pointValues.Count;
			if (graphType == graphTypes.combo && theSeries.comboType == WMG_Series.comboTypes.bar) {
				numComboBarSeries++;
			    if (maxNumBars < theSeries.pointValues.Count) {
					maxNumBars = theSeries.pointValues.Count;
				}
			}
		}
		// Update max series point count
		maxSeriesPointCount = maxNumValues;
		maxSeriesBarCount = maxNumBars;
		// Update total values
		for (int i = 0; i < maxNumValues; i++) {
			if (totalPointValues.Count <= i) {
				totalPointValues.Add(0);
			}
			totalPointValues[i] = 0;
			for (int j = 0; j < lineSeries.Count; j++) {
				if (!activeInHierarchy(lineSeries[j])) continue;
				WMG_Series theSeries = lineSeries[j].GetComponent<WMG_Series>();
				if (theSeries.pointValues.Count > i) {
					if (orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
						totalPointValues[i] += (theSeries.pointValues[i].y - theSeries.yAxis.AxisMinValue);
					}
					else {
						totalPointValues[i] += (theSeries.pointValues[i].y - xAxis.AxisMinValue);
					}
				}
			}
		}
	}

	void UpdateBarWidth() {
		if (autoUpdateBarWidth) { // Don't do anything if not auto updating bar width
			if (graphType == graphTypes.line || graphType == graphTypes.line_stacked) return;
			
			float axisLength = xAxisLength;
			if (orientationType == orientationTypes.horizontal) {
				axisLength = yAxisLength;
			}
			
			int numBars = (maxSeriesPointCount * lineSeries.Count) + 1;
			
			if (graphType == graphTypes.combo) {
				numBars = (maxSeriesBarCount * numComboBarSeries) +1;
			}
			
			if (graphType == graphTypes.bar_stacked || graphType == graphTypes.bar_stacked_percent) {
				numBars = maxSeriesPointCount;
			}
			
			// ensure a percentage
			_autoUpdateBarWidthSpacing = Mathf.Clamp01(autoUpdateBarWidthSpacing);
			
			barWidth = (1 - autoUpdateBarWidthSpacing) * (axisLength - maxSeriesPointCount) / numBars;
		}
		
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series aSeries = lineSeries[j].GetComponent<WMG_Series>();
			aSeries.updateXdistBetween();
			aSeries.updateExtraXSpace();
		}
		
		UpdateSeriesParentPositions();
	}

	void UpdateTitles() {
		if (graphTitle != null) {
			changeLabelText(graphTitle, graphTitleString);
			changeSpritePositionTo(graphTitle, new Vector3(xAxisLength / 2 + graphTitleOffset.x, yAxisLength + graphTitleOffset.y));
		}
		yAxis.UpdateTitle();
		if (axesType == axesTypes.DUAL_Y) {
			yAxis2.UpdateTitle();
		}
		xAxis.UpdateTitle();
	}
	
	void UpdateTooltip() {
		// Add or remove tooltip events
		theTooltip.subscribeToEvents(tooltipEnabled);
	}
	
	void UpdateAutoAnimEvents() {
		// Add or remove automatic animation events
		autoAnim.subscribeToEvents(autoAnimationsEnabled);
	}
	
	public List<float> TotalPointValues {
		get { return totalPointValues; }
	}

	public float getDistBetween(int pointsCount, float theAxisLength) {

		float xDistBetweenPoints = 0;
		if ((pointsCount - 1) <= 0) {
			xDistBetweenPoints = theAxisLength;
			if (graphType == WMG_Axis_Graph.graphTypes.bar_side) {
				xDistBetweenPoints -= (lineSeries.Count * barWidth);
			}
			else if (graphType == graphTypes.combo) {
				xDistBetweenPoints -= (numComboBarSeries * barWidth);
			}
			else if (graphType == WMG_Axis_Graph.graphTypes.bar_stacked) {
				xDistBetweenPoints -= barWidth;
			}
			else if (graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent) {
				xDistBetweenPoints -= barWidth;
			}
		}
		else {

			int numPoints = (pointsCount - 1);
			if (graphType != graphTypes.line && graphType != graphTypes.line_stacked) numPoints += 1;

			xDistBetweenPoints = theAxisLength / numPoints;
			if (graphType == WMG_Axis_Graph.graphTypes.bar_side) {
				xDistBetweenPoints -= (lineSeries.Count * barWidth) / numPoints;
			}
			else if (graphType == graphTypes.combo) {
				xDistBetweenPoints -= (numComboBarSeries * barWidth) / numPoints;
			}
			else if (graphType == WMG_Axis_Graph.graphTypes.bar_stacked) {
				xDistBetweenPoints -= barWidth / numPoints;
			}
			else if (graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent) {
				xDistBetweenPoints -= barWidth / numPoints;
			}
		}
		return xDistBetweenPoints;
	}

	[System.Obsolete("Use xAxis.GetAxisTickNodes")]
	public List<WMG_Node> getXAxisTicks() {
		return xAxis.GetAxisTickNodes();
	}

	[System.Obsolete("Use xAxis.GetAxisLabelNodes")]
	public List<WMG_Node> getXAxisLabels() {
		return xAxis.GetAxisLabelNodes();
	}

	[System.Obsolete("Use yAxis.GetAxisTickNodes")]
	public List<WMG_Node> getYAxisTicks() {
		return yAxis.GetAxisTickNodes();
	}

	[System.Obsolete("Use yAxis.GetAxisLabelNodes")]
	public List<WMG_Node> getYAxisLabels() {
		return yAxis.GetAxisLabelNodes();
	}
	
	public void changeAllLinePivots(WMGpivotTypes newPivot) {
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series aSeries = lineSeries[j].GetComponent<WMG_Series>();
			List<GameObject> lines = aSeries.getLines();
			for (int i = 0; i < lines.Count; i++) {
				changeSpritePivot(lines[i], newPivot);
				WMG_Link aLink = lines[i].GetComponent<WMG_Link>();
				aLink.Reposition();
			}
		}
	}
	
	public List<Vector3> getSeriesScaleVectors(bool useLineWidthForX, float x, float y) {
		List<Vector3> results = new List<Vector3>();
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series aSeries = lineSeries[j].GetComponent<WMG_Series>();
			if (useLineWidthForX) {
				results.Add(new Vector3(aSeries.lineScale, y, 1));
			}
			else {
				results.Add(new Vector3(x, y, 1));
			}
		}
		return results;
	}

	public float getMaxPointSize() {
		if (graphType == graphTypes.line || graphType == graphTypes.line_stacked || (graphType == graphTypes.combo && numComboBarSeries == 0)) {
			float size = 0;
			for (int j = 0; j < lineSeries.Count; j++) {
				if (!activeInHierarchy(lineSeries[j])) continue;
				WMG_Series aSeries = lineSeries[j].GetComponent<WMG_Series>();
				if (aSeries.pointWidthHeight > size) size = aSeries.pointWidthHeight;
			}
			return size;
		}
		else {
			float size = barWidth;
			if (graphType == graphTypes.combo) {
				for (int j = 0; j < lineSeries.Count; j++) {
					if (!activeInHierarchy(lineSeries[j])) continue;
					WMG_Series aSeries = lineSeries[j].GetComponent<WMG_Series>();
					if (aSeries.comboType == WMG_Series.comboTypes.line && aSeries.pointWidthHeight > size) size = aSeries.pointWidthHeight;
				}
			}
			return size;
		}
	}

	public int getMaxNumPoints() {
		return maxSeriesPointCount;
	}

	public void setAxesDualYaxis() {
		xAxis.setDualYAxes();
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
		yAxis2.anchorVec = new Vector2 (1, 0.5f);
	}
	
	public void setAxesQuadrant1() {
		xAxis.setAxisTopRight(false);
		yAxis.setAxisTopRight(false);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	public void setAxesQuadrant2() {
		xAxis.setAxisBotLeft(false);
		yAxis.setAxisTopRight(true);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (1, 0.5f);
	}
	
	public void setAxesQuadrant3() {
		xAxis.setAxisBotLeft(true);
		yAxis.setAxisBotLeft(true);
		xAxis.anchorVec = new Vector2 (0.5f, 1);
		yAxis.anchorVec = new Vector2 (1, 0.5f);
	}
	
	public void setAxesQuadrant4() {
		xAxis.setAxisTopRight(true);
		yAxis.setAxisBotLeft(false);
		xAxis.anchorVec = new Vector2 (0.5f, 1);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	public void setAxesQuadrant1_2_3_4() {
		xAxis.setAxisMiddle(false);
		yAxis.setAxisMiddle(false);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	public void setAxesQuadrant1_2() {
		xAxis.setAxisMiddle(false);
		yAxis.setAxisTopRight(false);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	public void setAxesQuadrant3_4() {
		xAxis.setAxisMiddle(true);
		yAxis.setAxisBotLeft(false);
		xAxis.anchorVec = new Vector2 (0.5f, 1);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	public void setAxesQuadrant2_3() {
		xAxis.setAxisBotLeft(false);
		yAxis.setAxisMiddle(true);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (1, 0.5f);
	}
	
	public void setAxesQuadrant1_4() {
		xAxis.setAxisTopRight(false);
		yAxis.setAxisMiddle(false);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}

	Vector2 getAxesOffsetFactor() {
		if (axesType == axesTypes.I || axesType == axesTypes.DUAL_Y) {
			return new Vector2(-1,-1);
		}
		else if (axesType == axesTypes.II) {
			return new Vector2(1,-1);
		}
		else if (axesType == axesTypes.III) {
			return new Vector2(1,1);
		}
		else if (axesType == axesTypes.IV) {
			return new Vector2(-1,1);
		}
		else if (axesType == axesTypes.CENTER) {
			return new Vector2(0,0);
		}
		else if (axesType == axesTypes.I_II) {
			return new Vector2(0,-1);
		}
		else if (axesType == axesTypes.III_IV) {
			return new Vector2(0,1);
		}
		else if (axesType == axesTypes.II_III) {
			return new Vector2(1,0);
		}
		else if (axesType == axesTypes.I_IV) {
			return new Vector2(-1,0);
		}
		else if (axesType == axesTypes.AUTO_ORIGIN || axesType == axesTypes.AUTO_ORIGIN_X || axesType == axesTypes.AUTO_ORIGIN_Y) {
			float x = 0;
			float y = 0;
			if (axesType == axesTypes.AUTO_ORIGIN || axesType == axesTypes.AUTO_ORIGIN_Y) {
				if (xAxis.AxisMinValue >= theOrigin.x) {
					y = -1;
				}
				else if (xAxis.AxisMaxValue <= theOrigin.x) {
					y = 1;
				}
			}
			if (axesType == axesTypes.AUTO_ORIGIN || axesType == axesTypes.AUTO_ORIGIN_X) {
				if (yAxis.AxisMinValue >= theOrigin.y) {
					x = -1;
				}
				else if (yAxis.AxisMaxValue <= theOrigin.y) {
					x = 1;
				}
			}
			return new Vector2(x, y);
		}
		return new Vector2(0,0);
	}
	
	// Animate all the points in all the series simultaneously
	public void animScaleAllAtOnce(bool isPoint, float duration, float delay, Ease anEaseType, List<Vector3> before, List<Vector3> after) {
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series aSeries = lineSeries[j].GetComponent<WMG_Series>();
			List<GameObject> objects;
			if (isPoint) objects = aSeries.getPoints();
			else objects = aSeries.getLines();
			for (int i = 0; i < objects.Count; i++) {
				objects[i].transform.localScale = before[j];
				WMG_Anim.animScale(objects[i], duration, anEaseType, after[j], delay);
			}
		}
	}
	
	// Animate all the points in a single series simultaneously, and then proceed to the next series
	public void animScaleBySeries(bool isPoint, float duration, float delay, Ease anEaseType, List<Vector3> before, List<Vector3> after) {
		Sequence sequence = DOTween.Sequence();
		float individualDuration = duration / lineSeries.Count;
		float individualDelay = delay / lineSeries.Count;
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			WMG_Series aSeries = lineSeries[j].GetComponent<WMG_Series>();
			List<GameObject> objects;
			if (isPoint) objects = aSeries.getPoints();
			else objects = aSeries.getLines();
			float insertTimeLoc = j * individualDuration + (j+1) * individualDelay;
			for (int i = 0; i < objects.Count; i++) {
				objects[i].transform.localScale = before[j];
				WMG_Anim.animScaleSeqInsert(ref sequence, insertTimeLoc, objects[i], individualDuration, anEaseType, after[j], individualDelay);
			}
		}
	    sequence.Play();
	}
	
	// Animate the points across multiple series simultaneously, and then proceed to the next points.
	public void animScaleOneByOne(bool isPoint, float duration, float delay, Ease anEaseType, List<Vector3> before, List<Vector3> after, int loopDir) {
		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy(lineSeries[j])) continue;
			Sequence sequence = DOTween.Sequence();
			WMG_Series aSeries = lineSeries[j].GetComponent<WMG_Series>();
			List<GameObject> objects;
			if (isPoint) objects = aSeries.getPoints();
			else objects = aSeries.getLines();
			float individualDuration = duration / objects.Count;
			float individualDelay = delay / objects.Count;
			if (loopDir == 0) {
				// Loop from left to right
				for (int i = 0; i < objects.Count; i++) {
					objects[i].transform.localScale = before[j];
					WMG_Anim.animScaleSeqAppend(ref sequence, objects[i], individualDuration, anEaseType, after[j], individualDelay);
				}
			}
			else if (loopDir == 1) {
				// Loop from right to left
				for (int i = objects.Count-1; i >= 0; i--) {
					objects[i].transform.localScale = before[j];
					WMG_Anim.animScaleSeqAppend(ref sequence, objects[i], individualDuration, anEaseType, after[j], individualDelay);
				}
			}
			else if (loopDir == 2) {
				// Loop from the center point to the edges, alternating sides.
				int max = objects.Count - 1;
				int i = max / 2;
				int dir = -1;
				int difVal = 0;
				bool reachedMin = false;
				bool reachedMax = false;
				while (true) {
					
					if (reachedMin && reachedMax) break;
					
					if (i >= 0 && i <= max) {
						objects[i].transform.localScale = before[j];
						WMG_Anim.animScaleSeqAppend(ref sequence, objects[i], individualDuration, anEaseType, after[j], individualDelay);
					}
					
					difVal++;
					dir *= -1;
					i = i + (dir * difVal);
					
					if (i < 0) reachedMin = true;
					if (i > max) reachedMax = true;
					
				}
			}
	        sequence.Play();
		}
	}
	
	public WMG_Series addSeries() {
		return addSeriesAt(lineSeries.Count);
	}
	
	public void deleteSeries() {
		if (lineSeries.Count > 0) {
			deleteSeriesAt(lineSeries.Count-1);
		}
	}

	public WMG_Series addSeriesAt(int index, WMG_Series.comboTypes comboType = WMG_Series.comboTypes.line) {
		if (Application.isPlaying) {
			Init ();
		}
		GameObject curObj = Instantiate(seriesPrefab) as GameObject;
		curObj.name = "Series" + (index+1);
		changeSpriteParent(curObj, seriesParent);
		curObj.transform.localScale = Vector3.one;
		WMG_Series theSeries = curObj.GetComponent<WMG_Series>();
		if (autoAnimationsEnabled) autoAnim.addSeriesForAutoAnim(theSeries);
		theSeries.theGraph = this;
		lineSeries.Insert(index, curObj);
		
		// set various series properties before init for performance improvement
		theSeries.comboType = comboType;
		
		theSeries.Init(index);
		return curObj.GetComponent<WMG_Series>();
	}

	public void deleteSeriesAt(int index) {
		if (Application.isPlaying) {
			Init ();
		}
		GameObject seriesToDelete = lineSeries[index];
		WMG_Series theSeries = seriesToDelete.GetComponent<WMG_Series>();
		lineSeries.Remove(seriesToDelete);
		if (Application.isPlaying) {
			theSeries.deleteAllNodesFromGraphManager ();
			legend.deleteLegendEntry (index);
		}
		DestroyImmediate(seriesToDelete);
		graphC.Changed();
		if (graphType != graphTypes.line && graphType != graphTypes.line_stacked) seriesNoCountC.Changed();
		legend.legendC.Changed();
	}

	void UpdateFromContainer() {
		if (resizeEnabled) {

			bool fontsChangeScale = true;
			
			// Calculate the percentage factor, orientation independent factor, and smaller factor for use with resizing additional properties
			Vector2 percentFactor = new Vector2(cachedContainerWidth / origWidth, cachedContainerHeight / origHeight);
			Vector2 orientationIndependentPF = percentFactor; 
			if (orientationType == orientationTypes.horizontal) {
				orientationIndependentPF = new Vector2(percentFactor.y, percentFactor.x);
			}
			float smallerFactor = percentFactor.x;
			if (percentFactor.y < smallerFactor) smallerFactor = percentFactor.y;

			// Misc
			if ((resizeProperties & ResizeProperties.BorderPadding) == ResizeProperties.BorderPadding) {
				if (autoFitLabels) {
					if (xAxis.AxisTicksRightAbove) {
						paddingLeftRight = new Vector2(getNewResizeVariable(smallerFactor, origPaddingLeftRight.x),
						                                paddingLeftRight.y);
					}
					else {
						paddingLeftRight = new Vector2(paddingLeftRight.x,
						                                getNewResizeVariable(smallerFactor, origPaddingLeftRight.y));
					}
					if (yAxis.AxisTicksRightAbove) {
						paddingTopBottom = new Vector2(paddingTopBottom.x,
						                                getNewResizeVariable(smallerFactor, origPaddingTopBottom.y));
					}
					else {
						paddingTopBottom = new Vector2(getNewResizeVariable(smallerFactor, origPaddingTopBottom.x),
						                                paddingTopBottom.y);
					}
				}
				else {
					paddingLeftRight = new Vector2(getNewResizeVariable(smallerFactor, origPaddingLeftRight.x),
					                                getNewResizeVariable(smallerFactor, origPaddingLeftRight.y));
					paddingTopBottom = new Vector2(getNewResizeVariable(smallerFactor, origPaddingTopBottom.x),
					                                getNewResizeVariable(smallerFactor, origPaddingTopBottom.y));
				}
			}
			if ((resizeProperties & ResizeProperties.AutofitPadding) == ResizeProperties.AutofitPadding) {
				autoFitPadding = getNewResizeVariable(smallerFactor, origAutoFitPadding);
			}
			if ((resizeProperties & ResizeProperties.TickSize) == ResizeProperties.TickSize) {
				tickSize = new Vector2(getNewResizeVariable(smallerFactor, origTickSize.x),
				                        getNewResizeVariable(smallerFactor, origTickSize.y));
			}
			// Axes
			if ((resizeProperties & ResizeProperties.AxesWidth) == ResizeProperties.AxesWidth) {
				axisWidth = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, origAxisWidth));
			}
			if ((resizeProperties & ResizeProperties.AxesLabelSize) == ResizeProperties.AxesLabelSize) {
				if (fontsChangeScale) {
					yAxis.setLabelScales(getNewResizeVariable(smallerFactor, 1));
					xAxis.setLabelScales(getNewResizeVariable(smallerFactor, 1));
				}
				else {
					yAxis.AxisLabelSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, yAxis.origAxisLabelSize));
					xAxis.AxisLabelSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, xAxis.origAxisLabelSize));
				}
			}
			if ((resizeProperties & ResizeProperties.AxesLabelOffset) == ResizeProperties.AxesLabelOffset) {
				yAxis.AxisLabelSpaceOffset = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, yAxis.origAxisLabelSpaceOffset));
				xAxis.AxisLabelSpaceOffset = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, xAxis.origAxisLabelSpaceOffset));
			}
			if ((resizeProperties & ResizeProperties.AxesLabelOffset) == ResizeProperties.AxesLabelOffset) {
				yAxis.AxisTitleFontSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, yAxis.origAxisTitleFontSize));
				xAxis.AxisTitleFontSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, xAxis.origAxisTitleFontSize));
			}
			if ((resizeProperties & ResizeProperties.AxesLinePadding) == ResizeProperties.AxesLinePadding) {
				yAxis.AxisLinePadding = getNewResizeVariable(smallerFactor, yAxis.origAxisLinePadding);
				xAxis.AxisLinePadding = getNewResizeVariable(smallerFactor, xAxis.origAxisLinePadding);
			}
			if ((resizeProperties & ResizeProperties.AxesArrowSize) == ResizeProperties.AxesArrowSize) {
				Vector2 newYArrowSize = new Vector2(getNewResizeVariable(smallerFactor, yAxis.origAxisArrowSize.x),
				                                    getNewResizeVariable(smallerFactor, yAxis.origAxisArrowSize.y));
				changeSpriteSize(yAxis.AxisArrowDL, Mathf.RoundToInt(newYArrowSize.x), Mathf.RoundToInt(newYArrowSize.y));
				changeSpriteSize(yAxis.AxisArrowUR, Mathf.RoundToInt(newYArrowSize.x), Mathf.RoundToInt(newYArrowSize.y));
				Vector2 newXArrowSize = new Vector2(getNewResizeVariable(smallerFactor, xAxis.origAxisArrowSize.x),
				                                    getNewResizeVariable(smallerFactor, xAxis.origAxisArrowSize.y));
				changeSpriteSize(xAxis.AxisArrowDL, Mathf.RoundToInt(newXArrowSize.x), Mathf.RoundToInt(newXArrowSize.y));
				changeSpriteSize(xAxis.AxisArrowUR, Mathf.RoundToInt(newXArrowSize.x), Mathf.RoundToInt(newXArrowSize.y));
			}
			// Legend
			if ((resizeProperties & ResizeProperties.LegendFontSize) == ResizeProperties.LegendFontSize) {
				if (fontsChangeScale) {
					legend.setLabelScales(getNewResizeVariable(smallerFactor, 1));
				}
				else {
					legend.legendEntryFontSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, legend.origLegendEntryFontSize));
				}
			}
			if ((resizeProperties & ResizeProperties.LegendEntrySize) == ResizeProperties.LegendEntrySize) {
				if (!legend.setWidthFromLabels) {
					legend.legendEntryWidth = getNewResizeVariable(smallerFactor, legend.origLegendEntryWidth);
				}
				legend.legendEntryHeight = getNewResizeVariable(smallerFactor, legend.origLegendEntryHeight);
			}
			if ((resizeProperties & ResizeProperties.LegendOffset) == ResizeProperties.LegendOffset) {
				legend.offset = getNewResizeVariable(smallerFactor, legend.origOffset);
			}
			if ((resizeProperties & ResizeProperties.SeriesPointSize) == ResizeProperties.SeriesPointSize) {
				legend.legendEntryLinkSpacing = getNewResizeVariable(smallerFactor, legend.origLegendEntryLinkSpacing);
				legend.legendEntrySpacing = getNewResizeVariable(smallerFactor, legend.origLegendEntrySpacing);
			}
			// Properties that affect series
			if ((resizeProperties & ResizeProperties.SeriesPointSize) == ResizeProperties.SeriesPointSize) {
				barWidth = getNewResizeVariable(orientationIndependentPF.x, origBarWidth);
			}
			if ((resizeProperties & ResizeProperties.SeriesPointSize) == ResizeProperties.SeriesPointSize ||
			    (resizeProperties & ResizeProperties.SeriesLineWidth) == ResizeProperties.SeriesLineWidth ||
			    (resizeProperties & ResizeProperties.SeriesDataLabelSize) == ResizeProperties.SeriesDataLabelSize ||
			    (resizeProperties & ResizeProperties.SeriesDataLabelOffset) == ResizeProperties.SeriesDataLabelOffset) {
				for (int i = 0; i < lineSeries.Count; i++) {
					if (!activeInHierarchy(lineSeries[i])) continue;
					WMG_Series theSeries = lineSeries[i].GetComponent<WMG_Series>();
					
					if ((resizeProperties & ResizeProperties.SeriesPointSize) == ResizeProperties.SeriesPointSize) {
						theSeries.pointWidthHeight = getNewResizeVariable(smallerFactor, theSeries.origPointWidthHeight);
					}
					if ((resizeProperties & ResizeProperties.SeriesLineWidth) == ResizeProperties.SeriesLineWidth) {
						theSeries.lineScale = getNewResizeVariable(smallerFactor, theSeries.origLineScale);
					}
					if ((resizeProperties & ResizeProperties.SeriesDataLabelSize) == ResizeProperties.SeriesDataLabelSize) {
						theSeries.dataLabelsFontSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, theSeries.origDataLabelsFontSize));
					}
					if ((resizeProperties & ResizeProperties.SeriesDataLabelOffset) == ResizeProperties.SeriesDataLabelOffset) {
						theSeries.dataLabelsOffset = new Vector2(getNewResizeVariable(smallerFactor, theSeries.origDataLabelOffset.x), 
						                                         getNewResizeVariable(smallerFactor, theSeries.origDataLabelOffset.y));
					}

				}
			}

		}
	}
	
	private float getNewResizeVariable(float sizeFactor, float variable) {
		return variable + ((sizeFactor - 1) * variable);
	}



}
