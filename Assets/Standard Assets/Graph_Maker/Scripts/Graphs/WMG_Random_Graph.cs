using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A class used to create random graphs.
/// </summary>
public class WMG_Random_Graph : WMG_Graph_Manager {

	/// <summary>
	/// The prefab used to create the nodes.
	/// </summary>
	public Object nodePrefab;
	/// <summary>
	/// The prefab used to create the links.
	/// </summary>
	public Object linkPrefab;
	/// <summary>
	/// The total number of nodes that can appear in this graph.
	/// </summary>
	public int numNodes;
	/// <summary>
	/// The minimum possible angle between neighbor nodes. So if this is 15, then there should not exist any 2 neighbors that are less than 15 degrees apart from one another.
	/// </summary>
	public float minAngle;
	/// <summary>
	/// This can be used to control the direction in which the graph propagates. For example, to create procedural lightning looking graphs you would want 
	/// to set this to a narrow range like 0 - 45.
	/// </summary>
	public float minAngleRange;
	/// <summary>
	/// This can be used to control the direction in which the graph propagates. For example, to create procedural lightning looking graphs you would want 
	/// to set this to a narrow range like 0 - 45.
	/// </summary>
	public float maxAngleRange;
	/// <summary>
	/// This controls how many neighbors there are for each node.
	/// </summary>
	public int minRandomNumberNeighbors;
	/// <summary>
	/// This controls how many neighbors there are for each node.
	/// </summary>
	public int maxRandomNumberNeighbors;
	/// <summary>
	/// Determines the distance between nodes. Setting a high range will create more sporadic looking graphs, while setting the values equal will generate grid like graphs.
	/// </summary>
	public float minRandomLinkLength;
	/// <summary>
	/// Determines the distance between nodes. Setting a high range will create more sporadic looking graphs, while setting the values equal will generate grid like graphs.
	/// </summary>
	public float maxRandomLinkLength;
	/// <summary>
	/// Determines how the propagation proceeds. If this is false, then a node is randomly picked from the set of unprocessed nodes to process. 
	/// Processing a node randomly generates neighbors for that node, marks the node processed, and moves on to another node process. 
	/// If this is true, then the next node processed will be the oldest one that was created. 
	/// </summary>
	public bool centerPropogate;
	/// <summary>
	/// This ensures that a randomly generated link does not intersect with any existing links. 
	/// This should generally always be set to true unless you want to create some strange overlapping graph.
	/// </summary>
	public bool noLinkIntersection;
	/// <summary>
	/// This ensures that a randomly generated node will not intersect with any existing nodes. Circle intersection checks are done using the radii of the nodes. 
	/// This should generally always be set to true unless you want nodes to possibly overlap.
	/// </summary>
	public bool noNodeIntersection;
	/// <summary>
	/// This adds onto the radii used in the circle intersection checks used for the node intersection checks. 
	/// Increasing this value will ensure that nodes are more spaced apart from one another.
	/// </summary>
	public float noNodeIntersectionRadiusPadding;
	/// <summary>
	/// Sometimes highly depending on the parameters used, the graph will fail to produce any results, or fail to produce all the nodes specified. 
	/// If this happens a warning is logged to the console saying how many nodes were produced which was less than the number of nodes you specified. 
	/// This generally means your parameters were too specific. To help with this, you can increase this number to try and fully complete the graph. 
	/// The default is 100, meaning while processing a neighbor, up to 100 random angles and link lengths are generated. 
	/// Failing any of the checks such as the min neighbor angle or intersection checks will increase the attempt number and generate a new possibility.
	/// </summary>
	public int maxNeighborAttempts;
	/// <summary>
	/// Ensures that creating a new node does not intersect an existing link, or that creating a new link does not intersect an existing node. 
	/// This performs circle-line intersection checks with the creating link / node with all existing nodes / links.
	/// </summary>
	public bool noLinkNodeIntersection;
	/// <summary>
	/// Increases the radius of the node used in the circle-line intersection checks. 
	/// A higher value will ensure a graph that has links and nodes that are more spaced apart from each other.
	/// </summary>
	public float noLinkNodeIntersectionRadiusPadding;
	/// <summary>
	/// If true, then the #GenerateGraph function is called in Awake(). If you want to change parameters at run-time and 
	/// then generate the graph yourself then you would set this to false, get a reference to this script and call #GenerateGraph manually.
	/// </summary>
	public bool createOnStart;
	/// <summary>
	/// This can be useful to troubleshoot what is happening during the random graph generation process.
	/// </summary>
	public bool debugRandomGraph;

