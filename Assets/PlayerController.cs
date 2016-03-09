using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;
	public float turnSpeed;
	Vector3 playerDir;
	Quaternion playerTurn;
	Rigidbody rb;

	bool grounded;

	GameObject mainCam;
	Vector3 mainCamStartPos;

	void Start () {
		mainCam = GameObject.FindGameObjectWithTag("MainCamera");
		mainCamStartPos = mainCam.transform.position;
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = Vector3.zero;
	}

	void Update () {
		if (grounded) {
			Controls();
		}
		CameraFollow();
	}

	void Controls () {

		Vector3 newDir = Vector3.zero;
		float newTurnY = 0;

		if (Input.GetKey(KeyCode.A)) {
			newTurnY--;
		}
		if (Input.GetKey(KeyCode.D)) {
			newTurnY++;
		}
		if (Input.GetKey(KeyCode.W)) {
			newDir += transform.forward;
		}
		if (Input.GetKey(KeyCode.S)) {
			newDir += -transform.forward;
		}
		playerDir = newDir.normalized * speed * Time.deltaTime;
		//playerTurn = Quaternion.Euler(0, newTurnY * turnSpeed * Time.deltaTime, 0);
		rb.AddTorque(0, newTurnY * turnSpeed * Time.deltaTime, 0, ForceMode.Force);
		rb.AddForce(playerDir);
	}
	
	void CameraFollow () {
		mainCam.transform.position = mainCamStartPos + transform.position;
	}

	void OnCollisionStay (Collision c) {
		grounded = true;
		print(grounded);
	}
	void OnCollisionExit (Collision c) {
		grounded = false;
		print(grounded);
	}
}
