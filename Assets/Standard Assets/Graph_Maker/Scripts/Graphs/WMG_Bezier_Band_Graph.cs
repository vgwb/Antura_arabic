using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class used for creating a bezier band graph, which is a collection of textures procedurally generated from bezier curves.
/// </summary>
public class WMG_Bezier_Band_Graph : WMG_Graph_Manager {

	[SerializeField] private List<float> _values;
	/// <summary>
	/// The values associated with each bezier band.
	/// </summary>
	public WMG_List<float> values = new WMG_List<float>();
	[SerializeField] private List<string> _labels;
	/// <summary>
	/// The text label for each bezier band.
	/// </summary>
	public WMG_List<string> labels = new WMG_List<string>();
	[SerializeField] private List<Color> _fillColors;
	/// <summary>
	/// The color for each bezier band.
	/// </summary>
	public WMG_List<Color> fillColors = new WMG_List<Color>();

	/// <summary>
	/// The color of the lines bordering the bezier bands.
	/// </summary>
	/// <value>The color of the band line.</value>
	public Color bandLineColor { get {return _bandLineColor;} 
		set {
			if (_bandLineColor != value) {
				_bandLineColor = value;
				colorsC.Changed();
			}
		}
	}
	/// <summary>
	/// The starting height of all the bezier bands as a percentage of total height.
	/// </summary>
	/// <value>The start height percent.</value>
	public float startHeightPercent { get {return _startHeightPercent;} 
		set {
			if (_startHeightPercent != value) {
				_startHeightPercent = value;
				cubicBezierC.Changed();
			}
		}
	}
	/// <summary>
	/// The amount of spacing between each band.
	/// </summary>
	/// <value>The band spacing.</value>
	public int bandSpacing { get {return _bandSpacing;} 
		set {
			if (_bandSpacing != value) {
				_bandSpacing = value;
				cubicBezierC.Changed();
			}
		}
	}
	/// <summary>
	/// The width of the lines bordering the bezier bands.
	/// </summary>
	/// <value>The width of the band line.</value>
	public int bandLineWidth { get {return _bandLineWidth;} 
		set {
			if (_bandLineWidth != value) {
				_bandLineWidth = value;
				cubicBezierC.Changed();
			}
		}
	}
	/// <summary>
	/// Bezier point coordinate which controls the overall shape of the graph.
	/// </summary>
	/// <value>The cubic bezier p1.</value>
	public Vector2 cubicBezierP1 { get {return _cubicBezierP1;} 
		set {
			if (_cubicBezierP1 != value) {
				_cubicBezierP1 = value;
				cubicBezierC.Changed();
			}
		}
	}
	/// <summary>
	/// Bezier point coordinate which controls the overall shape of the graph.
	/// </summary>
	/// <value>The cubic bezier p2.</value>
	public Vector2 cubicBezierP2 { get {return _cubicBezierP2;} 
		set {
			if (_cubicBezierP2 != value) {
				_cubicBezierP2 = value;
				cubicBezierC.Changed();
			}
		}
	}
	/// <summary>
	/// Determines number of decimal used when displaying band text labels.
	/// </summary>
	/// <value>The number decimals.</value>
	public int numDecimals { get {return _numDecimals;} 
		set {
			if (_numDecimals != value) {
				_numDecimals = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Font size of band text labels.
	/// </summary>
	/// <value>The size of the font.</value>
	public int fontSize { get {return _fontSize;} 
		set {
			if (_fontSize != value) {
				_fontSize = value;
				fontC.Changed();
			}
		}
	}
	/// <summary>
	/// Resolution of the procedurally generated band textures.
	/// </summary>
	public int textureResolution = 2048;
	/// <summary>
	/// The parent of the bezier bands.
	/// </summary>
	public GameObject bandsParent;
	/// <summary>
	/// The prefab used to create each bezier band.
	/// </summary>
	public Object bandPrefab;

	[SerializeField] private Color _bandLineColor;
	[SerializeField] private float _startHeightPercent;
	[SerializeField] private int _bandSpacing;
	[SerializeField] private int _bandLineWidth;
	[SerializeField] private Vector2 _cubicBezierP1;
	[SerializeField] private Vector2 _cubicBezierP2;
	[SerializeField] private int _numDecimals;
	[SerializeField] private int _fontSize;

	// public getter
	/// <summary>
	/// Get the bezier bands.
	/// </summary>
	/// <value>The bands.</value>
	public List<WMG_Bezier_Band> bands { get; private set; }
	/// <summary>
	/// Get the total value of all the bands.
	/// </summary>
	/// <value>The total value.</value>
	public float TotalVal { get; private set; }

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	private WMG_Change_Obj allC = new WMG_Change_Obj();
	private WMG_Change_Obj cubicBezierC = new WMG_Change_Obj();
	private WMG_Change_Obj labelsC = new WMG_Change_Obj();
	private WMG_Change_Obj colorsC = new WMG_Change_Obj();
	private WMG_Change_Obj fontC = new WMG_Change_Obj();

	private bool hasInit;

	void Start () {
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
		
		changeObjs.Add(allC);
		changeObjs.Add(cubicBezierC);
		changeObjs.Add(labelsC);
		changeObjs.Add(colorsC);
		changeObjs.Add(fontC);
		
		bands = new List<WMG_Bezier_Band>();
		
		values.SetList (_values);
		values.Changed += valuesChanged;
		
		fillColors.SetList (_fillColors);
		fillColors.Changed += fillColorsChanged;
		
		labels.SetList (_labels);
		labels.Changed += labelsChanged;
		
		allC.OnChange += AllChanged;
		cubicBezierC.OnChange += CubicBezierChanged;
		labelsC.OnChange += UpdateLabels;
		colorsC.OnChange += UpdateColors;
		fontC.OnChange += UpdateFontSize;
		
		allC.Changed();
	}

	void Update () {
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

	public void valuesChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref values, ref _values, oneValChanged, index);
		allC.Changed ();
	}

	public void fillColorsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref fillColors, ref _fillColors, oneValChanged, index);
		colorsC.Changed();
	}

