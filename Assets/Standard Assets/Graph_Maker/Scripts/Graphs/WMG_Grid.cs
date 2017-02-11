using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The grid class used for creating quadrilateral or hexagonal grids.
/// </summary>
public class WMG_Grid : WMG_Graph_Manager {

	/// <summary>
	/// Automatically refreshes the grid, if a parameter has changed from its previously cached value.
	/// </summary>
	public bool autoRefresh = true;
	
	public enum gridTypes {quadrilateral, hexagonal_flat_top, hexagonal_flat_side};
	/// <summary>
	/// Determines whether a grid is quadrilateral or hexagonal.
	/// </summary>
	public gridTypes gridType;
	/// <summary>
	/// The prefab used to instantiate nodes of the grid.
	/// </summary>
	public Object nodePrefab;
	/// <summary>
	/// The prefab used to instantiate links of the grid.
	/// </summary>
	public Object linkPrefab;
	/// <summary>
	/// The number of nodes in the x dimension.
	/// </summary>
	public int gridNumNodesX;
	/// <summary>
	/// The number of nodes in the y dimension.
	/// </summary>
	public int gridNumNodesY;
	/// <summary>
	/// The amount of space between the nodes in the x direction.
	/// </summary>
	public float gridLinkLengthX;
	/// <summary>
	/// The amount of space between the nodes in the y direction.
	/// </summary>
	public float gridLinkLengthY;
	/// <summary>
	/// Whether or not links are created between nodes in the grid.
	/// </summary>
	public bool createLinks;
	/// <summary>
	/// If #createLinks = true, this determines whether links are only created in the horizontal direction.
	/// </summary>
	public bool noVerticalLinks;
	/// <summary>
	/// If #createLinks = true, this determines whether links are only created in the vertical direction.
	/// </summary>
	public bool noHorizontalLinks;
	/// <summary>
	/// The color of the links.
	/// </summary>
	public Color linkColor = Color.white;
	/// <summary>
	/// The width of the links.
	/// </summary>
	public int linkWidth;
	
	private List<List<WMG_Node>> gridNodesXY = new List<List<WMG_Node>>();
	private List<GameObject> gridLinks = new List<GameObject>();
	
	// cache
	private gridTypes cachedGridType;
	private Object cachedNodePrefab;
	private Object cachedLinkPrefab;
	private int cachedGridNumNodesX;
	private int cachedGridNumNodesY;
	private float cachedGridLinkLengthX;
	private float cachedGridLinkLengthY;
	private bool cachedCreateLinks;
	private bool cachedNoVerticalLinks;
	private bool cachedNoHorizontalLinks;
	private Color cachedLinkColor;
	private int cachedLinkWidth;
	private bool gridChanged = true;
	
	void Update () {
		if (autoRefresh) {
			Refresh();
		}
	}

	/// <summary>
	/// Refreshes this grid, if a grid property has changed from its previously cached value.
	/// </summary>
	public void Refresh() {
		checkCache();
		if (gridChanged) {
			refresh();
		}
		setCacheFlags(false);
	}
	
	void checkCache() {
		WMG_Util.updateCacheAndFlag<gridTypes>(ref cachedGridType, gridType, ref gridChanged);
		WMG_Util.updateCacheAndFlag<Object>(ref cachedNodePrefab, nodePrefab, ref gridChanged);
		WMG_Util.updateCacheAndFlag<Object>(ref cachedLinkPrefab, linkPrefab, ref gridChanged);
		WMG_Util.updateCacheAndFlag<int>(ref cachedGridNumNodesX, gridNumNodesX, ref gridChanged);
		WMG_Util.updateCacheAndFlag<int>(ref cachedGridNumNodesY, gridNumNodesY, ref gridChanged);
		WMG_Util.updateCacheAndFlag<float>(ref cachedGridLinkLengthX, gridLinkLengthX, ref gridChanged);
		WMG_Util.updateCacheAndFlag<float>(ref cachedGridLinkLengthY, gridLinkLengthY, ref gridChanged);
		WMG_Util.updateCacheAndFlag<bool>(ref cachedCreateLinks, createLinks, ref gridChanged);
		WMG_Util.updateCacheAndFlag<bool>(ref cachedNoVerticalLinks, noVerticalLinks, ref gridChanged);
		WMG_Util.updateCacheAndFlag<bool>(ref cachedNoHorizontalLinks, noHorizontalLinks, ref gridChanged);
		WMG_Util.updateCacheAndFlag<Color>(ref cachedLinkColor, linkColor, ref gridChanged);
		WMG_Util.updateCacheAndFlag<int>(ref cachedLinkWidth, linkWidth, ref gridChanged);
	}
	
