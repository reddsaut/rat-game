using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class BallController : MonoBehaviour {

    public Transform playerCamera;
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
    }

    void Update ()
    {
        CheckCanWallClimb();
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
                }
                break;
            case State.wallclimb:
                rb.useGravity = false;
                WallMovement();
                if(Input.GetKeyDown(KeyCode.H))
                {
                    state = State.walk;
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

        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
    }

    private void CapSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if(flatVelocity.magnitude > speed)
        {
            Vector3 velocityCap = flatVelocity.normalized * speed;
            rb.linearVelocity = new Vector3(velocityCap.x, rb.linearVelocity.y, velocityCap.z);
        }
    }

    void GroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

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
        Vector3 up = Vector3.up;
        Vector3 right = Vector3.right;
        moveDirection = up * moveUpDown; // todo
        rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
    }

    void CheckCanWallClimb()
    {
        Vector3 forward = Vector3.ProjectOnPlane(playerCamera.forward, Vector3.up);
        canSwitch = Physics.Raycast(transform.position, forward, playerLength * 0.5f + 0.2f, groundLayer);

        if(canSwitch)
        {
            // some visual cue
        }
        else
        {
            // remove visual cue
        }
    }
}