using UnityEngine;
using UnityEngine.Serialization;

public class EnemyVision : MonoBehaviour
{
    [Tooltip("Body parts of the rat to raycast to")]
    public Vector3[] goals;

    public Vector3[] patrolPoints;
    [SerializeField] private int patrolPointIndex;
    public float viewDistance;
    public float howCloseToSwitchPatrolPoint = 10f;
    public float myFOV = 80f;
    public GameObject player;
    [FormerlySerializedAs("alertSpeed")] [Tooltip("Seconds until enemy goes from alert to chasing")]
    public float alertTime = 5f;
    [Tooltip("How much slower an enemy loses alertness")]
    public float unalertModifier = 4f;
    
    private Collider playerCollider;
    private float howAlert; // 0 is not alert, 1 is hyper alert
    private enum State {Patrol, Alert, Chase};
    [SerializeField] private State myState;

    private void Start()
    {
        myState = State.Patrol;
        playerCollider = player.GetComponent<Collider>();
        enemyMovement.target = patrolPoints[patrolPointIndex];
    }

    private void Update()
    {
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
        // TODO: go towards suspicious area
        
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
                if (Vector3.Angle(transform.position, t) < myFOV)
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
        // TODO: kill player
    }

    private void Patrol()
    {
        bool sawAnything = false;
        foreach (Vector3 t in goals)
        {
            if (Vector3.Angle(transform.position, t) < myFOV)
            {
                Vector3 directionOfRat = t - transform.position;
                Physics.Raycast(transform.position, directionOfRat, out RaycastHit hit, viewDistance);
                if (hit.collider == playerCollider)
                {
                    sawAnything = true;
                    myState = State.Alert;
                    howAlert = 0.1f;
                    enemyMovement.target = hit.point;
                    break;
                    
                    // TODO: give player notice somehow
                }
            }
            else
            {
                Debug.Log("outside of FOV");
            }
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
