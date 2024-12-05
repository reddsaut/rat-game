using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class CharacterController : MonoBehaviour
{

    private Transform playerCamera;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private enum State { walk, climb, fall };
    private State state = State.walk;

    [Header("Movement")]
    public float speed = 7;
    public float groundDrag = 5;
    public float airDrag = 7;
    bool wallTarget = false;

    [Header("Ground Check")]
    public float playerHeight = 1;
    public float playerLength = 1;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    bool grounded;

    float fromTouchedWall = 0;
    Vector3 wallNormal;
    Animator animatorRat;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = FindFirstObjectByType<CameraController>().transform;
        animatorRat = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        CapSpeed();
        animatorRat.SetFloat("speed", rb.linearVelocity.magnitude);
        switch (state)
        {
            case State.walk:
                RaycastHit hit = CheckWall();
                if (!GroundCheck())
                {
                    SwitchToFallState();
                }
                HandleMovement();
                if (wallTarget && Input.GetKeyDown(KeyCode.Space))
                {
                    wallNormal = hit.normal;
                    SwitchToClimbState();
                }
                break;
            case State.climb:
                WallMovement();
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SwitchToFallState();
                }
                if (!GroundCheck())
                {
                    if (fromTouchedWall > 0.5f) // a little bit of "coyote time" in the fall off the wallclimb
                    {
                        SwitchToFallState();
                    }
                    else
                    {
                        fromTouchedWall += Time.deltaTime;
                    }
                }
                break;
            case State.fall:
                HandleMovement();
                if (GroundCheck())
                {
                    SwitchToWalkState();
                }
                break;
        }
    }

    private void HandleMovement()
    {
        float moveLeftRight = Input.GetAxis("Horizontal");
        float moveForwardBack = Input.GetAxis("Vertical");
        Vector3 forward = Vector3.ProjectOnPlane(playerCamera.forward, Vector3.up);
        forward.Normalize();
        moveDirection = forward * moveForwardBack + playerCamera.right * moveLeftRight;
        moveDirection.Normalize();
        if (moveDirection.magnitude > 0)
        { // rotate the model to the vector of movement
            Quaternion lookAtQuat = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAtQuat, 20 * Time.deltaTime);
        }
        rb.AddForce(moveDirection * speed, ForceMode.Force);
    }

    private void CapSpeed()
    {
        Vector3 velocity;
        if (rb.useGravity == true)
        {
            velocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }
        else
        {
            velocity = rb.linearVelocity;
        }

        if (velocity.magnitude > speed)
        {
            Vector3 velocityCap = velocity.normalized * speed;
            if (rb.useGravity == true)
            {
                rb.linearVelocity = new Vector3(velocityCap.x, rb.linearVelocity.y, velocityCap.z);
            }
            else
            {
                rb.linearVelocity = velocityCap;
            }
        }
    }

    bool GroundCheck()
    {
        return Physics.Raycast(transform.position, -transform.up, playerHeight * 0.5f + 0.1f, groundLayer);
    }

    // This method works very similarly to the ground movement method, the main difference is that in ground movement the rat moves relative to the camera, and in wall movement it moves relative to universal up.
    void WallMovement()
    {
        float moveLeftRight = Input.GetAxis("Horizontal");
        float moveUpDown = Input.GetAxis("Vertical");
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit)) // adjusts position to be in-line with the wall
        {
            rb.position = Vector3.Lerp(rb.position, hit.point + hit.normal * 0.15f, 5f * Time.deltaTime);
            wallNormal = hit.normal;

        }

        Vector3 right = -Vector3.Cross(Vector3.up, wallNormal); // the wall's right
        moveDirection = Vector3.up * moveUpDown + right.normalized * moveLeftRight;
        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);

        if (moveDirection.magnitude > 0)
        { // rotate the model to the vector of movement
            // float angle = Vector3.SignedAngle(moveDirection, transform.forward, wallNormal);
            // transform.RotateAround(transform.position, wallNormal, angle);
            Quaternion lookAtQuat = Quaternion.LookRotation(moveDirection, wallNormal);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAtQuat, 20 * Time.deltaTime);
        }
    }

    RaycastHit CheckWall()
    {
        RaycastHit hit;
        wallTarget = Physics.Raycast(transform.position, transform.forward, out hit, playerLength * 0.5f + 0.02f, groundLayer);

        if (wallTarget)
        {
            // some visual cue
            Debug.Log("beep");
        }
        else
        {
            // remove visual cue
        }
        return hit;
    }

    void SwitchToFallState()
    {
        rb.useGravity = true;
        rb.linearDamping = 0;
        fromTouchedWall = 0;
        transform.up = Vector3.up;
        state = State.fall;
    }

    void SwitchToWalkState()
    {
        rb.useGravity = true;
        rb.linearDamping = groundDrag;
        state = State.walk;
    }

    void SwitchToClimbState()
    {
        rb.useGravity = false;
        rb.linearDamping = groundDrag;
        fromTouchedWall = 0;
        transform.up = wallNormal;
        state = State.climb;
    }
}