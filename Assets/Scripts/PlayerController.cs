using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerController : MonoBehaviour {
    public string CarDeveloperName = "Nobody";
	public float speed;
	public float turnAngle;
	public List<WheelCollider> tires;
	public Transform centerOfMassEmpty;

    List<GameObject> tireModels;
	GameObject[] nodes;
	Vector3 nextNodePos;
	Rigidbody rb;
	Vector3 playerDeltaPos;

	int nodeIndex;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMassEmpty.localPosition;
    }

    void Start () {
		nodes = GameObject.FindGameObjectsWithTag("Node");
		tireModels = new List<GameObject>();
		for (int i = 0; i < tires.Count; i++) {
			tireModels.Add(tires[i].transform.Find("Tire Model").gameObject);
		}
	}

	void Update () {
        //input acceleration
		for (int i = 0; i < tires.Count; i++) {
			tires[i].motorTorque = speed * Input.GetAxis("Vertical");
		}

        //input turning
		for (int i = 0; i < tires.Count / 2; i++) {
			tires[i].steerAngle = turnAngle * Input.GetAxis("Horizontal");
			tireModels[i].transform.localRotation = Quaternion.Euler(0, turnAngle * Input.GetAxis("Horizontal"), 90);
		}
	}

	public void PassedNode (Vector3 nextNodePosition) {
		if (nodeIndex < nodes.Length - 1) {
			nodeIndex++;
		} else {
			nodeIndex = 0;
		}
		nextNodePos = nextNodePosition;
	}

    public void SetAsSelectionMenuCar(Vector3 position, Quaternion rotation) {
        transform.localPosition = position;
        transform.rotation = rotation;

        enabled = false;
        rb.useGravity = false;
        gameObject.layer = LayerMask.NameToLayer("UI");

        var children = transform.GetComponentsInChildren<Transform>();

        foreach (var child in children) {
            child.gameObject.layer = gameObject.layer;
        }
    }

    public Vector3 GetNextNodePos () {
		return nextNodePos;
	}

	public int GetNodeIndex () {
		return nodeIndex;
	}
}
