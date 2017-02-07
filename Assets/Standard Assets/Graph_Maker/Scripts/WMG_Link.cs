using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WMG_Link : WMG_GUI_Functions {
	public int id;	// Each link has a unique id
	// Node reference
	public GameObject fromNode;
	public GameObject toNode;
	// References to children of this link that could be interesting to use in scripts
	public GameObject objectToScale;
	public GameObject objectToColor;
	public GameObject objectToLabel;
	public bool weightIsLength;	// Updates the link weight based on its length
	public bool updateLabelWithLength; // Updates the objectToLabel with the link length
	public bool isSelected = false;	// Used in the editor when the link is selected
	public bool wasSelected = false; // Used in the editor for drag select operations
	public float weight;	// A link's weight, used in find shortest path weighted algorithms
	
	public void Setup(GameObject fromNode, GameObject toNode, int linkId, bool repos) {
		// Setup references and give a default name of the link based on node IDs
		this.fromNode = fromNode;
		this.toNode = toNode;
		SetId(linkId);
		WMG_Node fromN = fromNode.GetComponent<WMG_Node>();
		WMG_Node toN = toNode.GetComponent<WMG_Node>();
		this.name = "WMG_Link_" + fromN.id + "_" + toN.id;
		if (repos) Reposition();	// Update position and scale based on connected nodes
	}
	
	public void Reposition() {
		float posXdif = getSpritePositionX(toNode) - getSpritePositionX(fromNode);
		float posYdif = getSpritePositionY(toNode) - getSpritePositionY(fromNode);
		
		float angle = Mathf.Atan2(posYdif,posXdif)*Mathf.Rad2Deg + 90;
		
		WMG_Node fromN = fromNode.GetComponent<WMG_Node>();
		WMG_Node toN = toNode.GetComponent<WMG_Node>();
		
		SetNodeAngles(angle,fromN,toN);	// Set angles in node references, so they don't need to be calculated in various places
		
		float radiuses = fromN.radius + toN.radius;
		float length = Mathf.Sqrt(Mathf.Pow(posYdif,2) + Mathf.Pow(posXdif,2)) - radiuses;
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
		
		// NGUI
		this.transform.localPosition = new Vector3 (getSpriteFactorY2(this.objectToScale) * posXdif + fromNode.transform.localPosition.x + radiusDifPosX + squareDifPosX, 
													getSpriteFactorY2(this.objectToScale) * posYdif + fromNode.transform.localPosition.y + radiusDifPosY + squareDifPosY, 
													this.transform.localPosition.z);
		
		// Daikon
//		changeSpritePositionRelativeToObjBy(this.gameObject, fromNode, 
//											new Vector3(getSpriteFactorY(this.gameObject) * posXdif + 
//														-getSpriteOffsetX(this.gameObject) +
//														Mathf.Cos(Mathf.Deg2Rad * angle) * 0.5f * getSpriteWidth(this.gameObject) +
//														Mathf.Cos(Mathf.Deg2Rad * angle) * (getSpriteFactorX(this.gameObject) - 1) * getSpriteWidth(this.gameObject) +
//														getSpriteOffsetX(fromNode) + radiusDifPosX + squareDifPosX, 
//														getSpriteFactorY(this.gameObject) * posYdif +
//														getSpriteOffsetY(this.gameObject) +
//														-Mathf.Sin(Mathf.Deg2Rad * angle) * 0.5f * getSpriteWidth(this.gameObject) +
//														Mathf.Sin(Mathf.Deg2Rad * angle) * getSpriteFactorX(this.gameObject) * getSpriteWidth(this.gameObject) +
//														-getSpriteOffsetY(fromNode) + radiusDifPosY + squareDifPosY, 1));
		
		changeSpriteHeight(objectToScale, Mathf.RoundToInt(length));
		this.transform.localEulerAngles = new Vector3 (0,0,angle);
		
	}
	
	public void SetId(int linkId) {
		this.id = linkId;
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
	
	private float getSquareCircleOffsetLength(WMG_Node theNode, float angle, bool isFrom) {
		if (theNode.isSquare) {
			int angleOffset = getSquareCircleOffsetAngle(angle, isFrom);
			float squareOffsetFromX = theNode.radius - theNode.radius * Mathf.Cos(Mathf.Deg2Rad * angleOffset);
			float squareOffsetFromY = squareOffsetFromX * Mathf.Tan(Mathf.Deg2Rad * angleOffset);
			return Mathf.Sqrt(squareOffsetFromX * squareOffsetFromX + squareOffsetFromY * squareOffsetFromY);
		}
		else return 0;
	}
	
	private int getSquareCircleOffsetAngle(float angle, bool isFrom) {
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
