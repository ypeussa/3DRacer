using UnityEngine;

public class SkidMarks : MonoBehaviour {

    public Rigidbody rb;
    public ParticleSystem ps;
    [Range(0, 1f)]
    public float skidMarkThreshold = 0.99f;
    public float skidmarkDuration = 0.25f;
    float skidmarkTimer;

    void Update() {
        float dot = Vector3.Dot(rb.velocity.normalized, transform.forward);

        var velocity = rb.velocity;
        velocity.y = 0;
        if (velocity.magnitude < 1) dot = 0;

        if (dot == 0) return;

        if (dot < skidMarkThreshold) {
            skidmarkTimer = Time.time + skidmarkDuration;
            print(dot);
        }

        if (skidmarkTimer > Time.time) {
            ps.Emit(1);
        }
    }
}
