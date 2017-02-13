using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The node class primarily used to create the points / bars of axis graphs.
/// </summary>
public class WMG_Node : WMG_GUI_Functions {

	/// <summary>
	/// The unique id of this node per instance of WMG_Graph_Manager.
	/// </summary>
	public int id;
	/// <summary>
	/// The radius of this node if it represents a circle, and half the width of this node if it represents a square.
	/// </summary>
	public float radius;
	/// <summary>
	/// Whether or not this node represents a square or a circle.
	/// </summary>
	public bool isSquare;
	/// <summary>
	/// The number of links connected with this node.
	/// </summary>
	public int numLinks = 0;
	/// <summary>
	/// The links connected with this node.
	/// </summary>
	public List<GameObject> links = new List<GameObject>();
	/// <summary>
	/// The angles of the links connected with this node.
	/// </summary>
	public List<float> linkAngles = new List<float>();
	/// <summary>
	/// Reference to the object that should change scale for this node, typically it is the same object to which this node is attached.
	/// </summary>
	public GameObject objectToScale;
	/// <summary>
	/// Reference to the object that should change color for this node, could be a separate child object of this node.
	/// </summary>
	public GameObject objectToColor;
	/// <summary>
	/// Reference to the object that should change label for this node, could be a separate child object of this node.
	/// </summary>
	public GameObject objectToLabel;
	public bool isSelected = false;		// Only Used in Editor - Used in the editor when the node is selected
	public bool wasSelected = false;	// Only Used in Editor - Used in the editor for drag select operations
	public bool BFS_mark;	// Used in shortest path unweighted
	public int BFS_depth;	// Used in shortest path unweighted
	public float Dijkstra_depth;	// Used in shortest path weighted
	public WMG_Series seriesRef; // Used for series legend event delegates
	
	/// <summary>
	/// Create a link between two nodes. Sets node references, and repositions to connect the 2 nodes.
	/// </summary>
	/// <returns>The link.</returns>
	/// <param name="target">Target.</param>
	/// <param name="prefabLink">Prefab link.</param>
	/// <param name="linkId">Link identifier.</param>
	/// <param name="parent">Parent.</param>
	/// <param name="repos">If set to <c>true</c> repos.</param>
	public GameObject CreateLink (GameObject target, Object prefabLink, int linkId, GameObject parent, bool repos) {
		GameObject objLink = Instantiate(prefabLink) as GameObject;
		Vector3 linkLocalPos = objLink.transform.localPosition;
		GameObject theParent = parent;
		if (parent == null) theParent = target.transform.parent.gameObject;
		changeSpriteParent(objLink, theParent);
		objLink.transform.localScale = Vector3.one;
		objLink.transform.localPosition = linkLocalPos;
		WMG_Link theLink = objLink.GetComponent<WMG_Link>();
		links.Add(objLink);
		linkAngles.Add(0);
		WMG_Node theTarget = target.GetComponent<WMG_Node>();
		theTarget.links.Add(objLink);
		theTarget.linkAngles.Add(0);
		theTarget.numLinks++;
		numLinks++;
		theLink.Setup(this.gameObject, target, linkId, repos); // automatically repositions and scales the link based on the nodes
		return objLink;
	}
	
	/// <summary>
	/// Update the position of this node. Also repositions and rotates its links to correctly correspond with the new node position.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public virtual void Reposition (float x, float y) {
		changeSpritePositionTo(this.gameObject, new Vector3(x, y, 1));
		for (int i = 0; i < numLinks; i++) {
			WMG_Link theLink = links[i].GetComponent<WMG_Link>();
			theLink.Reposition();
		}
	}
	
	/// <summary>
	/// Set node Id. Node Id can change during node deletion, but node Id remains unique.
	/// </summary>
	/// <param name="newID">New I.</param>
	public void SetID(int newID) {
		id = newID;
		this.name = "WMG_Node_" + id;
	}
	
	/// <summary>
	/// Repositions this node relative to another node.
	/// </summary>
	/// <param name="fromNode">From node.</param>
	/// <param name="fixAngle">If set to <c>true</c> fix angle.</param>
	/// <param name="degreeStep">Degree step.</param>
	/// <param name="lengthStep">Length step.</param>
	public void RepositionRelativeToNode (WMG_Node fromNode, bool fixAngle, int degreeStep, float lengthStep) {
		// This is used to reposition the node and associated links based on a fixed angle or fixed length step relative to another node
		float posXdif = (this.transform.localPosition.x - fromNode.transform.localPosition.x);
		float posYdif = (this.transform.localPosition.y - fromNode.transform.localPosition.y);
		
		float angle = Mathf.Atan2(posYdif,posXdif)*Mathf.Rad2Deg;
		if (angle < 0) angle += 360;
		
		float length = Mathf.Sqrt(Mathf.Pow(posYdif,2) + Mathf.Pow(posXdif,2));
		if (length < 0) length = 0;
		
		float newAngle = angle;
		if (fixAngle) {
			newAngle = 0;
			for (int i = 0; i < 360 / degreeStep; i++) {
				if (angle >= i*degreeStep - 0.5f*degreeStep && angle < (i+1)*degreeStep - 0.5f*degreeStep) {
					newAngle = i*degreeStep;
				}
			}
		}
		else {
			float mod = length % lengthStep;
			length -= mod;
			if (lengthStep - mod < lengthStep / 2) length += lengthStep;
		}
		
		this.transform.localPosition = new Vector3 (fromNode.transform.localPosition.x + length * Mathf.Cos(Mathf.Deg2Rad*(newAngle)), 
													fromNode.transform.localPosition.y + length * Mathf.Sin(Mathf.Deg2Rad*(newAngle)), 
													this.transform.localPosition.z);
		
		for (int i = 0; i < numLinks; i++) {
			WMG_Link theLink = links[i].GetComponent<WMG_Link>();
			theLink.Reposition();
		}
	}
}
