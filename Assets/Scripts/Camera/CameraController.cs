// <copyright file="CameraController.cs" company="DIS Copenhagen">
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private Transform target;

    public LayerMask obstacleLayerMask;

    public float distance = 10;
    public float height;
    public float minPitch = -80;
    public float maxPitch = 80;
    public float rotationSpeed = 300;
    private float pitch;
    private float yaw;

    void Start()
    {
        target = FindFirstObjectByType<CharacterController>().transform;
        pitch = -20;
        yaw = 0;
    }

    void Update()
    {
        float pitchInput = -Input.GetAxisRaw("Mouse Y");
        float yawInput = Input.GetAxisRaw("Mouse X");

        pitch += pitchInput * Time.deltaTime * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);

        yaw += yawInput * Time.deltaTime * rotationSpeed;
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);

        Vector3 offset = new Vector3(0,1,-1);
        offset = pitchRotation * offset;
        offset = yawRotation * offset;

        transform.position = target.position + offset * distance;
        transform.rotation = Quaternion.LookRotation(-offset, Vector3.up);

        RaycastHit hit;
        Vector3 targetToCamera = transform.position - target.position;
        if (Physics.Raycast(target.position, targetToCamera, out hit, distance, obstacleLayerMask)) {
            transform.position = hit.point;
        }
        transform.position += height * Vector3.up;
    }

}