	void setCacheFlags(bool val) {
		gridChanged = val;
	}

	/// <summary>
	/// Gets the specified column of nodes given a column index.
	/// </summary>
	/// <returns>The column.</returns>
	/// <param name="colNum">Col number.</param>
	public List<WMG_Node> getColumn(int colNum) {
		if (gridNodesXY.Count <= colNum) return new List<WMG_Node>();
		return gridNodesXY[colNum];
	}

	/// <summary>
	/// Sets active the specified column of nodes given a column index.
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	/// <param name="colNum">Col number.</param>
	public void setActiveColumn(bool active, int colNum) {
		if (gridNodesXY.Count <= colNum) return;
		for (int i = 0; i < gridNodesXY[colNum].Count; i++) {
			SetActive(gridNodesXY[colNum][i].gameObject,active);
		}
	}

	/// <summary>
	/// Gets the specified row of nodes given a row index.
	/// </summary>
	/// <returns>The row.</returns>
	/// <param name="rowNum">Row number.</param>
	public List<WMG_Node> getRow(int rowNum) {
		List<WMG_Node> returnResults = new List<WMG_Node>();
		for (int i = 0; i < gridNodesXY.Count; i++) {
			returnResults.Add(gridNodesXY[i][rowNum]);
		}
		return returnResults;
	}

	/// <summary>
	/// Sets active the specified row of nodes given a column index.
	/// </summary>
	/// <param name="active">If set to <c>true</c> active.</param>
	/// <param name="rowNum">Row number.</param>
	public void setActiveRow(bool active, int rowNum) {
		for (int i = 0; i < gridNodesXY.Count; i++) {
			SetActive(gridNodesXY[i][rowNum].gameObject,active);
		}
	}

	/// <summary>
	/// Gets all the nodes and links of this grid.
	/// </summary>
	/// <returns>The nodes and links.</returns>
	public List<GameObject> GetNodesAndLinks() {
		List<GameObject> returnResults = new List<GameObject>();
		for (int i = 0; i < gridNodesXY.Count; i++) {
			for (int j = 0; j < gridNodesXY[i].Count; j++) {
				returnResults.Add(gridNodesXY[i][j].gameObject);
			}
		}
		for (int i = 0; i < gridLinks.Count; i++) {
			returnResults.Add(gridLinks[i]);
		}
		return returnResults;
	}
	
