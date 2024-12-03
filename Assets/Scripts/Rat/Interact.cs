using UnityEngine;

public class Interact : MonoBehaviour
{
    public float interactRadius = 6f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<ManController>() != null)
            {
                Debug.Log("hit " + hitCollider);
                Destroy(hitCollider.gameObject);
            }
        }
    }
}
