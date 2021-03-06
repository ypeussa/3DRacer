﻿using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarLapSystem))]
public class PlayerCar : MonoBehaviour {
    public Transform firstPersonCameraPosition;

    public CarLapSystem lapSystem { get; private set; }
    CarMovement carController;

    void Awake() {
        carController = GetComponent <CarMovement>();
        lapSystem = GetComponent<CarLapSystem>();
    }

    void Update() {
        float verticalAxis = Input.GetAxisRaw("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        carController.Accelerate(verticalAxis);
        carController.Turn(horizontalAxis);
    }

    public RandomAudioPlayer SelectedAudio;

    public void SelectCar() {
        if (SelectedAudio) SelectedAudio.Play();
    }

    public void SetAsSelectionMenuCar(Vector3 position, Quaternion rotation) {
        transform.localPosition = position;
        transform.rotation = rotation;

        enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        gameObject.layer = LayerMask.NameToLayer("UI");

        var children = transform.GetComponentsInChildren<Transform>();

        foreach (var child in children) {
            child.gameObject.layer = gameObject.layer;
        }
    }

}
