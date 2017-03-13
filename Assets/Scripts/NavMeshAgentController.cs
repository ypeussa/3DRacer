using UnityEngine;
using System.Collections.Generic;

public class NavMeshAgentController : MonoBehaviour {

    public float randomNodeOffset;
    public float lowerSpeedImpactThreshold = 5f;
    public float disableInputImpactThreshold = 10f;
    public float rotationLerpSpeed = 3f, positionLerpSpeed = 3f;

    public Vector2 RandomSpeedMultiplierMinMax = new Vector2(0.8f, 1.2f);

    Rigidbody rb;
    NodeScript[] nodes;
    UnityEngine.AI.NavMeshAgent agent;
    int nodeIndex;
    int currentLap;
    float disableTimer, lowerSpeedTimer;
    float maxSpeed;
    Vector3 targetPosition;

    void Awake() {
        //find and sort nodes
        nodes = FindObjectsOfType<NodeScript>();
        System.Array.Sort(nodes, (x, y) => x.nodeIndex - y.nodeIndex);

        //get components
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        agent.updatePosition = false;
        agent.updateRotation = false;

        float multiplier = Random.Range(RandomSpeedMultiplierMinMax.x, RandomSpeedMultiplierMinMax.y);
        agent.speed *= multiplier;
        agent.angularSpeed *= multiplier;
        agent.acceleration *= multiplier;
        maxSpeed = agent.speed;
    }

    void FixedUpdate() {
        //impact timers (lazily in FixedUpdate!)
        if (disableTimer > 0) {
            disableTimer -= Time.deltaTime;

            if (disableTimer <= 0) {
                agent.speed = maxSpeed;
                agent.enabled = true;
                agent.SetDestination(targetPosition);
            }
            return;
        } else if (lowerSpeedTimer > 0) {
            lowerSpeedTimer -= Time.deltaTime;

            if (lowerSpeedTimer <= 0) {
                agent.speed = maxSpeed;
            }
        }

        rb.MovePosition(Vector3.Lerp(rb.position, agent.nextPosition, positionLerpSpeed));

        if (agent.velocity.magnitude > 0)
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(agent.desiredVelocity, Vector3.up), Time.deltaTime * rotationLerpSpeed));
    }

    public void SetNextPath() {
        float randomX = Random.Range(-randomNodeOffset, randomNodeOffset);
        targetPosition = nodes[nodeIndex].transform.position + (nodes[nodeIndex].transform.right * randomX);

        if (agent.enabled)
            agent.SetDestination(targetPosition);

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

    public void OnCollisionEnter(Collision collision) {
        if (collision.relativeVelocity.magnitude > disableInputImpactThreshold) {
            lowerSpeedTimer = 0;
            disableTimer += collision.relativeVelocity.magnitude * 0.1f;
            agent.enabled = false;
        } else if (collision.relativeVelocity.magnitude > lowerSpeedImpactThreshold) {
            lowerSpeedTimer = collision.relativeVelocity.magnitude * 0.5f;
            agent.speed *= Random.Range(0.2f, 0.5f);
        }
    }
}
