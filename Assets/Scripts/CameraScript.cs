using UnityEngine;
using System.Collections;
using System;

public class CameraScript : MonoBehaviour {

    public Camera cameraObject;
	public bool posOffsetLocal;
	public Vector3 cameraPosOffset;
	public bool lookAtOffsetLocal;
	public float moveSpeed;
	public float nodePlayerDiv;

	Vector3 cameraLookAtOffset;
    PlayerController player;
	Vector3 newPos;
	Vector3 target;

	Vector3 nextNodePos;
	Transform playerT;

	bool follow;

    public void Init(Vector3 position, Quaternion rotation, float gameStartCamSize, PlayerController player) {

        transform.position = position;
        transform.rotation = rotation;
        cameraObject.orthographicSize = gameStartCamSize;

        this.player = player;
        playerT = player.transform;
        nextNodePos = GameObject.Find("Node0").transform.position;
        follow = true;
    }

    void FixedUpdate () {
		if (follow) {
			CameraMovement();
		}
	}

	void CameraMovement () {

		nextNodePos = player.GetNextNodePos();

		if (posOffsetLocal) {
			newPos = playerT.position + (playerT.right * cameraPosOffset.x) + (playerT.up * cameraPosOffset.y) + (playerT.forward * cameraPosOffset.z);
		} else {
			newPos = playerT.position + cameraPosOffset;
		}

		if (lookAtOffsetLocal) {
			target = playerT.position + ((nextNodePos - playerT.position).normalized * (nextNodePos - playerT.position).magnitude / nodePlayerDiv);
		} else {
			target = playerT.position + ((nextNodePos - playerT.position).normalized * (nextNodePos - playerT.position).magnitude / nodePlayerDiv);
		}

		newPos = (target - playerT.position) + newPos;
		transform.position = Vector3.Slerp(transform.position, newPos, moveSpeed * Time.deltaTime);
	}
}
