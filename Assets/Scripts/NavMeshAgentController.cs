using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Unity navmesh agent runs in update. We have to use fixed update for smooth movement as the player car and the camera both use it as well.
/// This requires a bit of wrangling.
/// </summary>
public class NavMeshAgentController : MonoBehaviour {

    public float speed = 15;
    public float angularSpeed = 110;
    public float acceleration = 10;
    public Vector2 RandomSpeedMultiplierMinMax = new Vector2(0.8f, 1.2f);

    public float lowerSpeedImpactThreshold = 5f;
    public float disableInputImpactThreshold = 10f;
    public float randomNodePositionOffset;
    public float PathRefreshInterval = 1f;

    Rigidbody rb;
    NodeScript[] nodes;
    int nodeIndex;
    int currentLap;
    float disableTimer, lowerSpeedTimer;
    float maxSpeed;
    Vector3 targetPosition;
    NavMeshPath path;
    int pathIndex = 0;
    float pathRefreshTimer = 0;

    void Awake() {
        //find and sort nodes
        nodes = FindObjectsOfType<NodeScript>();
        System.Array.Sort(nodes, (x, y) => x.nodeIndex - y.nodeIndex);

        //get components
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();

        float multiplier = Random.Range(RandomSpeedMultiplierMinMax.x, RandomSpeedMultiplierMinMax.y);
        speed *= multiplier;
        angularSpeed *= multiplier;
        acceleration *= multiplier;
        maxSpeed = speed;
    }

    void FixedUpdate() {
        //impact timers
        if (disableTimer > 0) {
            disableTimer -= Time.deltaTime;

            if (disableTimer <= 0) {
                speed = maxSpeed;
                pathRefreshTimer = 0;
            }
            return;
        } else if (lowerSpeedTimer > 0) {
            lowerSpeedTimer -= Time.deltaTime;

            if (lowerSpeedTimer <= 0) {
                speed = maxSpeed;
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
#endif

            var distanceVector = path.corners[pathIndex] - transform.position;
            distanceVector.y = 0;

            if (Vector3.Dot(distanceVector, transform.forward) < rb.velocity.magnitude * 0.5f) {
                pathIndex++;
                if (pathIndex == path.corners.Length) {

                    if (Vector3.Dot(targetPosition - transform.position, transform.forward) < rb.velocity.magnitude * 0.6f) //quick hack
                        SetPathToNextNode();
                    else path.ClearCorners();
                }
            }

            rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(distanceVector.normalized, Vector3.up), Time.deltaTime * angularSpeed));

            rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * speed, acceleration * Time.deltaTime);
        }
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
        float randomX = Random.Range(-randomNodePositionOffset, randomNodePositionOffset);
        targetPosition = nodes[nodeIndex].transform.position + (nodes[nodeIndex].transform.right * randomX);
        SetPath(targetPosition);

        if (nodeIndex < nodes.Length - 1) {
            nodeIndex++;
        } else {
            nodeIndex = 0;
        }
    }

    public void UpdateLap() {
        currentLap++;
    }

    public int GetCurrentLap() {
        return currentLap;
    }

    public int GetNodeIndex() {
        return nodeIndex;
    }

    public float GetDistanceToNode() {
        return Vector3.Distance(transform.position, nodes[nodeIndex].transform.position);
    }

    public void StartMoving() {
        SetPathToNextNode();
    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.relativeVelocity.magnitude > disableInputImpactThreshold) {
            lowerSpeedTimer = 0;
            disableTimer += collision.relativeVelocity.magnitude * 0.1f;
        } else if (collision.relativeVelocity.magnitude > lowerSpeedImpactThreshold) {
            lowerSpeedTimer = Random.Range(0.5f, 1f);
            speed *= Random.Range(0.4f, 0.5f);
        }
    }


}
