using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IWMG_Path_Finding {
	// Given two nodes return one or more shortest paths between the nodes based on the number of links (unweighted)
	List<WMG_Link> FindShortestPathBetweenNodes(WMG_Node fromNode, WMG_Node toNode);
	
	// Given two nodes return one or more shortest paths between the nodes based on the link weights (weighted), and also node radii if include radii is true
	List<WMG_Link> FindShortestPathBetweenNodesWeighted(WMG_Node fromNode, WMG_Node toNode, bool includeRadii);
}

public class WMG_Path_Finding : IWMG_Path_Finding {
	
	public List<GameObject> nodesParent;
	
	// Given two nodes return one or more shortest paths between the nodes based on the number of links (unweighted)
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
	
	// Given two nodes return one or more shortest paths between the nodes based on the link weights (weighted), and also node radii if include radii is true
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
