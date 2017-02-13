using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Used for all charts that use axes to position data.
/// </summary>
public class WMG_Axis_Graph : WMG_Graph_Manager {

	/// <summary>
	/// The y axis.
	/// </summary>
	[SerializeField] public WMG_Axis yAxis;
	/// <summary>
	/// The x axis.
	/// </summary>
	[SerializeField] public WMG_Axis xAxis;
	/// <summary>
	/// The secondary y axis when #axesType = DUAL_Y.
	/// </summary>
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
		AutoPaddingAmount		= 1 << 13,
		BorderPadding			= 1 << 14,
		TickSize				= 1 << 15,
		AxisTitleOffset			= 1 << 16,
		GraphTitleSize			= 1 << 17,
		GraphTitleOffset		= 1 << 18
	}

	[System.Flags]
	public enum AutoPaddingProperties {
		Legend 				= 1 << 0,
		XaxisTitle	 		= 1 << 1,
		YaxisTitle		 	= 1 << 2,
		Y2axisTitle			= 1 << 3,
		Title				= 1 << 4
	}

	[SerializeField] private List<string> _groups;
	/// <summary>
	/// The list of groups used, see #useGroups for info about groups.
	/// </summary>
	public WMG_List<string> groups = new WMG_List<string>();

	/// <summary>
	/// Gets or sets the type of the graph, which determines at a high level how data will be displayed.
	/// </summary>
	/// <value>The type of the graph.</value>
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
	/// <summary>
	/// Gets or sets the type of the orientation, where vertical means the y-axis data will be displayed vertically.
	/// </summary>
	/// <value>The type of the orientation.</value>
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
	/// <summary>
	/// Determines how axes are oriented, the roman numerals refer to quadrants where quadrant 1 is the top right, and quadrant 4 is bottom right.
	/// - Manual: Graph Maker will not do anything with regards to moving around the axes.
	/// - Center: Equivalent to all four quadrants or axes positioned in the center.
	/// - DUAL_Y: Use for dual y-axis charts, in this case WMG_Series::useSecondYaxis for each series should be set.
	/// - AUTO_ORIGIN: Automatically positions axes closest to the #theOrigin. In this case changing Axis min / max values will automatically reposition the axes.
	/// When this option is set, it is also recommended to set WMG_Axis::AxisUseNonTickPercent = true, so that the axes reposition freely instead of stay fixed to axis ticks.
	/// - AUTO_ORIGIN_X: Automatically positions only the X-axis closest to the #theOrigin.
	/// - AUTO_ORIGIN_Y: Automatically positions only the Y-axis closest to the #theOrigin.
	/// </summary>
	/// <value>The type of the axes.</value>
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
				autoPaddingC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines whether graph content (#resizeProperties) will resize post graph initialization based on the percentage change of the graph's rect transform width / height.
	/// </summary>
	/// <value><c>true</c> if resize enabled; otherwise, <c>false</c>.</value>
	public bool resizeEnabled { get {return _resizeEnabled;} 
		set {
			if (_resizeEnabled != value) {
				_resizeEnabled = value;
			}
		}
	}
	/// <summary>
	/// Specifies which graph content is resized when #resizeEnabled = true.
	/// </summary>
	/// <value>The resize properties.</value>
	public ResizeProperties resizeProperties { get {return _resizeProperties;} 
		set {
			if (_resizeProperties != value) {
				_resizeProperties = value;
			}
		}
	}
	/// <summary>
	/// Determines whether the data for each series corresponds to an element in #groups, where the (group index + 1) corresponds with WMG_Series::pointValues x value.
	/// This option is useful to display axes labels that correspond with data points rather than axis ticks (WMG_Axis::LabelType = groups).
	/// For example, you could have 12 groups that represents months of the year, and series data that corresponds to each month.
	/// You could also have a large number of groups like 365 days of the year and use WMG_Axis::AxisLabelSkipInterval to not clutter the axis with labels.
	/// Additionally, you can use this option to graph NULLs. For example, if you have 12 month groups, but one or more of your series does not have data for March,
	/// then you can pass in a negative number to represent NULL (e.g. (-3, 0) for March). Remember it is (group index + 1) so January is 1, and December is 12.
	/// </summary>
	/// <value><c>true</c> if use groups; otherwise, <c>false</c>.</value>
	public bool useGroups { get {return _useGroups;} 
		set {
			if (_useGroups != value) {
				_useGroups = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// The amount of space on the left / right sides of the graph content formed by bounding box of the axis lines, and set automatically if #autoPaddingEnabled = true.
	/// </summary>
	/// <value>The padding left right.</value>
	public Vector2 paddingLeftRight { get {return _paddingLeftRight;} 
		set {
			if (_paddingLeftRight != value) {
				_paddingLeftRight = value;
				graphC.Changed();
				seriesCountC.Changed();
			}
		}
	}
	/// <summary>
	/// The amount of space on the top / bottom sides of the graph content formed by bounding box of the axis lines, and set automatically if #autoPaddingEnabled = true.
	/// </summary>
	/// <value>The padding top bottom.</value>
	public Vector2 paddingTopBottom { get {return _paddingTopBottom;} 
		set {
			if (_paddingTopBottom != value) {
				_paddingTopBottom = value;
				graphC.Changed();
				seriesCountC.Changed();
			}
		}
	}
	/// <summary>
	/// When enabled, automatically sets #paddingLeftRight and #paddingTopBottom based on #autoPaddingProperties and #autoPaddingAmount.
	/// This is primarily useful to ensure axis labels do not exceed the border of the graph boundaries determined by the rectTransform's width / height of the graph.
	/// </summary>
	/// <value><c>true</c> if auto padding enabled; otherwise, <c>false</c>.</value>
	public bool autoPaddingEnabled { get {return _autoPaddingEnabled;} 
		set {
			if (_autoPaddingEnabled != value) {
				_autoPaddingEnabled = value;
				graphC.Changed();
				autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// The elements of graph content that is taken into consideration when #autoPaddingEnabled = true. Axis labels, ticks, arrows, and lines are all automatically considered,
	/// and thus cannot / do not need to be specified here. This is for specifiying additional graph features which you may or may not want to include, for example 
	/// you would specify legend here if you want the legend to be within the graph boundary, and the graph padding to be updated automatically according to the size of the legend.
	/// </summary>
	/// <value>The auto padding properties.</value>
	public AutoPaddingProperties autoPaddingProperties { get {return _autoPaddingProperties;} 
		set {
			if (_autoPaddingProperties != value) {
				_autoPaddingProperties = value;
				graphC.Changed();
				autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// This is the amount of padding space between the graph content and graph boundary that is used when #autoPaddingEnabled = true.
	/// </summary>
	/// <value>The auto padding amount.</value>
	public float autoPaddingAmount { get {return _autoPaddingAmount;} 
		set {
			if (_autoPaddingAmount != value) {
				_autoPaddingAmount = value;
				graphC.Changed();
				autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// The origin of the graph, affects position of axes if #axesType is of an AUTO_ORIGIN type, also affects #barAxisValue if #autoUpdateBarAxisValue = true.
	/// </summary>
	/// <value>The origin.</value>
	public Vector2 theOrigin { get {return _theOrigin;} 
		set {
			if (_theOrigin != value) {
				_theOrigin = value;
				graphC.Changed();
				seriesNoCountC.Changed();
				if (axesType == WMG_Axis_Graph.axesTypes.AUTO_ORIGIN_X || 
				    axesType == WMG_Axis_Graph.axesTypes.AUTO_ORIGIN_Y ||
				    axesType == axesTypes.AUTO_ORIGIN) {
					autoPaddingC.Changed();
				}
			}
		}
	}
	/// <summary>
	/// Determines the width of the series' bars for bar graphs. This is specified here instead of the series, since bar width cannot vary between series. 
	/// Series point sizing can be specified for each series using WMG_Series::pointWidthHeight.
	/// </summary>
	public float barWidth { get {return _barWidth;} 
		set {
			if (_barWidth != value) {
				_barWidth = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	/// <summary>
	/// Controls the starting point for bar charts. For example, if the y-axis min is 0 and y-axis max is 20, and this is set to 10, 
	/// then the base of the bars will start from 10 and either go up or down depending on the data set for each bar. 
	/// So, a bar representing a value of 5 will start from 10 and go downwards to 5, and a bar with a value of 15 will start from 10 and go upwards to 15.
	/// </summary>
	/// <value>The bar axis value.</value>
	public float barAxisValue { get {return _barAxisValue;} 
		set {
			if (_barAxisValue != value) {
				_barAxisValue = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	/// <summary>
	/// When enabled, automatically sets #theOrigin based on #axesType. For example, if axes type is quadrant I, and the min X axis value is -100, and min Y value is 50, 
	/// then the origin will be (-100, 50). Does not do anything if #axesType is of an AUTO_ORIGIN type. 
	/// </summary>
	/// <value><c>true</c> if auto update origin; otherwise, <c>false</c>.</value>
	public bool autoUpdateOrigin { get {return _autoUpdateOrigin;} 
		set {
			if (_autoUpdateOrigin != value) {
				_autoUpdateOrigin = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When enabled, automatically updates #barWidth based on #autoUpdateBarWidthSpacing. 
	/// Useful to ensure bars don't overlap when dynamically adding series by reducing the bar width as needed.
	/// </summary>
	/// <value><c>true</c> if auto update bar width; otherwise, <c>false</c>.</value>
	public bool autoUpdateBarWidth { get {return _autoUpdateBarWidth;} 
		set {
			if (_autoUpdateBarWidth != value) {
				_autoUpdateBarWidth = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	/// <summary>
	/// When #autoUpdateBarWidth = true, automatically updates #barWidth based on this specified percentage of the graph's axis length. 
	/// This ensures the total amount of space not occupied by bars is equal to this percent. For example at 0.3, 30% of the space occupied by bars is empty space. 
	/// </summary>
	/// <value>The auto update bar width spacing.</value>
	public float autoUpdateBarWidthSpacing { get {return _autoUpdateBarWidthSpacing;} 
		set {
			if (_autoUpdateBarWidthSpacing != value) {
				_autoUpdateBarWidthSpacing = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	/// <summary>
	/// When enabled, automatically updates WMG_Series::extraXSpace, which is the padding of space for the series from the axis line. 
	/// For example for non-stacked bar charts with multiple series, the later series will have a higher spacing such that they appear to the right of the previous series. 
	/// </summary>
	/// <value><c>true</c> if auto update series axis spacing; otherwise, <c>false</c>.</value>
	public bool autoUpdateSeriesAxisSpacing { get {return _autoUpdateSeriesAxisSpacing;} 
		set {
			if (_autoUpdateSeriesAxisSpacing != value) {
				_autoUpdateSeriesAxisSpacing = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	/// <summary>
	/// When enabled, automatically updates #barAxisValue based on the #theOrigin.
	/// </summary>
	/// <value><c>true</c> if auto update bar axis value; otherwise, <c>false</c>.</value>
	public bool autoUpdateBarAxisValue { get {return _autoUpdateBarAxisValue;} 
		set {
			if (_autoUpdateBarAxisValue != value) {
				_autoUpdateBarAxisValue = value;
				graphC.Changed();
				seriesNoCountC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines the width of the x / y axis lines.
	/// </summary>
	/// <value>The width of the axes.</value>
	public int axisWidth { get {return _axisWidth;} 
		set {
			if (_axisWidth != value) {
				_axisWidth = value;
				graphC.Changed();
				autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// When WMG_Axis::MinAutoShrink = true or WMG_Axis::MaxAutoShrink = true, determines percentage threshold at which an auto shrink occurs. It is a percentage of the total axis length.
	/// For example, if the y axis min is 0 and the max is 100, and this is 0.6 (60%), then a shrink will occur if all y-axis data for all WMG_Series is below 60.
	/// </summary>
	/// <value>The auto shrink at percent.</value>
	public float autoShrinkAtPercent { get {return _autoShrinkAtPercent;} 
		set {
			if (_autoShrinkAtPercent != value) {
				_autoShrinkAtPercent = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When WMG_Axis::MinAutoShrink = true, WMG_Axis::MaxAutoShrink = true, WMG_Axis::MinAutoGrow = true, or WMG_Axis::MaxAutoGrow = true, determines the amount of an auto shrink / growth.
	/// For example, if the y axis min is 0 and max is 100, and there exists series data that is above 100, and this parameter is 0.2 (20%), then the new max would be 120.
	/// </summary>
	/// <value>The auto grow and shrink by percent.</value>
	public float autoGrowAndShrinkByPercent { get {return _autoGrowAndShrinkByPercent;} 
		set {
			if (_autoGrowAndShrinkByPercent != value) {
				_autoGrowAndShrinkByPercent = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines whether to show a tooltip when the mouse hovers over a series data point / bar.
	/// </summary>
	/// <value><c>true</c> if tooltip enabled; otherwise, <c>false</c>.</value>
	public bool tooltipEnabled { get {return _tooltipEnabled;} 
		set {
			if (_tooltipEnabled != value) {
				_tooltipEnabled = value;
				tooltipEnabledC.Changed();
			}
		}
	}
	/// <summary>
	/// When enabled, certain changes will automatically play an animation such as a data point changing its y-value, or the graph orientation changing.
	/// Adding or removing data points from a series does not play an animation, it is recommended to disable this and code your own animation as is done in the X_Plot_Overtime example.
	/// </summary>
	/// <value><c>true</c> if auto animations enabled; otherwise, <c>false</c>.</value>
	public bool autoAnimationsEnabled { get {return _autoAnimationsEnabled;} 
		set {
			if (_autoAnimationsEnabled != value) {
				_autoAnimationsEnabled = value;
				autoAnimEnabledC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines the width and height of axis ticks, for which there are WMG_Axis::AxisNumTicks if WMG_Axis::hideTicks = false.
	/// </summary>
	/// <value>The size of the tick.</value>
	public Vector2 tickSize { get {return _tickSize;} 
		set {
			if (_tickSize != value) {
				_tickSize = value;
				graphC.Changed();
				autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// The string to display for the title of the graph, which appears at the top of the graph.
	/// </summary>
	/// <value>The graph title string.</value>
	public string graphTitleString { get {return _graphTitleString;} 
		set {
			if (_graphTitleString != value) {
				_graphTitleString = value;
				graphC.Changed();
				autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// The positional offset of #graphTitleString used to control how much space there is between the graph title and the graph. 
	/// </summary>
	/// <value>The graph title offset.</value>
	public Vector2 graphTitleOffset { get {return _graphTitleOffset;} 
		set {
			if (_graphTitleOffset != value) {
				_graphTitleOffset = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// The font size of #graphTitleString.
	/// </summary>
	/// <value>The size of the graph title.</value>
	public int graphTitleSize { get {return _graphTitleSize;} 
		set {
			if (_graphTitleSize != value) {
				_graphTitleSize = value;
				graphC.Changed();
				autoPaddingC.Changed();
			}
		}
	}

	// Public variables without change tracking
	/// <summary>
	/// The positional offset from the bottom left corner of the tooltip from the mouse cursor.
	/// </summary>
	public Vector2 tooltipOffset;
	/// <summary>
	/// The numbr of decimals used when data is displayed in the tooltip.
	/// </summary>
	public int tooltipNumberDecimals;
	/// <summary>
	/// Whether or not the tooltip also displays the WMG_Series::seriesName to the left of the data in the tooltip.
	/// </summary>
	public bool tooltipDisplaySeriesName;
	/// <summary>
	/// Whether or not an animation plays (e.g. size change of data point) when hovering mouse over a data point for tooltips.
	/// </summary>
	public bool tooltipAnimationsEnabled;
	/// <summary>
	/// The tooltip animations easetype.
	/// </summary>
	public Ease tooltipAnimationsEasetype;
	/// <summary>
	/// The duration of the tooltip animations.
	/// </summary>
	public float tooltipAnimationsDuration;
	/// <summary>
	/// The auto animations easetype.
	/// </summary>
	public Ease autoAnimationsEasetype;
	/// <summary>
	/// The duration of the auto animations.
	/// </summary>
	public float autoAnimationsDuration;
	/// <summary>
	/// The list of series for this graph, it is a GameObject list, but each GameObject must have a WMG_Series.
	/// </summary>
	public List<GameObject> lineSeries;
	/// <summary>
	/// The list of line series point prefabs to which WMG_Series::pointPrefab corresponds, for which a value of 0 will mean to use the first prefab in this list.
	/// </summary>
	public List<Object> pointPrefabs;
	/// <summary>
	/// The list of line series line prefabs to which WMG_Series::linkPrefab corresponds, for which a value of 0 will mean to use the first prefab in this list.
	/// </summary>
	public List<Object> linkPrefabs;
	/// <summary>
	/// For bar graphs, this is the prefab used in drawing the bars for all series.
	/// </summary>
	public Object barPrefab;
	/// <summary>
	/// Dynamically adding series with #addSeries or #addSeriesAt functions will use this prefab to create the new series.
	/// </summary>
	public Object seriesPrefab;
	/// <summary>
	/// The legend.
	/// </summary>
	public WMG_Legend legend;
	/// <summary>
	/// The graph title.
	/// </summary>
	public GameObject graphTitle;
	/// <summary>
	/// The graph background.
	/// </summary>
	public GameObject graphBackground;
	/// <summary>
	/// A GameObject similar to the Graph Background object, but whose child objects use anchoring to position such as the legend.
	/// </summary>
	public GameObject anchoredParent;
	/// <summary>
	/// A GameObject whose RectTransform defines the bounding box formed by the graph's axes.
	/// </summary>
	public GameObject graphAreaBoundsParent;
	/// <summary>
	/// The WMG_Series objects have this as their parent.
	/// </summary>
	public GameObject seriesParent;
	/// <summary>
	/// The tool tip panel / parent.
	/// </summary>
	public GameObject toolTipPanel;
	/// <summary>
	/// The tool tip label.
	/// </summary>
	public GameObject toolTipLabel;
	/// <summary>
	/// The tooltip.
	/// </summary>
	public WMG_Graph_Tooltip theTooltip;
	/// <summary>
	/// When #resizeEnabled = true, the resizing process for Text objects will change the objects scale instead of font size when this is true.
	/// </summary>
	public bool resizingChangesFontScaleInsteadOfFontSize = false;

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
	[WMG_EnumFlagAttribute] [SerializeField] private AutoPaddingProperties _autoPaddingProperties;
	[SerializeField] private bool _autoPaddingEnabled;
	[SerializeField] private float _autoPaddingAmount;
	[SerializeField] private Vector2 _tickSize;
	[SerializeField] private string _graphTitleString;
	[SerializeField] private Vector2 _graphTitleOffset;
	[SerializeField] private int _graphTitleSize = 18;

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

	public float xAxisLengthOrienInd { 
		get {
			if (orientationType == orientationTypes.vertical) {
				return xAxisLength;
			}
			else {
				return yAxisLength;
			}
		}
	}

	public float yAxisLengthOrienInd { 
		get {
			if (orientationType == orientationTypes.vertical) {
				return yAxisLength;
			}
			else {
				return xAxisLength;
			}
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
	/// <summary>
	/// Contains the summed values for series' data, used for stacked charts.
	/// </summary>
	/// <value>The total point values.</value>
	public List<float> TotalPointValues { get { return totalPointValues; }}

	private int maxSeriesPointCount;
	/// <summary>
	/// The max number of points across all series.
	/// </summary>
	/// <returns>The max number points.</returns>
	public int getMaxNumPoints() {return maxSeriesPointCount;}

	private int maxSeriesBarCount;
	/// <summary>
	/// The max number of bars across all series.
	/// </summary>
	/// <returns>The max series bar count.</returns>
	public int getMaxSeriesBarCount() {return maxSeriesBarCount;}

	private int numComboBarSeries;
	/// <summary>
	/// The number of series that are combo bar series.
	/// </summary>
	public int NumComboBarSeries() {return numComboBarSeries;}

	// Original property values for use with dynamic resizing
	private float origWidth;
	private float origHeight;
	private float origBarWidth;
	private float origAxisWidth;
	private float origAutoPaddingAmount;
	private Vector2 origTickSize;
	private Vector2 origPaddingLeftRight;
	private Vector2 origPaddingTopBottom;
	private Vector2 origGraphTitleOffset;
	private int origGraphTitleSize;

	// Cache
	private float cachedContainerWidth;
	private float cachedContainerHeight;

	private WMG_Graph_Auto_Anim autoAnim;

	private bool hasInit;
	
	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	public WMG_Change_Obj graphC = new WMG_Change_Obj();
	public WMG_Change_Obj seriesCountC = new WMG_Change_Obj();
	public WMG_Change_Obj seriesNoCountC = new WMG_Change_Obj();
	private WMG_Change_Obj tooltipEnabledC = new WMG_Change_Obj();
	private WMG_Change_Obj autoAnimEnabledC = new WMG_Change_Obj();
	private WMG_Change_Obj orientationC = new WMG_Change_Obj();
	private WMG_Change_Obj graphTypeC = new WMG_Change_Obj();
	public WMG_Change_Obj autoPaddingC = new WMG_Change_Obj();

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

	/// <summary>
	/// Initializes the graph, and should always be done before anything else, called automatically in Start(), but it never hurts to call this manually after instantiating a graph prefab.
	/// </summary>
	public void Init() {
		if (hasInit) return;
		hasInit = true;

		changeObjs.Add(orientationC);
		changeObjs.Add(graphTypeC);
		changeObjs.Add(graphC);
		changeObjs.Add(seriesCountC);
		changeObjs.Add(seriesNoCountC);
		changeObjs.Add(tooltipEnabledC);
		changeObjs.Add(autoAnimEnabledC);
//		changeObjs.Add(autoPaddingC); // due to ordering issues, this is controlled separately from other change objects in ResumeCallbacks / PauseCallbacks

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
		theTooltip.SetToolTipRefs ();
		if (tooltipEnabled) theTooltip.subscribeToEvents(true);
		autoAnim = this.gameObject.AddComponent<WMG_Graph_Auto_Anim>(); // Add automatic animations script
		autoAnim.hideFlags = HideFlags.HideInInspector; // Don't show automatic animations script
		autoAnim.theGraph = this; // Set automatic animations graph
		if (autoAnimationsEnabled) autoAnim.subscribeToEvents(true);
		
		groups.SetList (_groups);
		groups.Changed += groupsChanged;

		orientationC.OnChange += OrientationChanged;
		graphTypeC.OnChange += GraphTypeChanged;
		graphC.OnChange += GraphChanged;
		seriesCountC.OnChange += SeriesCountChanged;
		seriesNoCountC.OnChange += SeriesNoCountChanged;
		tooltipEnabledC.OnChange += TooltipEnabledChanged;
		autoAnimEnabledC.OnChange += AutoAnimationsEnabledChanged;
		autoPaddingC.OnChange += AutoPadding;

		setOriginalPropertyValues();
		PauseCallbacks();
	}


	void Update () {
		updateFromDataSource();

		Refresh();
	}

	/// <summary>
	/// Refreshes the graph, and happens automatically in Update(), but sometimes it is useful or necessary to call this manually, note that refresh updates
	/// only the parts of the graph affected by properties that have changed since a last refresh.
	/// </summary>
	public void Refresh() {
		ResumeCallbacks();
		PauseCallbacks();
	}

	void OnRectTransformDimensionsChange () {
		if (!hasInit) return;
		if (resizeEnabled) {
			ManualResize ();
		} else {
			cachedContainerWidth = getSpriteWidth(this.gameObject);
			cachedContainerHeight = getSpriteHeight(this.gameObject);
			graphC.Changed();
			seriesNoCountC.Changed();
			legend.legendC.Changed();
		}
	}

	/// <summary>
	/// Useful if #resizeEnabled = false, and you want to resize graph content manually by calling this when you want.
	/// </summary>
	public void ManualResize() {
//		PauseCallbacks();
		cachedContainerWidth = getSpriteWidth(this.gameObject);
		cachedContainerHeight = getSpriteHeight(this.gameObject);
		bool origResizeEnabled = resizeEnabled;
		resizeEnabled = true;
		UpdateFromContainer();
		resizeEnabled = origResizeEnabled;
		autoPaddingC.Changed();
//		ResumeCallbacks();
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
		autoPaddingC.changesPaused = true;
		autoPaddingC.changePaused = false;
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
		autoPaddingC.changesPaused = false;
		if (autoPaddingC.changePaused) autoPaddingC.Changed();
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

	void AllChanged() {
		graphC.Changed();
		autoPaddingC.Changed();
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

	/// <summary>
	/// Happens when a series points changed, this should not be used outside of Graph Maker code.
	/// </summary>
	/// <param name="countChanged">If set to <c>true</c> count changed.</param>
	/// <param name="instant">If set to <c>true</c> instant.</param>
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

	/// <summary>
	/// Happens when a series points changed, this should not be used outside of Graph Maker code.
	/// </summary>
	public void aSeriesPointsChanged() {
		if (!Application.isPlaying) return;
		UpdateTotals ();
		UpdateBarWidth ();
		UpdateAxesMinMaxValues ();
	}

	/// <summary>
	/// Happens when various elements of the graph has changed, this should not be used outside of Graph Maker code.
	/// </summary>
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

	void groupsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref groups, ref _groups, oneValChanged, index);
		graphC.Changed();
		autoPaddingC.Changed();
		if (oneValChanged) {
			seriesNoCountC.Changed();
		}
		else {
			seriesCountC.Changed();
		}
	}

	/// <summary>
	/// Caches initial property values that are used as the basis for #resizeEnabled functionality. 
	/// </summary>
	public void setOriginalPropertyValues() {
		cachedContainerWidth = getSpriteWidth(this.gameObject);
		cachedContainerHeight = getSpriteHeight(this.gameObject);
		origWidth = getSpriteWidth(this.gameObject);
		origHeight = getSpriteHeight(this.gameObject);
		origBarWidth = barWidth;
		origAxisWidth = axisWidth;
		origAutoPaddingAmount = autoPaddingAmount;
		origTickSize = tickSize;
		origPaddingLeftRight = paddingLeftRight;
		origPaddingTopBottom = paddingTopBottom;
		origGraphTitleOffset = graphTitleOffset;
		origGraphTitleSize = graphTitleSize;
	}

	/// <summary>
	/// Updates the graph orientation, this should not be used outside of Graph Maker code.
	/// </summary>
	public void UpdateOrientation() {
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
	}

	void getMinimumBorderDiffs(ref float dLeft, ref float dRight, ref float dBot, ref float dTop, Vector2 xDif, Vector2 yDif) {
		if (xDif.x < dLeft) dLeft = xDif.x;
		if (xDif.y < dRight) dRight = xDif.y;
		if (yDif.x < dBot) dBot = yDif.x;
		if (yDif.y < dTop) dTop = yDif.y;
	}

	void AutoPadding() {
		if (autoPaddingEnabled) {
			Vector2 xDif = Vector2.zero;
			Vector2 yDif = Vector2.zero;

			Vector3 currentRot = this.transform.eulerAngles;
			this.transform.eulerAngles = Vector3.zero; // temporarily set rotation to 0, to make calculations simple

			getAutoPaddingDifferentialFromGraphContent (ref xDif, ref yDif);

			xDif = new Vector2(xDif.x - autoPaddingAmount, xDif.y - autoPaddingAmount);
			yDif = new Vector2(yDif.x - autoPaddingAmount, yDif.y - autoPaddingAmount);

			// if significantly different, update the graph padding
			// don't allow final padding to be negative (max with 0)
			if (Mathf.Abs(xDif.x) > 0.1 || Mathf.Abs(xDif.y) > 0.1) {
				paddingLeftRight = new Vector2(Mathf.Max( paddingLeftRight.x - xDif.x, 0), Mathf.Max( paddingLeftRight.y - xDif.y, 0));
			}
			if (Mathf.Abs(yDif.x) > 0.1 || Mathf.Abs(yDif.y) > 0.1) {
				paddingTopBottom = new Vector2(Mathf.Max( paddingTopBottom.x - yDif.y, 0), Mathf.Max( paddingTopBottom.y - yDif.x, 0));
			}
			this.transform.eulerAngles = currentRot;
		}
	}

	// determines the max / minimum border differentials between certain graph content and graph borders. 
	// Used to determine how much to auto adjust border padding
	void getAutoPaddingDifferentialFromGraphContent(ref Vector2 xDif,  ref Vector2 yDif) {
		
		float dLeft = Mathf.Infinity;
		float dRight = Mathf.Infinity;
		float dBot = Mathf.Infinity;
		float dTop = Mathf.Infinity;
		
		// y labels
		if (!yAxis.hideLabels) {
			foreach (WMG_Node node in yAxis.GetAxisLabelNodes()) {
				forceUpdateText(node.objectToLabel); // label text / font size could have been set this frame, ensure label recttransform is up to date
				getRectDiffs (node.objectToLabel, this.gameObject, ref xDif, ref yDif);
				getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
			}
		}
		if (!yAxis.hideTicks) { // y ticks
			foreach (WMG_Node node in yAxis.GetAxisTickNodes()) {
				getRectDiffs (node.gameObject, this.gameObject, ref xDif, ref yDif);
				getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
			}
		}
		// x labels
		if (!xAxis.hideLabels) {
			foreach (WMG_Node node in xAxis.GetAxisLabelNodes()) {
				forceUpdateText(node.objectToLabel); // label text / font size could have been set this frame, ensure label recttransform is up to date
				getRectDiffs (node.objectToLabel, this.gameObject, ref xDif, ref yDif);
				getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
			}
		}
		if (!xAxis.hideTicks) { // x ticks
			foreach (WMG_Node node in xAxis.GetAxisTickNodes()) {
				getRectDiffs (node.gameObject, this.gameObject, ref xDif, ref yDif);
				getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
			}
		}
		// y2 labels
		if (axesType == axesTypes.DUAL_Y && !yAxis2.hideLabels) {
			foreach (WMG_Node node in yAxis2.GetAxisLabelNodes()) {
				forceUpdateText(node.objectToLabel); // label text / font size could have been set this frame, ensure label recttransform is up to date
				getRectDiffs (node.objectToLabel, this.gameObject, ref xDif, ref yDif);
				getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
			}
		}
		if (axesType == axesTypes.DUAL_Y && !yAxis2.hideTicks) { // y2 ticks
			foreach (WMG_Node node in yAxis2.GetAxisTickNodes()) {
				getRectDiffs (node.gameObject, this.gameObject, ref xDif, ref yDif);
				getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
			}
		}
		// axes arrows
		if (activeInHierarchy (yAxis.AxisArrowDL)) {
			getRectDiffs (yAxis.AxisArrowDL, this.gameObject, ref xDif, ref yDif);
			getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
		}
		if (activeInHierarchy (yAxis.AxisArrowUR)) {
			getRectDiffs (yAxis.AxisArrowUR, this.gameObject, ref xDif, ref yDif);
			getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
		}
		if (activeInHierarchy (xAxis.AxisArrowDL)) {
			getRectDiffs (xAxis.AxisArrowDL, this.gameObject, ref xDif, ref yDif);
			getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
		}
		if (activeInHierarchy (xAxis.AxisArrowUR)) {
			getRectDiffs (xAxis.AxisArrowUR, this.gameObject, ref xDif, ref yDif);
			getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
		}
		if (axesType == axesTypes.DUAL_Y && activeInHierarchy (yAxis2.AxisArrowDL)) {
			getRectDiffs (yAxis2.AxisArrowDL, this.gameObject, ref xDif, ref yDif);
			getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
		}
		if (axesType == axesTypes.DUAL_Y && activeInHierarchy (yAxis2.AxisArrowUR)) {
			getRectDiffs (yAxis2.AxisArrowUR, this.gameObject, ref xDif, ref yDif);
			getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
		}
		// axes lines
		if (activeInHierarchy (yAxis.AxisLine) && getSpriteAlpha(yAxis.AxisLine) > 0) {
			getRectDiffs (yAxis.AxisLine, this.gameObject, ref xDif, ref yDif);
			getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
		}
		if (activeInHierarchy (xAxis.AxisLine) && getSpriteAlpha(xAxis.AxisLine) > 0) {
			getRectDiffs (xAxis.AxisLine, this.gameObject, ref xDif, ref yDif);
			getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
		}
		if (axesType == axesTypes.DUAL_Y && activeInHierarchy (yAxis2.AxisLine) && getSpriteAlpha(yAxis2.AxisLine) > 0) {
			getRectDiffs (yAxis2.AxisLine, this.gameObject, ref xDif, ref yDif);
			getMinimumBorderDiffs(ref dLeft, ref dRight, ref dBot, ref dTop, xDif, yDif);
		}

		if ((autoPaddingProperties & AutoPaddingProperties.Legend) == AutoPaddingProperties.Legend) {
			// add space for legend
			// legend offset greater than 0 means legend should be within chart boundaries
			if (!legend.hideLegend && legend.offset > 0 && legend.NumEntries > 0) {
				if (legend.legendType == WMG_Legend.legendTypes.Bottom) {
					if (!legend.oppositeSideLegend) { // bot
						dBot -= legend.LegendHeight;
					} else { // top
						dTop -= legend.LegendHeight;
					}
				} else {
					if (!legend.oppositeSideLegend) { // right
						dRight -= legend.LegendWidth;
					} else { // left
						dLeft -= legend.LegendWidth;
					}
				}
			}
		}

		if ((autoPaddingProperties & AutoPaddingProperties.YaxisTitle) == AutoPaddingProperties.YaxisTitle) {
			if (activeInHierarchy (yAxis.AxisTitle) && !string.IsNullOrEmpty (yAxis.AxisTitleString)) {
				forceUpdateText (yAxis.AxisTitle);
				if (yAxis.anchorVec.x == 1) { // right
					dRight -= getSpriteHeight (yAxis.AxisTitle);
				}
				else {
					dLeft -= getSpriteHeight (yAxis.AxisTitle);
				}
			}
		}
		if ((autoPaddingProperties & AutoPaddingProperties.Y2axisTitle) == AutoPaddingProperties.Y2axisTitle) {
			if (activeInHierarchy (yAxis2.AxisTitle) && !string.IsNullOrEmpty (yAxis2.AxisTitleString)) {
				forceUpdateText (yAxis2.AxisTitle);
				if (yAxis2.anchorVec.x == 1) { // right
					dRight -= getSpriteHeight (yAxis2.AxisTitle);
				}
				else {
					dLeft -= getSpriteHeight (yAxis2.AxisTitle);
				}
			}
		}
		if ((autoPaddingProperties & AutoPaddingProperties.XaxisTitle) == AutoPaddingProperties.XaxisTitle) {
			if (activeInHierarchy (xAxis.AxisTitle) && !string.IsNullOrEmpty (xAxis.AxisTitleString)) {
				forceUpdateText (xAxis.AxisTitle);
				if (xAxis.anchorVec.y == 1) { // top
					dTop -= getSpriteHeight (xAxis.AxisTitle);
				}
				else {
					dBot -= getSpriteHeight (xAxis.AxisTitle);
				}
			}
		}
		if ((autoPaddingProperties & AutoPaddingProperties.Title) == AutoPaddingProperties.Title) {
			if (activeInHierarchy (graphTitle) && !string.IsNullOrEmpty (graphTitleString)) {
				forceUpdateText (graphTitle);
				dTop -= getSpriteHeight (graphTitle);
			}
		}
		
		xDif = new Vector2 (dLeft, dRight);
		yDif = new Vector2 (dBot, dTop);
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
						if (orientationType == orientationTypes.vertical) {
							changeSpritePositionRelativeToObjByX(lineSeries[j], lineSeries[0], barWidth/2 * totalNumberComboBars);
						}
						else {
							changeSpritePositionRelativeToObjByY(lineSeries[j], lineSeries[0], -barWidth/2 * totalNumberComboBars);
						}
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

	/// <summary>
	/// Update called from Editor window, should not be used outside of Graph Maker code.
	/// </summary>
	public void InEditorUpdate() {
		if (Application.isEditor) {
			Vector2 newSize = getSpriteSize (gameObject);
			changeSpriteSize (graphBackground, Mathf.RoundToInt (newSize.x), Mathf.RoundToInt (newSize.y));
			changeSpritePositionToX (graphBackground, -paddingLeftRight.x);
			changeSpritePositionToY (graphBackground, -paddingTopBottom.y);
			UpdateBGandSeriesParentPositions (newSize.x, newSize.y);
			// Update axes lines
			int newX = Mathf.RoundToInt (newSize.x - paddingLeftRight.x - paddingLeftRight.y + xAxis.AxisLinePadding);
			if (newX < 0)
				newX = 0;
			changeSpriteSize (xAxis.AxisLine, newX, axisWidth);
			changeSpritePositionToX (xAxis.AxisLine, newX / 2f);
			int newY = Mathf.RoundToInt (newSize.y - paddingTopBottom.x - paddingTopBottom.y + yAxis.AxisLinePadding);
			if (newY < 0)
				newY = 0;
			changeSpriteSize (yAxis.AxisLine, axisWidth, newY);
			changeSpritePositionToY (yAxis.AxisLine, newY / 2f);
		}
	}

	void UpdateBG() {
		changeSpriteSize(graphBackground, Mathf.RoundToInt(getSpriteWidth(this.gameObject)), Mathf.RoundToInt(getSpriteHeight(this.gameObject)));
		changeSpritePositionTo (graphBackground, new Vector3 (-paddingLeftRight.x, -paddingTopBottom.y, 0));
		changeSpriteSize(anchoredParent, Mathf.RoundToInt(getSpriteWidth(this.gameObject)), Mathf.RoundToInt(getSpriteHeight(this.gameObject)));
		changeSpritePositionTo (anchoredParent, new Vector3 (-paddingLeftRight.x, -paddingTopBottom.y, 0));
		changeSpriteSize(graphAreaBoundsParent, Mathf.RoundToInt(xAxisLength), Mathf.RoundToInt(yAxisLength));
		changeSpritePositionTo(graphAreaBoundsParent, new Vector3(xAxisLength / 2, yAxisLength / 2, 0));

		for (int j = 0; j < lineSeries.Count; j++) {
			if (!activeInHierarchy (lineSeries [j])) continue;
			WMG_Series theSeries = lineSeries [j].GetComponent<WMG_Series> ();
			if (theSeries.areaShadingUsesComputeShader) {
				if (theSeries.areaShadingCSPrefab == null || theSeries.areaShadingParent == null) continue;
				if (theSeries.areaShadingType != WMG_Series.areaShadingTypes.None && theSeries.getAreaShadingRects().Count == 1) {
					changeSpriteSizeFloat(theSeries.getAreaShadingRects()[0], xAxisLength, yAxisLength);
				}
			}
		}

		UpdateBGandSeriesParentPositions(cachedContainerWidth, cachedContainerHeight);
		OnGraphBackgroundChanged();
	}

	void UpdateBGandSeriesParentPositions (float x, float y) {
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
			
			int numBars = (maxSeriesPointCount * lineSeries.Count);
			
			if (graphType == graphTypes.combo) {
				numBars = (maxSeriesBarCount * numComboBarSeries);
			}
			
			if (graphType == graphTypes.bar_stacked || graphType == graphTypes.bar_stacked_percent) {
				numBars = maxSeriesPointCount;
			}
			
			// ensure a percentage
			_autoUpdateBarWidthSpacing = Mathf.Clamp01(autoUpdateBarWidthSpacing);

			if (numBars == 0) numBars++; // guard against divide by 0
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
			setAnchor(graphTitle, new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(graphTitleOffset.x, graphTitleOffset.y));
			changeLabelFontSize(graphTitle, graphTitleSize);
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

	public float getDistBetween(int pointsCount, float theAxisLength) {
		int numPoints = (pointsCount - 1);
		if (numPoints <= 0) {
			return theAxisLength;
		}
		else {
			if (graphType != graphTypes.line && graphType != graphTypes.line_stacked) numPoints += 1;
			return theAxisLength / numPoints;
		}
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

	void setAxesDualYaxis() {
		xAxis.setDualYAxes();
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
		yAxis2.anchorVec = new Vector2 (1, 0.5f);
	}
	
	void setAxesQuadrant1() {
		xAxis.setAxisTopRight(false);
		yAxis.setAxisTopRight(false);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	void setAxesQuadrant2() {
		xAxis.setAxisBotLeft(false);
		yAxis.setAxisTopRight(true);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (1, 0.5f);
	}
	
	void setAxesQuadrant3() {
		xAxis.setAxisBotLeft(true);
		yAxis.setAxisBotLeft(true);
		xAxis.anchorVec = new Vector2 (0.5f, 1);
		yAxis.anchorVec = new Vector2 (1, 0.5f);
	}
	
	void setAxesQuadrant4() {
		xAxis.setAxisTopRight(true);
		yAxis.setAxisBotLeft(false);
		xAxis.anchorVec = new Vector2 (0.5f, 1);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	void setAxesQuadrant1_2_3_4() {
		xAxis.setAxisMiddle(false);
		yAxis.setAxisMiddle(false);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	void setAxesQuadrant1_2() {
		xAxis.setAxisMiddle(false);
		yAxis.setAxisTopRight(false);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	void setAxesQuadrant3_4() {
		xAxis.setAxisMiddle(true);
		yAxis.setAxisBotLeft(false);
		xAxis.anchorVec = new Vector2 (0.5f, 1);
		yAxis.anchorVec = new Vector2 (0, 0.5f);
	}
	
	void setAxesQuadrant2_3() {
		xAxis.setAxisBotLeft(false);
		yAxis.setAxisMiddle(true);
		xAxis.anchorVec = new Vector2 (0.5f, 0);
		yAxis.anchorVec = new Vector2 (1, 0.5f);
	}
	
	void setAxesQuadrant1_4() {
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
	
	/// <summary>
	/// Animate all the points in all the series simultaneously.
	/// </summary>
	/// <param name="isPoint">If set to <c>true</c> is point.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="delay">Delay.</param>
	/// <param name="anEaseType">An ease type.</param>
	/// <param name="before">Before.</param>
	/// <param name="after">After.</param>
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
	
	/// <summary>
	/// Animate all the points in a single series simultaneously, and then proceed to the next series.
	/// </summary>
	/// <param name="isPoint">If set to <c>true</c> is point.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="delay">Delay.</param>
	/// <param name="anEaseType">An ease type.</param>
	/// <param name="before">Before.</param>
	/// <param name="after">After.</param>
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
	
	/// <summary>
	/// Animate the points across multiple series simultaneously, and then proceed to the next points.
	/// </summary>
	/// <param name="isPoint">If set to <c>true</c> is point.</param>
	/// <param name="duration">Duration.</param>
	/// <param name="delay">Delay.</param>
	/// <param name="anEaseType">An ease type.</param>
	/// <param name="before">Before.</param>
	/// <param name="after">After.</param>
	/// <param name="loopDir">Loop dir.</param>
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

	/// <summary>
	/// Create a new series and append it to the end.
	/// </summary>
	/// <returns>The series.</returns>
	public WMG_Series addSeries() {
		return addSeriesAt(lineSeries.Count);
	}

	/// <summary>
	/// Delete the last series.
	/// </summary>
	public void deleteSeries() {
		if (lineSeries.Count > 0) {
			deleteSeriesAt(lineSeries.Count-1);
		}
	}

	/// <summary>
	/// Create a new series and insert it at the specified index.
	/// </summary>
	/// <returns>The <see cref="WMG_Series"/>.</returns>
	/// <param name="index">Index.</param>
	/// <param name="comboType">Combo type.</param>
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
		if (!legend.hideLegend) {
			AutoPadding (); // legend swatch created and positioned in series init, so also need to run auto fit this frame
		}
		return curObj.GetComponent<WMG_Series>();
	}

	/// <summary>
	/// Delete a series at the specified index.
	/// </summary>
	/// <param name="index">Index.</param>
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
		autoPaddingC.Changed();
		if (graphType != graphTypes.line && graphType != graphTypes.line_stacked) seriesNoCountC.Changed();
		legend.legendC.Changed();
	}

	void UpdateFromContainer() {
		if (resizeEnabled) {
			
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
				if (!autoPaddingEnabled) {
					paddingLeftRight = new Vector2(getNewResizeVariable(percentFactor.x, origPaddingLeftRight.x),
					                               getNewResizeVariable(percentFactor.x, origPaddingLeftRight.y));
					paddingTopBottom = new Vector2(getNewResizeVariable(percentFactor.y, origPaddingTopBottom.x),
					                               getNewResizeVariable(percentFactor.y, origPaddingTopBottom.y));
				}
			}
			if ((resizeProperties & ResizeProperties.AutoPaddingAmount) == ResizeProperties.AutoPaddingAmount) {
				autoPaddingAmount = getNewResizeVariable(smallerFactor, origAutoPaddingAmount);
			}
			if ((resizeProperties & ResizeProperties.TickSize) == ResizeProperties.TickSize) {
				tickSize = new Vector2(getNewResizeVariable(smallerFactor, origTickSize.x),
				                        getNewResizeVariable(smallerFactor, origTickSize.y));
			}
			if ((resizeProperties & ResizeProperties.GraphTitleOffset) == ResizeProperties.GraphTitleOffset) {
				graphTitleOffset = new Vector2(getNewResizeVariable(smallerFactor, origGraphTitleOffset.x),
				                               getNewResizeVariable(smallerFactor, origGraphTitleOffset.y));
			}
			if ((resizeProperties & ResizeProperties.GraphTitleSize) == ResizeProperties.GraphTitleSize) {
				if (resizingChangesFontScaleInsteadOfFontSize) {
					graphTitle.transform.localScale = new Vector3(getNewResizeVariable(smallerFactor, 1), getNewResizeVariable(smallerFactor, 1), 1);
				}
				else {
					graphTitleSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, origGraphTitleSize));
				}
			}

			// Axes
			if ((resizeProperties & ResizeProperties.AxesWidth) == ResizeProperties.AxesWidth) {
				axisWidth = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, origAxisWidth));
			}
			if ((resizeProperties & ResizeProperties.AxesLabelSize) == ResizeProperties.AxesLabelSize) {
				if (resizingChangesFontScaleInsteadOfFontSize) {
					yAxis.setLabelScales(getNewResizeVariable(smallerFactor, 1));
					if (yAxis2 != null) yAxis2.setLabelScales(getNewResizeVariable(smallerFactor, 1));
					xAxis.setLabelScales(getNewResizeVariable(smallerFactor, 1));
				}
				else {
					yAxis.AxisLabelSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, yAxis.origAxisLabelSize));
					if (yAxis2 != null) yAxis2.AxisLabelSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, yAxis2.origAxisLabelSize));
					xAxis.AxisLabelSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, xAxis.origAxisLabelSize));
				}
			}
			if ((resizeProperties & ResizeProperties.AxesLabelOffset) == ResizeProperties.AxesLabelOffset) {
				yAxis.AxisLabelSpaceOffset = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, yAxis.origAxisLabelSpaceOffset));
				if (yAxis2 != null) yAxis2.AxisLabelSpaceOffset = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, yAxis2.origAxisLabelSpaceOffset));
				xAxis.AxisLabelSpaceOffset = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, xAxis.origAxisLabelSpaceOffset));
			}
			if ((resizeProperties & ResizeProperties.AxesTitleSize) == ResizeProperties.AxesTitleSize) {
				if (resizingChangesFontScaleInsteadOfFontSize) {
					yAxis.AxisTitle.transform.localScale = new Vector3(getNewResizeVariable(smallerFactor, 1), getNewResizeVariable(smallerFactor, 1), 1);
					if (yAxis2 != null) yAxis2.AxisTitle.transform.localScale = new Vector3(getNewResizeVariable(smallerFactor, 1), getNewResizeVariable(smallerFactor, 1), 1);
					xAxis.AxisTitle.transform.localScale = new Vector3(getNewResizeVariable(smallerFactor, 1), getNewResizeVariable(smallerFactor, 1), 1);
				}
				else {
					yAxis.AxisTitleFontSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, yAxis.origAxisTitleFontSize));
					if (yAxis2 != null) yAxis2.AxisTitleFontSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, yAxis2.origAxisTitleFontSize));
					xAxis.AxisTitleFontSize = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, xAxis.origAxisTitleFontSize));
				}
			}
			if ((resizeProperties & ResizeProperties.AxisTitleOffset) == ResizeProperties.AxisTitleOffset) { 
				yAxis.AxisTitleOffset = new Vector2(getNewResizeVariable(smallerFactor, yAxis.origAxisTitleOffset.x),
				                                    getNewResizeVariable(smallerFactor, yAxis.origAxisTitleOffset.y));
				if (yAxis2 != null) {
					yAxis2.AxisTitleOffset = new Vector2(getNewResizeVariable(smallerFactor, yAxis2.origAxisTitleOffset.x),
					                                     getNewResizeVariable(smallerFactor, yAxis2.origAxisTitleOffset.y));
				}
				xAxis.AxisTitleOffset = new Vector2(getNewResizeVariable(smallerFactor, xAxis.origAxisTitleOffset.x),
				                                    getNewResizeVariable(smallerFactor, xAxis.origAxisTitleOffset.y));
			}
			if ((resizeProperties & ResizeProperties.AxesLinePadding) == ResizeProperties.AxesLinePadding) {
				yAxis.AxisLinePadding = getNewResizeVariable(smallerFactor, yAxis.origAxisLinePadding);
				if (yAxis2 != null) yAxis2.AxisLinePadding = getNewResizeVariable(smallerFactor, yAxis2.origAxisLinePadding);
				xAxis.AxisLinePadding = getNewResizeVariable(smallerFactor, xAxis.origAxisLinePadding);
			}
			if ((resizeProperties & ResizeProperties.AxesArrowSize) == ResizeProperties.AxesArrowSize) {
				Vector2 newYArrowSize = new Vector2(getNewResizeVariable(smallerFactor, yAxis.origAxisArrowSize.x),
				                                    getNewResizeVariable(smallerFactor, yAxis.origAxisArrowSize.y));
				changeSpriteSize(yAxis.AxisArrowDL, Mathf.RoundToInt(newYArrowSize.x), Mathf.RoundToInt(newYArrowSize.y));
				changeSpriteSize(yAxis.AxisArrowUR, Mathf.RoundToInt(newYArrowSize.x), Mathf.RoundToInt(newYArrowSize.y));
				if (yAxis2 != null) {
					Vector2 newY2ArrowSize = new Vector2(getNewResizeVariable(smallerFactor, yAxis2.origAxisArrowSize.x),
						getNewResizeVariable(smallerFactor, yAxis2.origAxisArrowSize.y));
					changeSpriteSize(yAxis2.AxisArrowDL, Mathf.RoundToInt(newY2ArrowSize.x), Mathf.RoundToInt(newY2ArrowSize.y));
					changeSpriteSize(yAxis2.AxisArrowUR, Mathf.RoundToInt(newY2ArrowSize.x), Mathf.RoundToInt(newY2ArrowSize.y));
				}
				Vector2 newXArrowSize = new Vector2(getNewResizeVariable(smallerFactor, xAxis.origAxisArrowSize.x),
				                                    getNewResizeVariable(smallerFactor, xAxis.origAxisArrowSize.y));
				changeSpriteSize(xAxis.AxisArrowDL, Mathf.RoundToInt(newXArrowSize.x), Mathf.RoundToInt(newXArrowSize.y));
				changeSpriteSize(xAxis.AxisArrowUR, Mathf.RoundToInt(newXArrowSize.x), Mathf.RoundToInt(newXArrowSize.y));
			}
			// Legend
			if ((resizeProperties & ResizeProperties.LegendFontSize) == ResizeProperties.LegendFontSize) {
				if (resizingChangesFontScaleInsteadOfFontSize) {
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
	
	float getNewResizeVariable(float sizeFactor, float variable) {
		return variable + ((sizeFactor - 1) * variable);
	}

	[System.Obsolete("Use autoPaddingEnabled")]
	/// <summary>
	/// Obsolete.
	/// </summary>
	public bool autoFitLabels { get; set;}

	[System.Obsolete("Use autoPaddingAmount")]
	/// <summary>
	/// Obsolete.
	/// </summary>
	public float autoFitPadding { get; set;}

	[System.Obsolete("Use xAxis.GetAxisTickNodes")]
	/// <summary>
	/// Obsolete.
	/// </summary>
	public List<WMG_Node> getXAxisTicks() {
		return xAxis.GetAxisTickNodes();
	}
	
	[System.Obsolete("Use xAxis.GetAxisLabelNodes")]
	/// <summary>
	/// Obsolete.
	/// </summary>
	public List<WMG_Node> getXAxisLabels() {
		return xAxis.GetAxisLabelNodes();
	}
	
	[System.Obsolete("Use yAxis.GetAxisTickNodes")]
	/// <summary>
	/// Obsolete.
	/// </summary>
	public List<WMG_Node> getYAxisTicks() {
		return yAxis.GetAxisTickNodes();
	}
	
	[System.Obsolete("Use yAxis.GetAxisLabelNodes")]
	/// <summary>
	/// Obsolete.
	/// </summary>
	public List<WMG_Node> getYAxisLabels() {
		return yAxis.GetAxisLabelNodes();
	}
}
