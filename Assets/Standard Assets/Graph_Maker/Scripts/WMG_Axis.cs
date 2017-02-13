using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The axis used in WMG_Axis_Chart for X / Y  axes, as well as secondary Y axis in Dual Y axis charts.
/// </summary>
public class WMG_Axis : WMG_GUI_Functions {

	public WMG_Axis_Graph graph;

	public enum labelTypes {ticks, ticks_center, groups, manual};

	[SerializeField] private List<string> _axisLabels;
	public WMG_List<string> axisLabels = new WMG_List<string>();

	/// <summary>
	/// Determines where each point / bar in a series gets positioned.
	/// </summary>
	/// <value>The axis minimum value.</value>
	public float AxisMinValue { get {return _AxisMinValue;} 
		set {
			if (_AxisMinValue != value) {
				_AxisMinValue = value;
				graphC.Changed();
				seriesC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines where each point / bar in a series gets positioned.
	/// </summary>
	/// <value>The axis minimum value.</value>
	public float AxisMaxValue { get {return _AxisMaxValue;} 
		set {
			if (_AxisMaxValue != value) {
				_AxisMaxValue = value;
				graphC.Changed();
				seriesC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// This determines the number of ticks that appear on each axis. Each tick can also be associated with a tick label and the gridlines are also aligned with the ticks. 
	/// This value must be 2 or higher. If there is no need to have ticks for your graph, set #hideTicks = true.
	/// </summary>
	/// <value>The axis number ticks.</value>
	public int AxisNumTicks { get {return _AxisNumTicks;} 
		set {
			if (_AxisNumTicks != value) {
				_AxisNumTicks = value;
				if (_AxisNumTicks < 2) {
					Debug.LogWarning("Graph Maker - Axis Num Ticks must be > 1, use Hide Ticks instead");
					_AxisNumTicks = 2;
				}
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines whether the absolute value of #AxisMinValue automatically grows based on WMG_Series data, where growth amount is determined by WMG_Axis_Graph::autoGrowAndShrinkByPercent.
	/// </summary>
	/// <value><c>true</c> if minimum auto grow; otherwise, <c>false</c>.</value>
	public bool MinAutoGrow { get {return _MinAutoGrow;} 
		set {
			if (_MinAutoGrow != value) {
				_MinAutoGrow = value;
				graphC.Changed();
				seriesC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines whether the absolute value of #AxisMaxValue automatically grows based on WMG_Series data, where growth amount is determined by WMG_Axis_Graph::autoGrowAndShrinkByPercent.
	/// </summary>
	/// <value><c>true</c> if max auto grow; otherwise, <c>false</c>.</value>
	public bool MaxAutoGrow { get {return _MaxAutoGrow;} 
		set {
			if (_MaxAutoGrow != value) {
				_MaxAutoGrow = value;
				graphC.Changed();
				seriesC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines whether the absolute value of #AxisMinValue automatically shrinks based on WMG_Series data, where shrink occurs at WMG_Axis_Graph::autoShrinkAtPercent 
	/// and shrink amount is determined by WMG_Axis_Graph::autoGrowAndShrinkByPercent.
	/// </summary>
	/// <value><c>true</c> if minimum auto shrink; otherwise, <c>false</c>.</value>
	public bool MinAutoShrink { get {return _MinAutoShrink;} 
		set {
			if (_MinAutoShrink != value) {
				_MinAutoShrink = value;
				graphC.Changed();
				seriesC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines whether the absolute value of #AxisMaxValue automatically shrinks based on WMG_Series data, where shrink occurs at WMG_Axis_Graph::autoShrinkAtPercent 
	/// and shrink amount is determined by WMG_Axis_Graph::autoGrowAndShrinkByPercent.
	/// </summary>
	/// <value><c>true</c> if max auto shrink; otherwise, <c>false</c>.</value>
	public bool MaxAutoShrink { get {return _MaxAutoShrink;} 
		set {
			if (_MaxAutoShrink != value) {
				_MaxAutoShrink = value;
				graphC.Changed();
				seriesC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Controls how much more space is extended beyond the actual axis length, useful to control how far axis arrows are away from the last tick / gridline.
	/// </summary>
	/// <value>The axis line padding.</value>
	public float AxisLinePadding { get {return _AxisLinePadding;} 
		set {
			if (_AxisLinePadding != value) {
				_AxisLinePadding = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether or not axes are positioned based on percentages rather than on fixed grid ticks. This should be enabled when WMG_Axis_Graph::axesType is of an AUTO_ORIGIN type. 
	/// </summary>
	/// <value><c>true</c> if axis use non tick percent; otherwise, <c>false</c>.</value>
	public bool AxisUseNonTickPercent { get {return _AxisUseNonTickPercent;} 
		set {
			if (_AxisUseNonTickPercent != value) {
				_AxisUseNonTickPercent = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// The percentage (0-1) position of this axis.
	/// </summary>
	/// <value>The axis non tick percent.</value>
	public float AxisNonTickPercent { get {return _AxisNonTickPercent;} 
		set {
			if (_AxisNonTickPercent != value) {
				_AxisNonTickPercent = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether to hide the top / right axis arrow.
	/// </summary>
	/// <value><c>true</c> if hide axis arrow top right; otherwise, <c>false</c>.</value>
	public bool HideAxisArrowTopRight { get {return _HideAxisArrowTopRight;} 
		set {
			if (_HideAxisArrowTopRight != value) {
				_HideAxisArrowTopRight = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether to hide the bot / left axis arrow.
	/// </summary>
	/// <value><c>true</c> if hide axis arrow bot left; otherwise, <c>false</c>.</value>
	public bool HideAxisArrowBotLeft { get {return _HideAxisArrowBotLeft;} 
		set {
			if (_HideAxisArrowBotLeft != value) {
				_HideAxisArrowBotLeft = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether the top / right axis arrow is visible, this is automatically controlled by axes types, use #HideAxisArrowTopRight to always hide.
	/// </summary>
	/// <value><c>true</c> if axis arrow top right; otherwise, <c>false</c>.</value>
	public bool AxisArrowTopRight { get {return _AxisArrowTopRight;} 
		set {
			if (_AxisArrowTopRight != value) {
				_AxisArrowTopRight = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether the bot / left axis arrow is visible, this is automatically controlled by axes types, use #HideAxisArrowBotLeft to always hide.
	/// </summary>
	/// <value><c>true</c> if axis arrow bot left; otherwise, <c>false</c>.</value>
	public bool AxisArrowBotLeft { get {return _AxisArrowBotLeft;} 
		set {
			if (_AxisArrowBotLeft != value) {
				_AxisArrowBotLeft = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether axis ticks are to the right / above the axis line, this is automatically controlled by axes types.
	/// </summary>
	/// <value><c>true</c> if axis ticks right above; otherwise, <c>false</c>.</value>
	public bool AxisTicksRightAbove { get {return _AxisTicksRightAbove;} 
		set {
			if (_AxisTicksRightAbove != value) {
				_AxisTicksRightAbove = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// The tick index at which this axis is placed, this is automatically controlled.
	/// </summary>
	/// <value>The axis tick.</value>
	public int AxisTick { get {return _AxisTick;} 
		set {
			if (_AxisTick != value) {
				_AxisTick = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether the tick label of #AxisTick is hidden, this is automatically controlled.
	/// </summary>
	/// <value><c>true</c> if hide tick; otherwise, <c>false</c>.</value>
	public bool hideTick { get {return _hideTick;} 
		set {
			if (_hideTick != value) {
				_hideTick = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// The label type determines what labels are used and where they are positioned.
	/// - ticks: There are #AxisNumTicks labels, where each label is positioned next to a tick. The labels come from #axisLabels or automatically if #SetLabelsUsingMaxMin = true.
	/// - ticks_center: There are #AxisNumTicks - 1 labels, where each label is positioned between 2 ticks. The labels come from #axisLabels or automatically if #SetLabelsUsingMaxMin = true. 
	/// - groups: The labels come from WMG_Axis_Graph::groups, and are positioned next to the data points / bars.
	/// - manual: The labels come from #axisLabels and are positioned based on #AxisLabelSpacing and #AxisLabelDistBetween.
	/// </summary>
	public labelTypes LabelType { get {return _LabelType;} 
		set {
			if (_LabelType != value) {
				_LabelType = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines how often a label is shown on the axes. For example at 0, all labels are shown, at 1, every other label, and at 2 every other 2 labels.
	/// Useful if you have alot of data (e.g. 365 day strings stored in WMG_Axis_Graph::groups, and only want to display every week or couple weeks worth.
	/// </summary>
	/// <value>The axis label skip interval.</value>
	public int AxisLabelSkipInterval { get {return _AxisLabelSkipInterval;} 
		set {
			if (_AxisLabelSkipInterval != value) {
				_AxisLabelSkipInterval = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines how many labels to skip at the start to show on the axis, e.g. a value of 1 will skip the first label.
	/// </summary>
	/// <value>The axis label skip start.</value>
	public int AxisLabelSkipStart { get {return _AxisLabelSkipStart;} 
		set {
			if (_AxisLabelSkipStart != value) {
				_AxisLabelSkipStart = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Rotates all the labels with the specified number of degrees.
	/// </summary>
	/// <value>The axis label rotation.</value>
	public float AxisLabelRotation { get {return _AxisLabelRotation;} 
		set {
			if (_AxisLabelRotation != value) {
				_AxisLabelRotation = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// If this is true, then the #axisLabels get automatically set based on #AxisNumTicks, #AxisMaxValue, #AxisMinValue, and #numDecimalsAxisLabels. 
	/// </summary>
	/// <value><c>true</c> if set labels using max minimum; otherwise, <c>false</c>.</value>
	public bool SetLabelsUsingMaxMin { get {return _SetLabelsUsingMaxMin;} 
		set {
			if (_SetLabelsUsingMaxMin != value) {
				_SetLabelsUsingMaxMin = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the size of the axis labels.
	/// </summary>
	/// <value>The size of the axis labels.</value>
	public int AxisLabelSize { get {return _AxisLabelSize;} 
		set {
			if (_AxisLabelSize != value) {
				_AxisLabelSize = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the color of the axis labels.
	/// </summary>
	/// <value>The color of the axis labels.</value>
	public Color AxisLabelColor { get {return _AxisLabelColor;} 
		set {
			if (_AxisLabelColor != value) {
				_AxisLabelColor = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the axis label font style.
	/// </summary>
	/// <value>The axis label font style.</value>
	public FontStyle AxisLabelFontStyle { get {return _AxisLabelFontStyle;} 
		set {
			if (_AxisLabelFontStyle != value) {
				_AxisLabelFontStyle = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the axis label font.
	/// </summary>
	/// <value>The axis label font.</value>
	public Font AxisLabelFont { get {return _AxisLabelFont;} 
		set {
			if (_AxisLabelFont != value) {
				_AxisLabelFont = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the number of decimals used in axis labels, used when #SetLabelsUsingMaxMin = true.
	/// </summary>
	/// <value>The number decimals axis labels.</value>
	public int numDecimalsAxisLabels { get {return _numDecimalsAxisLabels;} 
		set {
			if (_numDecimalsAxisLabels != value) {
				_numDecimalsAxisLabels = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether or not all the axes labels for this axis are hidden.
	/// </summary>
	/// <value><c>true</c> if hide labels; otherwise, <c>false</c>.</value>
	public bool hideLabels { get {return _hideLabels;} 
		set {
			if (_hideLabels != value) {
				_hideLabels = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Controls the amount of space between the axis line and the axis labels.
	/// </summary>
	/// <value>The axis label space offset.</value>
	public float AxisLabelSpaceOffset { get {return _AxisLabelSpaceOffset;} 
		set {
			if (_AxisLabelSpaceOffset != value) {
				_AxisLabelSpaceOffset = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Only needs to be set manually for WMG_Axis_Graph::axesType = manual. Controls how much all of the labels are offset in the direction of the axis line. 
	/// </summary>
	/// <value>The axis label spacing.</value>
	public float AxisLabelSpacing { get {return _AxisLabelSpacing;} 
		set {
			if (_AxisLabelSpacing != value) {
				_AxisLabelSpacing = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Only needs to be set manually for WMG_Axis_Graph::axesType = manual. This is the amount of space between each label.
	/// </summary>
	/// <value>The axis label dist between.</value>
	public float AxisLabelDistBetween { get {return _AxisLabelDistBetween;} 
		set {
			if (_AxisLabelDistBetween != value) {
				_AxisLabelDistBetween = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines whether grid lines for this axis appear.
	/// </summary>
	/// <value><c>true</c> if hide grid; otherwise, <c>false</c>.</value>
	public bool hideGrid { get {return _hideGrid;} 
		set {
			if (_hideGrid != value) {
				_hideGrid = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines whether tick marks for this axis appear.
	/// </summary>
	/// <value><c>true</c> if hide ticks; otherwise, <c>false</c>.</value>
	public bool hideTicks { get {return _hideTicks;} 
		set {
			if (_hideTicks != value) {
				_hideTicks = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether or not the axis line is visible.
	/// </summary>
	/// <value><c>true</c> if hide axis line; otherwise, <c>false</c>.</value>
	public bool hideAxisLine { get {return _hideAxisLine;} 
		set {
			if (_hideAxisLine != value) {
				_hideAxisLine = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the axis title string.
	/// </summary>
	/// <value>The axis title string.</value>
	public string AxisTitleString { get {return _AxisTitleString;} 
		set {
			if (_AxisTitleString != value) {
				_AxisTitleString = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the axis title offset position relative to the axis line.
	/// </summary>
	/// <value>The axis title offset.</value>
	public Vector2 AxisTitleOffset { get {return _AxisTitleOffset;} 
		set {
			if (_AxisTitleOffset != value) {
				_AxisTitleOffset = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the size of the axis title font.
	/// </summary>
	/// <value>The size of the axis title font.</value>
	public int AxisTitleFontSize { get {return _AxisTitleFontSize;} 
		set {
			if (_AxisTitleFontSize != value) {
				_AxisTitleFontSize = value;
				graphC.Changed();
				graph.autoPaddingC.Changed();
			}
		}
	}

	// Public variables without change tracking
	public GameObject AxisTitle;
	public GameObject GridLines;
	public GameObject AxisTicks;
	public GameObject AxisLine;
	public GameObject AxisArrowUR;
	public GameObject AxisArrowDL;
	public GameObject AxisObj;
	public GameObject AxisLabelObjs;

	// Private backing variables
	[SerializeField] private float _AxisMinValue;
	[SerializeField] private float _AxisMaxValue;
	[SerializeField] private int _AxisNumTicks;
	[SerializeField] private bool _MinAutoGrow;
	[SerializeField] private bool _MaxAutoGrow;
	[SerializeField] private bool _MinAutoShrink;
	[SerializeField] private bool _MaxAutoShrink;
	[SerializeField] private float _AxisLinePadding;
	[SerializeField] private bool _AxisUseNonTickPercent;
	[SerializeField] private float _AxisNonTickPercent;
	[SerializeField] private bool _HideAxisArrowTopRight;
	[SerializeField] private bool _HideAxisArrowBotLeft;
	[SerializeField] private bool _AxisArrowTopRight;
	[SerializeField] private bool _AxisArrowBotLeft;
	[SerializeField] private bool _AxisTicksRightAbove;
	[SerializeField] private int _AxisTick;
	[SerializeField] private bool _hideTick;
	[SerializeField] private labelTypes _LabelType;
	[SerializeField] private int _AxisLabelSkipStart;
	[SerializeField] private int _AxisLabelSkipInterval;
	[SerializeField] private float _AxisLabelRotation;
	[SerializeField] private bool _SetLabelsUsingMaxMin;
	[SerializeField] private int _AxisLabelSize;
	[SerializeField] private Color _AxisLabelColor = Color.white;
	[SerializeField] private FontStyle _AxisLabelFontStyle = FontStyle.Normal;
	[SerializeField] private Font _AxisLabelFont; 
	[SerializeField] private int _numDecimalsAxisLabels;
	[SerializeField] private bool _hideLabels;
	[SerializeField] private float _AxisLabelSpaceOffset;
	[SerializeField] private float _AxisLabelSpacing;
	[SerializeField] private float _AxisLabelDistBetween;
	[SerializeField] private bool _hideGrid;
	[SerializeField] private bool _hideTicks;
	[SerializeField] private bool _hideAxisLine;
	[SerializeField] private string _AxisTitleString;
	[SerializeField] private Vector2 _AxisTitleOffset;
	[SerializeField] private int _AxisTitleFontSize;

	// Useful property getters
	public float AxisLength { 
		get {
			if (isY) return graph.yAxisLength;
			else return graph.xAxisLength;
		}
	}

	// Private variables
	private float GridLineLength;
	private float AxisLinePaddingTot;
	private float AxisPercentagePosition;
	
	// Original property values for use with dynamic resizing
	public int origAxisLabelSize { get; private set; }
	public float origAxisLabelSpaceOffset { get; private set; }
	public int origAxisTitleFontSize { get; private set; }
	public Vector2 origAxisTitleOffset { get; private set; }
	public float origAxisLinePadding { get; private set; }
	public Vector2 origAxisArrowSize { get; private set; }

	// Others
	private bool hasInit;
	private WMG_Axis otherAxis;
	private WMG_Axis otherAxis2;
	public bool isY { get; private set; }
	public bool isSecondary { get; private set; }
	public Vector2 anchorVec;
	
	public delegate string AxisLabelLabeler(WMG_Axis axis, int labelIndex);
	/// <summary>
	/// Use to override the default labeler for axis labels to put for example dollar signs for the labels.
	/// @code
	/// graph.yAxis.axisLabelLabeler = customYAxisLabelLabeler;
	/// string customYAxisLabelLabeler(WMG_Axis axis, int labelIndex) {}
	/// @endcode
	/// </summary>
	public AxisLabelLabeler axisLabelLabeler;


	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	private WMG_Change_Obj graphC = new WMG_Change_Obj();
	private WMG_Change_Obj seriesC = new WMG_Change_Obj();

	public void Init(WMG_Axis otherAxis, WMG_Axis otherAxis2, bool isY, bool isSecondary) {
		if (hasInit) return;
		hasInit = true;
		
		changeObjs.Add(graphC);
		changeObjs.Add(seriesC);
		
		this.otherAxis = otherAxis;
		this.otherAxis2 = otherAxis2;
		this.isY = isY;
		this.isSecondary = isSecondary;
		
		axisLabels.SetList (_axisLabels);
		axisLabels.Changed += axisLabelsChanged;
		
		graphC.OnChange += GraphChanged;
		seriesC.OnChange += SeriesChanged;

		axisLabelLabeler = defaultAxisLabelLabeler;
		
		setOriginalPropertyValues();
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

	void GraphChanged() {
		graph.graphC.Changed ();
	}

	void SeriesChanged() {
		graph.seriesNoCountC.Changed();
	}

	private void axisLabelsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref axisLabels, ref _axisLabels, oneValChanged, index);
		graphC.Changed();
		graph.autoPaddingC.Changed();
	}

	// Set initial property values for use with percentage based dynamic resizing 
	public void setOriginalPropertyValues() {
		origAxisLabelSize = AxisLabelSize;
		origAxisTitleFontSize = AxisTitleFontSize;
		origAxisTitleOffset = AxisTitleOffset;
		origAxisLabelSpaceOffset = AxisLabelSpaceOffset;
		origAxisLinePadding = AxisLinePadding;
		origAxisArrowSize = getSpriteSize (AxisArrowDL);
	}

	public void setDualYAxes() {
		// x-axis
		_AxisArrowTopRight = false;
		_AxisArrowBotLeft = false;
		_AxisTicksRightAbove = false;
		_hideTick = false;
		_AxisTick = 0;
		_AxisNonTickPercent = 0;
		// left y axis
		otherAxis.setOtherArrowTopRight(true);
		otherAxis.setOtherArrowBotLeft(false);
		otherAxis.setOtherRightAbove(false);
		otherAxis.setOtherHideTick (false);
		otherAxis.setOtherAxisTick (0);
		otherAxis.setOtherAxisNonTickPercent (0);
		// right y axis
		otherAxis2.setOtherArrowTopRight(true);
		otherAxis2.setOtherArrowBotLeft(false);
		otherAxis2.setOtherRightAbove(true);
		otherAxis2.setOtherHideTick (false);
		otherAxis2.setOtherAxisTick (AxisNumTicks - 1);
		otherAxis2.setOtherAxisNonTickPercent (1);
	}

	public void setAxisTopRight(bool rightAbove) {
		_AxisArrowTopRight = true;
		_AxisArrowBotLeft = false;
		otherAxis.setOtherHideTick (false);
		otherAxis.setOtherAxisTick (0);
		otherAxis.setOtherAxisNonTickPercent (0);
		_AxisTicksRightAbove = rightAbove;
	}

	public void setAxisBotLeft(bool rightAbove) {
		_AxisArrowTopRight = false;
		_AxisArrowBotLeft = true;
		otherAxis.setOtherHideTick (false);
		otherAxis.setOtherAxisTick (AxisNumTicks - 1);
		otherAxis.setOtherAxisNonTickPercent (1);
		_AxisTicksRightAbove = rightAbove;
	}

	public void setAxisMiddle(bool rightAbove) {
		_AxisArrowTopRight = true;
		_AxisArrowBotLeft = true;
		otherAxis.setOtherHideTick (true);
		otherAxis.setOtherAxisTick (AxisNumTicks / 2);
		otherAxis.setOtherAxisNonTickPercent (0.5f);
		_AxisTicksRightAbove = rightAbove;
	}

	public void setOtherAxisNonTickPercent(float val) {
		_AxisNonTickPercent = val;
	}

	public void setOtherAxisTick(int val) {
		_AxisTick = val;
	}

	public void setOtherHideTick(bool val) {
		_hideTick = val;
	}

	public void setOtherRightAbove(bool val) {
		_AxisTicksRightAbove = val;
	}

	public void setOtherArrowBotLeft(bool val) {
		_AxisArrowBotLeft = val;
	}

	public void setOtherArrowTopRight(bool val) {
		_AxisArrowTopRight = val;
	}

	public void possiblyHideTickBasedOnPercent() {
		// Ensure tick is not hidden if percent is being used and num ticks is even
		if (otherAxis.AxisUseNonTickPercent && AxisNumTicks % 2 == 0) {
			_hideTick = false;
		}
	}

	public void ChangeOrientation() {
		labelTypes tLabelType = LabelType;
		float tAxisMaxValue = AxisMaxValue;
		float tAxisMinValue = AxisMinValue;
		int tAxisNumTicks = AxisNumTicks;
		int tnumDecimalsAxisLabels = numDecimalsAxisLabels;
		bool tMinAutoGrow = MinAutoGrow;
		bool tMaxAutoGrow = MaxAutoGrow;
		bool tMinAutoShrink = MinAutoShrink;
		bool tMaxAutoShrink = MaxAutoShrink;
		bool tSetLabelsUsingMaxMin = SetLabelsUsingMaxMin;
		float tAxisLabelSpacing = AxisLabelSpacing;
		string tAxisTitleString = AxisTitleString;
		bool tHideTicks = hideTicks;
		bool tHideGrid = hideGrid;
		List<string> tLabels = new List<string>(_axisLabels);
		
		WMG_Axis otherAxis = this.otherAxis;
		if (!otherAxis) {
			otherAxis = graph.xAxis;
		}
		
		LabelType = otherAxis.LabelType;
		AxisMaxValue = otherAxis.AxisMaxValue;
		AxisMinValue = otherAxis.AxisMinValue;
		AxisNumTicks = otherAxis.AxisNumTicks;
		hideTicks = otherAxis.hideTicks;
		hideGrid = otherAxis.hideGrid;
		numDecimalsAxisLabels = otherAxis.numDecimalsAxisLabels;
		MinAutoGrow = otherAxis.MinAutoGrow;
		MaxAutoGrow = otherAxis.MaxAutoGrow;
		MinAutoShrink = otherAxis.MinAutoShrink;
		MaxAutoShrink = otherAxis.MaxAutoShrink;
		SetLabelsUsingMaxMin = otherAxis.SetLabelsUsingMaxMin;
		AxisLabelSpacing = otherAxis.AxisLabelSpacing;
		AxisTitleString = otherAxis.AxisTitleString;
		if (Application.isPlaying) {
			axisLabels.SetList(otherAxis.axisLabels);
		}
		else {
			axisLabels.SetListNoCb(otherAxis._axisLabels, ref _axisLabels);
		}
		otherAxis.ChangeOrientationEnd(tLabelType, tAxisMaxValue, tAxisMinValue, tAxisNumTicks, tnumDecimalsAxisLabels,
		                               tMinAutoGrow, tMaxAutoGrow, tMinAutoShrink, tMaxAutoShrink,
		                               tSetLabelsUsingMaxMin, tAxisLabelSpacing, tAxisTitleString, tLabels, tHideTicks, tHideGrid);
	}
	
	public void ChangeOrientationEnd(labelTypes tLabelType, float tAxisMaxValue, float tAxisMinValue, int tAxisNumTicks, int tnumDecimalsAxisLabels,
	                                 bool tMinAutoGrow, bool tMaxAutoGrow, bool tMinAutoShrink, bool tMaxAutoShrink,
	                                 bool tSetLabelsUsingMaxMin, float tAxisLabelSpacing, string tAxisTitleString, List<string> tLabels, bool tHideTicks, bool tHideGrid) {
		LabelType = tLabelType;
		AxisMaxValue = tAxisMaxValue;
		AxisMinValue = tAxisMinValue;
		AxisNumTicks = tAxisNumTicks;
		hideTicks = tHideTicks;
		hideGrid = tHideGrid;
		numDecimalsAxisLabels = tnumDecimalsAxisLabels;
		MinAutoGrow = tMinAutoGrow;
		MaxAutoGrow = tMaxAutoGrow;
		MinAutoShrink = tMinAutoShrink;
		MaxAutoShrink = tMaxAutoShrink;
		SetLabelsUsingMaxMin = tSetLabelsUsingMaxMin;
		AxisLabelSpacing = tAxisLabelSpacing;
		AxisTitleString = tAxisTitleString;
		if (Application.isPlaying) {
			axisLabels.SetList(tLabels);
		}
		else {
			axisLabels.SetListNoCb(tLabels, ref _axisLabels);
		}
	}

	public void updateAxesRelativeToOrigin(float originVal) {
		if (graph.axesType == WMG_Axis_Graph.axesTypes.AUTO_ORIGIN || graph.axesType == (isY ? WMG_Axis_Graph.axesTypes.AUTO_ORIGIN_Y : WMG_Axis_Graph.axesTypes.AUTO_ORIGIN_X)) {
			bool otherRightAbove = otherAxis.AxisTicksRightAbove;
			if (originVal >= otherAxis.AxisMaxValue) {
				otherAxis.setAxisBotLeft(false);
				_AxisTicksRightAbove = true;
			}
			else if (originVal <= otherAxis.AxisMinValue) {
				otherAxis.setAxisTopRight(false);
				_AxisTicksRightAbove = false;
			}
			else {
				otherAxis.setAxisMiddle(false);
				_AxisTicksRightAbove = false;
				_AxisTick = Mathf.RoundToInt((originVal - otherAxis.AxisMinValue) / (otherAxis.AxisMaxValue - otherAxis.AxisMinValue) * (otherAxis.AxisNumTicks - 1));
				_AxisNonTickPercent = (originVal - otherAxis.AxisMinValue) / (otherAxis.AxisMaxValue - otherAxis.AxisMinValue);
			}
			otherAxis.setOtherRightAbove(otherRightAbove);
		}
	}

	public void UpdateAxesGridsAndTicks() {
		// Calculate variables used in axis and grid positions
		// Ensure num ticks don't go below 1, update gridLineLength
		if (AxisNumTicks <= 1) {
			_AxisNumTicks = 1;
			GridLineLength = 0;
		}
		else {
			GridLineLength = AxisLength / (AxisNumTicks-1);
		}
		
		// update AxisPercentagePosition
		if (AxisUseNonTickPercent) { // position axis based on the percentage specified
			AxisPercentagePosition = AxisNonTickPercent;
		}
		else { // position axis based on the number of ticks and the specified tick
			if (otherAxis.AxisNumTicks == 1) AxisPercentagePosition = 1;
			else AxisPercentagePosition = AxisTick / (otherAxis.AxisNumTicks - 1f);
		}

		if (GridLines != null) {
			// Hide grids
			SetActive(GridLines, !hideGrid);

			if (!hideGrid) {
				// Update grid lines
				WMG_Grid gridLines = GridLines.GetComponent<WMG_Grid>();
				if (isY) {
					gridLines.gridNumNodesY = AxisNumTicks;
					gridLines.gridLinkLengthY = GridLineLength;
					gridLines.gridLinkLengthX = otherAxis.AxisLength;
				}
				else {
					gridLines.gridNumNodesX = AxisNumTicks;
					gridLines.gridLinkLengthX = GridLineLength;
					gridLines.gridLinkLengthY = otherAxis.AxisLength;
				}

				gridLines.Refresh();
			}
		}
		
		// Hide ticks
		SetActive(AxisTicks, !hideTicks);
		
		if (!hideTicks) {
			// Update ticks
			WMG_Grid ticks = AxisTicks.GetComponent<WMG_Grid>();
			if (isY) {
				ticks.gridNumNodesY = AxisNumTicks;
				ticks.gridLinkLengthY = GridLineLength;
			}
			else {
				ticks.gridNumNodesX = AxisNumTicks;
				ticks.gridLinkLengthX = GridLineLength;
			}
			ticks.Refresh();
			
			if (!AxisTicksRightAbove) {
				if (isY) {
					changeSpritePositionToX(AxisTicks, AxisPercentagePosition * otherAxis.AxisLength - graph.axisWidth / 2 - graph.tickSize.y / 2 );
				}
				else {
					changeSpritePositionToY(AxisTicks, AxisPercentagePosition * otherAxis.AxisLength - graph.axisWidth / 2 - graph.tickSize.y / 2 );
				}
			}
			else {
				if (isY) {
					changeSpritePositionToX(AxisTicks, AxisPercentagePosition * otherAxis.AxisLength + graph.axisWidth / 2 + graph.tickSize.y / 2);
				}
				else {
					changeSpritePositionToY(AxisTicks, AxisPercentagePosition * otherAxis.AxisLength + graph.axisWidth / 2 + graph.tickSize.y / 2);
				}
			}

			// Update size of ticks
			foreach (WMG_Node node in GetAxisTickNodes()) {
				changeSpriteSize(node.objectToScale, Mathf.RoundToInt(isY ? graph.tickSize.y : graph.tickSize.x), Mathf.RoundToInt(isY ? graph.tickSize.x : graph.tickSize.y));
			}
		}

		
		// update axis visuals
		AxisLinePaddingTot = 2 * AxisLinePadding;
		float axisRepos = 0;
		if (AxisArrowTopRight) axisRepos += AxisLinePadding / 2f;
		else AxisLinePaddingTot -= AxisLinePadding;
		if (AxisArrowBotLeft) axisRepos -= AxisLinePadding / 2f;
		else AxisLinePaddingTot -= AxisLinePadding;
		
		if (isY) {
			changeSpriteSize(AxisLine, graph.axisWidth, Mathf.RoundToInt(AxisLength + AxisLinePaddingTot));
			changeSpritePositionTo(AxisLine, new Vector3(0, axisRepos + AxisLength/2, 0));
			changeSpritePositionToX(AxisObj, AxisPercentagePosition * otherAxis.AxisLength);
		}
		else {
			changeSpriteSize(AxisLine, Mathf.RoundToInt(AxisLength + AxisLinePaddingTot), graph.axisWidth);
			changeSpritePositionTo(AxisLine, new Vector3(axisRepos + AxisLength/2, 0, 0));
			changeSpritePositionToY(AxisObj, AxisPercentagePosition * otherAxis.AxisLength);
		}
		
		// Update Arrows
		SetActiveAnchoredSprite(AxisArrowUR, HideAxisArrowTopRight ? false : AxisArrowTopRight);
		SetActiveAnchoredSprite(AxisArrowDL, HideAxisArrowBotLeft ? false : AxisArrowBotLeft);

		// Update axis line
		changeSpriteAlpha(AxisLine, hideAxisLine ? 0 : 1);
	}

	public void UpdateTitle() {
		if (AxisTitle != null) {
			changeLabelText(AxisTitle, AxisTitleString);
			changeLabelFontSize(AxisTitle, AxisTitleFontSize);
			// update rotation and position
			if (isY) {
				if (anchorVec.x == 1) { // right
					AxisTitle.transform.localEulerAngles = new Vector3 (0, 0, 270);
					setAnchor(AxisTitle, anchorVec, new Vector2(0.5f, 0f), new Vector2(AxisTitleOffset.x, AxisTitleOffset.y));
				}
				else { // left
					AxisTitle.transform.localEulerAngles = new Vector3 (0, 0, 90);
					setAnchor(AxisTitle, anchorVec, new Vector2(0.5f, 0f), new Vector2(-AxisTitleOffset.x, AxisTitleOffset.y));
				}
			}
			else {
				if (anchorVec.y == 1) { // top
					setAnchor(AxisTitle, anchorVec, new Vector2(0.5f, 0f), new Vector2(AxisTitleOffset.x, AxisTitleOffset.y));
				}
				else { // bot
					setAnchor(AxisTitle, anchorVec, new Vector2(0.5f, 1f), new Vector2(AxisTitleOffset.x, -AxisTitleOffset.y));
				}
			}
		}
	}

	public void UpdateAxesMinMaxValues() {
		if (!MinAutoGrow && !MaxAutoGrow && !MinAutoShrink && !MaxAutoShrink) return;
		float min = Mathf.Infinity;
		float max = Mathf.NegativeInfinity;
		for (int j = 0; j < graph.lineSeries.Count; j++) {
			if (!activeInHierarchy(graph.lineSeries[j])) continue;
			WMG_Series theSeries = graph.lineSeries[j].GetComponent<WMG_Series>();
			if (graph.axesType == WMG_Axis_Graph.axesTypes.DUAL_Y) {
				if (isY && isSecondary && !theSeries.useSecondYaxis) continue; // secondary y-axis, but this series is for primary
				if (isY && !isSecondary && theSeries.useSecondYaxis) continue; // primary y-axis, but this series is for secondary
			}
			// Find the current max and min point value data
			if (graph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) {
				for (int i = 0; i < theSeries.pointValues.Count; i++) {
					if (isY) {
						if (theSeries.pointValues[i].y < min) min = theSeries.pointValues[i].y;
						if (theSeries.pointValues[i].y > max) max = theSeries.pointValues[i].y;
						if (graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked || graph.graphType == WMG_Axis_Graph.graphTypes.line_stacked) {
							if (graph.TotalPointValues[i] + AxisMinValue > max) max = graph.TotalPointValues[i] + AxisMinValue;
						}
					}
					else {
						if (theSeries.pointValues[i].x < min) min = theSeries.pointValues[i].x;
						if (theSeries.pointValues[i].x > max) max = theSeries.pointValues[i].x;
					}
				}
			}
			else {
				for (int i = 0; i < theSeries.pointValues.Count; i++) {
					if (isY) {
						if (theSeries.pointValues[i].x < min) min = theSeries.pointValues[i].x;
						if (theSeries.pointValues[i].x > max) max = theSeries.pointValues[i].x;
					}
					else {
						if (theSeries.pointValues[i].y < min) min = theSeries.pointValues[i].y;
						if (theSeries.pointValues[i].y > max) max = theSeries.pointValues[i].y;
						if (graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked || graph.graphType == WMG_Axis_Graph.graphTypes.line_stacked) {
							if (graph.TotalPointValues[i] + AxisMinValue > max) max = graph.TotalPointValues[i] + AxisMinValue;
						}
					}
				}
			}
		}
		// If point data outside axis max / min then grow, if the point data significantly (percentage of total axis length variable) less than axis min / max then srhink 
		if (MinAutoGrow || MaxAutoGrow || MinAutoShrink || MaxAutoShrink) {
			if (min == Mathf.Infinity || max == Mathf.NegativeInfinity) return;
			float origMax = AxisMaxValue;
			float origMin = AxisMinValue;
			// grow - max
			if (MaxAutoGrow && max > origMax) {
				AutoSetAxisMinMax(max, min, true, true, origMin, origMax);
			}
			// grow - min
			if (MinAutoGrow && min < origMin) {
				AutoSetAxisMinMax(min, max, false, true, origMin, origMax);
			}
			if (max == min) return; // can't shrink when max and min are the same because it's percentage based and would shrink forever
			// shrink - max
			if (MaxAutoShrink && graph.autoShrinkAtPercent > (max - origMin) / (origMax - origMin) ) {
				AutoSetAxisMinMax(max, min, true, false, origMin, origMax);
			}
			// shrink - min
			if (MinAutoShrink && graph.autoShrinkAtPercent > (origMax - min) / (origMax - origMin) ) {
				AutoSetAxisMinMax(min, max, false, false, origMin, origMax);
			}
		}
	}

	// Helper function for update min max, ensures the new values have sensible level of precision
	void AutoSetAxisMinMax(float val, float val2, bool max, bool grow, float aMin, float aMax) {
		int numTicks = 0;
		numTicks = AxisNumTicks-1;
		
		float changeAmt = 1 + graph.autoGrowAndShrinkByPercent;
		
		// Find tentative new max / min value
		float temp = 0;
		if (max) {
			if (grow) temp = changeAmt * (val - aMin) / (numTicks);
			else temp = changeAmt * (val - val2) / (numTicks);
		}
		else {
			if (grow) temp = changeAmt * (aMax - val) / (numTicks);
			else temp = changeAmt * (val2 - val) / (numTicks);
		}
		
		if (temp == 0 || aMax <= aMin) return;
		
		// Determine level of precision of tentative new value
		float temp2 = temp;
		int pow = 0;
		
		if (Mathf.Abs(temp2) > 1) {
			while (Mathf.Abs(temp2) > 10) {
				pow++;
				temp2 /= 10f;
			}
		}
		else {
			while (Mathf.Abs(temp2) < 0.1f) {
				pow--;
				temp2 *= 10f;
			}
		}
		
		// Update tentative to sensible level of precision
		float temp3 = Mathf.Pow( 10f, pow-1);
		temp2 = temp - (temp % temp3) + temp3;
		
		float newVal = 0;
		if (max) {
			if (grow) newVal = (numTicks) * temp2 + aMin;
			else newVal = (numTicks) * temp2 + val2;
		}
		else {
			if (grow) newVal = aMax - (numTicks) * temp2;
			else newVal = val2 - (numTicks) * temp2;
		}
		
		// Set the min / max value to the newly calculated value
		if (max) {
			AxisMaxValue = newVal;
		}
		else {
			AxisMinValue = newVal;
		}
	}


	public void UpdateAxesLabels() {
		// Calculate the number of labels we have
		int numLabels = 0;
		if (LabelType == labelTypes.ticks) numLabels = AxisNumTicks;
		else if (LabelType == labelTypes.ticks_center) numLabels = AxisNumTicks - 1;
		else if (LabelType == labelTypes.groups) numLabels = graph.groups.Count;
		else numLabels = axisLabels.Count;
		
		// Update spacing between labels
		float distBetween = graph.getDistBetween(graph.groups.Count, AxisLength);
		if (LabelType == labelTypes.ticks) _AxisLabelDistBetween = AxisLength / (numLabels - 1);
		else if (LabelType == labelTypes.ticks_center) _AxisLabelDistBetween = AxisLength / numLabels;
		else if (LabelType == labelTypes.groups) _AxisLabelDistBetween = distBetween;
		
		// Actually create or delete the labels and apply the spacing
		WMG_Grid axisLabelGrid = AxisLabelObjs.GetComponent<WMG_Grid>();
		if (isY) {
			axisLabelGrid.gridNumNodesY = numLabels;
			axisLabelGrid.gridLinkLengthY = AxisLabelDistBetween;
		}
		else {
			axisLabelGrid.gridNumNodesX = numLabels;
			axisLabelGrid.gridLinkLengthX = AxisLabelDistBetween;
		}
		axisLabelGrid.Refresh();
		
		// Create or delete strings based on number of labels
		for (int i = 0; i < numLabels; i++) {
			if (axisLabels.Count <= i) {
				axisLabels.AddNoCb("", ref _axisLabels);
			}
		}
		for (int i = axisLabels.Count - 1; i >= 0; i--) {
			if (i >= numLabels) {
				axisLabels.RemoveAtNoCb(i, ref _axisLabels);
			}
		}
		
		// Update xSpacingx and ySpacingy
		if (LabelType == labelTypes.ticks) _AxisLabelSpacing = 0;
		else if (LabelType == labelTypes.ticks_center) {
			if (AxisNumTicks == 1) _AxisLabelSpacing = 0;
			else _AxisLabelSpacing = AxisLength / (AxisNumTicks - 1) / 2;
		}
		else if (LabelType == labelTypes.groups) {
			if (graph.graphType == WMG_Axis_Graph.graphTypes.line || graph.graphType == WMG_Axis_Graph.graphTypes.line_stacked) {
				_AxisLabelSpacing = 0;
			}
			else {
				if (graph.groups.Count == 0) _AxisLabelSpacing = 0;
				else _AxisLabelSpacing = AxisLength / (graph.groups.Count) / 2;
			}
		}

		
		// Position the label parent objects
		float tickOffset = 0;
		if (LabelType == labelTypes.ticks || (LabelType == labelTypes.groups && AxisNumTicks == graph.groups.Count)) {
			tickOffset = graph.tickSize.y;
		}
		if (isY) {
			if (!AxisTicksRightAbove) {
				changeSpritePositionToX(AxisLabelObjs, AxisPercentagePosition * otherAxis.AxisLength - tickOffset - graph.axisWidth / 2);
			}
			else {
				changeSpritePositionToX(AxisLabelObjs, AxisPercentagePosition * otherAxis.AxisLength + tickOffset + graph.axisWidth / 2);
			}
		}
		else {
			if (!AxisTicksRightAbove) {
				changeSpritePositionToY(AxisLabelObjs, AxisPercentagePosition * otherAxis.AxisLength - tickOffset - graph.axisWidth / 2);
			}
			else {
				changeSpritePositionToY(AxisLabelObjs, AxisPercentagePosition * otherAxis.AxisLength + tickOffset + graph.axisWidth / 2);
			}
		}

		// Get the label objects, change their position, and set their text
		List<WMG_Node> LabelNodes = GetAxisLabelNodes();
		
		if (LabelNodes == null) return;
		
		for (int i = 0; i < axisLabels.Count; i++) {
			if (i >= LabelNodes.Count) break;
			
			// Hide labels
			SetActive(LabelNodes[i].gameObject,!hideLabels);
			// Hide label that is the same as the axis
			if (LabelType == labelTypes.ticks) {
				if (hideTick && i == otherAxis.AxisTick) SetActive(LabelNodes[otherAxis.AxisTick].gameObject,false);
			}
			
			// Rotate the labels
			LabelNodes[i].objectToLabel.transform.localEulerAngles = new Vector3(0, 0, AxisLabelRotation);

			// Position and pivot the labels
			if (isY) {
				if (!AxisTicksRightAbove) {
					changeSpritePivot(LabelNodes[i].objectToLabel, WMG_Graph_Manager.WMGpivotTypes.Right);
					changeSpritePositionTo(LabelNodes[i].objectToLabel, new Vector3(-AxisLabelSpaceOffset, AxisLabelSpacing, 0));
				}
				else {
					changeSpritePivot(LabelNodes[i].objectToLabel, WMG_Graph_Manager.WMGpivotTypes.Left);
					changeSpritePositionTo(LabelNodes[i].objectToLabel, new Vector3(AxisLabelSpaceOffset, AxisLabelSpacing, 0));
				}
			}
			else {
				if (!AxisTicksRightAbove) {
					if (AxisLabelRotation > 0) {
						changeSpritePivot(LabelNodes[i].objectToLabel, WMG_Graph_Manager.WMGpivotTypes.TopRight);
					}
					else {
						changeSpritePivot(LabelNodes[i].objectToLabel, WMG_Graph_Manager.WMGpivotTypes.Top);
					}
					changeSpritePositionTo(LabelNodes[i].objectToLabel, new Vector3(AxisLabelSpacing, -AxisLabelSpaceOffset, 0));
				}
				else {
					if (AxisLabelRotation > 0) {
						changeSpritePivot(LabelNodes[i].objectToLabel, WMG_Graph_Manager.WMGpivotTypes.BottomLeft);
					}
					else {
						changeSpritePivot(LabelNodes[i].objectToLabel, WMG_Graph_Manager.WMGpivotTypes.Bottom);
					}
					changeSpritePositionTo(LabelNodes[i].objectToLabel, new Vector3(AxisLabelSpacing, AxisLabelSpaceOffset, 0));
				}
			}
			
			// Fontsize
			changeLabelFontSize(LabelNodes[i].objectToLabel, AxisLabelSize);

			// Font color
			changeLabelColor(LabelNodes[i].objectToLabel, AxisLabelColor);

			// Font Style
			changeLabelFontStyle(LabelNodes[i].objectToLabel, AxisLabelFontStyle);

			// Font
			if (AxisLabelFont != null) {
				changeLabelFont(LabelNodes[i].objectToLabel, AxisLabelFont);
			}

			// Update text string in the List of strings based on labeler function
			axisLabels.SetValNoCb(i, axisLabelLabeler(this, i), ref _axisLabels);

			// Actually set the text
			changeLabelText(LabelNodes[i].objectToLabel, axisLabels[i]);
		}
	}


	string defaultAxisLabelLabeler(WMG_Axis axis, int labelIndex) {
		if (axis.LabelType == WMG_Axis.labelTypes.groups) { // for groups return the string defined at the graph level for groups, respects skip settings
			return ((labelIndex - axis.AxisLabelSkipStart) % (axis.AxisLabelSkipInterval + 1) == 0) ? (labelIndex >= axis.AxisLabelSkipStart ? axis.graph.groups[labelIndex] : "") : "";
		}
		else { // all other label types
			if (axis.SetLabelsUsingMaxMin) { // if setting based on axis max and min values
				float num = axis.AxisMinValue + labelIndex * (axis.AxisMaxValue - axis.AxisMinValue) / (axis.axisLabels.Count-1);
				if (labelIndex == 0) num = axis.AxisMinValue;
				
				if (axis.graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent && ((axis.isY && axis.graph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) 
				                                                                              || (!axis.isY && axis.graph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal))) {
					num = labelIndex / (axis.axisLabels.Count-1f) * 100f;
				}
				float numberToMult = Mathf.Pow(10f, axis.numDecimalsAxisLabels);

				string returnString = ((labelIndex - axis.AxisLabelSkipStart) % (axis.AxisLabelSkipInterval + 1) == 0) ? (labelIndex >= axis.AxisLabelSkipStart ? (Mathf.Round(num*numberToMult)/numberToMult).ToString() : "") : "";
				if (axis.graph.graphType == WMG_Axis_Graph.graphTypes.bar_stacked_percent && ((axis.isY && axis.graph.orientationType == WMG_Axis_Graph.orientationTypes.vertical) 
				                                                                              || (!axis.isY && axis.graph.orientationType == WMG_Axis_Graph.orientationTypes.horizontal))) {
					return string.IsNullOrEmpty(returnString) ? "" : (returnString + "%");
				}
				else {
					return returnString;
				}
			}
			else { // otherwise just return the label that is in the string list
				return axis.axisLabels[labelIndex];
			}
		}
	}
	
	void setLabelRotations(List<WMG_Node> LabelNodes, float rotation) {
		foreach (WMG_Node node in LabelNodes) {
			node.objectToLabel.transform.localEulerAngles = new Vector3 (0, 0, rotation);
		}
	}
	
	void setFontSizeLabels(List<WMG_Node> LabelNodes, int newLabelSize) {
		foreach (WMG_Node node in LabelNodes) {
			changeLabelFontSize(node.objectToLabel, newLabelSize);
		}
	}

	public void setLabelScales(float newScale) {
		foreach (WMG_Node node in GetAxisLabelNodes()) {
			node.objectToLabel.transform.localScale = new Vector3(newScale, newScale, 1);
		}
	}

	/// <summary>
	/// Gets the axis label nodes.
	/// </summary>
	/// <returns>The axis label nodes.</returns>
	public List<WMG_Node> GetAxisLabelNodes() {
		WMG_Grid labelsGrid = AxisLabelObjs.GetComponent<WMG_Grid>();
		if (isY) {
			return labelsGrid.getColumn(0);
		}
		else {
			return labelsGrid.getRow(0);
		}
	}

	/// <summary>
	/// Gets the axis tick nodes.
	/// </summary>
	/// <returns>The axis tick nodes.</returns>
	public List<WMG_Node> GetAxisTickNodes() {
		WMG_Grid ticksGrid = AxisTicks.GetComponent<WMG_Grid>();
		if (isY) {
			return ticksGrid.getColumn(0);
		}
		else {
			return ticksGrid.getRow(0);
		}
	}

}
