using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The link class primarily used for the lines in line series in axis graphs.
/// </summary>
public class WMG_Link : WMG_GUI_Functions {

	/// <summary>
	/// The unique id of this link per instance of WMG_Graph_Manager.
	/// </summary>
	public int id;
	/// <summary>
	/// The node from which this link originates.
	/// </summary>
	public GameObject fromNode;
	/// <summary>
	/// To node to which this link terminates.
	/// </summary>
	public GameObject toNode;
	/// <summary>
	/// Reference to the object that should change scale for this link, typically it is the same object to which this link is attached.
	/// </summary>
	public GameObject objectToScale;
	/// <summary>
	/// Reference to the object that should change color for this link, could be a separate child object of this link.
	/// </summary>
	public GameObject objectToColor;
	/// <summary>
	/// Reference to the object that should change label for this link, could be a separate child object of this link.
	/// </summary>
	public GameObject objectToLabel;
	public bool weightIsLength;	// Updates the link weight based on its length
	public bool updateLabelWithLength; // Updates the objectToLabel with the link length
	public bool isSelected = false;	// Used in the editor when the link is selected
	public bool wasSelected = false; // Used in the editor for drag select operations
	public float weight;	// A link's weight, used in find shortest path weighted algorithms

	/// <summary>
	/// Initializes this link, and sets its nodes.
	/// </summary>
	/// <param name="fromNode">From node.</param>
	/// <param name="toNode">To node.</param>
	/// <param name="linkId">Link identifier.</param>
	/// <param name="repos">If set to <c>true</c> repos.</param>
	public void Setup(GameObject fromNode, GameObject toNode, int linkId, bool repos) {
		// Setup references and give a default name of the link based on node IDs
		this.fromNode = fromNode;
		this.toNode = toNode;
		this.id = linkId;
		WMG_Node fromN = fromNode.GetComponent<WMG_Node>();
		WMG_Node toN = toNode.GetComponent<WMG_Node>();
		this.name = "WMG_Link_" + fromN.id + "_" + toN.id;
		if (repos) Reposition();	// Update position and scale based on connected nodes
	}

	/// <summary>
	/// Reposition this link based on the positions of its from and to nodes.
	/// </summary>
	public virtual void Reposition() {
		float posXdif = getSpritePositionX(toNode) - getSpritePositionX(fromNode);
		float posYdif = getSpritePositionY(toNode) - getSpritePositionY(fromNode);
		
		float angle = Mathf.Atan2(posYdif,posXdif)*Mathf.Rad2Deg + 90;
		
		WMG_Node fromN = fromNode.GetComponent<WMG_Node>();
		WMG_Node toN = toNode.GetComponent<WMG_Node>();
		
		SetNodeAngles(angle,fromN,toN);	// Set angles in node references, so they don't need to be calculated in various places
		
		float radii = fromN.radius + toN.radius;
		float length = Mathf.Sqrt(Mathf.Pow(posYdif,2) + Mathf.Pow(posXdif,2)) - radii;
		if (length < 0) length = 0;
		
		// When the radii are different, need to offset the link position based on the difference of the radii
		float radiusDifPosX = (fromN.radius - toN.radius) / 2 * Mathf.Cos(Mathf.Deg2Rad * (angle-90));
		float radiusDifPosY = (fromN.radius - toN.radius) / 2 * Mathf.Sin(Mathf.Deg2Rad * (angle-90));
		
		// Handling cases when one or more of the from / to nodes are square instead of circle
		float squareLengthOffsetFrom = getSquareCircleOffsetLength(fromN, angle, true);
		float squareLengthOffsetTo = getSquareCircleOffsetLength(toN, angle, false);
		
		length = length - squareLengthOffsetFrom - squareLengthOffsetTo;
		
		float squareDifPosX = (squareLengthOffsetFrom - squareLengthOffsetTo) / 2 * Mathf.Cos(Mathf.Deg2Rad * (angle-90));
		float squareDifPosY = (squareLengthOffsetFrom - squareLengthOffsetTo) / 2 * Mathf.Sin(Mathf.Deg2Rad * (angle-90));
		
		if (weightIsLength) weight = length;
		if (updateLabelWithLength) {
			if (objectToLabel != null) {
				changeLabelText(objectToLabel, Mathf.Round(length).ToString());
				objectToLabel.transform.localEulerAngles = new Vector3 (0,0,360-angle);
			}
		}

		this.transform.localPosition = new Vector3 (getSpritePivotTopToBot(this.objectToScale) * posXdif + fromNode.transform.localPosition.x + radiusDifPosX + squareDifPosX, 
		                                            getSpritePivotTopToBot(this.objectToScale) * posYdif + fromNode.transform.localPosition.y + radiusDifPosY + squareDifPosY, 
													this.transform.localPosition.z);
		
		changeSpriteHeight(objectToScale, Mathf.RoundToInt(length));
		this.transform.localEulerAngles = new Vector3 (0,0,angle);
	}
	
	void SetNodeAngles(float angle, WMG_Node fromN, WMG_Node toN) {
		for (int i = 0; i < fromN.numLinks; i++) {
			WMG_Link fromNlink = fromN.links[i].GetComponent<WMG_Link>();
			if (fromNlink.id == this.id) {
				fromN.linkAngles[i] = angle - 90;
			}
		}
		for (int i = 0; i < toN.numLinks; i++) {
			WMG_Link toNlink = toN.links[i].GetComponent<WMG_Link>();
			if (toNlink.id == this.id) {
				toN.linkAngles[i] = angle + 90;
			}
		}
	}
	
	float getSquareCircleOffsetLength(WMG_Node theNode, float angle, bool isFrom) {
		if (theNode.isSquare) {
			int angleOffset = getSquareCircleOffsetAngle(angle, isFrom);
			float squareOffsetFromX = theNode.radius - theNode.radius * Mathf.Cos(Mathf.Deg2Rad * angleOffset);
			float squareOffsetFromY = squareOffsetFromX * Mathf.Tan(Mathf.Deg2Rad * angleOffset);
			return Mathf.Sqrt(squareOffsetFromX * squareOffsetFromX + squareOffsetFromY * squareOffsetFromY);
		}
		else return 0;
	}
	
	int getSquareCircleOffsetAngle(float angle, bool isFrom) {
		int returnvalue = 0;
		if (isFrom) {
			returnvalue = (Mathf.RoundToInt(angle)-90)%90;
		}
		else {
			returnvalue = (Mathf.RoundToInt(angle)+90)%90;
		}
		if (Mathf.Abs(returnvalue) > 45) {
			if (returnvalue > 0) {
				returnvalue = returnvalue - 2 * (returnvalue - 45);
			}
			else {
				returnvalue = returnvalue - 2 * (returnvalue + 45);
			}
		}
		return returnvalue;
	}
}
