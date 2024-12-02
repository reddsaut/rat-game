using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class EnemyVision : MonoBehaviour
{
    [Tooltip("Body parts of the rat to raycast to")]
    public Vector3[] goals;
    public NavMeshAgent agent;
    public Vector3[] patrolPoints;
    [SerializeField] private int patrolPointIndex;
    public float viewDistance;
    public float howCloseToSwitchPatrolPoint = 10f;
    public float myFOV = 80f;
    public float mySpeedMultiplier = 1;
    public GameObject player;
    [FormerlySerializedAs("alertSpeed")] [Tooltip("Seconds until enemy goes from alert to chasing")]
    public float alertTime = 5f;
    [Tooltip("How much slower an enemy loses alertness")]
    public float unalertModifier = 4f;
    
    private Collider playerCollider;
    private Transform playerTransform;
    private float howAlert; // 0 is not alert, 1 is hyper alert
    private enum State {Patrol, Alert, Chase};
    [SerializeField] private State myState;

    private void Start()
    {
        myState = State.Patrol;
        playerCollider = player.GetComponent<Collider>();
        playerTransform =  player.transform;
        enemyMovement.target = patrolPoints[patrolPointIndex];
        agent = GetComponent<Transform>().parent.GetComponent<NavMeshAgent>();

    }

    private void Update()
    {
        // Debug.Log(howAlert);
        // goals = new[]{playerTransform.position, playerTransform.position + playerTransform.right, playerTransform.position - playerTransform.right, playerTransform.position + playerTransform.forward,playerTransform.position - playerTransform.forward, playerTransform.position + playerTransform.up,playerTransform.position - playerTransform.up};
        goals = new[] {playerTransform.position};
        
    switch (myState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Alert:
                Alert();
                break;
            case State.Chase:
                Chase();
                break;
            default:
                Debug.LogError("yikes... " + myState + " doesn't exist");
                break;
        }
    }

    private void Alert()
    {
        agent.speed = 1.35f * mySpeedMultiplier;
        
        if (howAlert > 0.9)
        {
            myState = State.Chase;
        }
        else if (howAlert < 0.01)
        {
            myState = State.Patrol;
        }
        else
        {
            bool sawAnything = false;
            foreach (Vector3 t in goals)
            {
                Vector3 directionToRat = (t - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToRat);
                // Debug.Log(directionToRat + "," + angleToTarget + ", " +  transform.forward);

                if (angleToTarget < myFOV / 2)
                {
                    Vector3 directionOfRat = t - transform.position;
                    Physics.Raycast(transform.position, directionOfRat, out RaycastHit hit, viewDistance);
                    if (hit.collider == playerCollider)
                    {
                        howAlert += Time.deltaTime / alertTime;
                        enemyMovement.target = hit.point;
                        sawAnything = true;
                    }
                }
            }
            if (!sawAnything)
            {
                howAlert -= Time.deltaTime / alertTime / unalertModifier;
                if (howAlert < 0)
                {
                    myState = State.Patrol;
                }
            }
        }
    }

    private void Chase() // now enemy knows player exists. should probably "unalert" even slower
    {
        agent.speed = 1.5f* mySpeedMultiplier;

        // TODO: kill player
    }

    private void Patrol()
    {
        agent.speed = 1f * mySpeedMultiplier;

        bool sawAnything = false;
        foreach (Vector3 t in goals)
        {
            Vector3 directionToRat = (t - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToRat);
                // Debug.Log(directionToRat + "," + angleToTarget + ", " +  transform.forward);

                if (angleToTarget < myFOV / 2)
                {
                Physics.Raycast(transform.position, directionToRat, out RaycastHit hit, viewDistance);
                // Debug.Log(hit.collider);
                if (hit.collider == playerCollider)
                {
                    sawAnything = true;
                    Debug.Log("HYPEPEPEP");
                    myState = State.Alert;
                    howAlert += Time.deltaTime / alertTime;
                    enemyMovement.target = hit.point;
                    break;
                    
                    // TODO: give player notice somehow
                }
            }
            // else
            // {
            //     Debug.Log("outside of FOV");
            // }
        }

        if (!sawAnything)
        {
            if (Vector3.Distance(transform.position, patrolPoints[patrolPointIndex]) < howCloseToSwitchPatrolPoint)
            {
                patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Length;
                enemyMovement.target = patrolPoints[patrolPointIndex];
            }
        }
    }
}
