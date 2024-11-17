using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

    public Transform ballCamera;

    public float acceleration = 10;

    private Rigidbody rb;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update ()
    {
        float moveLeftRight = Input.GetAxis ("Horizontal");
        float moveForwardBack = Input.GetAxis ("Vertical");

        Vector3 xAcceleration = ballCamera.right * moveLeftRight * Time.deltaTime * acceleration;

        Vector3 forward = Vector3.ProjectOnPlane(ballCamera.forward, Vector3.up);
        forward.Normalize();

        Vector3 zAcceleration = forward * moveForwardBack * Time.deltaTime * acceleration;

        rb.linearVelocity += xAcceleration + zAcceleration;
    }
}