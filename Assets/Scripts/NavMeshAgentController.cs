using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavMeshAgentController : MonoBehaviour {

	public List<Transform> nodes;
	NavMeshAgent agent;
	int nodeIndex;

	void Start () {
		agent = GetComponent<NavMeshAgent>();
		SetNextPath();
	}

	public void SetNextPath () {
		float randomX = Random.Range(0, 5);
		agent.destination = nodes[nodeIndex].position + (nodes[nodeIndex].right * randomX);
		if (nodeIndex < nodes.Count - 1) {
			nodeIndex++;
		} else {
			nodeIndex = 0;
		}
	}
	public int GetNodeIndex () {
		return nodeIndex;
	}
}
