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

    void WallMovement()
    {
        float moveLeftRight = Input.GetAxis ("Horizontal");
        float moveUpDown = Input.GetAxis ("Vertical");
        // i need some sort of info from the wall? so i can find the appropriate horizontal axis
        // also need to find the normal of the wall to do groundchecks to make sure player is staying on the wall
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {            
            rb.position = Vector3.Lerp(rb.position, hit.point + hit.normal * 0.15f, 5f * Time.deltaTime);
            transform.up = hit.normal;
        }

        Vector3 up = Vector3.up;
        Vector3 right = Vector3.right;
        moveDirection = up * moveUpDown; // todo
        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
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