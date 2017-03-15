﻿using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Unity navmesh agent runs in update. We have to use fixed update for smooth movement as the player car and the camera both use it as well.
/// This required a bit of wrangling.
/// </summary>
[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(CarLapSystem))]
public class NavMeshAgentController : MonoBehaviour {
    public float decreaseAccelerationOnImpactThreshold = 2f;
    public float disableInputOnImpactThreshold = 10f;
    public float PathRefreshInterval = 1f;

    public CarLapSystem lapSystem { get; private set; }
    CarMovement carController;
    Rigidbody rb;
    public int nodeIndex;
    float disableTimer, lowerSpeedTimer;
    float maxAcceleration;
    Vector3 targetPosition;
    NavMeshPath path;
    int pathIndex = 0;
    float pathRefreshTimer = 0;
    float reverseRaycastLength = 4f;
    bool reverse = false;

    void Awake() {
        //get components
        rb = GetComponent<Rigidbody>();
        carController = GetComponent<CarMovement>();
        lapSystem = GetComponent<CarLapSystem>();
        path = new NavMeshPath();
        maxAcceleration = carController.acceleration;
    }

    public void MultiplyMaxAcceleration(float multiplier) {
        carController.acceleration *= multiplier;
        maxAcceleration = carController.acceleration;
    }

    void FixedUpdate() {
        //impact timers
        if (disableTimer > 0) {
            disableTimer -= Time.deltaTime;

            if (disableTimer <= 0) {
                carController.acceleration = maxAcceleration;
                pathRefreshTimer = 0;
            }
            return;
        } else if (lowerSpeedTimer > 0) {
            lowerSpeedTimer -= Time.deltaTime;

            if (lowerSpeedTimer <= 0) {
                carController.acceleration = maxAcceleration;
            }
        }

        if (pathRefreshTimer < Time.time) {
            SetPath(targetPosition);
            pathRefreshTimer = Time.time + PathRefreshInterval;
        }

        if (path != null && path.corners.Length > 1) {

#if UNITY_EDITOR
            for (int i = 0; i < path.corners.Length - 1; i++) {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }

            Debug.DrawRay(transform.position + Vector3.one * 0.5f, transform.forward * reverseRaycastLength, Color.magenta);
#endif

            bool obstacleInFront = Physics.Raycast(transform.position + Vector3.one * 0.5f, transform.forward, reverseRaycastLength,
                1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Car"));
            if (!reverse && rb.velocity.magnitude < 1f && obstacleInFront)
                reverse = true;
            if (reverse && !obstacleInFront)
                reverse = false;

            var pathPosition = path.corners[pathIndex];
            var directionVector = pathPosition - transform.position;
            directionVector.y = 0;
            directionVector.Normalize();

            if (!reverse) {
                var pathNormal = Vector3.zero;
                if (pathIndex == 0)
                    pathNormal = pathPosition - path.corners[1];
                else
                    pathNormal = path.corners[pathIndex - 1] - path.corners[pathIndex];
                pathNormal.Normalize();

                //check if next path corner reached
                if (CheckPlaneDistance(pathPosition, pathNormal, transform.position, rb.velocity.magnitude * 0.5f)) {
                    pathIndex++;

                    //last corner -> set new target
                    if (pathIndex == path.corners.Length) {
                        SetPathToNextNode();
                    }
                }
            }

            float turnDirection = Vector3.Angle(transform.forward, directionVector.normalized) * Mathf.Sign(Vector3.Cross(transform.forward, directionVector.normalized).y);
            turnDirection = Mathf.Clamp(turnDirection, -carController.maxTurnAngle, carController.maxTurnAngle) / carController.maxTurnAngle;

            float acceleration = 1;

            if (reverse) {
                acceleration = -1;
                turnDirection = -1;//situational. Reverse should check which turning direction is preferable.
                reverseRaycastLength = 8f;//delicious magic numbers!
            } else
                reverseRaycastLength = 4f;

            carController.Turn(turnDirection);
            carController.Accelerate(acceleration);
        }
    }

    bool CheckPlaneDistance(Vector3 planePosition, Vector3 planeNormal, Vector3 position, float distance) {

        var direction = planePosition - position;
#if UNITY_EDITOR
        Debug.DrawRay(planePosition + Vector3.one, planeNormal * 5f, Color.green);
        Debug.DrawRay(position + Vector3.one, direction.normalized * 5f, Color.yellow);
#endif
        return Mathf.Abs(Vector3.Dot(direction, planeNormal)) < distance;
    }

    void SetPath(Vector3 position) {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position + transform.forward, out hit, 4, 1)) {
            NavMesh.CalculatePath(hit.position, position, 1, path);
        } else
            path.ClearCorners();
        pathIndex = 0;
    }

    void SetPathToNextNode() {
        targetPosition = lapSystem.nodes[nodeIndex].transform.position;
        SetPath(targetPosition);

        nodeIndex++;
        if (nodeIndex == lapSystem.nodes.Length) nodeIndex = 0;
    }

    public void StartMoving() {
        SetPathToNextNode();
    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Track") return;
        //print("Collision!");
        if (collision.relativeVelocity.magnitude > disableInputOnImpactThreshold) {
            lowerSpeedTimer = 0;
            disableTimer += collision.relativeVelocity.magnitude * 0.1f;
        } else if (collision.relativeVelocity.magnitude > decreaseAccelerationOnImpactThreshold) {
            lowerSpeedTimer = Random.Range(0.5f, 1f);
            carController.acceleration *= Random.Range(0.4f, 0.5f);
        }
    }
}
