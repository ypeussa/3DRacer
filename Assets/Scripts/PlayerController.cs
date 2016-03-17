using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public bool followingCamera;
	public float speed;
	public float turnAngle;
	public float scrollSpeed;

	public List<WheelCollider> tires;
	List<GameObject> tireModels;

	Rigidbody rb;
	public Transform centerOfMassEmpty;

	GameObject mainCam;
	Vector3 playerLastPos;
	Vector3 playerDeltaPos;

	void Start () {
		mainCam = GameObject.FindGameObjectWithTag("MainCamera");
		playerLastPos = transform.position;
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = centerOfMassEmpty.localPosition;

		tireModels = new List<GameObject>();
		for (int i = 0; i < tires.Count; i++) {
			tireModels.Add(tires[i].transform.Find("Tire Model").gameObject);
		}
	}

	void Update () {

		Controls();
		if (followingCamera)
		CameraFollow();

		if (Input.GetKey(KeyCode.LeftShift)) {
			speed += Input.mouseScrollDelta.y * scrollSpeed;
		} else {
			turnAngle += Input.mouseScrollDelta.y * scrollSpeed;
		}
	}

	void Controls () {

		if (Input.GetKey(KeyCode.W)) {
			for (int i = 0; i < tires.Count; i++) {
				tires[i].motorTorque = speed * Input.GetAxis("Vertical");
			}
		}
		if (Input.GetKey(KeyCode.S)) {
			for (int i = 0; i < tires.Count; i++) {
				tires[i].motorTorque = speed * Input.GetAxis("Vertical");
			}
		}

		if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.W)) {
			for (int i = 0; i < tires.Count; i++) {
				tires[i].motorTorque = 0;
			}
		}

		if (Input.GetKey(KeyCode.A)) {
			for (int i = 0; i < tires.Count / 2; i++) {
				tires[i].steerAngle = turnAngle * Input.GetAxis("Horizontal");
				tireModels[i].transform.localRotation = Quaternion.Euler(0, turnAngle * Input.GetAxis("Horizontal"), 90);
			}
		}
		if (Input.GetKey(KeyCode.D)) {
			for (int i = 0; i < tires.Count / 2; i++) {
				tires[i].steerAngle = turnAngle * Input.GetAxis("Horizontal");
				tireModels[i].transform.localRotation = Quaternion.Euler(0, turnAngle * Input.GetAxis("Horizontal"), 90);
			}
		}

		if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
			for (int i = 0; i < tires.Count / 2; i++) {
				tires[i].steerAngle = 0;
			}
		}
	}
	
	void CameraFollow () {
		playerDeltaPos = transform.position - playerLastPos;
		mainCam.transform.position += playerDeltaPos;

		playerLastPos = transform.position;
	}
}