	// Use this for initialization
	void Awake () {
		if (createOnStart) {
			GenerateGraph();
		}
	}

	/// <summary>
	/// Creates a node, and then calls #GenerateGraphFromNode.
	/// </summary>
	/// <returns>The graph.</returns>
	public List<GameObject> GenerateGraph() {
		GameObject fromN = CreateNode(nodePrefab, null);
		WMG_Node fromNode = fromN.GetComponent<WMG_Node>();
		return GenerateGraphFromNode(fromNode);
	}

	/// <summary>
	/// Generates the random graph. Starts from a single node, where each node is processed by attempting to create neighbors and links to neighbors from the node.
	/// This function ends when all existing nodes have been fully processed (#maxNeighborAttempts reached for each node), or when #numNodes has been created.
	/// A node is processed if all of its neighbors were successfully created or if not all neighbors were created, but #maxNeighborAttempts was reached.
	/// #maxNeighborAttempts (a failed neighbor creation attempt) can get incremented for the following reasons:
	/// - When a randomly generated angle falls between existing neighbors that is less than #minAngle.
	/// - If #noLinkIntersection is true, a randomly generated angle and length would create a link that would cross an existing link in this graph's links parent.
	/// - If #noNodeIntersection is true, a randomly generated angle and length would create a node that that would circle interesect an existing node in this graph's nodes parent.
	/// - The same as above but #noNodeIntersectionRadiusPadding > 0, performs the circle intersections check with the nodes' radii increased by the specified padding.
	/// - If #noLinkNodeIntersection is true, a randomly generated node would intersect with an existing link or a randomly generated link would intersect with an existing node.
	/// - The same as above but #noLinkNodeIntersectionRadiusPadding > 0, performs circle - line intersections with the node radius increased by the specified padding.
	/// </summary>
	/// <returns>The graph from node.</returns>
	/// <param name="fromNode">From node.</param>
	public List<GameObject> GenerateGraphFromNode(WMG_Node fromNode) {
		// Given a starting node, generate a graph of nodes around the starting node
		// Returns the list of nodes and links composing the resulting graph
		List<GameObject> returnResults = new List<GameObject>();
		returnResults.Add(fromNode.gameObject);
		
		// Initialize various variables used in the algorithm
		GameObject[] nodes = new GameObject[numNodes];
		bool[] nodesProcessed = new bool[numNodes];
		GameObject curObj = fromNode.gameObject;
		int procNodeNum = 0;
		int numNodesProcessed = 0;
		int numNodesStarting = NodesParent.Count - 1;
		nodes[procNodeNum] = curObj;

		while (NodesParent.Count - numNodesStarting < numNodes) {
			
			WMG_Node procNode = nodes[procNodeNum].GetComponent<WMG_Node>();
			int numNeighbors = Random.Range(minRandomNumberNeighbors,maxRandomNumberNeighbors);
			if (debugRandomGraph) Debug.Log("Processesing Node: " + procNode.id + " with " + numNeighbors + " neighbors.");
			// Attempt to create a neighbor for the specified random number of neighbors
			for (int i = 0; i < numNeighbors; i++) {
				int curNeighborAttempt = 0;
				// For each neighbor, attempt to create the neighbor based on the maxNeighborAttempts
				while (curNeighborAttempt < maxNeighborAttempts) {
					// For this attempt, randomly generate an angle and length based on the specified parameters 
					float neighborAngle = Random.Range(minAngleRange,maxAngleRange);
					float neighborLength = Random.Range(minRandomLinkLength,maxRandomLinkLength);
					bool failedAttempt = false;
					
					if (debugRandomGraph) Debug.Log("Neighbor: " + i + " Attempt: " + curNeighborAttempt + " angle: " + Mathf.Round(neighborAngle));
					
					// Check to see that the randomly generated neighbor would not be too close to an existing neighbor (failure possibility #1)
					if (minAngle > 0) {
						for (int j = 0; j < procNode.numLinks; j++) {
							float angleDif = Mathf.Abs(procNode.linkAngles[j] - neighborAngle);
							if (angleDif > 180) angleDif = Mathf.Abs(angleDif - 360);
							if (angleDif < minAngle) {
								failedAttempt = true;
								break;
							}
						}
					}
					
					if (failedAttempt) {
						// Failed because random angle was smaller than the minAngle on either side of an existing neighbor
						if (debugRandomGraph) Debug.Log("Failed: Angle within minAngle of existing neighbor");
						curNeighborAttempt++;
						continue;
					}
					// Check if the randomly generated link intersects an existing link (failure possibility #2)
					if (noLinkIntersection) {
						float p1y = procNode.transform.localPosition.y + (neighborLength + procNode.radius) * Mathf.Sin(Mathf.Deg2Rad*neighborAngle);
						float p1x = procNode.transform.localPosition.x + (neighborLength + procNode.radius) * Mathf.Cos(Mathf.Deg2Rad*neighborAngle);
						float p2y = procNode.transform.localPosition.y + procNode.radius * Mathf.Sin(Mathf.Deg2Rad*neighborAngle);
						float p2x = procNode.transform.localPosition.x + procNode.radius * Mathf.Cos(Mathf.Deg2Rad*neighborAngle);
						foreach (GameObject child in LinksParent) {
							WMG_Link childLink = child.GetComponent<WMG_Link>();
							if (childLink.id == -1) continue; // Dummy editor link
							WMG_Node childLinkFrom = childLink.fromNode.GetComponent<WMG_Node>();
							WMG_Node childLinkTo = childLink.toNode.GetComponent<WMG_Node>();
							float p3y = childLinkFrom.transform.localPosition.y;
							float p3x = childLinkFrom.transform.localPosition.x;
							float p4y = childLinkTo.transform.localPosition.y;
							float p4x = childLinkTo.transform.localPosition.x;
							if (WMG_Util.LineSegmentsIntersect(p1x, p1y, p2x, p2y, p3x, p3y, p4x, p4y)) { // Links intersect
								if (debugRandomGraph) Debug.Log("Failed: Link intersected with existing link: " + childLink.id);
								failedAttempt = true;
								break;
							}
						}
					}
					if (failedAttempt) {
						// Failed because random link intersected an existing link
						curNeighborAttempt++;
						continue;
					}
					// Check if the randomly generated node intersects an existing node (failure possibility #3)
					if (noNodeIntersection) {
						float p1y = procNode.transform.localPosition.y + (neighborLength) * Mathf.Sin(Mathf.Deg2Rad*neighborAngle);
						float p1x = procNode.transform.localPosition.x + (neighborLength) * Mathf.Cos(Mathf.Deg2Rad*neighborAngle);
						foreach (GameObject child in NodesParent) {
							WMG_Node aNode = child.GetComponent<WMG_Node>();
							if (aNode.id == -1) continue; // Dummy editor node
							// Circles intersect if (R0-R1)^2 <= (x0-x1)^2+(y0-y1)^2 <= (R0+R1)^2
							if (Mathf.Pow((p1x - child.transform.localPosition.x),2) + Mathf.Pow((p1y - child.transform.localPosition.y),2) <= Mathf.Pow(2*(procNode.radius + noNodeIntersectionRadiusPadding),2)) {
								if (debugRandomGraph) Debug.Log("Failed: Node intersected with existing node: " + aNode.id);
								failedAttempt = true;
								break;
							}
						}
					}
					if (failedAttempt) {
						// Failed because random node intersected an existing node
						curNeighborAttempt++;
						continue;
					}
					// Check if the randomly generated link intersects an existing node
					if (noLinkNodeIntersection) {
						float p1y = procNode.transform.localPosition.y + (neighborLength + procNode.radius) * Mathf.Sin(Mathf.Deg2Rad*neighborAngle);
						float p1x = procNode.transform.localPosition.x + (neighborLength + procNode.radius) * Mathf.Cos(Mathf.Deg2Rad*neighborAngle);
						float p2y = procNode.transform.localPosition.y + procNode.radius * Mathf.Sin(Mathf.Deg2Rad*neighborAngle);
						float p2x = procNode.transform.localPosition.x + procNode.radius * Mathf.Cos(Mathf.Deg2Rad*neighborAngle);
						foreach (GameObject child in NodesParent) {
							WMG_Node aNode = child.GetComponent<WMG_Node>();
							if (procNode.id == aNode.id) continue; // Ignore the processesing node
							if (WMG_Util.LineIntersectsCircle(p1x, p1y, p2x, p2y, child.transform.localPosition.x, child.transform.localPosition.y, aNode.radius + noLinkNodeIntersectionRadiusPadding)) {
								if (debugRandomGraph) Debug.Log("Failed: Link intersected with existing node: " + aNode.id);
								failedAttempt = true;
								break;
							}
						}
					}
					if (failedAttempt) {
						// Failed because random link intersected an existing node
						curNeighborAttempt++;
						continue;
					}
					
					// Check if the randomly generated node intersects an existing link
					if (noLinkNodeIntersection) {
						float cy = procNode.transform.localPosition.y + (neighborLength + 2 * procNode.radius) * Mathf.Sin(Mathf.Deg2Rad*neighborAngle);
						float cx = procNode.transform.localPosition.x + (neighborLength + 2 * procNode.radius) * Mathf.Cos(Mathf.Deg2Rad*neighborAngle);
						foreach (GameObject child in LinksParent) {
							WMG_Link childLink = child.GetComponent<WMG_Link>();
							if (childLink.id == -1) continue; // Dummy editor link
							WMG_Node childLinkFrom = childLink.fromNode.GetComponent<WMG_Node>();
							WMG_Node childLinkTo = childLink.toNode.GetComponent<WMG_Node>();
							float p1y = childLinkFrom.transform.localPosition.y;
							float p1x = childLinkFrom.transform.localPosition.x;
							float p2y = childLinkTo.transform.localPosition.y;
							float p2x = childLinkTo.transform.localPosition.x;
							if (WMG_Util.LineIntersectsCircle(p1x, p1y, p2x, p2y, cx, cy, procNode.radius + noLinkNodeIntersectionRadiusPadding)) {
								if (debugRandomGraph) Debug.Log("Failed: Node intersected with existing link: " + childLink.id);
								failedAttempt = true;
								break;
							}
						}
					}
					if (failedAttempt) {
						// Failed because random node intersected an existing link
						curNeighborAttempt++;
						continue;
					}
					
					// The attempt did not fail, so create the node and the link and break out of the while attempt < maxAttempts loop
					curObj = CreateNode(nodePrefab, fromNode.transform.parent.gameObject);
					returnResults.Add(curObj);
					nodes[NodesParent.Count - numNodesStarting - 1] = curObj;
					
					float dx = Mathf.Cos(Mathf.Deg2Rad*neighborAngle)*neighborLength;
					float dy = Mathf.Sin(Mathf.Deg2Rad*neighborAngle)*neighborLength;
					curObj.transform.localPosition = new Vector3(procNode.transform.localPosition.x + dx, procNode.transform.localPosition.y + dy, 0);
					
					returnResults.Add(CreateLink(procNode, curObj, linkPrefab, null));
					break;
				}
				if (NodesParent.Count - numNodesStarting == numNodes) break; // Max number nodes specified was reached, we are done generating the graph
			}
			// Set the node as processed and increment the processed node counter
			nodesProcessed[procNodeNum] = true;
			numNodesProcessed++;
			// Process the oldest node added as the next node to process
			if (centerPropogate) { 
				procNodeNum++;
			}
			// Pick a random node as the next node to process from the nodes that have been created from this algorithm
			else { 
				int numPossibleProcNodes = NodesParent.Count - numNodesStarting - numNodesProcessed;
				if (numPossibleProcNodes > 0) {
					int[] possibleProcNodes = new int[numPossibleProcNodes];
					int j = 0;
					for (int i = 0; i < numNodes; i++) {
						if (!nodesProcessed[i] && i < NodesParent.Count - numNodesStarting) {
							possibleProcNodes[j] = i;
							j++;
						}
					}
					procNodeNum = possibleProcNodes[Random.Range(0,j-1)];
				}
			}
			
			// This happens (algorithm ends prematurely) when maxNeighborAttempts was reached for the starting node or all the nodes created
			if (NodesParent.Count - numNodesStarting == numNodesProcessed) { // Case where all nodes have been processed, but number nodes specfied were not created
				Debug.Log("WMG - Warning: Only generated " + (NodesParent.Count - numNodesStarting - 1) + " nodes with the given parameters.");
				break;
			}
		}
		
		
		return returnResults;
	}
	

}
