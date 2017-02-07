using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WMG_Graph_Manager : WMG_Events, IWMG_Data_Generators, IWMG_Caching_Functions, IWMG_Path_Finding {
	
	private List<GameObject> nodesParent = new List<GameObject>(); // Each gameobject in this list is a WMG Node
	private List<GameObject> linksParent = new List<GameObject>(); // Each gameobject in this list is a WMG Link
	
	// Data generation functions
	private WMG_Data_Generators data_gen = new WMG_Data_Generators();
	
	public List<Vector2> GenLinear(int numPoints, float minX, float maxX, float a, float b) {
		return data_gen.GenLinear(numPoints, minX, maxX, a, b);
	}
	
	public List<Vector2> GenQuadratic(int numPoints, float minX, float maxX, float a, float b, float c) {
		return data_gen.GenQuadratic(numPoints, minX, maxX, a, b, c);
	}
	
	public List<Vector2> GenExponential(int numPoints, float minX, float maxX, float a, float b, float c) {
		return data_gen.GenExponential(numPoints, minX, maxX, a, b, c);
	}
	
	public List<Vector2> GenLogarithmic(int numPoints, float minX, float maxX, float a, float b, float c) {
		return data_gen.GenLogarithmic(numPoints, minX, maxX, a, b, c);
	}

	/// <summary>
	/// Generate data of the form c^2 = (X - a)^2 * (Y - b)^2
	/// </summary>
	/// <returns>List of Vector2 forming a circle.</returns>
	public List<Vector2> GenCircular(int numPoints, float a, float b, float c) {
		return data_gen.GenCircular(numPoints, a, b, c);
	}

	public List<Vector2> GenCircular2(int numPoints, float a, float b, float c, float degreeOffset) {
		return data_gen.GenCircular2(numPoints, a, b, c, degreeOffset);
	}

	public List<Vector2> GenRadar(List<float> data, float a, float b, float degreeOffset) {
		return data_gen.GenRadar(data, a, b, degreeOffset);
	}
	
	public List<Vector2> GenRandomXY(int numPoints, float minX, float maxX, float minY, float maxY) {
		return data_gen.GenRandomXY(numPoints, minX, maxX, minY, maxY);
	}
	
	public List<Vector2> GenRandomY(int numPoints, float minX, float maxX, float minY, float maxY) {
		return data_gen.GenRandomY(numPoints, minX, maxX, minY, maxY);
	}

	public List<float> GenRandomList(int numPoints, float min, float max) {
		return data_gen.GenRandomList(numPoints, min, max);
	}
	
	// Caching functions
	private WMG_Caching_Functions caching = new WMG_Caching_Functions();

	public void updateCacheAndFlagList<T>(ref List<T> cache, List<T> val, ref bool flag) {
		caching.updateCacheAndFlagList(ref cache, val, ref flag);
	}

	public void updateCacheAndFlag<T>(ref T cache, T val, ref bool flag) {
		caching.updateCacheAndFlag(ref cache, val, ref flag);
	}

	public void SwapVals<T>(ref T val1, ref T val2) {
		caching.SwapVals(ref val1, ref val2);
	}

	public void SwapValsList<T>(ref List<T> val1, ref List<T> val2) {
		caching.SwapValsList(ref val1, ref val2);
	}
	
	// Path Finding Functions
	private WMG_Path_Finding path_find = new WMG_Path_Finding();
	
	public List<WMG_Link> FindShortestPathBetweenNodes(WMG_Node fromNode, WMG_Node toNode) {
		path_find.nodesParent = nodesParent;
		return path_find.FindShortestPathBetweenNodes(fromNode, toNode);
	}
	
	public List<WMG_Link> FindShortestPathBetweenNodesWeighted(WMG_Node fromNode, WMG_Node toNode, bool includeRadii) {
		path_find.nodesParent = nodesParent;
		return path_find.FindShortestPathBetweenNodesWeighted(fromNode, toNode, includeRadii);
	}

	// Graph Manager functions

	public string getLabelText(string text, WMG_Enums.labelTypes labelType, float value, float percent, int numDecimals) {
		string theText = text;
		float multiplier = Mathf.Pow(10f, numDecimals+2);
		
		if (labelType == WMG_Enums.labelTypes.None) {
			theText = "";
		}
		else if (labelType == WMG_Enums.labelTypes.Labels_Percents) {
			theText += " (" + (Mathf.Round(percent*multiplier)/multiplier*100).ToString() + "%)";
		}
		else if (labelType == WMG_Enums.labelTypes.Labels_Values) {
			theText += " (" + Mathf.Round(value).ToString() + ")";
		}
		else if (labelType == WMG_Enums.labelTypes.Labels_Values_Percents) {
			theText += " - " + Mathf.Round(value).ToString() + " (" + (Mathf.Round(percent*multiplier)/multiplier*100).ToString() + "%)";
		}
		else if (labelType == WMG_Enums.labelTypes.Values_Only) {
			theText = Mathf.Round(value).ToString();
		}
		else if (labelType == WMG_Enums.labelTypes.Percents_Only) {
			theText = (Mathf.Round(percent*multiplier)/multiplier*100).ToString() + "%";
		}
		else if (labelType == WMG_Enums.labelTypes.Values_Percents) {
			theText = Mathf.Round(value).ToString() + " (" + (Mathf.Round(percent*multiplier)/multiplier*100).ToString() + "%)";
		}
		return theText;
	}
	
	public List<GameObject> NodesParent {
		get { return nodesParent; }
	}
	
	public List<GameObject> LinksParent {
		get { return linksParent; }
	}
	
	public GameObject CreateNode(Object prefabNode, GameObject parent) {
		// Creates a node from a prefab, gives a default parent to this object, and adds the node to the manager's list
		GameObject curObj = Instantiate(prefabNode) as GameObject;
		Vector3 origScale = curObj.transform.localScale;
		Vector3 origRot = curObj.transform.localEulerAngles;
		GameObject theParent = parent;
		if (parent == null) theParent = this.gameObject;
		changeSpriteParent(curObj, theParent);
		curObj.transform.localScale = origScale;
		curObj.transform.localEulerAngles = origRot;
		WMG_Node curNode = curObj.GetComponent<WMG_Node>();
		curNode.SetID(nodesParent.Count);
		nodesParent.Add(curObj);
		return curObj;
	}
	
	public GameObject CreateLink(WMG_Node fromNode, GameObject toNode, Object prefabLink, GameObject parent) {
		GameObject createdLink = fromNode.CreateLink(toNode, prefabLink, linksParent.Count, parent, true);
		linksParent.Add(createdLink);
		return createdLink;
	}
	
	public GameObject CreateLinkNoRepos(WMG_Node fromNode, GameObject toNode, Object prefabLink, GameObject parent) {
		GameObject createdLink = fromNode.CreateLink(toNode, prefabLink, linksParent.Count, parent, false);
		linksParent.Add(createdLink);
		return createdLink;
	}
	
	public WMG_Link GetLink(WMG_Node fromNode, WMG_Node toNode) {
		foreach (GameObject link in fromNode.links) {
			WMG_Link aLink = link.GetComponent<WMG_Link>();
			WMG_Node toN = aLink.toNode.GetComponent<WMG_Node>();
			if (toN.id != toNode.id) toN = aLink.fromNode.GetComponent<WMG_Node>();
			if (toN.id == toNode.id) return aLink;
		}
		return null;
	}
	
	public GameObject ReplaceNodeWithNewPrefab(WMG_Node theNode, Object prefabNode) {
		// Used to swap prefabs of a node
		GameObject newNode = CreateNode(prefabNode, theNode.transform.parent.gameObject);
		WMG_Node newNode2 = newNode.GetComponent<WMG_Node>();
		newNode2.numLinks = theNode.numLinks;
		newNode2.links = theNode.links;
		newNode2.linkAngles = theNode.linkAngles;
		newNode2.SetID(theNode.id);
		newNode2.name = theNode.name;
		newNode.transform.position = theNode.transform.position;
		
		// Update link from / to node references
		for (int i = 0; i < theNode.numLinks; i++) {
			WMG_Link aLink = theNode.links[i].GetComponent<WMG_Link>();
			WMG_Node fromN = aLink.fromNode.GetComponent<WMG_Node>();
			if (fromN.id == theNode.id) {
				aLink.fromNode = newNode;
			}
			else {
				aLink.toNode = newNode;
			}
		}
		nodesParent.Remove(theNode.gameObject);
		Destroy(theNode.gameObject);
		
		return newNode;
	}
	
	public void DeleteNode(WMG_Node theNode) {
		// Deleting a node also deletes all of its link and swaps IDs with the largest node ID in the list
		int idToDelete = theNode.id;
		foreach (GameObject node in nodesParent) {
			WMG_Node aNode = node.GetComponent<WMG_Node>();
			if (aNode != null && aNode.id == nodesParent.Count - 1) {
				aNode.SetID(idToDelete);
				while (theNode.numLinks > 0) {
					WMG_Link aLink = theNode.links[0].GetComponent<WMG_Link>();
					DeleteLink(aLink);
				}
				nodesParent.Remove(theNode.gameObject);
				DestroyImmediate(theNode.gameObject);
				return;
			}
		}
	}
	
	public void DeleteLink(WMG_Link theLink) {
		// Deleting a link updates references in the to and from nodes and swaps IDs with the largest link ID in the list
		WMG_Node fromN = theLink.fromNode.GetComponent<WMG_Node>();
		WMG_Node toN = theLink.toNode.GetComponent<WMG_Node>();
		for (int i = 0; i < fromN.numLinks; i++) {
			WMG_Link aLink = fromN.links[i].GetComponent<WMG_Link>();
			if (aLink.id == theLink.id) {
				fromN.numLinks--;
				fromN.links[i] = fromN.links[fromN.numLinks];
				break;
			}
		}
		for (int i = 0; i < toN.numLinks; i++) {
			WMG_Link aLink = toN.links[i].GetComponent<WMG_Link>();
			if (aLink.id == theLink.id) {
				toN.numLinks--;
				toN.links[i] = toN.links[toN.numLinks];
				break;
			}
		}
		
		int idToDelete = theLink.id;
		foreach (GameObject child in linksParent) {
			WMG_Link aLink = child.GetComponent<WMG_Link>();
			if (aLink != null && aLink.id == linksParent.Count - 1) {
				aLink.SetId(idToDelete);
				linksParent.Remove(theLink.gameObject);
				DestroyImmediate(theLink.gameObject);
				fromN.links.RemoveAt(fromN.numLinks);
				fromN.linkAngles.RemoveAt(fromN.numLinks);
				toN.links.RemoveAt(toN.numLinks);
				toN.linkAngles.RemoveAt(toN.numLinks);
				return;
			}
		}
	}
	
}
