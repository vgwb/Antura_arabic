using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A class used for creating hierarchical tree type graphs.
/// </summary>
public class WMG_Hierarchical_Tree : WMG_Graph_Manager {

	[System.Flags]
	public enum ResizeProperties {
		NodeWidthHeight 		= 1 << 0,
		NodeRadius		 		= 1 << 1
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
	/// The parent of all the nodes.
	/// </summary>
	public GameObject nodeParent;
	/// <summary>
	/// The parent of all the links.
	/// </summary>
	public GameObject linkParent;
	/// <summary>
	/// The default node prefab used to create all nodes, which is used unless #nodePrefabs is populated.
	/// </summary>
	public Object defaultNodePrefab;
	/// <summary>
	/// The prefab used to create all links..
	/// </summary>
	public Object linkPrefab;
	/// <summary>
	/// The total number of nodes in this tree (excluding invisible nodes).
	/// </summary>
	public int numNodes;
	/// <summary>
	/// The total number of links in this tree.
	/// </summary>
	public int numLinks;
	/// <summary>
	/// Can be used to specify individual prefabs for each node.
	/// </summary>
	public List<Object> nodePrefabs;
	/// <summary>
	/// Each element in this list represents a node (excluding invisible nodes), and this value is the node's column position.
	/// </summary>
	public List<int> nodeColumns;
	/// <summary>
	/// Each element in this list represents a node (excluding invisible nodes), and this value is the node's row position.
	/// </summary>
	public List<int> nodeRows;
	/// <summary>
	/// This is the list of links and each link's from node. The ID here corresponds to the element in the #nodeColumns list. 
	/// For example node column element 0 is node ID 1. This list also applies for invisible nodes, however the node IDs will be denoted by a negative number. 
	/// </summary>
	public List<int> linkNodeFromIDs;
	/// <summary>
	/// Same as #linkNodeFromIDs, except this is the to node or the ending point for the links.
	/// </summary>
	public List<int> linkNodeToIDs;
	/// <summary>
	/// The invisible node prefab.
	/// </summary>
	public Object invisibleNodePrefab;
	/// <summary>
	/// The number of invisible nodes.
	/// </summary>
	public int numInvisibleNodes;
	/// <summary>
	/// Invisible nodes are used to create angled links where no node appears. This is the invisible node equivalent of #nodeColumns.
	/// </summary>
	public List<int> invisibleNodeColumns;
	/// <summary>
	/// Invisible nodes are used to create angled links where no node appears. This is the invisible node equivalent of #nodeRows.
	/// </summary>
	public List<int> invisibleNodeRows;
	/// <summary>
	/// Determines the width and height of all nodes.
	/// </summary>
	public int nodeWidthHeight { get {return _nodeWidthHeight;} 
		set {
			if (_nodeWidthHeight != value) {
				_nodeWidthHeight = value;
				treeC.Changed();
			}
		}
	}
	/// <summary>
	/// This is the radius of all circle nodes or half the width / height of all square nodes. This determine the starting and ending points for the links.
	/// </summary>
	/// <value>The node radius.</value>
	public float nodeRadius { get {return _nodeRadius;} 
		set {
			if (_nodeRadius != value) {
				_nodeRadius = value;
				treeC.Changed();
			}
		}
	}
	/// <summary>
	/// This sets a boolean on all nodes to tell whether or not the node is represented as a square instead of a circle. 
	/// Square nodes will have the effect of making the link start and end points be based on the square's edge.
	/// </summary>
	/// <value><c>true</c> if square nodes; otherwise, <c>false</c>.</value>
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
		CreateLinks();
		treeC.Changed();
	}

	/// <summary>
	/// Initializes this graph, and should be called before anything else.
	/// </summary>
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

	/// <summary>
	/// Caches initial property values that are used as the basis for #resizeEnabled functionality, called automatically during #Init. 
	/// </summary>
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

	float getNewResizeVariable(float sizeFactor, float variable) {
		return variable + ((sizeFactor - 1) * variable);
	}

	void updateFromResize() {
		bool resizeChanged = false;
		WMG_Util.updateCacheAndFlag<float>(ref cachedContainerWidth, getSpriteWidth(this.gameObject), ref resizeChanged);
		WMG_Util.updateCacheAndFlag<float>(ref cachedContainerHeight, getSpriteHeight(this.gameObject), ref resizeChanged);
		if (resizeChanged) {
			treeC.Changed();
			resizeC.Changed();
		}
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

	/// <summary>
	/// Gets the nodes in the specified row.
	/// </summary>
	/// <returns>The nodes in row.</returns>
	/// <param name="rowNum">Row number.</param>
	public List<GameObject> getNodesInRow(int rowNum) {
		List<GameObject> returnList = new List<GameObject>();
		for (int i = 0; i < treeNodes.Count; i++) {
			if (Mathf.Approximately(getSpritePositionY(treeNodes[i]), -rowNum*_gridLengthY)) returnList.Add(treeNodes[i]);
		}
		return returnList;
	}

	/// <summary>
	/// Gets the nodes in the specified column.
	/// </summary>
	/// <returns>The nodes in column.</returns>
	/// <param name="colNum">Col number.</param>
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
	
	void CreateNodes() {
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
	
	void CreateLinks() {
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
