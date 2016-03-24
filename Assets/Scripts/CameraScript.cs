using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	GameObject player;

	public bool posOffsetLocal;
	public Vector3 cameraPosOffset;
	public bool lookAtOffsetLocal;
	Vector3 cameraLookAtOffset;
	public float moveSpeed;
	public float nodePlayerDiv;

	Vector3 newPos;
	Vector3 target;

	Vector3 nextNodePos;
	Transform playerT;

	bool follow;

	void FixedUpdate () {
		if (follow) {
			CameraMovement();
		}
	}

	void CameraMovement () {

		nextNodePos = player.GetComponent<PlayerController>().GetNextNodePos();

		if (posOffsetLocal) {
			newPos = playerT.position + (playerT.right * cameraPosOffset.x) + (playerT.up * cameraPosOffset.y) + (playerT.forward * cameraPosOffset.z);
		} else {
			newPos = playerT.position + cameraPosOffset;
		}

		if (lookAtOffsetLocal) {
			target = playerT.position + ((nextNodePos - playerT.position).normalized * (nextNodePos - playerT.position).magnitude / nodePlayerDiv);
			//target += (playerT.right * cameraLookAtOffset.x) + (playerT.up * cameraLookAtOffset.y) + (playerT.forward * cameraLookAtOffset.z);
		} else {
			// target = point between player and next node + offset
			target = playerT.position + ((nextNodePos - playerT.position).normalized * (nextNodePos - playerT.position).magnitude / nodePlayerDiv);
			//target += cameraLookAtOffset;
		}

		/*
		Quaternion targetRot = Quaternion.LookRotation(target - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, lookSpeed * Time.deltaTime);
		*/

		newPos = (target - playerT.position) + newPos;
		transform.position = Vector3.Slerp(transform.position, newPos, moveSpeed * Time.deltaTime);
	}

	public void CameraFollow (bool b) {
		player = GameObject.FindGameObjectWithTag("Player");
		playerT = player.transform;
		nextNodePos = GameObject.Find("Node0").transform.position;
		follow = b;
	}
}
