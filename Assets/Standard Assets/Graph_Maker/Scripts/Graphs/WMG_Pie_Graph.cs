using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Class used for creating pie graphs.
/// </summary>
public class WMG_Pie_Graph : WMG_Graph_Manager {
	
	public enum sortMethod {None, Largest_First, Smallest_First, Alphabetically, Reverse_Alphabetically};
	[System.Flags]
	public enum ResizeProperties {
		LabelExplodeLength	= 1 << 0,
		LabelFontSize		= 1 << 1,
		LegendFontSize	 	= 1 << 2,
		LegendSwatchSize	= 1 << 3,
		LegendEntrySize		= 1 << 4,
		LegendOffset		= 1 << 5,
		AutoCenterPadding	= 1 << 6
	}

	[SerializeField] private List<float> _sliceValues;
	/// <summary>
	/// The values each pie slice represents.
	/// </summary>
	public WMG_List<float> sliceValues = new WMG_List<float>();
	[SerializeField] private List<string> _sliceLabels;
	/// <summary>
	/// The text labels each pie slice represents.
	/// </summary>
	public WMG_List<string> sliceLabels = new WMG_List<string>();
	[SerializeField] private List<Color> _sliceColors;
	/// <summary>
	/// The color of each pie slice.
	/// </summary>
	public WMG_List<Color> sliceColors = new WMG_List<Color>();

