using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavMeshAgentController : MonoBehaviour {

	public List<Transform> nodes;
	NavMeshAgent agent;
	int nodeIndex;
	int currentLap;

	//[HideInInspector]
	//public bool aheadOfPlayer;

	void Start () {
		agent = GetComponent<NavMeshAgent>();
	}

	public void SetNextPath () {
		float randomX = Random.Range(0, 5);
		agent.SetDestination(nodes[nodeIndex].position + (nodes[nodeIndex].right * randomX));
		//agent.destination = nodes[nodeIndex].position + (nodes[nodeIndex].right * randomX);
		if (nodeIndex < nodes.Count - 1) {
			nodeIndex++;
		} else {
			nodeIndex = 0;
		}
	}

	public void UpdateLap () {
		currentLap++;
	}

	public int GetCurrentLap () {
		return currentLap;
	}

	public int GetNodeIndex () {
		return nodeIndex;
	}

	public float GetDistanceToNode () {
		return Vector3.Distance(transform.position, nodes[nodeIndex].position);
	}
}
