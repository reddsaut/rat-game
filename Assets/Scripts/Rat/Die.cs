using UnityEngine;

public class Die : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        enemyMovement manController = other.GetComponent<enemyMovement>();
        if (manController != null)
        {
            Debug.Log("Died");
        }
    }
}
