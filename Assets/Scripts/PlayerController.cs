using UnityEngine;

[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarLapSystem))]
public class PlayerController : MonoBehaviour {
    public Transform firstPersonCameraPosition;

    public CarLapSystem lapSystem { get; private set; }
    CarMovement carController;

    void Awake() {
        carController = GetComponent <CarMovement>();
        lapSystem = GetComponent<CarLapSystem>();
    }

    void Update() {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        carController.Accelerate(verticalAxis);
        carController.Turn(horizontalAxis);
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
