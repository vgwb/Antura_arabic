using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WMG_Hierarchical_Tree : WMG_Graph_Manager {

	[System.Flags]
	public enum ResizeProperties {
		NodeWidthHeight 		= 1 << 0,
		NodeRadius		 		= 1 << 1
	}

	public ResizeProperties resizeProperties { get {return _resizeProperties;} 
		set {
			if (_resizeProperties != value) {
				_resizeProperties = value;
				resizeC.Changed();
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

	public GameObject nodeParent;
	public GameObject linkParent;
	
	public Object defaultNodePrefab;
	public Object linkPrefab;
	
	public int numNodes;
	public int numLinks;
	
	public List<Object> nodePrefabs;
	public List<int> nodeColumns;
	public List<int> nodeRows;
	public List<int> linkNodeFromIDs;
	public List<int> linkNodeToIDs;
	
	public Object invisibleNodePrefab;
	public int numInvisibleNodes;
	public List<int> invisibleNodeColumns;
	public List<int> invisibleNodeRows;

	// Determines size of the nodes. The nodeRadius and squareNodes determine how the links connect to the nodes.
	public int nodeWidthHeight { get {return _nodeWidthHeight;} 
		set {
			if (_nodeWidthHeight != value) {
				_nodeWidthHeight = value;
				treeC.Changed();
			}
		}
	}
	public float nodeRadius { get {return _nodeRadius;} 
		set {
			if (_nodeRadius != value) {
				_nodeRadius = value;
				treeC.Changed();
			}
		}
	}
	public bool squareNodes { get {return _squareNodes;} 
		set {
			if (_squareNodes != value) {
				_squareNodes = value;
				treeC.Changed();
			}
		}
	}

	private float _gridLengthX;
	private float _gridLengthY;
	[SerializeField] private int _nodeWidthHeight;
	[SerializeField] private float _nodeRadius;
	[SerializeField] private bool _squareNodes;
	[SerializeField] private bool _resizeEnabled;
	[WMG_EnumFlagAttribute] [SerializeField] private ResizeProperties _resizeProperties;


	private float cachedContainerWidth;
	private float cachedContainerHeight;

	// Original property values for use with dynamic resizing
	private float origWidth;
	private float origHeight;
	private int origNodeWidthHeight;
	private float origNodeRadius;


	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();
	private WMG_Change_Obj treeC = new WMG_Change_Obj();
	public WMG_Change_Obj resizeC = new WMG_Change_Obj();

	private List<GameObject> treeNodes = new List<GameObject>();
	private List<GameObject> treeLinks = new List<GameObject>();
	private List<GameObject> treeInvisibleNodes = new List<GameObject>();

	private bool hasInit;
	
	void Start() {
		Init ();
		PauseCallbacks();
		CreateNodes();
		CreatedLinks();
		treeC.Changed();
	}

	public void Init() {
		if (hasInit) return;
		hasInit = true;
		
		changeObjs.Add(treeC);
		changeObjs.Add(resizeC);

		treeC.OnChange += refresh;
		resizeC.OnChange += ResizeChanged;

		setOriginalPropertyValues();
		PauseCallbacks();
	}

	void Update() {
		updateFromResize();
		Refresh();
	}

	public void setOriginalPropertyValues() {
		cachedContainerWidth = getSpriteWidth(this.gameObject);
		cachedContainerHeight = getSpriteHeight(this.gameObject);
		origWidth = getSpriteWidth(this.gameObject);
		origHeight = getSpriteHeight(this.gameObject);
		origNodeWidthHeight = nodeWidthHeight;
		origNodeRadius = nodeRadius;
	}

	void ResizeChanged() {
		UpdateFromContainer();
	}

	void UpdateFromContainer() {
		if (resizeEnabled) {
			Vector2 percentFactor = new Vector2(cachedContainerWidth / origWidth, cachedContainerHeight / origHeight);
			float smallerFactor = percentFactor.x;
			if (percentFactor.y < smallerFactor) smallerFactor = percentFactor.y;
			if ((resizeProperties & ResizeProperties.NodeWidthHeight) == ResizeProperties.NodeWidthHeight) {
				nodeWidthHeight = Mathf.RoundToInt(getNewResizeVariable(smallerFactor, origNodeWidthHeight));
			}
			if ((resizeProperties & ResizeProperties.NodeRadius) == ResizeProperties.NodeRadius) {
				nodeRadius = getNewResizeVariable(smallerFactor, origNodeRadius);
			}
		}
	}

	private float getNewResizeVariable(float sizeFactor, float variable) {
		return variable + ((sizeFactor - 1) * variable);
	}

	void updateFromResize() {
		bool resizeChanged = false;
		updateCacheAndFlag<float>(ref cachedContainerWidth, getSpriteWidth(this.gameObject), ref resizeChanged);
		updateCacheAndFlag<float>(ref cachedContainerHeight, getSpriteHeight(this.gameObject), ref resizeChanged);
		if (resizeChanged) {
			treeC.Changed();
			resizeC.Changed();
		}
	}

	
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

	public List<GameObject> getNodesInRow(int rowNum) {
		List<GameObject> returnList = new List<GameObject>();
		for (int i = 0; i < treeNodes.Count; i++) {
			if (Mathf.Approximately(getSpritePositionY(treeNodes[i]), -rowNum*_gridLengthY)) returnList.Add(treeNodes[i]);
		}
		return returnList;
	}
	
	public List<GameObject> getNodesInColumn(int colNum) {
		List<GameObject> returnList = new List<GameObject>();
		for (int i = 0; i < treeNodes.Count; i++) {
			if (Mathf.Approximately(getSpritePositionX(treeNodes[i]), colNum*_gridLengthX)) returnList.Add(treeNodes[i]);
		}
		return returnList;
	}
	
	void refresh() {
		int maxNumRows = -1;
		int maxNumCols = -1;
		for (int i = 0; i < treeNodes.Count; i++) {
			if (nodeRows[i] > maxNumRows) {
				maxNumRows = nodeRows[i];
			}
			if (nodeColumns[i] > maxNumCols) {
				maxNumCols = nodeColumns[i];
			}
		}
		Vector2 rectSize = new Vector2(getSpriteWidth(this.gameObject), getSpriteHeight(this.gameObject));
		_gridLengthX = (rectSize.x / maxNumCols);
		_gridLengthY = (rectSize.y / maxNumRows);
		// Update node positions
		for (int i = 0; i < treeNodes.Count; i++) {
			changeSpriteSize(treeNodes[i], nodeWidthHeight, nodeWidthHeight);
			treeNodes[i].GetComponent<WMG_Node>().radius = nodeRadius;
			treeNodes[i].GetComponent<WMG_Node>().isSquare = squareNodes;
			float xPos = (nodeColumns[i]-0.5f)*_gridLengthX - rectSize.x / 2;
			float yPos = (nodeRows[i]-0.5f)*_gridLengthY - rectSize.y / 2;
			treeNodes[i].GetComponent<WMG_Node>().Reposition(xPos, -yPos);
		}
		// Update invisible node positions
		for (int i = 0; i < treeInvisibleNodes.Count; i++) {
			changeSpritePivot(treeInvisibleNodes[i],WMG_GUI_Functions.WMGpivotTypes.Center);
			changeSpriteSize(treeInvisibleNodes[i], nodeWidthHeight, nodeWidthHeight);
			float xPos = (invisibleNodeColumns[i]-0.5f)*_gridLengthX - rectSize.x / 2;
			float yPos = (invisibleNodeRows[i]-0.5f)*_gridLengthY - rectSize.y / 2;
			treeInvisibleNodes[i].GetComponent<WMG_Node>().Reposition(xPos, -yPos);
		}
	}
	
	public void CreateNodes() {
		// Create nodes based on numNodes
		for (int i = 0; i < numNodes; i++) {
			if (treeNodes.Count <= i) {
				Object nodePrefab = defaultNodePrefab;
				if (nodePrefabs.Count > i) nodePrefab = nodePrefabs[i];
				WMG_Node curNode = CreateNode(nodePrefab, nodeParent).GetComponent<WMG_Node>();
				treeNodes.Add(curNode.gameObject);
			}
		}
		// Create invisible nodes
		for (int i = 0; i < numInvisibleNodes; i++) {
			if (treeInvisibleNodes.Count <= i) {
				WMG_Node curNode = CreateNode(invisibleNodePrefab, nodeParent).GetComponent<WMG_Node>();
				treeInvisibleNodes.Add(curNode.gameObject);
			}
		}
	}
	
	public void CreatedLinks() {
		// Create links based on numLinks
		for (int i = 0; i < numLinks; i++) {
			if (treeLinks.Count <= i) {
				GameObject fromNode = null;
				if (linkNodeFromIDs[i] > 0) { // Regular node
					fromNode = treeNodes[linkNodeFromIDs[i]-1];
				}
				else { // Invisible node
					fromNode = treeInvisibleNodes[-linkNodeFromIDs[i]-1];
				}
				GameObject toNode = null;
				if (linkNodeToIDs[i] > 0) { // Regular node
					toNode = treeNodes[linkNodeToIDs[i]-1];
				}
				else { // Invisible node
					toNode = treeInvisibleNodes[-linkNodeToIDs[i]-1];
				}
				treeLinks.Add(CreateLinkNoRepos(fromNode.GetComponent<WMG_Node>(), toNode, linkPrefab, linkParent));
			}
		}
	}
}
