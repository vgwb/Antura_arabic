using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class used for creating radar graphs.
/// </summary>
public class WMG_Radar_Graph : WMG_Axis_Graph {

	[SerializeField] private List<Color> _dataSeriesColors;
	/// <summary>
	/// Colors of the series.
	/// </summary>
	public WMG_List<Color> dataSeriesColors = new WMG_List<Color>();
	[SerializeField] private List<string> _labelStrings;
	/// <summary>
	/// Labels for each point of the radar grid.
	/// </summary>
	public WMG_List<string> labelStrings = new WMG_List<string>();
	/// <summary>
	/// Sets random data for demonstration purposes, should be set to false and set your own data.
	/// </summary>
	public bool randomData;
	/// <summary>
	/// Controls how many points or edges there are for the radar graph, so setting 5 here will make all the grids be pentagons.
	/// </summary>
	/// <value>The number points.</value>
	public int numPoints { get {return _numPoints;} 
		set {
			if (_numPoints != value) {
				_numPoints = value;
				radarGraphC.Changed();
			}
		}
	}
	/// <summary>
	/// This can be used for moving around the radar graph without moving the graph as a whole.
	/// </summary>
	/// <value>The offset.</value>
	public Vector2 offset { get {return _offset;} 
		set {
			if (_offset != value) {
				_offset = value;
				radarGraphC.Changed();
			}
		}
	}
	/// <summary>
	/// This allows rotating the content of the radar graph.
	/// </summary>
	/// <value>The degree offset.</value>
	public float degreeOffset { get {return _degreeOffset;} 
		set {
			if (_degreeOffset != value) {
				_degreeOffset = value;
				radarGraphC.Changed();
			}
		}
	}
	/// <summary>
	/// Defines the minimum value of the radar graph.
	/// </summary>
	/// <value>The radar minimum value.</value>
	public float radarMinVal { get {return _radarMinVal;} 
		set {
			if (_radarMinVal != value) {
				_radarMinVal = value;
				radarGraphC.Changed();
			}
		}
	}
	/// <summary>
	/// Defines the maximum value of the radar graph, which determines what value represents the outer radius of the radar grids.
	/// </summary>
	/// <value>The radar max value.</value>
	public float radarMaxVal { get {return _radarMaxVal;} 
		set {
			if (_radarMaxVal != value) {
				_radarMaxVal = value;
				radarGraphC.Changed();
			}
		}
	}
	/// <summary>
	/// The number of grids that appear in the background for this radar graph.
	/// </summary>
	/// <value>The number grids.</value>
	public int numGrids { get {return _numGrids;} 
		set {
			if (_numGrids != value) {
				_numGrids = value;
				gridsC.Changed();
			}
		}
	}
	/// <summary>
	/// The line width of the radar grids.
	/// </summary>
	/// <value>The width of the grid line.</value>
	public float gridLineWidth { get {return _gridLineWidth;} 
		set {
			if (_gridLineWidth != value) {
				_gridLineWidth = value;
				gridsC.Changed();
			}
		}
	}
	/// <summary>
	/// The color of the radar grids.
	/// </summary>
	/// <value>The color of the grid.</value>
	public Color gridColor { get {return _gridColor;} 
		set {
			if (_gridColor != value) {
				_gridColor = value;
				gridsC.Changed();
			}
		}
	}
	/// <summary>
	/// The number of data series.
	/// </summary>
	/// <value>The number data series.</value>
	public int numDataSeries { get {return _numDataSeries;} 
		set {
			if (_numDataSeries != value) {
				_numDataSeries = value;
				dataSeriesC.Changed();
			}
		}
	}
	/// <summary>
	/// The line width of the data series.
	/// </summary>
	/// <value>The width of the data series line.</value>
	public float dataSeriesLineWidth { get {return _dataSeriesLineWidth;} 
		set {
			if (_dataSeriesLineWidth != value) {
				_dataSeriesLineWidth = value;
				dataSeriesC.Changed();
			}
		}
	}
	/// <summary>
	/// The color of the text labels.
	/// </summary>
	/// <value>The color of the labels.</value>
	public Color labelsColor { get {return _labelsColor;} 
		set {
			if (_labelsColor != value) {
				_labelsColor = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Positional offset of the labels.
	/// </summary>
	/// <value>The labels offset.</value>
	public float labelsOffset { get {return _labelsOffset;} 
		set {
			if (_labelsOffset != value) {
				_labelsOffset = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Font size of the labels.
	/// </summary>
	/// <value>The size of the font.</value>
	public int fontSize { get {return _fontSize;} 
		set {
			if (_fontSize != value) {
				_fontSize = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether to hide the labels.
	/// </summary>
	/// <value><c>true</c> if hide labels; otherwise, <c>false</c>.</value>
	public bool hideLabels { get {return _hideLabels;} 
		set {
			if (_hideLabels != value) {
				_hideLabels = value;
				labelsC.Changed();
			}
		}
	}

	// Private backing variables
	[SerializeField] private int _numPoints;
	[SerializeField] private Vector2 _offset;
	[SerializeField] private float _degreeOffset;
	[SerializeField] private float _radarMinVal;
	[SerializeField] private float _radarMaxVal;
	[SerializeField] private int _numGrids;
	[SerializeField] private float _gridLineWidth;
	[SerializeField] private Color _gridColor;
	[SerializeField] private int _numDataSeries;
	[SerializeField] private float _dataSeriesLineWidth;
	[SerializeField] private Color _labelsColor;
	[SerializeField] private float _labelsOffset;
	[SerializeField] private int _fontSize;
	[SerializeField] private bool _hideLabels;

	/// <summary>
	/// Reference to the radar grids.
	/// </summary>
	public List<WMG_Series> grids;
	/// <summary>
	/// Reference to the radar data series.
	/// </summary>
	public List<WMG_Series> dataSeries;
	/// <summary>
	/// The radar labels.
	/// </summary>
	public WMG_Series radarLabels;

	private bool createdLabels;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	private WMG_Change_Obj radarGraphC = new WMG_Change_Obj();
	private WMG_Change_Obj gridsC = new WMG_Change_Obj();
	private WMG_Change_Obj labelsC = new WMG_Change_Obj();
	private WMG_Change_Obj dataSeriesC = new WMG_Change_Obj();

	private bool hasInit2; // same field serialized in base class error ?
	
	void Start () {
		Init ();
		PauseCallbacks();
		radarGraphC.Changed();
	}

	/// <summary>
	/// Initializes the graph, and should always be done before anything else, called automatically in Start(), but it
	/// never hurts to call this manually after instantiating a graph prefab.
	/// </summary>
	public new void Init() {
		if (hasInit2) return;
		hasInit2 = true;

		changeObjs.Add(radarGraphC);
		changeObjs.Add(gridsC);
		changeObjs.Add(labelsC);
		changeObjs.Add(dataSeriesC);

		dataSeriesColors.SetList (_dataSeriesColors);
		dataSeriesColors.Changed += dataSeriesColorsChanged;

		labelStrings.SetList (_labelStrings);
		labelStrings.Changed += labelStringsChanged;

		radarGraphC.OnChange += GraphChanged;
		gridsC.OnChange += GridsChanged;
		labelsC.OnChange += LabelsChanged;
		dataSeriesC.OnChange += DataSeriesChanged;

		PauseCallbacks();
	}

	void Update () {
		Refresh();
	}

	/// <summary>
	/// Refreshes the graph, and happens automatically in Update(), but sometimes it is useful or necessary to call this
	/// manually, note that refresh updates only the parts of the graph affected by properties that have changed since a last refresh.
	/// </summary>
	public new void Refresh() {
		ResumeCallbacks();
		PauseCallbacks();
	}

	void PauseCallbacks() {
		for (int i = 0; i < changeObjs.Count; i++) {
			changeObjs[i].changesPaused = true;
			changeObjs[i].changePaused = false;
		}
	}
	
	void ResumeCallbacks() {
		for (int i = 0; i < changeObjs.Count; i++) {
			changeObjs[i].changesPaused = false;
			if (changeObjs[i].changePaused) changeObjs[i].Changed();
		}
	}

	public void dataSeriesColorsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref dataSeriesColors, ref _dataSeriesColors, oneValChanged, index);
		dataSeriesC.Changed();
	}

	public void labelStringsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref labelStrings, ref _labelStrings, oneValChanged, index);
		labelsC.Changed();
	}

	public void GridsChanged() {
		updateGrids();
	}

	public void DataSeriesChanged() {
		updateDataSeries();
	}

	public void LabelsChanged() {
		updateLabels();
	}

	public new void GraphChanged() {
		updateGrids();
		updateDataSeries();
		updateLabels();
	}

	void updateLabels() {
		if (!createdLabels) {
			WMG_Series labels = addSeriesAt(numDataSeries+numGrids);
			labels.hideLines = true;
			createdLabels = true;
			labels.pointPrefab = 3;
			radarLabels = labels;
		}

		for (int i = 0; i < numPoints; i++) {
			if (labelStrings.Count <= i) {
				labelStrings.AddNoCb("", ref _labelStrings);
			}
		}
		for (int i = labelStrings.Count - 1; i >= 0; i--) {
			if (labelStrings[i] != null && i >= numPoints) {
				labelStrings.RemoveAtNoCb(i, ref _labelStrings);
			}
		}

		radarLabels.hidePoints = hideLabels;
		radarLabels.pointValues.SetList(WMG_Util.GenCircular(numPoints, offset.x, offset.y, labelsOffset + (radarMaxVal - radarMinVal), degreeOffset));
		List<GameObject> labelGOs = radarLabels.getPoints();
		for (int i = 0; i < labelGOs.Count; i++) {
			if (i >= numPoints) break;
			changeLabelFontSize(labelGOs[i], fontSize);
			changeLabelText(labelGOs[i], labelStrings[i]);
		}
		radarLabels.pointColor = labelsColor;
	}

	void updateDataSeries() {
		for (int i = 0; i < numDataSeries; i++) {
			if (dataSeries.Count <= i) {
				WMG_Series aSeries = addSeriesAt(numGrids+i);
				aSeries.connectFirstToLast = true;
				aSeries.hidePoints = true;
				dataSeries.Add(aSeries);
			}
			if (dataSeriesColors.Count <= i) {
				dataSeriesColors.AddNoCb(new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1), ref _dataSeriesColors);
			}
		}
		for (int i = dataSeries.Count - 1; i >= 0; i--) {
			if (dataSeries[i] != null && i >= numDataSeries) {
				deleteSeriesAt(numGrids+i);
				dataSeries.RemoveAt(i);
			}
		}
		for (int i = dataSeriesColors.Count - 1; i >= 0; i--) {
			if (i >= numDataSeries) {
				dataSeriesColors.RemoveAtNoCb(i, ref _dataSeriesColors);
			}
		}
		for (int i = 0; i < numDataSeries; i++) {
			WMG_Series aSeries = lineSeries[i + numGrids].GetComponent<WMG_Series>();
			if (randomData) {
				aSeries.pointValues.SetList(WMG_Util.GenRadar(WMG_Util.GenRandomList(numPoints, radarMinVal, radarMaxVal), offset.x, offset.y, degreeOffset));
			}
			aSeries.lineScale = dataSeriesLineWidth;
			aSeries.linePadding = dataSeriesLineWidth;
			aSeries.lineColor = dataSeriesColors[i];
		}
	}

	void updateGrids() {
		for (int i = 0; i < numGrids; i++) {
			if (grids.Count <= i) {
				WMG_Series aGrid = addSeriesAt(i);
				aGrid.connectFirstToLast = true;
				aGrid.hidePoints = true;
				grids.Add(aGrid);
			}
		}
		for (int i = grids.Count - 1; i >= 0; i--) {
			if (grids[i] != null && i >= numGrids) {
				deleteSeriesAt(i);
				grids.RemoveAt(i);
			}
		}
		for (int i = 0; i < numGrids; i++) {
			WMG_Series aGrid = lineSeries[i].GetComponent<WMG_Series>();
			aGrid.pointValues.SetList(WMG_Util.GenCircular(numPoints, offset.x, offset.y, (i+1f) / numGrids * (radarMaxVal - radarMinVal), degreeOffset));
			aGrid.lineScale = gridLineWidth;
			aGrid.linePadding = gridLineWidth;
			aGrid.lineColor = gridColor;
		}
	}

}
