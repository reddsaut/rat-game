using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class RatController : MonoBehaviour
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
    UiManager uiManager;
    float coyoteTime = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = FindFirstObjectByType<CameraController>().transform;
        animatorRat = GetComponentInChildren<Animator>();
        uiManager = FindFirstObjectByType<UiManager>();
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
                    if (fromTouchedWall > coyoteTime) // a little bit of "coyote time" in the fall off the wallclimb
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

    // this method makes sure the model doesn't accelerate into the sun. groundDrag caps this a good bit but isn't perfect.
    private void CapSpeed()
    {
        Vector3 velocity;
        // the useGravity checks make sure that the rat falls as expected, but also that speed can be effectively capped while the rat is walking on walls.
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
            Quaternion lookAtQuat = Quaternion.LookRotation(moveDirection, wallNormal);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAtQuat, 20 * Time.deltaTime);
        }
    }

    RaycastHit CheckWall()
    {
        RaycastHit hit;
        wallTarget = Physics.Raycast(transform.position, transform.forward, out hit, playerLength * 0.5f + 0.02f, groundLayer);

        // consider: adding some sort of wallclimb indicator
        return hit;
    }

    void SwitchToFallState()
    {
        rb.linearDamping = 0;
        fromTouchedWall = 0;
        if(rb.useGravity == false)
        { // i stopped setting transform directions because it leads to some bad situations. loving transform.lookAt and setting with Quaternions
            transform.LookAt(transform.position - transform.up, Vector3.up);
            rb.useGravity = true;
        }
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
        transform.LookAt(transform.position + Vector3.up, wallNormal);
        state = State.climb;
    }

    private void OnTriggerEnter(Collider other)
    {
        ManController manController = other.GetComponent<ManController>();
        // death logic
        if (manController != null)
        {
            Debug.Log("Died");
            uiManager.Death();
        }
    }
}