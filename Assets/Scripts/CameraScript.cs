﻿using UnityEngine;

public class CameraScript : MonoBehaviour {
    public Camera cameraObject;
    public Vector3 cameraPosOffset;
    public float moveSpeed;
    public float nodePlayerDiv;

    Vector3 cameraLookAtOffset;
    PlayerController player;

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

    void FixedUpdate() {
        if (follow) {
            CameraMovement();
        }
    }

    void CameraMovement() {
        nextNodePos = player.GetNextNodePos();

        Vector3 newPos = playerT.position + cameraPosOffset;

        //no lerp
        //transform.position = newPos;
        //return;

        Vector3 target = playerT.position + ((nextNodePos - playerT.position).normalized * (nextNodePos - playerT.position).magnitude / nodePlayerDiv);

        newPos = (target - playerT.position) + newPos;
        transform.position = Vector3.Slerp(transform.position, newPos, moveSpeed * Time.fixedDeltaTime);
    }
}
