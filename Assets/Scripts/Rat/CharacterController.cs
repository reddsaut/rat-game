using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class CharacterController : MonoBehaviour {

    private Transform playerCamera;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private enum State {walk, wallclimb};
    private State state = State.walk;

    [Header("Movement")]
    public float speed = 7;
    public float groundDrag = 5;
    public float airDrag = 7;
    bool canSwitch = false; 

    [Header("Ground Check")]
    public float playerHeight = 1;
    public float playerLength = 1;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    bool grounded;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = FindFirstObjectByType<CameraController>().transform;
    }

    void Update ()
    {
        RaycastHit hit = CheckCanWallClimb();
        if(canSwitch)
        {
            Debug.Log("beep");
        }
        switch (state)
        {
            case State.walk:
                rb.useGravity = true;
                GroundCheck();
                HandleMovement();
                CapSpeed();
                if(canSwitch && Input.GetKeyDown(KeyCode.H))
                {
                    state = State.wallclimb;
                    //Debug.Log("beep");
                    transform.up = hit.normal;
                }
                break;
            case State.wallclimb:
                rb.useGravity = false;
                WallMovement();
                if(Input.GetKeyDown(KeyCode.H))
                {
                    state = State.walk;
                    transform.up = Vector3.up;
                }
                break;
        }
    }

    private void HandleMovement()
    {

        float moveLeftRight = Input.GetAxis ("Horizontal");
        float moveForwardBack = Input.GetAxis ("Vertical");
        Vector3 forward = Vector3.ProjectOnPlane(playerCamera.forward, Vector3.up);
        forward.Normalize();
        moveDirection = forward * moveForwardBack + playerCamera.right * moveLeftRight;
        moveDirection.Normalize();
        if(moveDirection.magnitude > 0) { // rotate the model to the vector of movement
            float lerpVal = 10f;
            float angle = Vector3.Angle(transform.forward, moveDirection);
            if(angle > 170f && angle < 190f)
            {
                lerpVal = 20f;
            }
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, lerpVal * Time.deltaTime);
        }
        rb.AddForce(moveDirection * speed, ForceMode.Force);
    }

    private void CapSpeed()
    {
        Vector3 velocity;
        if(rb.useGravity == true) {
            velocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        } 
        else
        {
            velocity = rb.linearVelocity;
        }
        
        if(velocity.magnitude > speed)
        {
            Vector3 velocityCap = velocity.normalized * speed;
            if(rb.useGravity == true) {
                rb.linearVelocity = new Vector3(velocityCap.x, rb.linearVelocity.y, velocityCap.z);
            } else {
                rb.linearVelocity = velocityCap;
            }
        }
    }

    void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, -transform.up, playerHeight * 0.5f + 0.1f, groundLayer);

        if(grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }
// This method works very similarly to the ground movement method, the main difference is that in ground movement the rat moves relative to the camera, and in wall movement it moves relative to universal up.
    void WallMovement()
    {
        float moveLeftRight = Input.GetAxis ("Horizontal");
        float moveUpDown = Input.GetAxis ("Vertical");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit)) // adjusts position to be in-line with the wall
        {            
            rb.position = Vector3.Lerp(rb.position, hit.point + hit.normal * 0.15f, 5f * Time.deltaTime);
            transform.up = hit.normal;
        }

        float diff = Mathf.Deg2Rad * Vector3.Angle(Vector3.up, transform.forward);

        Vector3 right = Vector3.RotateTowards(transform.right, Vector3.up, diff, 0);
        moveDirection = Vector3.up * moveUpDown + right.normalized*moveLeftRight;
        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);

        /*if(moveDirection.magnitude > 0) { // rotate the model to the vector of movement
            float lerpVal = 10f;
            float angle = Vector3.Angle(transform.forward, moveDirection);
            if(angle > 170f && angle < 190f)
            {
                lerpVal = 20f;
            }
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, lerpVal * Time.deltaTime);
        }

        Debug.DrawRay(transform.position, right, Color.red);*/
    }

    RaycastHit CheckCanWallClimb()
    {
        RaycastHit hit;
        canSwitch = Physics.Raycast(transform.position, transform.forward, out hit,playerLength * 0.5f + 0.02f, groundLayer);

        if(canSwitch)
        {
            // some visual cue
        }
        else
        {
            // remove visual cue
        }
        return hit;
    }
}