	void refresh() {
		// Create nodes based on gridNumNodes
		for (int i = 0; i < gridNumNodesX; i++) {
			if (gridNodesXY.Count <= i) {
				List<WMG_Node> aList = new List<WMG_Node>();
				gridNodesXY.Add(aList);
				for (int j = 0; j < gridNumNodesY; j++) {
					WMG_Node curNode = CreateNode(nodePrefab, this.gameObject).GetComponent<WMG_Node>();
					gridNodesXY[i].Add(curNode);
				}
			}
		}
		for (int i = 0; i < gridNumNodesX; i++) {
			for (int j = 0; j < gridNumNodesY; j++) {
				if (gridNodesXY[i].Count <= j) {
					WMG_Node curNode = CreateNode(nodePrefab, this.gameObject).GetComponent<WMG_Node>();
					gridNodesXY[i].Add(curNode);
				}
			}
		}
		
		// Delete nodes based on gridNumNodes
		for (int i = 0; i < gridNumNodesX; i++) {
			for (int j = gridNodesXY[i].Count - 1; j >= 0; j--) {
				if (j >= gridNumNodesY) {
					DeleteNode(gridNodesXY[i][j]);
					gridNodesXY[i].RemoveAt(j);
				}
			}
		}
		for (int i = gridNodesXY.Count - 1; i >= 0; i--) {
			if (i >= gridNumNodesX) {
				for (int j = gridNumNodesY - 1; j >= 0; j--) {
					DeleteNode(gridNodesXY[i][j]);
					gridNodesXY[i].RemoveAt(j);
				}
				gridNodesXY.RemoveAt(i);
			}
		}
		
		// Create or delete links
		for (int i = 0; i < gridNumNodesX; i++) {
			for (int j = 0; j < gridNumNodesY; j++) {
				if (j + 1 < gridNumNodesY) {
					CreateOrDeleteLink(gridNodesXY[i][j], gridNodesXY[i][j+1], noVerticalLinks);
				}
				if (i + 1 < gridNumNodesX) {
					CreateOrDeleteLink(gridNodesXY[i][j], gridNodesXY[i+1][j], noHorizontalLinks);
					if (gridType == gridTypes.hexagonal_flat_top || gridType == gridTypes.hexagonal_flat_side) {
						if (i % 2 == 1) {
							if (j + 1 < gridNumNodesY) {
								CreateOrDeleteLink(gridNodesXY[i][j], gridNodesXY[i+1][j+1], noHorizontalLinks);
							}
						}
						else {
							if (j > 0) {
								CreateOrDeleteLink(gridNodesXY[i][j], gridNodesXY[i+1][j-1], noHorizontalLinks);
							}
						}
					}
					else if (gridType == gridTypes.quadrilateral) {
						if (i % 2 == 1) {
							if (j + 1 < gridNumNodesY) {
								CreateOrDeleteLink(gridNodesXY[i][j], gridNodesXY[i+1][j+1], true);
							}
						}
						else {
							if (j > 0) {
								CreateOrDeleteLink(gridNodesXY[i][j], gridNodesXY[i+1][j-1], true);
							}
						}
					}
				}
			}
		}
		
		// Update node positions
		for (int j = 0; j < gridNumNodesY; j++) {
			for (int i = 0; i < gridNumNodesX; i++) {
				float xPos = 0;
				float yPos = 0;
				if (gridType == gridTypes.quadrilateral) {
					xPos = (i)*gridLinkLengthX + (i)*2*gridNodesXY[i][j].radius;
					yPos = (j)*gridLinkLengthY + (j)*2*gridNodesXY[i][j].radius;
				}
				else if (gridType == gridTypes.hexagonal_flat_top) {
					int modX = i % 2;
					xPos = (i)*gridLinkLengthX * Mathf.Cos(30*Mathf.Deg2Rad) + (i)*Mathf.Sqrt(3)*gridNodesXY[i][j].radius;
					yPos = (j)*gridLinkLengthY + (j)*2*gridNodesXY[i][j].radius + modX * gridNodesXY[i][j].radius + modX * gridLinkLengthY * Mathf.Sin(30*Mathf.Deg2Rad);
				}
				else if (gridType == gridTypes.hexagonal_flat_side) {
					int modY = j % 2;
					xPos = (i)*gridLinkLengthX + (i)*2*gridNodesXY[i][j].radius + modY * gridNodesXY[i][j].radius + modY * gridLinkLengthX * Mathf.Sin(30*Mathf.Deg2Rad);
					yPos = (j)*gridLinkLengthY * Mathf.Cos(30*Mathf.Deg2Rad) + (j)*Mathf.Sqrt(3)*gridNodesXY[i][j].radius;
				}
				gridNodesXY[i][j].Reposition(xPos, yPos);
			}
		}
		
		// Update link visuals
		for (int i = 0; i < gridLinks.Count; i++) {
			if (gridLinks[i] != null) {
				changeSpriteColor(gridLinks[i], linkColor);
				changeSpriteWidth(gridLinks[i], linkWidth);
			}
		}
	}
	
	void CreateOrDeleteLink(WMG_Node fromNode, WMG_Node toNode, bool noVertHoriz) {
		WMG_Link aLink = GetLink(fromNode, toNode);
		if (aLink == null) {
			if (createLinks && !noVertHoriz) {
				gridLinks.Add(CreateLink(fromNode, toNode.gameObject, linkPrefab, this.gameObject));
			}
		}
		else {
			if (!createLinks || noVertHoriz) {
				gridLinks.Remove(aLink.gameObject);
				DeleteLink(aLink);
			}
		}
	}
}
