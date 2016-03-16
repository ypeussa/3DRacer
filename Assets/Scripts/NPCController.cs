using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour {

	public float speed;
	public float maxSteerAngle;
	public float steerLerp;
	float lastSteerAngle;
	float finalSteerAngle;
	public List<WheelCollider> tires;
	List<GameObject> tireModels;

	public List<Transform> allNodes;
	GameObject[] nodesArray;
	public int nextNodeIndex = -1;
	Vector3 nextNodePos;
	public float nodePosMaxOffset;
	Vector3 nextPathPoint;
	Vector3 posToPathPoint;

	public float searchPathInterval;
	float pathTimer;

	NavMeshPath path;

	Vector3 lastPos;
	public float deltaSpeed;

	bool start;

	void Start () {
		path = new NavMeshPath();
		AddNodesToList();
		SetNodePos(allNodes[0].GetComponent<BoxCollider>().size.x);
		SearchNextPoint();
		start = true;

		tireModels = new List<GameObject>();
		for (int i = 0; i < tires.Count; i++) {
			tireModels.Add(tires[i].transform.Find("Tire Model").gameObject);
		}
	}

	void Update () {
		NPCMovement();
	}

	void FixedUpdate () {
		deltaSpeed = (transform.position - lastPos).magnitude;
		lastPos = transform.position;
	}

	void NPCMovement () {

		pathTimer -= Time.deltaTime;
		if (pathTimer < 0) {
			SearchNextPoint();
			pathTimer = searchPathInterval;
		}
		
		posToPathPoint = nextPathPoint - transform.position;

		float tireAngle = Vector3.Angle(posToPathPoint, transform.forward);
		float nodeSideDot = Vector3.Dot(posToPathPoint.normalized, transform.right);
		float nodeFrontDot = Vector3.Dot(posToPathPoint.normalized, transform.forward);
		finalSteerAngle = Mathf.Lerp(lastSteerAngle, tireAngle * nodeSideDot, steerLerp);
		finalSteerAngle = Mathf.Clamp(finalSteerAngle, -maxSteerAngle, maxSteerAngle);

		for (int i = 0; i < tires.Count / 2; i++) {
			if (nodeFrontDot > 0.1f) {
				tires[i].steerAngle = finalSteerAngle;
				tireModels[i].transform.localRotation = Quaternion.Euler(0, finalSteerAngle, 90);
			} else if (nodeFrontDot < -0.1f) {
				tires[i].steerAngle = -finalSteerAngle;
				tireModels[i].transform.localRotation = Quaternion.Euler(0, -finalSteerAngle, 90);
			} else {
				tires[i].steerAngle = finalSteerAngle;
				tireModels[i].transform.localRotation = Quaternion.Euler(0, -finalSteerAngle, 90);
			}
		}

		for (int i = 0; i < tires.Count; i++) {
			if (nodeFrontDot > 0.1f) {
				tires[i].motorTorque = speed * nodeFrontDot;
			} else if (nodeFrontDot < -0.1f) {
				tires[i].motorTorque = -speed * (1 - nodeFrontDot);
			} else {
				tires[i].motorTorque = speed;
			}
		}
		lastSteerAngle = finalSteerAngle;
	}

	void OnDrawGizmos () {
		// Called in OnDrawGizmos for visualization
		if (start) {
			if (path.corners.Length > 1) {
				Gizmos.DrawSphere(path.corners[1], 1f);
			} else if (path.corners.Length > 0) {
				Gizmos.DrawSphere(path.corners[0], 1f);
			}
			for (int i = 0; i < path.corners.Length - 1; i++) {
				Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
			}
		}
	}

	public void SetNodePos (float nodeWidght) {
		if (allNodes.Count - 1 == nextNodeIndex) {
			nextNodeIndex = 0;
		} else {
			nextNodeIndex++;
		}
		
		float randomX = Random.Range(-nodePosMaxOffset, nodePosMaxOffset);
		randomX = Mathf.Clamp(randomX, -nodeWidght / 2, nodeWidght / 2);
		nextNodePos = allNodes[nextNodeIndex].position + allNodes[nextNodeIndex].right * randomX;
	}

	void SearchNextPoint () {
		if (GetComponent<NavMeshObstacle>().isActiveAndEnabled) {
			NavMesh.CalculatePath(transform.position + (posToPathPoint.normalized * 2), nextNodePos, NavMesh.AllAreas, path);
		} else {
			NavMesh.CalculatePath(transform.position, nextNodePos, NavMesh.AllAreas, path);
		}
		if (path.corners.Length > 1) {
			nextPathPoint = path.corners[1];
		} else if (path.corners.Length == 1) {
			nextPathPoint = path.corners[0];
		}
	}


	void AddNodesToList () {
		nodesArray = GameObject.FindGameObjectsWithTag("Node");
		allNodes.Capacity = nodesArray.Length;
		for (int i = 0; i < nodesArray.Length; i++) {
			allNodes.Add(nodesArray[i].transform);
		}

		for (int i = 0; i < nodesArray.Length; i++) {
			int x = int.Parse(nodesArray[i].name.Remove(0, 4));
			allNodes.RemoveAt(x);
			allNodes.Insert(x, nodesArray[i].transform);
		}
	}
}