	public void labelsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref labels, ref _labels, oneValChanged, index);
		labelsC.Changed();
	}

	void AllChanged() {
		CreateOrDeleteBasedOnValues ();
		UpdateColors();
		UpdateBands ();
		UpdateLabelPositions();
		UpdateLabels();
	}

	void CubicBezierChanged() {
		UpdateBands();
	}

	void CreateOrDeleteBasedOnValues() {
		// Create bands based on values data
		for (int i = 0; i < values.Count; i++) {
			if (fillColors.Count <= i) fillColors.AddNoCb(Color.white, ref _fillColors);
			if (labels.Count <= i) labels.AddNoCb("", ref _labels);
			if (bands.Count <= i) {
				GameObject obj = GameObject.Instantiate(bandPrefab) as GameObject;
				changeSpriteParent(obj, bandsParent);
				WMG_Bezier_Band band = obj.GetComponent<WMG_Bezier_Band>();
				if (band == null) {
					Debug.Log(obj.name);
					Debug.Log("asdf");
				}
				band.Init(this);
				bands.Add(band);
			}
		}
		for (int i = bands.Count - 1; i >= 0; i--) {
			if (bands[i] != null && i >= values.Count) {
				Destroy(bands[i].gameObject);
				bands.RemoveAt(i);
			}
		}
	}

	void UpdateColors() {
		UpdateBandFillColors();
		UpdateBandLineColors();
	}

	void UpdateBandFillColors() {
		for (int i = 0; i < values.Count; i++) {
			bands[i].setFillColor(fillColors[i]);
		}
	}

	void UpdateBandLineColors() {
		for (int i = 0; i < values.Count; i++) {
			bands[i].setLineColor(bandLineColor);
		}
	}

	void UpdateBands() {
		TotalVal = 0;
		for (int i = 0; i < values.Count; i++) {
			TotalVal += values[i];
		}
		float currentVal = 0;
		float prevVal = 0;
		for (int i = 0; i < values.Count; i++) {
			currentVal += values[i];
			bands[i].setCumulativePercents(currentVal, prevVal);
			prevVal += values[i];
			bands[i].UpdateBand();
		}
	}

	void UpdateLabelPositions() {
		for (int i = 0; i < values.Count; i++) {
			setAnchor(bands[i].labelParent, 
			          new Vector2(1, (1-bands[i].cumulativePercent) + (bands[i].cumulativePercent - bands[i].prevCumulativePercent)/2f), 
			          new Vector2(0.5f, 0.5f), Vector2.zero);
		}
	}

	void UpdateLabels() {
		for (int i = 0; i < values.Count; i++) {
			changeLabelText(bands[i].percentLabel, WMG_Util.FormatValueLabel("", WMG_Enums.labelTypes.Percents_Only, 0, (bands[i].cumulativePercent - bands[i].prevCumulativePercent), numDecimals));
			changeLabelText(bands[i].label, labels[i]);
		}
	}

	void UpdateFontSize() {
		for (int i = 0; i < values.Count; i++) {
			changeLabelFontSize(bands[i].percentLabel, fontSize);
			changeLabelFontSize(bands[i].label, fontSize);
		}
	}
	


}
