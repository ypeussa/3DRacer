using UnityEngine;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour {
    public float acceleration;
    public float maxTurnAngle;
    public List<WheelCollider> tires;
    public Transform centerOfMass;

    List<GameObject> tireModels;
    Rigidbody rb;
    float turnAngle;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
    }

    void Start() {
        tireModels = new List<GameObject>();
        for (int i = 0; i < tires.Count; i++) {
            tireModels.Add(tires[i].transform.Find("Tire Model").gameObject);
        }
    }

    private void Update() {
        //rotate all wheels based on speed
        for (int i = 0; i < tires.Count; i++) {
            tireModels[i].transform.localRotation *= Quaternion.AngleAxis(tires[i].rpm / 60 * 360 * Time.deltaTime, Vector3.right);
        }

        //turn front wheels based in input
        for (int i = 0; i < 2; i++) {
            var euler = tireModels[i].transform.localRotation.eulerAngles;
            if (euler.z == 180 && euler.y != 0) euler.x = 180 - euler.x;//weird fix for a weird problem
            tireModels[i].transform.localRotation = Quaternion.Euler(euler.x, turnAngle, 0);
        }

    }

    public void Accelerate(float accelerationAxis) {
        for (int i = 0; i < tires.Count; i++) {
            tires[i].motorTorque = acceleration * accelerationAxis;
        }
    }

    public void Turn(float turnAxis) {
        turnAngle = maxTurnAngle * turnAxis;

        for (int i = 0; i < 2; i++) {
            tires[i].steerAngle = turnAngle;
        }

    }
}
