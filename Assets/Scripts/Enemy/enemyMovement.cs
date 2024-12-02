using UnityEngine;
using UnityEngine.AI;

public class enemyMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public NavMeshAgent agent;
    public static Vector3 target;
    public float speed = 36;
    
    // private EnemyVision.State myState;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // myState = EnemyVision.myState;
        agent.SetDestination(target);
    }
}
