using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CarController))]
public class PlayerController : MonoBehaviour {
    public string CarDeveloperName = "Nobody";
    public Transform firstPersonCameraPosition;

    CarController carController;
    GameObject[] nodes;
    Vector3 nextNodePos;
    int nodeIndex;

    void Awake() {
        carController = GetComponent <CarController>();
    }

    void Start() {
        nodes = GameObject.FindGameObjectsWithTag("Node");
    }

    void Update() {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        carController.Accelerate(verticalAxis);
        carController.Turn(horizontalAxis);
    }

    public void PassNode(Vector3 nextNodePosition) {
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
        GetComponent<Rigidbody>().useGravity = false;
        gameObject.layer = LayerMask.NameToLayer("UI");

        var children = transform.GetComponentsInChildren<Transform>();

        foreach (var child in children) {
            child.gameObject.layer = gameObject.layer;
        }
    }

    public Vector3 GetNextNodePos() {
        return nextNodePos;
    }

    public int GetNodeIndex() {
        return nodeIndex;
    }
}
