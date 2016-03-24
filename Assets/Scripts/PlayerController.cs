using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public float speed;
	public float turnAngle;
	public List<WheelCollider> tires;
	List<GameObject> tireModels;
	GameObject[] nodes;
	Vector3 nextNodePos;

	Rigidbody rb;
	public Transform centerOfMassEmpty;

	int nodeIndex;

	Vector3 playerDeltaPos;

	void Start () {
		nodes = GameObject.FindGameObjectsWithTag("Node");
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = centerOfMassEmpty.localPosition;

		tireModels = new List<GameObject>();
		for (int i = 0; i < tires.Count; i++) {
			tireModels.Add(tires[i].transform.Find("Tire Model").gameObject);
		}
	}

	void Update () {
		Controls();
	}

	void Controls () {
		for (int i = 0; i < tires.Count; i++) {
			tires[i].motorTorque = speed * Input.GetAxis("Vertical");
		}

		for (int i = 0; i < tires.Count / 2; i++) {
			tires[i].steerAngle = turnAngle * Input.GetAxis("Horizontal");
			tireModels[i].transform.localRotation = Quaternion.Euler(0, turnAngle * Input.GetAxis("Horizontal"), 90);
		}
	}
	public void PassedNode (Vector3 nodePos) {
		if (nodeIndex < nodes.Length - 1) {
			nodeIndex++;
		} else {
			nodeIndex = 0;
		}
		nextNodePos = nodePos;
	}

	public Vector3 GetNextNodePos () {
		return nextNodePos;
	}

	public int GetNodeIndex () {
		return nodeIndex;
	}

	public float GetDistanceToNode () {
		return Vector3.Distance(transform.position, nextNodePos);
	}
}
