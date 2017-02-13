using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The base graph class from which most graphs are derived.
/// </summary>
public class WMG_Graph_Manager : WMG_Events {
	
	private List<GameObject> nodesParent = new List<GameObject>(); // Each gameobject in this list is a WMG Node
	private List<GameObject> linksParent = new List<GameObject>(); // Each gameobject in this list is a WMG Link

	/// <summary>
	/// The list of GameObjects which are WMG_Nodes for this graph.
	/// </summary>
	public List<GameObject> NodesParent {
		get { return nodesParent; }
	}

	/// <summary>
	/// The list of GameObjects which are WMG_Links for this graph.
	/// </summary>
	public List<GameObject> LinksParent {
		get { return linksParent; }
	}

	/// <summary>
	/// Creates a node for this graph.
	/// </summary>
	/// <returns>The node.</returns>
	/// <param name="prefabNode">Prefab node.</param>
	/// <param name="parent">Parent.</param>
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

	/// <summary>
	/// Creates a link for this graph, and repositions the link.
	/// </summary>
	/// <returns>The link.</returns>
	/// <param name="fromNode">From node.</param>
	/// <param name="toNode">To node.</param>
	/// <param name="prefabLink">Prefab link.</param>
	/// <param name="parent">Parent.</param>
	public GameObject CreateLink(WMG_Node fromNode, GameObject toNode, Object prefabLink, GameObject parent) {
		GameObject createdLink = fromNode.CreateLink(toNode, prefabLink, linksParent.Count, parent, true);
		linksParent.Add(createdLink);
		return createdLink;
	}

	/// <summary>
	/// Creates a link for this graph, and does not repoisition the link.
	/// </summary>
	/// <returns>The link no repos.</returns>
	/// <param name="fromNode">From node.</param>
	/// <param name="toNode">To node.</param>
	/// <param name="prefabLink">Prefab link.</param>
	/// <param name="parent">Parent.</param>
	public GameObject CreateLinkNoRepos(WMG_Node fromNode, GameObject toNode, Object prefabLink, GameObject parent) {
		GameObject createdLink = fromNode.CreateLink(toNode, prefabLink, linksParent.Count, parent, false);
		linksParent.Add(createdLink);
		return createdLink;
	}

	/// <summary>
	/// Given two nodes, get the link between those nodes for this graph.
	/// </summary>
	/// <returns>The link.</returns>
	/// <param name="fromNode">From node.</param>
	/// <param name="toNode">To node.</param>
	public WMG_Link GetLink(WMG_Node fromNode, WMG_Node toNode) {
		foreach (GameObject link in fromNode.links) {
			WMG_Link aLink = link.GetComponent<WMG_Link>();
			WMG_Node toN = aLink.toNode.GetComponent<WMG_Node>();
			if (toN.id != toNode.id) toN = aLink.fromNode.GetComponent<WMG_Node>();
			if (toN.id == toNode.id) return aLink;
		}
		return null;
	}

	/// <summary>
	/// Given a node, and a prefab, re-instantiate the node using the specified prefab which has a WMG_Node script attached.
	/// </summary>
	/// <returns>The node with new prefab.</returns>
	/// <param name="theNode">The node.</param>
	/// <param name="prefabNode">Prefab node.</param>
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

	/// <summary>
	/// Deletes a node.
	/// </summary>
	/// <param name="theNode">The node.</param>
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

	/// <summary>
	/// Deletes a link.
	/// </summary>
	/// <param name="theLink">The link.</param>
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
				aLink.id = idToDelete;
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
	
	/// <summary>
	/// Given two nodes return one or more shortest paths between the nodes based on the number of links (unweighted), using Breadth-first search algorithm.
	/// </summary>
	/// <returns>The shortest path between nodes.</returns>
	/// <param name="fromNode">From node.</param>
	/// <param name="toNode">To node.</param>
	public List<WMG_Link> FindShortestPathBetweenNodes(WMG_Node fromNode, WMG_Node toNode) {
		
		List<WMG_Link> linksBetweenToAndFrom = new List<WMG_Link>();
		
		// Reset BFS data needed for this algorithm
		foreach (GameObject node in nodesParent) {
			WMG_Node aNode = node.GetComponent<WMG_Node>();
			if (aNode != null) {
				aNode.BFS_mark = false;
				aNode.BFS_depth = 0;
			}
		}
		
		Queue<WMG_Node> mapSysQ = new Queue<WMG_Node>();
		
		// This calculates and stores the depth of every node between the starting and ending nodes
		// This is exactly the BFS (Breadth-first search) algorithm
		mapSysQ.Enqueue(fromNode);
		fromNode.BFS_mark =  true;
		while (mapSysQ.Count > 0) {
			WMG_Node temp = mapSysQ.Dequeue();
			if (toNode.id == temp.id) break; // Reached the target node so we are done
			// Add the current node neighbors to the queue if they haven't been added in the past and calculate the depth
			for (int i = 0; i < temp.numLinks; i++) {
				WMG_Link aLink = temp.links[i].GetComponent<WMG_Link>();
				WMG_Node temp2 = aLink.toNode.GetComponent<WMG_Node>();
				if (temp2.id == temp.id) temp2 = aLink.fromNode.GetComponent<WMG_Node>();
				if (!temp2.BFS_mark) {
					temp2.BFS_mark = true;
					temp2.BFS_depth = temp.BFS_depth + 1;
					mapSysQ.Enqueue(temp2);
				}
			}
		}
		
		// If all we cared about was the shortest distance between the two nodes we could end here, but we might also want the links themselves
		// This finds the shortest path of links between the starting and ending nodes using the previously calculated depths
		mapSysQ.Clear();
		mapSysQ.Enqueue(toNode);
		while (mapSysQ.Count > 0) {
			WMG_Node temp = mapSysQ.Dequeue();
			if (fromNode.id == temp.id) break;
			for (int i = 0; i < temp.numLinks; i++) {
				WMG_Link aLink = temp.links[i].GetComponent<WMG_Link>();
				WMG_Node temp2 = aLink.toNode.GetComponent<WMG_Node>();
				if (temp2.id == temp.id) temp2 = aLink.fromNode.GetComponent<WMG_Node>();
				if (temp.BFS_depth == temp2.BFS_depth + 1) {
					if (temp2.BFS_depth == 0 && temp2.id != fromNode.id) continue;
					linksBetweenToAndFrom.Add(aLink);
					if (!mapSysQ.Contains(temp2)) mapSysQ.Enqueue(temp2);
				}
			}
		}
		return linksBetweenToAndFrom;
	}

