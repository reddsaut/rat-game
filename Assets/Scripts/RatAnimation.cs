using UnityEngine;

public class RatAnimation : MonoBehaviour
{
    public float animationSpeed;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.speed = animationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
