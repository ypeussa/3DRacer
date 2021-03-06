﻿using UnityEngine;

public class CameraSystem : MonoBehaviour {
    public Camera topDownCamera, firstPersonCamera;
    public Vector3 cameraPosOffset;
    public float moveSpeed;
    public float nodePlayerDiv;

    PlayerCar player;
    Vector3 nextNodePos;
    Transform playerT;
    bool follow;

    public void Init(Vector3 position, Quaternion rotation, float gameStartCamSize, PlayerCar player) {

        transform.position = position;
        transform.rotation = rotation;
        topDownCamera.orthographicSize = gameStartCamSize;

        this.player = player;
        playerT = player.transform;
        follow = true;
    }

    void FixedUpdate() {
        if (follow) {
            CameraMovement();
        }
    }

    void Update() {
        if (follow && Input.GetButtonDown("CameraMode")) {
            if (topDownCamera.gameObject.activeSelf) {
                topDownCamera.gameObject.SetActive(false);
                firstPersonCamera.gameObject.SetActive(true);

                firstPersonCamera.transform.SetParent(player.firstPersonCameraPosition, false);
                firstPersonCamera.transform.localPosition = Vector3.zero;
                firstPersonCamera.transform.localRotation = Quaternion.identity;

            } else {
                topDownCamera.gameObject.SetActive(true);
                firstPersonCamera.gameObject.SetActive(false);

                firstPersonCamera.transform.parent = transform;
            }

        }
    }

    void CameraMovement() {
        nextNodePos = player.lapSystem.GetNextNodePos();
        Vector3 newPos = playerT.position + cameraPosOffset;

        Vector3 target = playerT.position + ((nextNodePos - playerT.position).normalized * (nextNodePos - playerT.position).magnitude / nodePlayerDiv);

        newPos = (target - playerT.position) + newPos;
        transform.position = Vector3.Slerp(transform.position, newPos, moveSpeed * Time.fixedDeltaTime);
    }
}
