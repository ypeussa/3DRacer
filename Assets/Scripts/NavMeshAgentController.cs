using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavMeshAgentController : MonoBehaviour {

	public float randomNodeOffset;
	public float speedChangeInterval;
	public float intervalMinMaxOffset;
	float intervalOffset;
	public Vector2 minMaxSpeed;
	float speedTimer;

	List<Transform> nodes;
	NavMeshAgent agent;
	int nodeIndex;
	int currentLap;

	//[HideInInspector]
	//public bool aheadOfPlayer;

	void Awake() {
		InitializeNodesList();
		agent = GetComponent<NavMeshAgent>();
	}

	void Update () {
		RandomizeSpeed();
	}

	void InitializeNodesList () {
		nodes = new List<Transform>();
		for (int i = 0; i < GameObject.FindGameObjectsWithTag("Node").Length; i++) {
			string n = "Node" + i;
			GameObject node = GameObject.Find(n);
			nodes.Add(node.transform);
		}
	}

	void RandomizeSpeed () {
		speedTimer += Time.deltaTime;
		if (speedTimer > speedChangeInterval + intervalOffset) {
			speedTimer = 0;
			intervalOffset = Random.Range(-intervalMinMaxOffset, intervalMinMaxOffset);
			agent.speed = Random.Range(minMaxSpeed.x, minMaxSpeed.y);
		}
	}

	public void SetNextPath () {
		float randomX = Random.Range(-randomNodeOffset, randomNodeOffset);
		agent.SetDestination(nodes[nodeIndex].position + (nodes[nodeIndex].right * randomX));
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

	// TESTING
	void OnTriggerEnter (Collider c) {
		if (c.tag == "Player") {
			//agent.velocity += c.GetComponent<Rigidbody>().velocity;
		}
	}
}
