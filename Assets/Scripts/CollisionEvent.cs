using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour {
    public string tag = "";
    public float collisionForce = 10f;
    public float eventMinDelay = 0f;
    public UnityEvent OnCollisionEvent;

    float delayTimer;

    private void OnCollisionEnter(Collision collision)
    {
        if (tag != "" && collision.collider.tag == tag)
        {
            var velocity = collision.relativeVelocity;
            velocity.y = 0;

            if (delayTimer < Time.time && velocity.magnitude > collisionForce)
            {
                OnCollisionEvent.Invoke();
                delayTimer = Time.time + eventMinDelay;
            }
        }
    }
}
