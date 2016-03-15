using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour {

	public List<Transform> allNodes;
	enum Nodes { n1, n2, n3, n4};
	Nodes nextNode;

	public int nextNodeIndex;

	NavMeshPath path;

	//Vector3 npcCurrentForward;
	public Vector3 nextNodePos;
	public Vector3 nextPathPoint;

	bool start;

	void Start () {
		nextNodePos = allNodes[0].position;
		start = true;
		path = new NavMeshPath();
		
	}

	void Update () {
		if (Vector3.Distance(transform.position, nextNodePos) < 5f) {
			if (nextNode == Nodes.n1) {
				nextNode = Nodes.n2;
				nextNodePos = allNodes[1].position;
				nextNodeIndex = 1;
			} else if (nextNode == Nodes.n2) {
				nextNode = Nodes.n3;
				nextNodePos = allNodes[2].position;
				nextNodeIndex = 2;
			} else if (nextNode == Nodes.n3) {
				nextNode = Nodes.n4;
				nextNodePos = allNodes[3].position;
				nextNodeIndex = 3;
			} else if (nextNode == Nodes.n4) {
				nextNode = Nodes.n1;
				nextNodePos = allNodes[0].position;
				nextNodeIndex = 0;
			}
		}
	}

	void OnDrawGizmos () {
		if (start) {
			SearchNextPoint();
		}
	}

	void SearchNextPoint () {
		NavMesh.CalculatePath(transform.position, allNodes[nextNodeIndex].position, NavMesh.AllAreas, path);
		nextPathPoint = path.corners[0];
		Gizmos.DrawSphere (path.corners[1], 1f);
		for (int i = 0; i < path.corners.Length-1; i++) {
			Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
		}
	}
}
