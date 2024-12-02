using UnityEngine;

public class EnemyWalk : MonoBehaviour
{
    public float speed;
    public bool spin;
    public float rotationPerFrame = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        if (spin)
        {
            transform.Rotate(new Vector3(rotationPerFrame * Time.deltaTime, 0, 0));
        }

        // rotating about y axis is just a good way to lerp changing movement direction
    }
}