	/// <summary>
	/// Determines whether graph content (#resizeProperties) will resize post graph initialization based on the percentage change of the graph's rect transform width / height.
	/// </summary>
	/// <value><c>true</c> if resize enabled; otherwise, <c>false</c>.</value>
	public bool resizeEnabled { get {return _resizeEnabled;} 
		set {
			if (_resizeEnabled != value) {
				_resizeEnabled = value;
				resizeC.Changed();
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
				resizeC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the left right padding.
	/// </summary>
	/// <value>The left right padding.</value>
	public Vector2 leftRightPadding { get {return _leftRightPadding;} 
		set {
			if (_leftRightPadding != value) {
				_leftRightPadding = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the top bot padding.
	/// </summary>
	/// <value>The top bot padding.</value>
	public Vector2 topBotPadding { get {return _topBotPadding;} 
		set {
			if (_topBotPadding != value) {
				_topBotPadding = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Controls how much space is between the background circle sprite and the pie slice outer edges.
	/// </summary>
	/// <value>The background circle offset.</value>
	public float bgCircleOffset { get {return _bgCircleOffset;} 
		set {
			if (_bgCircleOffset != value) {
				_bgCircleOffset = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When enabled, ensures the pie graph and legend both stay in the center of the graph's rect transform. This will automatically adjust the 
	/// background padding as well, using #autoCenterMinPadding as the minimum for the padding.
	/// </summary>
	/// <value><c>true</c> if auto center; otherwise, <c>false</c>.</value>
	public bool autoCenter { get {return _autoCenter;} 
		set {
			if (_autoCenter != value) {
				_autoCenter = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When #autoCenter = true, controls how much padding there is between the pie graph and its rect transform bounds.
	/// </summary>
	/// <value>The auto center minimum padding.</value>
	public float autoCenterMinPadding { get {return _autoCenterMinPadding;} 
		set {
			if (_autoCenterMinPadding != value) {
				_autoCenterMinPadding = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// This controls the order that the pie slices appear. The default is Largest_First, meaning that the slice with the highest value will appear in the top right, 
	/// and subsequent slices go in clockwise order. For "None" sorting, the slice value corresponding to the 0 index of the Values list appears in the top right corner. 
	/// </summary>
	/// <value>The sort by.</value>
	public sortMethod sortBy { get {return _sortBy;} 
		set {
			if (_sortBy != value) {
				_sortBy = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When set to true, if the 3rd slice is green and the 3rd slice value gets increased so that it becomes the first slice after sorting, 
	/// then the green color will swap to be in the top right slice. If this is set to false, then the colors remain static, meaning green will always be the third slice. 
	/// </summary>
	/// <value><c>true</c> if swap colors during sort; otherwise, <c>false</c>.</value>
	public bool swapColorsDuringSort { get {return _swapColorsDuringSort;} 
		set {
			if (_swapColorsDuringSort != value) {
				_swapColorsDuringSort = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Controls how the labels on the slices display.
	/// </summary>
	/// <value>The type of the slice label.</value>
	public WMG_Enums.labelTypes sliceLabelType { get {return _sliceLabelType;} 
		set {
			if (_sliceLabelType != value) {
				_sliceLabelType = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// This is the radial distance the  pie slices are from the center. A small value here usually enhances the visual appeal of the graph.
	/// </summary>
	/// <value>The length of the explode.</value>
	public float explodeLength { get {return _explodeLength;} 
		set {
			if (_explodeLength != value) {
				_explodeLength = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When the explode length is > 0, this determines whether the outer edge of the slices align.
	/// </summary>
	/// <value><c>true</c> if explode symmetrical; otherwise, <c>false</c>.</value>
	public bool explodeSymmetrical { get {return _explodeSymmetrical;} 
		set {
			if (_explodeSymmetrical != value) {
				_explodeSymmetrical = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether or not to carve out a center circle to create a doughnut graph.
	/// </summary>
	/// <value><c>true</c> if use doughnut; otherwise, <c>false</c>.</value>
	public bool useDoughnut { get {return _useDoughnut;} 
		set {
			if (_useDoughnut != value) {
				_useDoughnut = value;
				doughnutC.Changed();
			}
		}
	}
	/// <summary>
	/// When #useDoughnut = true, controls the size of the center circle carved out of the graph.
	/// </summary>
	/// <value>The doughnut percentage.</value>
	public float doughnutPercentage { get {return _doughnutPercentage;} 
		set {
			if (_doughnutPercentage != value) {
				_doughnutPercentage = value;
				doughnutC.Changed();
			}
		}
	}
	/// <summary>
	/// When enabled, the pie graph will limit the number of slices displayed. 
	/// So if there are 10 values, and 3 is set #maxNumberSlices, then only 3 of the 10 will be used, the 3 used depends on #sortBy.
	/// </summary>
	/// <value><c>true</c> if limit number slices; otherwise, <c>false</c>.</value>
	public bool limitNumberSlices { get {return _limitNumberSlices;} 
		set {
			if (_limitNumberSlices != value) {
				_limitNumberSlices = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When #limitNumberSlices = true, and has cut out some slices, then this determines whether to lump all of the slices that got excluded into an others slice.
	/// </summary>
	/// <value><c>true</c> if include others; otherwise, <c>false</c>.</value>
	public bool includeOthers { get {return _includeOthers;} 
		set {
			if (_includeOthers != value) {
				_includeOthers = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When #limitNumberSlices = true, this determines the maximum number of slices that are displayed not including the others slice if #includeOthers = true.
	/// </summary>
	/// <value>The max number slices.</value>
	public int maxNumberSlices { get {return _maxNumberSlices;} 
		set {
			if (_maxNumberSlices != value) {
				_maxNumberSlices = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When #includeOthers = true, this is the text label given to the others slice.
	/// </summary>
	/// <value>The include others label.</value>
	public string includeOthersLabel { get {return _includeOthersLabel;} 
		set {
			if (_includeOthersLabel != value) {
				_includeOthersLabel = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// When #includeOthers = true, this is the others slice color.
	/// </summary>
	/// <value>The color of the include others.</value>
	public Color includeOthersColor { get {return _includeOthersColor;} 
		set {
			if (_includeOthersColor != value) {
				_includeOthersColor = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the duration of the animation.
	/// </summary>
	/// <value>The duration of the animation.</value>
	public float animationDuration { get {return _animationDuration;} 
		set {
			if (_animationDuration != value) {
				_animationDuration = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the duration of the sort animation.
	/// </summary>
	/// <value>The duration of the sort animation.</value>
	public float sortAnimationDuration { get {return _sortAnimationDuration;} 
		set {
			if (_sortAnimationDuration != value) {
				_sortAnimationDuration = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the length of space the slice labels appear relative to the outer edge of the slices.
	/// </summary>
	/// <value>The length of the slice label explode.</value>
	public float sliceLabelExplodeLength { get {return _sliceLabelExplodeLength;} 
		set {
			if (_sliceLabelExplodeLength != value) {
				_sliceLabelExplodeLength = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the size of the slice labels.
	/// </summary>
	/// <value>The size of the slice label font.</value>
	public int sliceLabelFontSize { get {return _sliceLabelFontSize;} 
		set {
			if (_sliceLabelFontSize != value) {
				_sliceLabelFontSize = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the number decimals displayed in value labels.
	/// </summary>
	/// <value>The number decimals in percents.</value>
	public int numberDecimalsInPercents { get {return _numberDecimalsInPercents;} 
		set {
			if (_numberDecimalsInPercents != value) {
				_numberDecimalsInPercents = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Gets or sets the color of the slice labels.
	/// </summary>
	/// <value>The color of the slice label.</value>
	public Color sliceLabelColor { get {return _sliceLabelColor;} 
		set {
			if (_sliceLabelColor != value) {
				_sliceLabelColor = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// If a pie slice has a value of 0, do not show it in the legend.
	/// </summary>
	/// <value><c>true</c> if hide zero value legend entry; otherwise, <c>false</c>.</value>
	public bool hideZeroValueLegendEntry { get {return _hideZeroValueLegendEntry;} 
		set {
			if (_hideZeroValueLegendEntry != value) {
				_hideZeroValueLegendEntry = value;
				graphC.Changed();
			}
		}
	}
	/// <summary>
	/// Whether to raise mouse click / hover events for this pie graph.
	/// </summary>
	/// <value><c>true</c> if interactivity enabled; otherwise, <c>false</c>.</value>
	public bool interactivityEnabled { get {return _interactivityEnabled;} 
		set {
			if (_interactivityEnabled != value) {
				_interactivityEnabled = value;
				interactivityC.Changed();
			}
		}
	}

	// Public variables without change tracking
	/// <summary>
	/// Reference to WMG_Data_Source which can be used to populate #sliceValues.
	/// </summary>
	public WMG_Data_Source sliceValuesDataSource;
	/// <summary>
	/// Reference to WMG_Data_Source which can be used to populate #sliceLabels.
	/// </summary>
	public WMG_Data_Source sliceLabelsDataSource;
	/// <summary>
	/// Reference to WMG_Data_Source which can be used to populate #sliceColors.
	/// </summary>
	public WMG_Data_Source sliceColorsDataSource;
	/// <summary>
	/// The background object.
	/// </summary>
	public GameObject background;
	/// <summary>
	/// The background circle sprite.
	/// </summary>
	public GameObject backgroundCircle;
	/// <summary>
	/// The pie slices parent.
	/// </summary>
	public GameObject slicesParent;
	/// <summary>
	/// The legend.
	/// </summary>
	public WMG_Legend legend;
	/// <summary>
	/// The prefab used to create each legend entry for each pie slice.
	/// </summary>
	public Object legendEntryPrefab;
	/// <summary>
	/// The prefab used to create the pie slices.
	/// </summary>
	public Object nodePrefab;

	// Private backing variables
	[SerializeField] private bool _resizeEnabled;
	[WMG_EnumFlagAttribute] [SerializeField] private ResizeProperties _resizeProperties;
	[SerializeField] private Vector2 _leftRightPadding;
	[SerializeField] private Vector2 _topBotPadding;
	[SerializeField] private float _bgCircleOffset;
	[SerializeField] private bool _autoCenter;
	[SerializeField] private float _autoCenterMinPadding;
	[SerializeField] private sortMethod _sortBy;
	[SerializeField] private bool _swapColorsDuringSort;
	[SerializeField] private WMG_Enums.labelTypes _sliceLabelType;
	[SerializeField] private float _explodeLength;
	[SerializeField] private bool _explodeSymmetrical;
	[SerializeField] private bool _useDoughnut;
	[SerializeField] private float _doughnutPercentage;
	[SerializeField] private bool _limitNumberSlices;
	[SerializeField] private bool _includeOthers;
	[SerializeField] private int _maxNumberSlices;
	[SerializeField] private string _includeOthersLabel;
	[SerializeField] private Color _includeOthersColor;
	[SerializeField] private float _animationDuration;
	[SerializeField] private float _sortAnimationDuration;
	[SerializeField] private float _sliceLabelExplodeLength;
	[SerializeField] private int _sliceLabelFontSize;
	[SerializeField] private int _numberDecimalsInPercents;
	[SerializeField] private Color _sliceLabelColor;
	[SerializeField] private bool _hideZeroValueLegendEntry;
	[SerializeField] private bool _interactivityEnabled;


	// Useful property getters / data structures
	public float pieSize { 
		get {
			return Mathf.Min(getSpriteWidth(this.gameObject) - leftRightPadding.x - leftRightPadding.y + 2*explodeLength,
			                 getSpriteHeight(this.gameObject) - topBotPadding.x - topBotPadding.y + 2*explodeLength);
		}
	}
	/// <summary>
	/// Mapping of slice labels from #sliceLabels to WMG_Pie_Graph_Slice.
	/// </summary>
	public Dictionary<string, WMG_Pie_Graph_Slice> LabelToSliceMap = new Dictionary<string, WMG_Pie_Graph_Slice>();

	// Original property values for use with dynamic resizing
	private float origPieSize;
	private float origSliceLabelExplodeLength;
	private int origSliceLabelFontSize;
	private float origAutoCenterPadding;

	// Cache
	private float cachedContainerWidth;
	private float cachedContainerHeight;
	
	private List<GameObject> slices = new List<GameObject>();
	/// <summary>
	/// Get the pie slices.
	/// </summary>
	/// <returns>The slices.</returns>
	public List<GameObject> getSlices() {
		return slices;
	}
	private int numSlices = 0;
	private bool isOtherSlice = false;
	private float otherSliceValue = 0;
	private float totalVal = 0;
	private bool animSortSwap;
	private bool isAnimating;

	// texture variables used for doughnut
	private Color[] colors;
	private Color[] origColors;
	private Sprite pieSprite;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	public WMG_Change_Obj graphC = new WMG_Change_Obj();
	private WMG_Change_Obj resizeC = new WMG_Change_Obj();
	private WMG_Change_Obj doughnutC = new WMG_Change_Obj();
	private WMG_Change_Obj interactivityC = new WMG_Change_Obj();

	private bool hasInit;
	private bool setOrig;
	private bool doughnutHasInit;

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

		legend.Init ();

		changeObjs.Add(graphC);
		changeObjs.Add(resizeC);
		changeObjs.Add(doughnutC);
		changeObjs.Add(interactivityC);
		
		if (animationDuration > 0) UpdateVisuals(true);

		if (useDoughnut) {
			initDoughnut();
		}
		cachedContainerWidth = getSpriteWidth(this.gameObject);
		cachedContainerHeight = getSpriteHeight(this.gameObject);
		
		sliceValues.SetList (_sliceValues);
		sliceValues.Changed += sliceValuesChanged;
		
		sliceLabels.SetList (_sliceLabels);
		sliceLabels.Changed += sliceLabelsChanged;
		
		sliceColors.SetList (_sliceColors);
		sliceColors.Changed += sliceColorsChanged;

		graphC.OnChange += GraphChanged;
		resizeC.OnChange += ResizeChanged;
		doughnutC.OnChange += DoughtnutChanged;
		interactivityC.OnChange += InteractivityChanged;
		
//		setOriginalPropertyValues();
		PauseCallbacks();
	}

	void initDoughnut() {
		if (doughnutHasInit) return;
		doughnutHasInit = true;
		GameObject temp = GameObject.Instantiate(nodePrefab) as GameObject;
		Texture2D origTex = getTexture(temp.GetComponent<WMG_Pie_Graph_Slice>().objectToColor);
		colors = origTex.GetPixels();
		origColors = origTex.GetPixels();
		pieSprite = WMG_Util.createSprite(origTex.width, origTex.height);
		Destroy(temp);
		for (int i = 0; i < slices.Count; i++) {
			WMG_Pie_Graph_Slice pieSlice = slices[i].GetComponent<WMG_Pie_Graph_Slice>();
			setTexture(pieSlice.objectToColor, pieSprite);
			setTexture(pieSlice.objectToMask, pieSprite);
		}
	}

	void Update () {
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
		legend.PauseCallbacks();
	}
	
	void ResumeCallbacks() {
		for (int i = 0; i < changeObjs.Count; i++) {
			changeObjs[i].changesPaused = false;
			if (changeObjs[i].changePaused) changeObjs[i].Changed();
		}
		legend.ResumeCallbacks();
	}

	void updateFromResize() {
		bool resizeChanged = false;
		WMG_Util.updateCacheAndFlag<float>(ref cachedContainerWidth, getSpriteWidth(this.gameObject), ref resizeChanged);
		WMG_Util.updateCacheAndFlag<float>(ref cachedContainerHeight, getSpriteHeight(this.gameObject), ref resizeChanged);
		if (resizeChanged) {
			resizeC.Changed();
		}
	}

	void updateFromDataSource() {
		if (sliceValuesDataSource != null) {
			sliceValues.SetList(sliceValuesDataSource.getData<float>());
		}
		if (sliceLabelsDataSource != null) {
			sliceLabels.SetList(sliceLabelsDataSource.getData<string>());
		}
		if (sliceColorsDataSource != null) {
			sliceColors.SetList(sliceColorsDataSource.getData<Color>());
		}
		if (sliceValuesDataSource != null || sliceLabelsDataSource != null || sliceColorsDataSource != null) {
			if (sortBy != sortMethod.None) sortData();
		}
	}

	void ResizeChanged() {
		UpdateFromContainer();
		UpdateVisuals(true);
	}

	void DoughtnutChanged() {
		if (useDoughnut) {
			initDoughnut();
			UpdateDoughnut();
		}
	}

	void InteractivityChanged() {
		if (interactivityEnabled) {
			explodeSymmetrical = false;
		}
		for (int i = 0; i < slices.Count; i++) {
			WMG_Pie_Graph_Slice pieSlice = slices[i].GetComponent<WMG_Pie_Graph_Slice>();
			WMG_Raycatcher pieSliceRaycatcher = pieSlice.objectToColor.GetComponent<WMG_Raycatcher>();
			if (pieSliceRaycatcher != null && !interactivityEnabled) {
				Destroy(pieSliceRaycatcher);
			}
			if (pieSliceRaycatcher == null && interactivityEnabled) {
				pieSlice.objectToColor.AddComponent<WMG_Raycatcher>();
			}
			if (interactivityEnabled) {
				setAsNotInteractible(pieSlice.objectToLabel);
			}
		}

		Canvas canvas = null;
		getFirstCanvasOnSelfOrParent(this.transform, ref canvas);
		if (!canvas) {
			Debug.LogError("No Canvas found for Pie Graph");
			return;
		}

		WMG_Raycaster canvasRaycaster = canvas.GetComponent<WMG_Raycaster>();
		if (canvasRaycaster == null) {
			if (interactivityEnabled) {
				canvas.gameObject.AddComponent<WMG_Raycaster>();
			}
		}
		else {
			if (!interactivityEnabled) {
				DestroyImmediate(canvasRaycaster);
				addRaycaster(canvas.gameObject);
			}
		}


	}

	void GraphChanged() {
		if (!isAnimating) UpdateVisuals(false);
	}

	void AllChanged() {
		if (useDoughnut) {
			initDoughnut();
			UpdateDoughnut();
		}
		InteractivityChanged();
		if (!isAnimating) UpdateVisuals(false);
	}

	void sliceValuesChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref sliceValues, ref _sliceValues, oneValChanged, index);
		graphC.Changed ();
	}

	void sliceLabelsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref sliceLabels, ref _sliceLabels, oneValChanged, index);
		graphC.Changed ();
	}

	void sliceColorsChanged(bool editorChange, bool countChanged, bool oneValChanged, int index) {
		WMG_Util.listChanged (editorChange, ref sliceColors, ref _sliceColors, oneValChanged, index);
		graphC.Changed ();
	}

	void setOriginalPropertyValues() {
		origPieSize = pieSize;
		origSliceLabelExplodeLength = sliceLabelExplodeLength;
		origSliceLabelFontSize = sliceLabelFontSize;
		origAutoCenterPadding = autoCenterMinPadding;
	}

	void UpdateFromContainer() {
		if (resizeEnabled) {
			// Adjust background padding for percentage padding, otherwise keep padding in pixels

			float sizeFactor = pieSize / origPieSize;

			// Labels
			if ((resizeProperties & ResizeProperties.LabelExplodeLength) == ResizeProperties.LabelExplodeLength) {
				sliceLabelExplodeLength = getNewResizeVariable(sizeFactor, origSliceLabelExplodeLength);
			}
			if ((resizeProperties & ResizeProperties.LabelFontSize) == ResizeProperties.LabelFontSize) {
				sliceLabelFontSize = Mathf.RoundToInt(getNewResizeVariable(sizeFactor, origSliceLabelFontSize));
			}

			// Legend
			if ((resizeProperties & ResizeProperties.LegendFontSize) == ResizeProperties.LegendFontSize) {
				legend.legendEntryFontSize = Mathf.RoundToInt(getNewResizeVariable(sizeFactor, legend.origLegendEntryFontSize));
			}
			if ((resizeProperties & ResizeProperties.LegendEntrySize) == ResizeProperties.LegendEntrySize) {
				if (!legend.setWidthFromLabels) {
					legend.legendEntryWidth = getNewResizeVariable(sizeFactor, legend.origLegendEntryWidth);
				}
				legend.legendEntryHeight = getNewResizeVariable(sizeFactor, legend.origLegendEntryHeight);
			}
			if ((resizeProperties & ResizeProperties.LegendSwatchSize) == ResizeProperties.LegendSwatchSize) {
				legend.pieSwatchSize = getNewResizeVariable(sizeFactor, legend.origPieSwatchSize);
			}
			if ((resizeProperties & ResizeProperties.LegendOffset) == ResizeProperties.LegendOffset) {
				legend.offset = getNewResizeVariable(sizeFactor, legend.origOffset);
			}

			// Others
			if ((resizeProperties & ResizeProperties.AutoCenterPadding) == ResizeProperties.AutoCenterPadding) {
				autoCenterMinPadding = Mathf.RoundToInt(getNewResizeVariable(sizeFactor, origAutoCenterPadding));
			}
		}
	}

	float getNewResizeVariable(float sizeFactor, float variable) {
		return variable + ((sizeFactor - 1) * variable) / 2;
	}

	public void updateBG(int thePieSize) {
		changeSpriteSize(backgroundCircle, Mathf.RoundToInt(thePieSize + bgCircleOffset), Mathf.RoundToInt(thePieSize + bgCircleOffset));
		Vector2 offset = getPaddingOffset();
		changeSpritePositionTo(slicesParent, new Vector3(-offset.x, -offset.y));
	}

	Vector2 getPaddingOffset() {
		Vector2 pivot = getSpritePivot(this.gameObject);
		float offX = Mathf.RoundToInt(getSpriteWidth(this.gameObject)) * (pivot.x - 0.5f);
		float offY = Mathf.RoundToInt(getSpriteHeight(this.gameObject)) * (pivot.y - 0.5f);
		return new Vector2(-leftRightPadding.x * 0.5f + leftRightPadding.y * 0.5f + offX, topBotPadding.x * 0.5f - topBotPadding.y * 0.5f + offY);
	}

	void UpdateData() {
		// Find the total number of slices
		isOtherSlice = false;
		numSlices = sliceValues.Count;
		if (limitNumberSlices) {
			if (numSlices > maxNumberSlices) {
				numSlices = maxNumberSlices;
				if (includeOthers) {
					isOtherSlice = true;
					numSlices++;
				}
			}
		}
		
		// Find Other Slice Value and Total Value
		otherSliceValue = 0;
		totalVal = 0;
		for (int i = 0; i < sliceValues.Count; i++) {
			totalVal += sliceValues[i];
			if (isOtherSlice && i >= maxNumberSlices) {
				otherSliceValue += sliceValues[i];
			}
			if (limitNumberSlices && !isOtherSlice && i >= maxNumberSlices) {
				totalVal -= sliceValues[i];
			}
		}
	}

	void CreateOrDeleteSlicesBasedOnValues() {
		LabelToSliceMap.Clear ();
		// Create pie slices based on sliceValues data
		for (int i = 0; i < numSlices; i++) {
			if (sliceLabels.Count <= i) sliceLabels.Add("");
			if (sliceColors.Count <= i) sliceColors.Add(Color.white);
			if (slices.Count <= i) {
				GameObject curObj = CreateNode(nodePrefab, slicesParent);
				slices.Add(curObj);
				WMG_Pie_Graph_Slice pieSlice = curObj.GetComponent<WMG_Pie_Graph_Slice>();
				pieSlice.pieRef = this;
				pieSlice.sliceIndex = i;
				if (useDoughnut) {
					setTexture(pieSlice.objectToColor, pieSprite);
					setTexture(pieSlice.objectToMask, pieSprite);
				}
				if (interactivityEnabled) {
					pieSlice.objectToColor.AddComponent<WMG_Raycatcher>();
					setAsNotInteractible(pieSlice.objectToLabel);
				}
				addPieSliceMouseEnterEvent(pieSlice.objectToColor);
				addPieSliceClickEvent(pieSlice.objectToColor);
			}
			if (legend.legendEntries.Count <= i) {
				WMG_Legend_Entry legendEntry = legend.createLegendEntry(legendEntryPrefab);
				addPieLegendEntryClickEvent(legendEntry.gameObject);
			}
		}
		for (int i = slices.Count - 1; i >= 0; i--) {
			if (slices[i] != null && i >= numSlices) {
				WMG_Pie_Graph_Slice theSlice = slices[i].GetComponent<WMG_Pie_Graph_Slice>();
				DeleteNode(theSlice);
				slices.RemoveAt(i);
			}
		}
		
		// If there are more sliceLegendEntries or slices than sliceValues data, delete the extras
		for (int i = legend.legendEntries.Count - 1; i >= 0; i--) {
			if (legend.legendEntries[i] != null && i >= numSlices) {
				legend.deleteLegendEntry(i);
			}
		}
	}
	
	void UpdateVisuals(bool noAnim) {
		// Update internal bookkeeping variables
		UpdateData();

		// Creates and deletes slices and slice legend objects based on the slice values
		CreateOrDeleteSlicesBasedOnValues();

		if (totalVal == 0 && numSlices > 0) return; // all values are 0, or mixed negative and positive values

		// Update explode symmetrical
		for (int i = 0; i < numSlices; i++) {
			WMG_Pie_Graph_Slice pieSlice =  slices[i].GetComponent<WMG_Pie_Graph_Slice>();
			SetActive(pieSlice.objectToMask, explodeSymmetrical);
			if (explodeSymmetrical) {
				changeSpriteParent(pieSlice.objectToColor, pieSlice.objectToMask);
			}
			else {
				changeSpriteParent(pieSlice.objectToColor, pieSlice.gameObject);
				bringSpriteToFront(pieSlice.objectToLabel);
			}
		}

		int thePieSize = Mathf.RoundToInt(pieSize);

		updateBG(thePieSize);

		if (animationDuration == 0 && sortBy != sortMethod.None) sortData();
		float curTotalRot = 0;
		if (!noAnim) animSortSwap = false; // Needed because if sortAnimationDuration = 0, nothing sets animSortSwap to false
		for (int i = 0; i < numSlices; i++) {
			// Update Pie Slices
			float newAngle =  -1 * curTotalRot;
			if (newAngle < 0) newAngle += 360;
			WMG_Pie_Graph_Slice pieSlice =  slices[i].GetComponent<WMG_Pie_Graph_Slice>();
			if (sliceLabelType != WMG_Enums.labelTypes.None && !activeInHierarchy(pieSlice.objectToLabel)) SetActive(pieSlice.objectToLabel,true);
			if (sliceLabelType == WMG_Enums.labelTypes.None && activeInHierarchy(pieSlice.objectToLabel)) SetActive(pieSlice.objectToLabel,false);

			if (!explodeSymmetrical) {
				changeSpriteSize(pieSlice.objectToColor, thePieSize, thePieSize);
			}
			else {
				changeSpriteSize(pieSlice.objectToColor, thePieSize, thePieSize);
				changeSpriteSize(pieSlice.objectToMask, 
				                 thePieSize + Mathf.RoundToInt(explodeLength*4), 
				                 thePieSize + Mathf.RoundToInt(explodeLength*4));
			}

			// Set Slice Data and maybe Other Slice Data
			Color sliceColor = sliceColors[i];
			string sliceLabel = sliceLabels[i];
			float sliceValue = sliceValues[i];
			if (isOtherSlice && i == numSlices - 1) {
				sliceColor = includeOthersColor;
				sliceLabel = includeOthersLabel;
				sliceValue = otherSliceValue;
			}
			if (!LabelToSliceMap.ContainsKey(sliceLabel)) LabelToSliceMap.Add(sliceLabel, pieSlice);

			// Hide if 0
			if (sliceValue == 0) {
				SetActive(pieSlice.objectToLabel, false);
			}

			float slicePercent = sliceValue / totalVal;
			pieSlice.slicePercent = slicePercent * 360;
			float afterExplodeAngle = newAngle * -1 + 0.5f * slicePercent * 360;
			float sliceLabelRadius = sliceLabelExplodeLength + thePieSize / 2;
			float sin = Mathf.Sin(afterExplodeAngle * Mathf.Deg2Rad);
			float cos = Mathf.Cos(afterExplodeAngle * Mathf.Deg2Rad);

			if (!noAnim && animationDuration > 0) {
				isAnimating = true;
				WMG_Anim.animFill(pieSlice.objectToColor, animationDuration, Ease.Linear, slicePercent);
				WMG_Anim.animPosition(pieSlice.objectToLabel, animationDuration, Ease.Linear, new Vector3(sliceLabelRadius * sin, 
				                                                                                              sliceLabelRadius * cos));
				int newI = i;
				WMG_Anim.animPositionCallbackC(slices[i], animationDuration, Ease.Linear, new Vector3(explodeLength * sin, 
				                                                                                          explodeLength * cos), ()=> shrinkSlices(newI));
				if (!explodeSymmetrical) {
					WMG_Anim.animRotation(pieSlice.objectToColor, animationDuration, Ease.Linear, new Vector3(0, 0, newAngle), false);
					WMG_Anim.animPosition(pieSlice.objectToColor, animationDuration, Ease.Linear, Vector3.zero);
				}
				else {
					WMG_Anim.animRotation(pieSlice.objectToColor, animationDuration, Ease.Linear, Vector3.zero, false);
					Vector2 newPos = new Vector2(-explodeLength * sin, -explodeLength * cos);
					float sin2 = Mathf.Sin(newAngle * Mathf.Deg2Rad);
					float cos2 = Mathf.Cos(newAngle * Mathf.Deg2Rad);
					WMG_Anim.animPosition(pieSlice.objectToColor, animationDuration, Ease.Linear, new Vector3( cos2 * newPos.x + sin2 * newPos.y, cos2 * newPos.y - sin2 * newPos.x));
					// Mask
					WMG_Anim.animRotation(pieSlice.objectToMask, animationDuration, Ease.Linear, new Vector3(0, 0, newAngle), false);
					WMG_Anim.animFill(pieSlice.objectToMask, animationDuration, Ease.Linear, slicePercent);
				}
			}
			else {
				changeSpriteFill(pieSlice.objectToColor, slicePercent);
				pieSlice.objectToLabel.transform.localPosition = new Vector3(sliceLabelRadius * sin, 
				                                                             sliceLabelRadius * cos);
				slices[i].transform.localPosition =  new Vector3(explodeLength * sin, 
				                                                 explodeLength * cos);
				if (!explodeSymmetrical) {
					pieSlice.objectToColor.transform.localEulerAngles = new Vector3(0, 0, newAngle);
					pieSlice.objectToColor.transform.localPosition = Vector3.zero;
				}
				else {
					pieSlice.objectToColor.transform.localEulerAngles = Vector3.zero;
					Vector2 newPos = new Vector2(-explodeLength * sin, -explodeLength * cos);
					float sin2 = Mathf.Sin(newAngle * Mathf.Deg2Rad);
					float cos2 = Mathf.Cos(newAngle * Mathf.Deg2Rad);
					pieSlice.objectToColor.transform.localPosition = new Vector3( cos2 * newPos.x + sin2 * newPos.y, cos2 * newPos.y - sin2 * newPos.x);
					// Mask
					pieSlice.objectToMask.transform.localEulerAngles = new Vector3(0, 0, newAngle);
					changeSpriteFill(pieSlice.objectToMask, slicePercent);
				}
			}

			// Update slice color
			changeSpriteColor(pieSlice.objectToColor, sliceColor);
			changeSpriteColor(pieSlice.objectToMask, sliceColor);

			// Update slice labels
			changeLabelText(pieSlice.objectToLabel, WMG_Util.FormatValueLabel(sliceLabel, sliceLabelType, sliceValue, slicePercent, numberDecimalsInPercents));
			changeLabelFontSize(pieSlice.objectToLabel, sliceLabelFontSize);
			changeSpriteColor(pieSlice.objectToLabel, sliceLabelColor);

			// Update Gameobject names
			slices[i].name = sliceLabel;
			legend.legendEntries[i].name = sliceLabel;

			curTotalRot += slicePercent * 360;

			pieSlice.slicePercentPosition = curTotalRot - pieSlice.slicePercent/2;

			// Update legend
			WMG_Legend_Entry entry = legend.legendEntries[i];
			changeLabelText(entry.label, WMG_Util.FormatValueLabel(sliceLabel, legend.labelType, sliceValue, slicePercent, legend.numDecimals));
			changeSpriteColor(entry.swatchNode, sliceColor);
			// Hide legend if 0
			if (hideZeroValueLegendEntry) {
				if (sliceValue == 0) {
					SetActive(entry.gameObject, false);
				}
				else {
					SetActive(entry.gameObject, true);
				}
			}
			else {
				SetActive(entry.gameObject, true);
			}

		}
		legend.updateLegend ();

		updateAutoCenter ();

		if (!setOrig) {
			setOrig = true;
			setOriginalPropertyValues();
		}
	}

	void updateAutoCenter() {
		if (autoCenter) {
			float newPadding = autoCenterMinPadding + explodeLength + bgCircleOffset/2;
			if (legend.hideLegend) {
				leftRightPadding = new Vector2(newPadding, newPadding);
				topBotPadding = new Vector2(newPadding, newPadding);
			}
			else {
				if (legend.legendType == WMG_Legend.legendTypes.Right) {
					topBotPadding = new Vector2(newPadding, newPadding);
					if (legend.oppositeSideLegend) {
						leftRightPadding = new Vector2(newPadding + legend.LegendWidth + Mathf.Abs(legend.offset), newPadding);
					}
					else {
						leftRightPadding = new Vector2(newPadding, newPadding + legend.LegendWidth + Mathf.Abs(legend.offset));
					}
				}
				else {
					leftRightPadding = new Vector2(newPadding, newPadding);
					if (!legend.oppositeSideLegend) {
						topBotPadding = new Vector2(newPadding, newPadding + legend.LegendHeight + Mathf.Abs(legend.offset));
					}
					else {
						topBotPadding = new Vector2(newPadding + legend.LegendHeight + Mathf.Abs(legend.offset), newPadding);
					}
				}
			}
		}
	}

	void shrinkSlices(int sliceNum) {
		if (!animSortSwap && sortBy != sortMethod.None) animSortSwap = sortData();
		if (animSortSwap) {
			if (sortAnimationDuration > 0) {
				WMG_Anim.animScaleCallbackC(slices[sliceNum], sortAnimationDuration / 2, Ease.Linear, Vector3.zero, ()=> enlargeSlices(sliceNum));
			}
			else {
				isAnimating = false;
				UpdateVisuals(true);
			}
		}
		else {
			isAnimating = false;
		}
	}

	void enlargeSlices(int sliceNum) {
		if (sliceNum == 0) {
			UpdateVisuals(true);
		}
		WMG_Anim.animScaleCallbackC(slices[sliceNum], sortAnimationDuration / 2, Ease.Linear, Vector3.one, ()=> endSortAnimating(sliceNum));
	}

	void endSortAnimating(int sliceNum) {
		if (sliceNum == numSlices - 1) {
			animSortSwap = false;
			isAnimating = false;
		}
	}
	
	bool sortData() {
		bool wasASwap = false;
		bool flag = true;
		bool shouldSwap = false;
		float temp;
		string tempL;
		GameObject tempGo;
		int numLength = numSlices;
		for (int i = 1; (i <= numLength) && flag; i++) {
			flag = false;
			for (int j = 0; j < (numLength - 1); j++ ) {
				shouldSwap = false;
				if (sortBy == sortMethod.Largest_First) {
					if (sliceValues[j+1] > sliceValues[j]) shouldSwap = true;
				}
				else if (sortBy == sortMethod.Smallest_First) {
					if (sliceValues[j+1] < sliceValues[j]) shouldSwap = true;
				}
				else if (sortBy == sortMethod.Alphabetically) {
					if (sliceLabels[j+1].CompareTo(sliceLabels[j]) == -1) shouldSwap = true;
				}
				else if (sortBy == sortMethod.Reverse_Alphabetically) {
					if (sliceLabels[j+1].CompareTo(sliceLabels[j]) == 1) shouldSwap = true;
				}
				if (shouldSwap) {
					// Swap values
					temp = sliceValues[j];
					sliceValues.SetValNoCb(j, sliceValues[j+1], ref _sliceValues);
					sliceValues.SetValNoCb(j+1, temp, ref _sliceValues);
					// Swap labels
					tempL = sliceLabels[j];
					sliceLabels.SetValNoCb(j, sliceLabels[j+1], ref _sliceLabels);
					sliceLabels.SetValNoCb(j+1, tempL, ref _sliceLabels);
					// Swap Slices
					tempGo = slices[j];
					slices[j].GetComponent<WMG_Pie_Graph_Slice>().sliceIndex = j+1;
					slices[j] = slices[j+1];
					slices[j+1].GetComponent<WMG_Pie_Graph_Slice>().sliceIndex = j;
					slices[j+1] = tempGo;

					// Swap Colors
					if (swapColorsDuringSort) {
						Color tempC = sliceColors[j];
						sliceColors.SetValNoCb(j, sliceColors[j+1], ref _sliceColors);
						sliceColors.SetValNoCb(j+1, tempC, ref _sliceColors);
					}
					flag = true;
					wasASwap = true;
				}
			}
		}
		return wasASwap;
	}

	void UpdateDoughnut() {
		WMG_Util.updateBandColors (ref colors, pieSize, doughnutPercentage*pieSize/2, pieSize/2, true, 2, origColors);
		pieSprite.texture.SetPixels(colors);
		pieSprite.texture.Apply();
	}

	/// <summary>
	/// Given a pie slice (identified by its label string), and floating point amount of space, return a new pie slice position that can be used 
	/// to animate pie slices outwards during for example a mouse click event.
	/// </summary>
	/// <returns>The callout slice position.</returns>
	/// <param name="label">Label.</param>
	/// <param name="amt">Amt.</param>
	public Vector3 getCalloutSlicePosition(string label, float amt) {
		if (LabelToSliceMap.ContainsKey (label)) {
			return getPositionFromExplode(LabelToSliceMap[label], amt);
		}
		return Vector3.zero;
	}

	/// <summary>
	/// Given a pie slice, and floating point amount of space, return a new pie slice position that can be used 
	/// to animate pie slices outwards during for example a mouse click event.
	/// </summary>
	/// <returns>The position from explode.</returns>
	/// <param name="slice">Slice.</param>
	/// <param name="amt">Amt.</param>
	public Vector3 getPositionFromExplode(WMG_Pie_Graph_Slice slice, float amt) {
		float angle = Mathf.Deg2Rad * (-slice.slicePercentPosition + 90);
		return new Vector3(amt * Mathf.Cos(angle), amt * Mathf.Sin(angle), 0);
	}
}
