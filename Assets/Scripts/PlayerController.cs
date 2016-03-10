using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public float speed;
	public float turnAngle;
	public float scrollSpeed;

	public List<WheelCollider> tires;

	Rigidbody rb;
	public Transform centerOfMassEmpty;

	GameObject mainCam;
	Vector3 mainCamStartPos;

	void Start () {
		mainCam = GameObject.FindGameObjectWithTag("MainCamera");
		mainCamStartPos = mainCam.transform.position;
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = centerOfMassEmpty.localPosition;
	}

	void Update () {

		Controls();
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
				tires[i].motorTorque = speed;
			}
		}
		if (Input.GetKey(KeyCode.S)) {
			for (int i = 0; i < tires.Count; i++) {
				tires[i].motorTorque = -speed;
			}
		}

		if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.W)) {
			for (int i = 0; i < tires.Count; i++) {
				tires[i].motorTorque = 0;
			}
		}

		if (Input.GetKey(KeyCode.A)) {
			for (int i = 0; i < tires.Count / 2; i++) {
				tires[i].steerAngle = -turnAngle;
			}
		}
		if (Input.GetKey(KeyCode.D)) {
			for (int i = 0; i < tires.Count / 2; i++) {
				tires[i].steerAngle = turnAngle;
			}
		}

		if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
			for (int i = 0; i < tires.Count / 2; i++) {
				tires[i].steerAngle = 0;
			}
		}
	}
	
	void CameraFollow () {
		mainCam.transform.position = mainCamStartPos + transform.position;
	}
}
