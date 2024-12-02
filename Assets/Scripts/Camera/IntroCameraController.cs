using UnityEngine;

public class IntroCameraController : MonoBehaviour
{
    public float rotationPerFrame = -2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.AngleAxis(rotationPerFrame, Vector3.up);
        //transform.Rotate(new Vector3(0, 0, rotationPerFrame * Time.deltaTime));
    }
}
