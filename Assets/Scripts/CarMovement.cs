using UnityEngine;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour
{
    public float acceleration;
    public float maxTurnAngle;
    public List<WheelCollider> tires;
    public Transform centerOfMass;

    List<GameObject> tireModels;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
    }

    void Start()
    {
        tireModels = new List<GameObject>();
        for (int i = 0; i < tires.Count; i++)
        {
            tireModels.Add(tires[i].transform.Find("Tire Model").gameObject);
        }
    }
    public void Accelerate(float accelerationAxis) {
        for (int i = 0; i < tires.Count; i++) {
            tires[i].motorTorque = acceleration * accelerationAxis;
            tireModels[i].transform.localRotation *= Quaternion.AngleAxis(tires[i].rpm / 60 * 360 * Time.deltaTime, Vector3.right);
        }
    }
    
    public void Turn(float turnAxis) {
        for (int i = 0; i < 2; i++)
        {
            tires[i].steerAngle = maxTurnAngle * turnAxis;
            var euler = tireModels[i].transform.localRotation.eulerAngles;
            if (euler.z == 180 && euler.y != 0) euler.x = 180 - euler.x;//weird fix for a weird problem
            tireModels[i].transform.localRotation = Quaternion.Euler(euler.x, maxTurnAngle * turnAxis, 0);
        }
    }
}
