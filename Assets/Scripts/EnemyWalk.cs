using UnityEngine;

public class EnemyWalk : MonoBehaviour
{
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (transform.rotation * Vector3.forward * speed * Time.deltaTime);
    }
}
