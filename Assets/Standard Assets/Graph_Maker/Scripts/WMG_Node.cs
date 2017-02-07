using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WMG_Node : WMG_GUI_Functions {
	
	public int id;	// Each node has a unique id
	public float radius;	// Nodes are represented as a circle, mostly used in random graph generation
	public bool isSquare;	// Square nodes change the link length based on the radius being half the width / height of the square
	// Each node is connected to 0 or more other nodes via links
	public int numLinks = 0;
	public List<GameObject> links = new List<GameObject>();
	public List<float> linkAngles = new List<float>();
	// References to children objects of this node that could be useful in scripts
	public GameObject objectToScale;
	public GameObject objectToColor;
	public GameObject objectToLabel;
	public bool isSelected = false;		// Only Used in Editor - Used in the editor when the node is selected
	public bool wasSelected = false;	// Only Used in Editor - Used in the editor for drag select operations
	public bool BFS_mark;	// Used in shortest path unweighted
	public int BFS_depth;	// Used in shortest path unweighted
	public float Dijkstra_depth;	// Used in shortest path weighted
	public WMG_Series seriesRef; // Used for series legend event delegates
	
	public GameObject CreateLink (GameObject target, Object prefabLink, int linkId, GameObject parent, bool repos) {
		// Creating a link between two nodes populates all needed references and automatically repositions and scales the link based on the nodes
		GameObject objLink = Instantiate(prefabLink) as GameObject;
		Vector3 linkLocalPos = objLink.transform.localPosition;
		GameObject theParent = parent;
		if (parent == null) theParent = target.transform.parent.gameObject;
		changeSpriteParent(objLink, theParent);
//		objLink.transform.parent = target.transform.parent;
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
	
	public void Reposition (float x, float y) {
		// Updates the local position of this node and all associated links
		changeSpritePositionTo(this.gameObject, new Vector3(x, y, 1));
		for (int i = 0; i < numLinks; i++) {
			WMG_Link theLink = links[i].GetComponent<WMG_Link>();
			theLink.Reposition();
		}
	}
	
	public void SetID(int newID) {
		// When nodes are deleted the ID can change but is still unique
		id = newID;
		this.name = "WMG_Node_" + id;
	}
	
	// Only Used in Editor - 
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
