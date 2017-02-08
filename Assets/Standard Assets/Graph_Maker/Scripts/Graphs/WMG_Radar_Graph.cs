using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WMG_Radar_Graph : WMG_Axis_Graph {

	[SerializeField] private List<Color> _dataSeriesColors;
	public WMG_List<Color> dataSeriesColors = new WMG_List<Color>();
	[SerializeField] private List<string> _labelStrings;
	public WMG_List<string> labelStrings = new WMG_List<string>();

	public bool randomData; // sets random data for demonstration purposes, set this to false to use your own data

	public int numPoints { get {return _numPoints;} 
		set {
			if (_numPoints != value) {
				_numPoints = value;
				radarGraphC.Changed();
			}
		}
	}
	public Vector2 offset { get {return _offset;} 
		set {
			if (_offset != value) {
				_offset = value;
				radarGraphC.Changed();
			}
		}
	}
	public float degreeOffset { get {return _degreeOffset;} 
		set {
			if (_degreeOffset != value) {
				_degreeOffset = value;
				radarGraphC.Changed();
			}
		}
	}
	public float radarMinVal { get {return _radarMinVal;} 
		set {
			if (_radarMinVal != value) {
				_radarMinVal = value;
				radarGraphC.Changed();
			}
		}
	}
	public float radarMaxVal { get {return _radarMaxVal;} 
		set {
			if (_radarMaxVal != value) {
				_radarMaxVal = value;
				radarGraphC.Changed();
			}
		}
	}
	public int numGrids { get {return _numGrids;} 
		set {
			if (_numGrids != value) {
				_numGrids = value;
				gridsC.Changed();
			}
		}
	}
	public float gridLineWidth { get {return _gridLineWidth;} 
		set {
			if (_gridLineWidth != value) {
				_gridLineWidth = value;
				gridsC.Changed();
			}
		}
	}
	public Color gridColor { get {return _gridColor;} 
		set {
			if (_gridColor != value) {
				_gridColor = value;
				gridsC.Changed();
			}
		}
	}
	public int numDataSeries { get {return _numDataSeries;} 
		set {
			if (_numDataSeries != value) {
				_numDataSeries = value;
				dataSeriesC.Changed();
			}
		}
	}
	public float dataSeriesLineWidth { get {return _dataSeriesLineWidth;} 
		set {
			if (_dataSeriesLineWidth != value) {
				_dataSeriesLineWidth = value;
				dataSeriesC.Changed();
			}
		}
	}
	public Color labelsColor { get {return _labelsColor;} 
		set {
			if (_labelsColor != value) {
				_labelsColor = value;
				labelsC.Changed();
			}
		}
	}
	public float labelsOffset { get {return _labelsOffset;} 
		set {
			if (_labelsOffset != value) {
				_labelsOffset = value;
				labelsC.Changed();
			}
		}
	}
	public int fontSize { get {return _fontSize;} 
		set {
			if (_fontSize != value) {
				_fontSize = value;
				labelsC.Changed();
			}
		}
	}
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

	public List<WMG_Series> grids;
	public List<WMG_Series> dataSeries;
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
		radarLabels.pointValues.SetList(GenCircular2(numPoints, offset.x, offset.y, labelsOffset + (radarMaxVal - radarMinVal), degreeOffset));
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
				aSeries.pointValues.SetList(GenRadar(GenRandomList(numPoints, radarMinVal, radarMaxVal), offset.x, offset.y, degreeOffset));
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
			aGrid.pointValues.SetList(GenCircular2(numPoints, offset.x, offset.y, (i+1f) / numGrids * (radarMaxVal - radarMinVal), degreeOffset));
			aGrid.lineScale = gridLineWidth;
			aGrid.linePadding = gridLineWidth;
			aGrid.lineColor = gridColor;
		}
	}

}
