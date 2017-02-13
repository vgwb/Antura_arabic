using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Class for creating ring graphs or pie graphs.
/// </summary>
public class WMG_Ring_Graph : WMG_Graph_Manager {

	[SerializeField] public List<Color> _bandColors;
	/// <summary>
	/// This can be used to override colors of individual bands if it is needed that the color be different than #bandColor.
	/// </summary>
	public WMG_List<Color> bandColors = new WMG_List<Color>();
	[SerializeField] private List<float> _values;
	/// <summary>
	/// The values representing each ring.
	/// </summary>
	public WMG_List<float> values = new WMG_List<float>();
	[SerializeField] private List<string> _labels;
	/// <summary>
	/// The text labels representing each ring.
	/// </summary>
	public WMG_List<string> labels = new WMG_List<string>();
	[SerializeField] private List<string> _ringIDs;
	/// <summary>
	/// This can be used to assign a string ID to each ring, and then use #getRing to retrieve a ring with the ID.
	/// </summary>
	public WMG_List<string> ringIDs = new WMG_List<string>();
	[SerializeField] private List<bool> _hideRings;
	/// <summary>
	/// Whether or not to hide rings.
	/// </summary>
	public WMG_List<bool> hideRings = new WMG_List<bool>();

	// public properties
	/// <summary>
	/// When band mode is enabled, each ring has a corresponding band, except for the outer most ring. When disabled, only rings will appear. 
	/// </summary>
	/// <value><c>true</c> if band mode; otherwise, <c>false</c>.</value>
	public bool bandMode { get {return _pieMode ? true : _bandMode;} 
		set {
			if (_bandMode != value) {
				_bandMode = value;
				textureC.Changed();
				hideRingsC.Changed ();
				if (_useComputeShader) {
					degreesC.Changed();
				}
			}
		}
	}
	/// <summary>
	/// When enabled, the bands of the ring graph are positioned like a doughnut / pie graph where each band represents a percentage of all the bands. 
	/// In this mode #bandMode, #degrees, #minValue, #maxValue, #ringWidth, and #bandPadding no longer matter.
	/// </summary>
	/// <value><c>true</c> if pie mode; otherwise, <c>false</c>.</value>
	public bool pieMode { get {return _pieMode;} 
		set {
			if (_pieMode != value) {
				_pieMode = value;
				textureC.Changed();
				hideRingsC.Changed ();
				degreesC.Changed();
				if (!_pieMode && values.Count > 0) {
					beforeValCount.Add (0);
					afterValCount.Add (values.Count);
				}
			}
		}
	}
	/// <summary>
	/// When #pieMode = true, determines how much space is between each band.
	/// </summary>
	/// <value>The pie mode padding degrees.</value>
	public float pieModePaddingDegrees { get {return _pieModePaddingDegrees;} 
		set {
			if (_pieModePaddingDegrees != value) {
				_pieModePaddingDegrees = value;
				degreesC.Changed();
			}
		}
	}
	/// <summary>
	/// When #pieMode = true, determines how much the content of the entire graph is rotated.
	/// </summary>
	/// <value>The pie mode degree offset.</value>
	public float pieModeDegreeOffset { get {return _pieModeDegreeOffset;} 
		set {
			if (_pieModeDegreeOffset != value) {
				_pieModeDegreeOffset = value;
				degreesC.Changed();
			}
		}
	}
	/// <summary>
	/// This is the radius of the innermost ring relative to the outermost ring. The radius of the outermost ring is determined by the width / height of the graph rect transform. 
	/// </summary>
	/// <value>The inner radius percentage.</value>
	public float innerRadiusPercentage { get {return _innerRadiusPercentage;} 
		set {
			if (_innerRadiusPercentage != value) {
				_innerRadiusPercentage = value;
				textureC.Changed();
				if (_useComputeShader) {
					degreesC.Changed();
				}
			}
		}
	}
	/// <summary>
	/// This is the number of degrees cut out from all the rings and bands. For example if 90 is specified, then 3/4 of a circle will appear for all rings and bands.
	/// </summary>
	/// <value>The degrees.</value>
	public float degrees { get {return _pieMode ? 0 : _degrees;} 
		set {
			if (_degrees != value) {
				_degrees = value;
				degreesC.Changed();
			}
		}
	}
	/// <summary>
	/// This is the value that represents the minimum of the ring graph (the left-most side).
	/// </summary>
	/// <value>The minimum value.</value>
	public float minValue { get {return _minValue;} 
		set {
			if (_minValue != value) {
				_minValue = value;
				degreesC.Changed();
			}
		}
	}
	/// <summary>
	/// This is the value that represents the maximum of the ring graph (the right-most side).
	/// </summary>
	/// <value>The max value.</value>
	public float maxValue { get {return _maxValue;} 
		set {
			if (_maxValue != value) {
				_maxValue = value;
				degreesC.Changed();
			}
		}
	}
	/// <summary>
	/// This is the base color given to all bands.
	/// </summary>
	/// <value>The color of the band.</value>
	public Color bandColor { get {return _bandColor;} 
		set {
			if (_bandColor != value) {
				_bandColor = value;
				bandColorC.Changed();
			}
		}
	}
	/// <summary>
	/// When enabled, this automatically adjusts the alpha of #bandColors, such that the bands gradually fade out, the closer the band is to the center of the graph.
	/// </summary>
	/// <value><c>true</c> if auto update band alpha; otherwise, <c>false</c>.</value>
	public bool autoUpdateBandAlpha { get {return _autoUpdateBandAlpha;} 
		set {
			if (_autoUpdateBandAlpha != value) {
				_autoUpdateBandAlpha = value;
				bandColorC.Changed();
			}
		}
	}
	/// <summary>
	/// Reverses the ordering of the band alpha fading for #autoUpdateBandAlpha.
	/// </summary>
	/// <value><c>true</c> if auto update band alpha reverse; otherwise, <c>false</c>.</value>
	public bool autoUpdateBandAlphaReverse { get {return _autoUpdateBandAlphaReverse;} 
		set {
			if (_autoUpdateBandAlphaReverse != value) {
				_autoUpdateBandAlphaReverse = value;
				bandColorC.Changed();
			}
		}
	}
	/// <summary>
	/// This is the color given to all the rings
	/// </summary>
	/// <value>The color of the ring.</value>
	public Color ringColor { get {return _ringColor;} 
		set {
			if (_ringColor != value) {
				_ringColor = value;
				ringColorC.Changed();
			}
		}
	}
	/// <summary>
	/// This is the width of all of the rings.
	/// </summary>
	/// <value>The width of the ring.</value>
	public float ringWidth { get {return _pieMode ? 0 : _ringWidth;} 
		set {
			if (_ringWidth != value) {
				_ringWidth = value;
				textureC.Changed();
				if (_useComputeShader) {
					degreesC.Changed();
				}
			}
		}
	}
	/// <summary>
	/// Determines the size of the label points (circles from which label lines are drawn to indicate which ring the to which the label corresponds).
	/// </summary>
	/// <value>The ring point width factor.</value>
	public float ringPointWidthFactor { get {return _ringPointWidthFactor;} 
		set {
			if (_ringPointWidthFactor != value) {
				_ringPointWidthFactor = value;
				textureC.Changed();
				if (_useComputeShader) {
					degreesC.Changed();
				}
			}
		}
	}
	/// <summary>
	/// This is the padding between rings and bands.
	/// </summary>
	/// <value>The band padding.</value>
	public float bandPadding { get {return _pieMode ? 0 : _bandPadding;} 
		set {
			if (_bandPadding != value) {
				_bandPadding = value;
				textureC.Changed();
				if (_useComputeShader) {
					degreesC.Changed();
				}
			}
		}
	}
	/// <summary>
	/// Whether to hide the zero label line.
	/// </summary>
	/// <value><c>true</c> if hide zero label line; otherwise, <c>false</c>.</value>
	public bool hideZeroLabelLine { get {return _hideZeroLabelLine;} 
		set {
			if (_hideZeroLabelLine != value) {
				_hideZeroLabelLine = value;
				degreesC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether label lines originate from the center of a band or from the band outer edge.
	/// </summary>
	/// <value><c>true</c> if label start centered on band; otherwise, <c>false</c>.</value>
	public bool labelStartCenteredOnBand { get {return _labelStartCenteredOnBand;} 
		set {
			if (_labelStartCenteredOnBand != value) {
				_labelStartCenteredOnBand = value;
				radiusC.Changed();
				degreesC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the size of the label point.
	/// </summary>
	/// <value>The size of the label point.</value>
	public Vector2 labelPointSize { get {return _labelPointSize;} 
		set {
			if (_labelPointSize != value) {
				_labelPointSize = value;
				radiusC.Changed();
			}
		}
	}
	/// <summary>
	/// The amount of space the label lines extend beyond the outer most ring.
	/// </summary>
	/// <value>The label line padding.</value>
	public float labelLinePadding { get {return _labelLinePadding;} 
		set {
			if (_labelLinePadding != value) {
				_labelLinePadding = value;
				radiusC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the label text offset.
	/// </summary>
	/// <value>The label text offset.</value>
	public float labelTextOffset { get {return _labelTextOffset;} 
		set {
			if (_labelTextOffset != value) {
				_labelTextOffset = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the color of the label line.
	/// </summary>
	/// <value>The color of the label line.</value>
	public Color labelLineColor { get {return _labelLineColor;} 
		set {
			if (_labelLineColor != value) {
				_labelLineColor = value;
				labelLineColorC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the color of the label point.
	/// </summary>
	/// <value>The color of the label point.</value>
	public Color labelPointColor { get {return _labelPointColor;} 
		set {
			if (_labelPointColor != value) {
				_labelPointColor = value;
				labelLineColorC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the size of the labels font.
	/// </summary>
	/// <value>The size of the labels font.</value>
	public int labelsFontSize { get {return _labelsFontSize;} 
		set {
			if (_labelsFontSize != value) {
				_labelsFontSize = value;
				labelsC.Changed();
				labelLineC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the color of the labels.
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
	/// Gets or sets the labels font style.
	/// </summary>
	/// <value>The labels font style.</value>
	public FontStyle labelsFontStyle { get {return _labelsFontStyle;} 
		set {
			if (_labelsFontStyle != value) {
				_labelsFontStyle = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the labels font.
	/// </summary>
	/// <value>The labels font.</value>
	public Font labelsFont { get {return _labelsFont;} 
		set {
			if (_labelsFont != value) {
				_labelsFont = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether to show data labels.
	/// </summary>
	/// <value><c>true</c> if show data labels; otherwise, <c>false</c>.</value>
	public bool showDataLabels { get {return _showDataLabels;} 
		set {
			if (_showDataLabels != value) {
				_showDataLabels = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the size of the data labels font.
	/// </summary>
	/// <value>The size of the data labels font.</value>
	public int dataLabelsFontSize { get {return _dataLabelsFontSize;} 
		set {
			if (_dataLabelsFontSize != value) {
				_dataLabelsFontSize = value;
				labelsC.Changed();
				labelLineC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the color of the data labels.
	/// </summary>
	/// <value>The color of the data labels.</value>
	public Color dataLabelsColor { get {return _dataLabelsColor;} 
		set {
			if (_dataLabelsColor != value) {
				_dataLabelsColor = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the data labels font style.
	/// </summary>
	/// <value>The data labels font style.</value>
	public FontStyle dataLabelsFontStyle { get {return _dataLabelsFontStyle;} 
		set {
			if (_dataLabelsFontStyle != value) {
				_dataLabelsFontStyle = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the data labels font.
	/// </summary>
	/// <value>The data labels font.</value>
	public Font dataLabelsFont { get {return _dataLabelsFont;} 
		set {
			if (_dataLabelsFont != value) {
				_dataLabelsFont = value;
				labelsC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the left right padding of the graph.
	/// </summary>
	/// <value>The left right padding.</value>
	public Vector2 leftRightPadding { get {return _leftRightPadding;} 
		set {
			if (_leftRightPadding != value) {
				_leftRightPadding = value;
				radiusC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the top bot padding of the graph.
	/// </summary>
	/// <value>The top bot padding.</value>
	public Vector2 topBotPadding { get {return _topBotPadding;} 
		set {
			if (_topBotPadding != value) {
				_topBotPadding = value;
				radiusC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether to apply anti-aliasing during texture generation.
	/// </summary>
	/// <value><c>true</c> if anti aliasing; otherwise, <c>false</c>.</value>
	public bool antiAliasing { get {return _antiAliasing;} 
		set {
			if (_antiAliasing != value) {
				_antiAliasing = value;
				textureC.Changed();
				if (_useComputeShader) {
					degreesC.Changed();
				}
			}
		}
	}
	/// <summary>
	/// The strength of the anti-aliasing applied during texture generation.
	/// </summary>
	/// <value>The anti aliasing strength.</value>
	public float antiAliasingStrength { get {return _antiAliasingStrength;} 
		set {
			if (_antiAliasingStrength != value) {
				_antiAliasingStrength = value;
				textureC.Changed();
				if (_useComputeShader) {
					degreesC.Changed();
				}
			}
		}
	}
	/// <summary>
	/// Whether or not textures for the rings / bands are generated via Compute Shader.
	/// Compute shader should usually always be used, unless the platform to which you are publishing does not support Compute shaders (refer to Unity documentation on Compute Shader).
	/// </summary>
	/// <value><c>true</c> if use compute shader; otherwise, <c>false</c>.</value>
	public bool useComputeShader { get {return _useComputeShader;} 
		set {
			if (_useComputeShader != value) {
				_useComputeShader = value;
				computeShaderC.Changed();
				bandColorC.Changed();
				ringColorC.Changed();
				textureC.Changed();
				degreesC.Changed();
			}
		}
	}

	// Public variables without change tracking
	public bool animateData;
	public float animDuration;
	public Ease animEaseType;
	public Object ringPrefab;
	public GameObject extraRing;
	public GameObject background;
	public GameObject zeroLine;
	public GameObject zeroLineText;
	public GameObject ringsParent;
	public GameObject ringLabelsParent;
	public GameObject contentParent;
	public WMG_Data_Source valuesDataSource;
	public WMG_Data_Source labelsDataSource;
	public WMG_Data_Source ringIDsDataSource;
	public Sprite labelLineSprite;
	public Sprite botLeftCorners;
	public Sprite botRightCorners;
	public ComputeShader computeShader;
	/// <summary>
	/// The resolution of the generated ring / band textures.
	/// </summary>
	public int textureResolution = 512;

	// Private backing variables
	[SerializeField] private bool _bandMode;
	[SerializeField] private bool _pieMode;
	[SerializeField] private float _pieModePaddingDegrees;
	[SerializeField] private float _pieModeDegreeOffset;
	[SerializeField] private float _innerRadiusPercentage;
	[SerializeField] private float _degrees;
	[SerializeField] private float _minValue;
	[SerializeField] private float _maxValue;
	[SerializeField] private Color _bandColor;
	[SerializeField] public bool _autoUpdateBandAlpha;
	[SerializeField] private bool _autoUpdateBandAlphaReverse;
	[SerializeField] private Color _ringColor;
	[SerializeField] private float _ringWidth;
	[SerializeField] private float _ringPointWidthFactor;
	[SerializeField] private float _bandPadding;
	[SerializeField] private bool _hideZeroLabelLine;
	[SerializeField] private bool _labelStartCenteredOnBand;
	[SerializeField] private Vector2 _labelPointSize = new Vector2(12, 12);
	[SerializeField] private float _labelLinePadding;
	[SerializeField] private float _labelTextOffset = 10;
	[SerializeField] private Color _labelLineColor = Color.white;
	[SerializeField] private Color _labelPointColor = Color.white;
	[SerializeField] private int _labelsFontSize = 12;
	[SerializeField] private Color _labelsColor = Color.white;
	[SerializeField] private FontStyle _labelsFontStyle = FontStyle.Normal;
	[SerializeField] private Font _labelsFont; 
	[SerializeField] private bool _showDataLabels;
	[SerializeField] private int _dataLabelsFontSize = 12;
	[SerializeField] private Color _dataLabelsColor = Color.white;
	[SerializeField] private FontStyle _dataLabelsFontStyle = FontStyle.Normal;
	[SerializeField] private Font _dataLabelsFont; 
	[SerializeField] private Vector2 _leftRightPadding;
	[SerializeField] private Vector2 _topBotPadding;
	[SerializeField] private bool _antiAliasing;
	[SerializeField] private float _antiAliasingStrength;
	[SerializeField] private bool _useComputeShader;

	// various useful getter properties
	public float outerRadius { 
		get {
			return Mathf.Min ((getSpriteWidth (this.gameObject) - leftRightPadding.x - leftRightPadding.y) / 2,
			                  (getSpriteHeight (this.gameObject) - topBotPadding.x - topBotPadding.y) / 2); 
		}
	}
	public float RingWidthFactor { get { return ((1 - innerRadiusPercentage) * outerRadius) / origGraphWidth; } }
	public float labelLineStartOffset { get { return (pieMode ? (labelStartCenteredOnBand ? (outerRadius/2f * (1 - innerRadiusPercentage)) : 0) : outerRadius); } }
	public float MaxDataVal { get; private set; }
	public float MinDataVal { get; private set; }

	// Original property values for use with dynamic resizing
	private float origGraphWidth;

	// cache
	private float containerWidthCached;
	private float containerHeightCached;

	// public getter
	public List<WMG_Ring> rings { get; private set; }

	// private
	private Sprite extraRingSprite;
	private Color[] texColors;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	private WMG_Change_Obj numberRingsC = new WMG_Change_Obj();
	private WMG_Change_Obj bandColorC = new WMG_Change_Obj();
	private WMG_Change_Obj ringColorC = new WMG_Change_Obj();
	private WMG_Change_Obj labelLineColorC = new WMG_Change_Obj();
	private WMG_Change_Obj labelsC = new WMG_Change_Obj();
	private WMG_Change_Obj degreesC = new WMG_Change_Obj();
	private WMG_Change_Obj aRingValC = new WMG_Change_Obj();
	private WMG_Change_Obj radiusC = new WMG_Change_Obj();
	private WMG_Change_Obj textureC = new WMG_Change_Obj();
	private WMG_Change_Obj hideRingsC = new WMG_Change_Obj();
	private WMG_Change_Obj labelLineC = new WMG_Change_Obj();
	private WMG_Change_Obj computeShaderC = new WMG_Change_Obj();

	private List<int> aRingValChangeIndices = new List<int>();
	private List<int> beforeValCount = new List<int> ();
	private List<int> afterValCount = new List<int> ();
	private bool hasInit;

	public delegate string DataLabeler(WMG_Ring_Graph graph, float val);
	public DataLabeler dataLabeler;
	public string defaultDataLabeler(WMG_Ring_Graph graph, float val) {
		float numberToMult = Mathf.Pow(10f, 2);
		return (Mathf.Round(val * numberToMult) / numberToMult).ToString();
	}
	
	public delegate void TextureChanger(GameObject obj, Sprite sprite, int textureNum, float maxSize, float inner, float outer, bool antiAliasing, float antiAliasingStrength, float fill);
	public TextureChanger textureChanger;
	
	public delegate void ColorChanger(GameObject obj, Color color);
	public ColorChanger colorChanger;

	public delegate void BandColorsChanger();
	public BandColorsChanger bandColorsChanger;

	void Start() {
		Init ();
		PauseCallbacks();
		AllChanged();
		if (values.Count > 0) {
			beforeValCount.Add (0);
			afterValCount.Add (values.Count);
		}
	}

	/// <summary>
	/// Initializes the graph, and should always be done before anything else, called automatically in Start(), but it never hurts to call this manually after instantiating a graph prefab.
	/// </summary>
	public void Init() {
		if (hasInit) return;
		hasInit = true;
		MaxDataVal = Mathf.NegativeInfinity;
		MinDataVal = Mathf.Infinity;
		
		changeObjs.Add(numberRingsC);
		changeObjs.Add(hideRingsC);
		changeObjs.Add(computeShaderC);
		changeObjs.Add(textureC);
		changeObjs.Add(degreesC);
		changeObjs.Add(aRingValC);
		changeObjs.Add(ringColorC);
		changeObjs.Add(labelLineColorC);
		changeObjs.Add(bandColorC);
		changeObjs.Add(radiusC);
		changeObjs.Add(labelsC);
		changeObjs.Add(labelLineC);
		
		textureChanger = defaultTextureChanger;
		colorChanger = defaultColorChanger;
		bandColorsChanger = defaultUpdateBandColors;
		
		extraRingSprite = WMG_Util.createSprite(textureResolution, textureResolution);
		texColors = new Color[textureResolution * textureResolution];
		setTexture(extraRing, extraRingSprite);
		rings = new List<WMG_Ring>();
		origGraphWidth = ((1 - innerRadiusPercentage) * outerRadius);
		
		bandColors.SetList (_bandColors);
		bandColors.Changed += bandColorsChanged;
		
		values.SetList (_values);
		values.Changed += valuesChanged;
		
		labels.SetList (_labels);
		labels.Changed += labelsChanged;

		hideRings.SetList (_hideRings);
		hideRings.Changed += hideRingsChanged;
		
		ringIDs.SetList (_ringIDs);
		ringIDs.Changed += ringIDsChanged;
		
		numberRingsC.OnChange += NumberRingsChanged;
		bandColorC.OnChange += BandColorChanged;
		ringColorC.OnChange += RingColorChanged;
		labelLineColorC.OnChange += LabelLineColorChanged;
		labelsC.OnChange += LabelsChanged;
		degreesC.OnChange += DegreesChanged;
		aRingValC.OnChange += DegreesChangedAring;
		radiusC.OnChange += RadiusChanged;
		textureC.OnChange += TextureChanged;
		hideRingsC.OnChange += HideRingsChanged;
		labelLineC.OnChange += updateAllRings;
		computeShaderC.OnChange += UseComputeShaderChanged;

		dataLabeler = defaultDataLabeler;
		
		PauseCallbacks();
	}

	void Update() {
		updateFromDataSource();
		updateFromResize();
		
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

	void defaultTextureChanger(GameObject obj, Sprite sprite, int textureNum, float maxSize, float inner, float outer, bool antiAliasing, float antiAliasingStrength, float fill) {
		if (activeInHierarchy (obj)) {
			WMG_Util.updateBandColors (ref texColors, maxSize, inner, outer, antiAliasing, antiAliasingStrength);
			sprite.texture.SetPixels (texColors);
			sprite.texture.Apply ();
		}
	}

	void defaultTextureChangerCS(GameObject obj, Sprite sprite, int textureNum, float maxSize, float inner, float outer, bool antiAliasing, float antiAliasingStrength, float fill) {
		if (activeInHierarchy (obj)) {
			WMG_Compute_Shader cs = obj.GetComponent<WMG_Compute_Shader>();
			if (cs) {
				cs.computeShader.SetFloats ("floatArgs", new float[]{ maxSize, inner, outer, antiAliasing ? 1 : 0, antiAliasingStrength, fill * 360, 1, 270 });
				cs.dispatchAndUpdateImage();
			}
		}
	}
	
	void defaultColorChanger(GameObject obj, Color color) {
		changeSpriteColor (obj, color);
	}

	void updateFromResize() {
		bool resizeChanged = false;
		WMG_Util.updateCacheAndFlag<float>(ref containerWidthCached, getSpriteWidth(this.gameObject), ref resizeChanged);
		WMG_Util.updateCacheAndFlag<float>(ref containerHeightCached, getSpriteHeight(this.gameObject), ref resizeChanged);
		if (resizeChanged) {
			radiusC.Changed();
			labelLineC.Changed();
		}
	}

	void updateFromDataSource() {
		if (valuesDataSource != null) {
			values.SetList(valuesDataSource.getData<float>());
		}
		if (labelsDataSource != null) {
			labels.SetList(labelsDataSource.getData<string>());
		}
		if (ringIDsDataSource != null) {
			ringIDs.SetList(ringIDsDataSource.getData<string>());
		}
	}

	void NumberRingsChanged() {
		updateNumberRings();
	}

	void ToggleComputeShaderTexture(GameObject obj, Sprite sprite) {
		if (useComputeShader) {
			WMG_Compute_Shader cs = obj.GetComponent<WMG_Compute_Shader>();
			if (cs == null) {
				DestroyImmediate(obj.GetComponent<UnityEngine.UI.Image>());
				obj.AddComponent<UnityEngine.UI.RawImage>();
				cs = obj.AddComponent<WMG_Compute_Shader>();
				cs.computeShader = computeShader;
				cs.Init(textureResolution);
			}
		} else {
			WMG_Compute_Shader cs = obj.GetComponent<WMG_Compute_Shader>();
			if (cs != null) {
				DestroyImmediate(cs);
				DestroyImmediate(obj.GetComponent<UnityEngine.UI.RawImage>());
				UnityEngine.UI.Image img = obj.AddComponent<UnityEngine.UI.Image>();
				setTexture(obj, sprite);
				img.type = UnityEngine.UI.Image.Type.Filled;
				img.fillMethod = UnityEngine.UI.Image.FillMethod.Radial360;
			}
		}
	}

	void UseComputeShaderChanged() {
		if (useComputeShader) {
			textureChanger = defaultTextureChangerCS;
		}
		else {
			textureChanger = defaultTextureChanger;
		}

		ToggleComputeShaderTexture (extraRing, extraRingSprite);

		foreach (WMG_Ring ring in rings) {
			ToggleComputeShaderTexture (ring.band, ring.bandSprite);
			ToggleComputeShaderTexture (ring.ring, ring.ringSprite);
		}
	}

	void TextureChanged() {
		updateRingAndBandTextures();
		updateSizesOfAllObjects();
	}

	void DegreesChanged() {
		if (!pieMode && beforeValCount.Count == 1) {
			updateDegreesAllRings(false, true);
		} else { // pie mode or count changed more than once in a frame, update all
			updateDegreesAllRings(true, beforeValCount.Count > 0);
		}
		
		beforeValCount.Clear ();
		afterValCount.Clear ();
	}
	
	void DegreesChangedAring() {
		if (pieMode || aRingValChangeIndices.Count > 1) { // pie mode or multiple vals changed in a frame, update all
			updateDegreesAllRings(true, true);
		} else {
			updateDegreesAring (aRingValChangeIndices[0], false, 0, 0);
		}
		aRingValChangeIndices.Clear ();
	}

	void RingColorChanged() {
		updateRingColors();
	}

	void LabelLineColorChanged() {
		updateLabelLineColors();
	}

	void BandColorChanged() {
		bandColorsChanger();
	}

	void RadiusChanged() {
		updateSizesOfAllObjects();
	}

	void LabelsChanged() {
		updateLabelsText();
	}

	void HideRingsChanged() {
		updateRingsActive ();
	}

	void AllChanged() {
		numberRingsC.Changed();
		computeShaderC.Changed();
		textureC.Changed();
		degreesC.Changed();
		ringColorC.Changed();
		bandColorC.Changed();
		labelLineColorC.Changed();
		radiusC.Changed();
		labelsC.Changed();
		hideRingsC.Changed ();
	}

	public void bandColorsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref bandColors, ref _bandColors, oneValChanged, index);
		bandColorC.Changed();
	}

	public void valuesChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		if (countChanged) {
			if (editorChange) {
				beforeValCount.Add (values.Count);
				afterValCount.Add (_values.Count);
			} else {
				beforeValCount.Add (_values.Count);
				afterValCount.Add (values.Count);
			}
		}
		WMG_Util.listChanged (editorChange, ref values, ref _values, oneValChanged, index);
		if (countChanged) {
			AllChanged ();
		} else {
			if (oneValChanged) {
				aRingValChangeIndices.Add (index);
				aRingValC.Changed ();
			} else {
				degreesC.Changed ();
			}
		}
		if (showDataLabels) {
			labelsC.Changed ();
		}
	}

	public void labelsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref labels, ref _labels, oneValChanged, index);
		labelsC.Changed();
	}

	public void hideRingsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref hideRings, ref _hideRings, oneValChanged, index);
		hideRingsC.Changed();
	}

	public void ringIDsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref ringIDs, ref _ringIDs, oneValChanged, index);
	}

	void updateNumberRings() {
		// Create rings based on values data
		for (int i = 0; i < values.Count; i++) {
			if (labels.Count <= i) labels.AddNoCb("Ring " + (i + 1), ref _labels);
			if (hideRings.Count <= i) hideRings.AddNoCb(false, ref _hideRings);
			if (bandColors.Count <= i) bandColors.AddNoCb(bandColor, ref _bandColors);
			if (rings.Count <= i) {
				GameObject obj = GameObject.Instantiate(ringPrefab) as GameObject;
				changeSpriteParent(obj, ringsParent);
				WMG_Ring ring = obj.GetComponent<WMG_Ring>();
				obj.name = "Ring" + (i+1);
				ring.initialize(this);
				rings.Add(ring);
			}
		}
		for (int i = rings.Count - 1; i >= 0; i--) {
			if (rings[i] != null && i >= values.Count) {
				Destroy(rings[i].label);
				Destroy(rings[i].gameObject);
				rings.RemoveAt(i);
			}
		}
	}

	void updateSizesOfAllObjects() {
		int newSize = Mathf.RoundToInt(outerRadius*2);
		// extra ring
		changeSpriteSize(extraRing, newSize, newSize);
		// rings and bands
		for (int i = 0; i < rings.Count; i++) {
			changeSpriteSize(rings[i].ring, newSize, newSize);
			changeSpriteSize(rings[i].band, newSize, newSize);
			changeSpriteSize(rings[i].interactibleObj, newSize, newSize);
			changeSpriteHeight(rings[i].label, Mathf.RoundToInt(labelLineStartOffset + labelLinePadding));
		}
		// zero line
		changeSpriteHeight(zeroLine, Mathf.RoundToInt(outerRadius + labelLinePadding));
		// label line points
		for (int i = 0; i < rings.Count; i++) {
			if (!pieMode) {
				changeSpritePivot (rings[i].labelPoint, WMGpivotTypes.Center);
				float ringRadius = getRingRadius (pieMode ? 0 : i);
				// label points
				if (bandMode && labelStartCenteredOnBand) { // center on bands
					float nextRingRadius = (pieMode ? outerRadius : (getRingRadius (i + 1)));
					changeSpritePositionToY (rings[i].labelPoint, -(ringRadius + (nextRingRadius - ringRadius) / 2 - RingWidthFactor * ringWidth / 2));
				} else { // center on rings
					changeSpritePositionToY (rings[i].labelPoint, -(ringRadius - RingWidthFactor * ringWidth / 2));
				}
				int pointWidthHeight = Mathf.RoundToInt (RingWidthFactor * ringWidth + RingWidthFactor * ringPointWidthFactor);
				changeSpriteSize (rings[i].labelPoint, pointWidthHeight, pointWidthHeight);
			}
			else {
				changeSpritePivot (rings[i].labelPoint, labelStartCenteredOnBand ? WMGpivotTypes.Center : WMGpivotTypes.Bottom);
				changeSpritePositionToY(rings[i].labelPoint, 0);
				changeSpriteSizeFloat(rings[i].labelPoint, labelPointSize.x, labelPointSize.y);
			}
		}
		// Update position of content based on padding
		Vector3 contentOffset = new Vector3((leftRightPadding.y - leftRightPadding.x)/2f, (topBotPadding.y - topBotPadding.x)/2f);
		changeSpritePositionTo(contentParent, contentOffset);
	}

	void updateLabelsText() {
		for (int i = 0; i < rings.Count; i++) {
			// regular labels
			// Update font size
			changeLabelFontSize(rings[i].labelText, labelsFontSize);
			// Font Color
			changeLabelColor(rings[i].labelText, labelsColor);
			// Font Style
			changeLabelFontStyle(rings[i].labelText, labelsFontStyle);
			// Font
			if (dataLabelsFont != null) {
				changeLabelFont(rings[i].labelText, labelsFont);
			}
			// Set Text
			changeLabelText(rings[i].labelText, labels [i]);

			// data labels
			if (showDataLabels) {
				// Update font size
				changeLabelFontSize(rings[i].lowerLabelText, dataLabelsFontSize);
				// Font Color
				changeLabelColor(rings[i].lowerLabelText, dataLabelsColor);
				// Font Style
				changeLabelFontStyle(rings[i].lowerLabelText, dataLabelsFontStyle);
				// Font
				if (dataLabelsFont != null) {
					changeLabelFont(rings[i].lowerLabelText, dataLabelsFont);
				}
				// Set Text
				changeLabelText(rings[i].lowerLabelText, dataLabeler(this, values[i]));
			}
			SetActive(rings[i].lowerLabelText, showDataLabels);
		}
		for (int i = 0; i < rings.Count; i++) {
			float labelsWidth = 0;
			if (showDataLabels) {
				labelsWidth = Mathf.Max(getTextWidth(rings[i].labelText), getTextWidth(rings[i].lowerLabelText));
			}
			else {
				labelsWidth = getTextWidth(rings[i].labelText);
			}
			changeSpriteHeight(rings[i].textLine, Mathf.RoundToInt(labelsWidth + labelTextOffset));
		}
	}

	void updateRingsActive() {
		SetActive (extraRing, bandMode && !pieMode);
		for (int i = 0; i < rings.Count; i++) {
			WMG_Ring ring = rings[i];
			bool shouldNotHide = !hideRings [i];
			SetActive (ring.band, bandMode && shouldNotHide);
			SetActive (ring.ring, shouldNotHide && !pieMode);
			SetActive (ring.interactibleObj, shouldNotHide && pieMode);
			SetActive (ring.label, shouldNotHide);
		}
	}

	void updateRingAndBandTextures() {
		if (useComputeShader) return;
		// extra ring
		if (bandMode) {
			float ringRadius = getRingRadius(pieMode ? 0 : rings.Count);
			textureChanger(extraRing, extraRingSprite, 0, outerRadius*2, ringRadius - ringWidth, ringRadius, antiAliasing, antiAliasingStrength, 1);
		}
		// rings and bands
		for (int i = 0; i < rings.Count; i++) {
			rings[i].updateRingTexture(i, 1, 1);
		}
	}

	public float getRingRadius(int index) {
		int numRingsToDivide = rings.Count - 1;
		if (bandMode) numRingsToDivide++;
		if (numRingsToDivide == 0) return outerRadius; // Only happens in non-band mode with only 1 ring
		float ringInterval = (1-innerRadiusPercentage) * outerRadius / numRingsToDivide;
		return innerRadiusPercentage * outerRadius + index * ringInterval;
	}

	void updateDegreesAllRings(bool updateAll, bool countChanged) {
		if (updateAll) {
			// calculate total val
			float totalVal = 0;
			for (int i = 0; i < rings.Count; i++) {
				totalVal += values[i];
			}
			float previousCumulativeValue = 0;
			for (int i = 0; i < rings.Count; i++) {
				updateDegreesAring (i, countChanged, totalVal, previousCumulativeValue);
				previousCumulativeValue += values[i];
			}
		} else {
			for (int i = afterValCount [0] < beforeValCount [0] ? 0 : beforeValCount [0]; i < rings.Count; i++) {
				updateDegreesAring (i, countChanged, 0, 0);
			}
		}
	}

	void updateDegreesAring(int i, bool countChanged, float totalVal, float previousCumulativeValue) {
		if (MaxDataVal < values [i]) {
			MaxDataVal = values [i];
		}
		if (MinDataVal > values [i]) {
			MinDataVal = values [i];
		}
		if (pieMode || useComputeShader) {
			countChanged = true;
		}
		Vector3 baseRotation = new Vector3 (0, 0, pieMode ? (-previousCumulativeValue / totalVal * 360f - pieModeDegreeOffset) : -degrees/2);
		float valPercent = pieMode ? (values[i] / totalVal - 2 * pieModePaddingDegrees / 360f) : (values[i] / (maxValue - minValue));

		float newFill = (360 - degrees) / 360f;


		if (useComputeShader) {
			// extra ring
			changeRadialSpriteRotation (extraRing, baseRotation);
			float ringRadius = getRingRadius(pieMode ? 0 : rings.Count);
			textureChanger(extraRing, extraRingSprite, 0, outerRadius*2, ringRadius - ringWidth, ringRadius, antiAliasing, antiAliasingStrength, newFill);
		}

		if (countChanged) {
			if (useComputeShader) {
				// rings and bands
				changeRadialSpriteRotation(rings[i].ring, baseRotation);
				changeRadialSpriteRotation (rings [i].band, baseRotation);
				rings[i].updateRingTexture(i, newFill, 0);
			}
			else {
				// extra ring
				changeRadialSpriteRotation (extraRing, baseRotation);
				changeSpriteFill (extraRing, newFill);
				// rings
				changeRadialSpriteRotation(rings[i].ring, baseRotation);
				changeSpriteFill(rings[i].ring, newFill);
				// bands
				changeRadialSpriteRotation (rings [i].band, baseRotation);
				changeSpriteFill (rings [i].band, 0);

			}
			changeRadialSpriteRotation (rings [i].interactibleObj, baseRotation);
			changeSpriteFill(rings[i].interactibleObj, newFill);
		}

		if (animateData) {
			rings[i].animBandFill(i, newFill * valPercent);
		}
		else {
			if (!useComputeShader) {
				changeSpriteFill(rings[i].band, newFill * valPercent);
			}
			else {
				rings[i].updateRingTexture(i, newFill, newFill * valPercent);
			}
			if (pieMode) {
				changeSpriteFill(rings[i].interactibleObj, newFill * valPercent);
			}
		}

		updateLabelLineAring (i, countChanged, totalVal, baseRotation, valPercent);

		// zero line
		bool ringIsZero = false;
		if (valPercent == 0 || pieMode || hideZeroLabelLine) {
			ringIsZero = true;
		}
		zeroLine.transform.localEulerAngles = baseRotation;
		zeroLineText.transform.localEulerAngles = -baseRotation;
		if (i == 0) {
			SetActive (zeroLine, !ringIsZero);
		} else {
			if (zeroLine.activeSelf) { // previously not 0
				SetActive (zeroLine, !ringIsZero);
			} else { // there was a zero at one point in the past, always hide
				SetActive (zeroLine, false);
			}
		}
	}

	void updateAllRings() {
		float totalVal = 0;
		for (int i = 0; i < rings.Count; i++) {
			totalVal += values[i];
		}
		float previousCumulativeValue = 0;
		for (int i = 0; i < rings.Count; i++) {
			Vector3 baseRotation = new Vector3 (0, 0, pieMode ? (-previousCumulativeValue / totalVal * 360f - pieModeDegreeOffset) : -degrees/2);
			float valPercent = pieMode ? (values[i] / totalVal - 2 * pieModePaddingDegrees / 360f) : (values[i] / (maxValue - minValue));

			updateLabelLineAring (i, true, totalVal, baseRotation, valPercent);
			previousCumulativeValue += values[i];
		}
	}

	void updateLabelLineAring(int i, bool countChanged, float totalVal, Vector3 baseRotation, float valPercent) {
		WMG_Ring ring = rings[i];
		// labels
		int numOverlapping = 0;
		if (!pieMode) {
			for (int j = i-1; j >= 0; j--) {
				float valPercentPrev = pieMode ? (values [j] / totalVal) : (values [j] / (maxValue - minValue));
				if (Mathf.Abs (valPercent - valPercentPrev) < 0.01f) { // within 1%
					numOverlapping++;
					valPercent = valPercentPrev;
				}
			}
		}
		
		Vector3 labelAngle = new Vector3(0, 0, baseRotation.z + (pieMode ? -valPercent/2f : -valPercent) * (360 - degrees));
		float sin_la = Mathf.Sin(labelAngle.z * Mathf.Deg2Rad);
		float cos_la = Mathf.Cos(labelAngle.z * Mathf.Deg2Rad);
		float pieBandWidth = outerRadius * (1 - innerRadiusPercentage);
		if (!pieMode) {
			changeSpritePositionTo (rings[i].label, Vector3.zero);
		}
		float labelLineStartPosition = (labelStartCenteredOnBand ? outerRadius - pieBandWidth/2f : outerRadius);
		if (animateData) {
			if (DOTween.IsTweening(rings[i].label.transform) || !countChanged) { // if already animating, then don't animate relative to current rotation
				updateLabelRotationAndPosition (ring, 0, numOverlapping, false);
				float degOffset = 90;
				if (rings[i].label.transform.localEulerAngles.z < 180) {
					degOffset *= -1;
				}
				WMG_Anim.animRotation(rings[i].label, animDuration, animEaseType, labelAngle + new Vector3(0,0,360), false);
				WMG_Anim.animRotationCallbacks(rings[i].textLine, animDuration, animEaseType, -labelAngle + new Vector3(0,0, degOffset), false, ()=> labelRotationUpdated(ring, degOffset, numOverlapping), ()=> labelRotationComplete(ring, degOffset, numOverlapping));
				if (pieMode) {
					WMG_Anim.animPosition(rings[i].label, animDuration, animEaseType, new Vector3(labelLineStartPosition * sin_la, labelLineStartPosition * -cos_la, 0));
				}
			}
			else {
				rings[i].label.transform.localEulerAngles = baseRotation;
				rings[i].textLine.transform.localEulerAngles = -baseRotation + new Vector3(0,0,90);
				if (pieMode) {
					changeSpritePositionTo (rings[i].label, new Vector3(innerRadiusPercentage*outerRadius * sin_la, innerRadiusPercentage*outerRadius * -cos_la, 0));
				}
				WMG_Anim.animRotation(rings[i].label, animDuration, animEaseType, (labelAngle - baseRotation), true);
				WMG_Anim.animRotationCallbacks(rings[i].textLine, animDuration, animEaseType, -(labelAngle - baseRotation), true, ()=> labelRotationUpdated(ring, 0, numOverlapping), ()=> labelRotationComplete(ring, 0, numOverlapping));
				if (pieMode) {
					WMG_Anim.animPosition(rings[i].label, animDuration, animEaseType, new Vector3(labelLineStartPosition * sin_la, labelLineStartPosition * -cos_la, 0));
				}
			}
		}
		else {
			updateLabelLineBasedOnOverlap (ring, numOverlapping);
			rings[i].label.transform.localEulerAngles = labelAngle;
			rings[i].textLine.transform.localEulerAngles = -labelAngle + new Vector3(0,0,90);
			updateLabelRotationAndPosition (ring, 0, numOverlapping, false);
			if (pieMode) {
				changeSpritePositionTo (rings[i].label, new Vector3(labelLineStartPosition * sin_la, labelLineStartPosition * -cos_la, 0));
			}
		}
	}
	
	void updateLabelLineBasedOnOverlap(WMG_Ring ring, int numOverlapping) {
		changeSpriteWidth (ring.textLine, Mathf.RoundToInt(2 + getSpriteHeight(ring.labelBackground) * numOverlapping));
		bool labelsOverlap = numOverlapping > 0;
		if (!labelsOverlap) {
			SetActiveImage (ring.line, true);
			changeSpritePivot (ring.textLine, WMGpivotTypes.Bottom);
			setTexture (ring.textLine, labelLineSprite);
			setAnchor (ring.labelBackground, new Vector2 (0, 1), new Vector2 (1, 0), Vector2.zero);
			setAnchor (ring.lowerLabelBackground, new Vector2 (0, 1), new Vector2 (1, 0), Vector2.zero);
		} else {
			SetActiveImage (ring.line, false);
		}
		
	}
	
	void updateLabelRotationAndPosition(WMG_Ring ring, float degOffset, int numOverlapping, bool onComplete) {
		bool labelsOverlap = numOverlapping > 0;
		bool labelOnRightSide = ring.label.transform.localEulerAngles.z < 180;
		if (labelOnRightSide) {
			if (labelsOverlap) {
				changeSpritePivot (ring.textLine, WMGpivotTypes.BottomLeft);
				setTexture (ring.textLine, botRightCorners);
				setAnchor (ring.labelBackground, Vector2.one, new Vector2 (1, 0), Vector2.zero);
				setAnchor (ring.lowerLabelBackground, Vector2.one, new Vector2 (1, 0), Vector2.zero);
			}
			if (!onComplete) {
				if (degOffset == 0 || degOffset == 90) {
					ring.textLine.transform.localEulerAngles = new Vector3 (ring.textLine.transform.localEulerAngles.x, 
					                                                        ring.textLine.transform.localEulerAngles.y, 
					                                                        ring.textLine.transform.localEulerAngles.z - 180);
				}
				ring.labelBackground.transform.localEulerAngles = new Vector3 (0, 0, 90);
				ring.lowerLabelBackground.transform.localEulerAngles = new Vector3 (0, 0, 90);
				
			}
			changeSpritePivot (ring.labelBackground, WMGpivotTypes.BottomRight);
			changeSpritePivot (ring.lowerLabelBackground, WMGpivotTypes.BottomRight);
		} 
		else {
			if (labelsOverlap) {
				changeSpritePivot (ring.textLine, WMGpivotTypes.BottomRight);
				setTexture (ring.textLine, botLeftCorners);
				setAnchor (ring.labelBackground, new Vector2 (0, 1), new Vector2 (1, 0), Vector2.zero);
				setAnchor (ring.lowerLabelBackground, new Vector2 (0, 1), new Vector2 (1, 0), Vector2.zero);
			}
			if (!onComplete) {
				if (degOffset == -90) {
					ring.textLine.transform.localEulerAngles = new Vector3 (ring.textLine.transform.localEulerAngles.x, 
					                                                        ring.textLine.transform.localEulerAngles.y, 
					                                                        ring.textLine.transform.localEulerAngles.z + 180);
				}
				ring.labelBackground.transform.localEulerAngles = new Vector3 (0, 0, -90);
				ring.lowerLabelBackground.transform.localEulerAngles = new Vector3 (0, 0, -90);
				
			}
			changeSpritePivot (ring.labelBackground, WMGpivotTypes.BottomLeft);
			changeSpritePivot (ring.lowerLabelBackground, WMGpivotTypes.BottomLeft);
		}
		changeSpritePositionToX (ring.lowerLabelBackground, getSpriteHeight(ring.lowerLabelBackground) * (labelOnRightSide ? 1 : -1));
	}
	
	void labelRotationComplete(WMG_Ring ring, float degOffset, int numOverlapping) {
		bool labelsOverlap = numOverlapping > 0;
		if (labelsOverlap) { // if label is overlapping only update at the end
			updateLabelLineBasedOnOverlap (ring, numOverlapping);
			updateLabelRotationAndPosition (ring, degOffset, numOverlapping, true);
		}
	}
	
	void labelRotationUpdated(WMG_Ring ring, float degOffset, int numOverlapping) {
		updateLabelLineBasedOnOverlap (ring, 0);
		updateLabelRotationAndPosition (ring, degOffset, 0, false);
	}

	public List<int> getRingsSortedByValue() {
		List<float> newVals = new List<float>(values);
		newVals.Sort();
		List<int> ringIndices = new List<int>();
		for (int i = 0; i < newVals.Count; i++) {
			for (int j = 0; j < values.Count; j++) {
				if (Mathf.Approximately(values[j], newVals[i])) {
					ringIndices.Add(j);
					break;
				}
			}
		}
		return ringIndices;
	}

	void updateRingColors() {
		colorChanger (extraRing, ringColor);
		for (int i = 0; i < rings.Count; i++) {
			colorChanger (rings[i].ring, ringColor);
		}
	}

	void updateLabelLineColors() {
		for (int i = 0; i < rings.Count; i++) {
			changeSpriteColor(rings[i].line, labelLineColor);
			changeSpriteColor(rings[i].textLine, labelLineColor);
			changeSpriteColor(rings[i].labelPoint, labelPointColor);
		}
	}
	
	void defaultUpdateBandColors() {
		for (int i = 0; i < rings.Count; i++) {
			if (autoUpdateBandAlpha) {
				bandColors.SetValNoCb(i, new Color(bandColors[i].r, bandColors[i].g, bandColors[i].b, 
				                                   (autoUpdateBandAlphaReverse ? (rings.Count - i) : (i + 1f)) / rings.Count), ref _bandColors);
			}
			colorChanger (rings[i].band, bandColors[i]);
		}
	}

	/// <summary>
	/// Get a ring using the ring ID, where ring IDs are assigned with #ringIDs.
	/// </summary>
	/// <returns>The ring.</returns>
	/// <param name="id">Identifier.</param>
	public WMG_Ring getRing(string id) {
		for (int i = 0; i < ringIDs.Count; i++) {
			if (id == ringIDs[i]) return rings[i];
		}
		Debug.LogError("No ring found with id: " + id);
		return null;
	}

	/// <summary>
	/// Highlight the specified ring using ring ID from #ringIDs by setting alpha of all rings except the specified ring to 0.
	/// </summary>
	/// <param name="id">Identifier.</param>
	public void HighlightRing(string id) {
		for (int i = 0; i < rings.Count; i++) {
			colorChanger (rings[i].band, new Color(bandColor.r, bandColor.g, bandColor.b, 0));
		}
		colorChanger (getRing(id).band, new Color(bandColor.r, bandColor.g, bandColor.b, 1));
	}

	/// <summary>
	/// Reverts ring highlighting function from #HighlightRing.
	/// </summary>
	public void RemoveHighlights() {
		bandColorC.Changed ();
	}

}
