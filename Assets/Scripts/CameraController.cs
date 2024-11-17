// <copyright file="CameraController.cs" company="DIS Copenhagen">
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public LayerMask obstacleLayerMask;

    public float distance = 10;
    public float minPitch = -80;
    public float maxPitch = 80;

    public float rotationSpeed = 300;

    private float pitch;
    private float yaw;

    void Start()
    {
        pitch = 45;
        yaw = 0;
    }

    void Update()
    {
        float pitchInput = Input.GetAxisRaw("Mouse Y");
        pitch += pitchInput * Time.deltaTime * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);

        float yawInput = Input.GetAxisRaw("Mouse X");
        yaw += yawInput * Time.deltaTime * rotationSpeed;
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);

        Vector3 offset = new Vector3(0,1,-1);
        offset = pitchRotation * offset;
        offset = yawRotation * offset;

        transform.position = target.position + offset * distance;
        transform.rotation = Quaternion.LookRotation(-offset, Vector3.up);
    }

}