	/// <summary>
	/// Given two nodes return one or more shortest paths between the nodes based on the link weights (weighted), and also node radii if include radii is true, using Dijkstra's algorithm.
	/// </summary>
	/// <returns>The shortest path between nodes weighted.</returns>
	/// <param name="fromNode">From node.</param>
	/// <param name="toNode">To node.</param>
	/// <param name="includeRadii">If set to <c>true</c> include radii.</param>
	public List<WMG_Link> FindShortestPathBetweenNodesWeighted(WMG_Node fromNode, WMG_Node toNode, bool includeRadii) {
		
		List<WMG_Link> linksBetweenToAndFrom = new List<WMG_Link>();
		List<WMG_Node> Dijkstra_nodes = new List<WMG_Node>();
		// Reset data needed for this algorithm
		foreach (GameObject node in nodesParent) {
			WMG_Node aNode = node.GetComponent<WMG_Node>();
			if (aNode != null) {
				if (aNode.id == fromNode.id) aNode.Dijkstra_depth = 0;
				else aNode.Dijkstra_depth = Mathf.Infinity;
				Dijkstra_nodes.Add(aNode);
			}
		}
		Dijkstra_nodes.Sort (delegate(WMG_Node x, WMG_Node y) { return x.Dijkstra_depth.CompareTo(y.Dijkstra_depth); });
		
		// This is exactly Dijkstra's algorithm
		while (Dijkstra_nodes.Count > 0) {
			WMG_Node temp = Dijkstra_nodes[0];
			Dijkstra_nodes.RemoveAt(0);
			if (toNode.id == temp.id) break; // Reached the target node so we are done
			if (temp.Dijkstra_depth == Mathf.Infinity) break;
			for (int i = 0; i < temp.numLinks; i++) {
				WMG_Link aLink = temp.links[i].GetComponent<WMG_Link>();
				WMG_Node temp2 = aLink.toNode.GetComponent<WMG_Node>();
				if (temp2.id == temp.id) temp2 = aLink.fromNode.GetComponent<WMG_Node>();
				float alt = temp.Dijkstra_depth + aLink.weight;
				if (includeRadii) alt += temp.radius + temp2.radius;
				if (alt < temp2.Dijkstra_depth) {
					temp2.Dijkstra_depth = alt;
					Dijkstra_nodes.Sort (delegate(WMG_Node x, WMG_Node y) { return x.Dijkstra_depth.CompareTo(y.Dijkstra_depth); });
				}
			}
		}
		
		// If all we cared about was the shortest distance between the two nodes we could end here, but we might also want the links themselves
		// This finds the shortest path of links between the starting and ending nodes using the previously calculated depths
		Queue<WMG_Node> mapSysQ = new Queue<WMG_Node>();
		mapSysQ.Enqueue(toNode);
		while (mapSysQ.Count > 0) {
			WMG_Node temp = mapSysQ.Dequeue();
			if (fromNode.id == temp.id) break;
			for (int i = 0; i < temp.numLinks; i++) {
				WMG_Link aLink = temp.links[i].GetComponent<WMG_Link>();
				WMG_Node temp2 = aLink.toNode.GetComponent<WMG_Node>();
				if (temp2.id == temp.id) temp2 = aLink.fromNode.GetComponent<WMG_Node>();
				float alt = temp2.Dijkstra_depth + aLink.weight;
				if (includeRadii) alt += temp.radius + temp2.radius;
				if (Mathf.Approximately(temp.Dijkstra_depth, alt)) {
					linksBetweenToAndFrom.Add(aLink);
					if (!mapSysQ.Contains(temp2)) mapSysQ.Enqueue(temp2);
				}
			}
		}
		return linksBetweenToAndFrom;
	}
	
}
