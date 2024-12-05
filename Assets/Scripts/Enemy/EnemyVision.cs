using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
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
    private AudioSource audioSource;
    [SerializedDictionary("Name","Audio")]
    public SerializedDictionary<string, AudioClip> audioClips;
    int layerMask;
    private float timeSinceLastSpotted;
    
    private Collider playerCollider;
    private Transform playerTransform;
    private float howAlert; // 0 is not alert, 1 is hyper alert
    private enum State {Patrol, Alert, Chase};
    [SerializeField] private State myState;

    private void Start()
    {
        layerMask =~ LayerMask.GetMask("Enemy");
        audioSource = GetComponent<AudioSource>();
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
        agent.speed = 3f * mySpeedMultiplier;
        
        if (howAlert > 0.6)
        {
            myState = State.Chase;
            // audioSource.PlayOneShot(audioClips["Chase"]);
        }
        else
        {
            bool sawAnything = false;
            foreach (Vector3 t in goals)
            {
                Vector3 directionToRat = (t - transform.position).normalized;
                float angleToTarget = Vector3.Angle((transform.forward - transform.up).normalized, directionToRat);
                // Debug.Log(directionToRat + "," + angleToTarget + ", " +  transform.forward);

                if (angleToTarget < myFOV / 2)
                {
                    Vector3 directionOfRat = t - transform.position;
                    Physics.Raycast(transform.position, directionOfRat, out RaycastHit hit, viewDistance, layerMask);
                    if (hit.collider == playerCollider)
                    {
                        timeSinceLastSpotted = 0;
                        howAlert += Time.deltaTime / alertTime;
                        howAlert = Mathf.Min(howAlert, 0.05f);
                        enemyMovement.target = hit.point;
                        sawAnything = true;
                    }
                }
            }
            if (!sawAnything)
            {
                timeSinceLastSpotted += Time.deltaTime / unalertModifier;
                howAlert -= Time.deltaTime / alertTime / unalertModifier;
                if (howAlert < 0 && timeSinceLastSpotted > 1)
                {
                    myState = State.Patrol;
                    audioSource.PlayOneShot(audioClips["Nevermind"]);
                    Debug.Log("nevermind alert");
                }
            }
        }
    }

    private void Chase() // now enemy knows player exists. should probably "unalert" even slower
    {
        agent.speed = 5* mySpeedMultiplier;
        myFOV = 120f;
        
        {
            bool sawAnything = false;
            foreach (Vector3 t in goals)
            {
                Vector3 directionToRat = (t - transform.position).normalized;
                float angleToTarget = Mathf.Min(Vector3.Angle(transform.forward, directionToRat), Vector3.Angle(-transform.up, directionToRat)); // always also looking down lol
                // Debug.Log(directionToRat + "," + angleToTarget + ", " +  transform.forward);

                if (angleToTarget < myFOV / 2)
                {
                    Physics.Raycast(transform.position, directionToRat, out RaycastHit hit, viewDistance, layerMask);
                    if (hit.collider == playerCollider)
                    {
                        timeSinceLastSpotted = 0;
                        howAlert += Time.deltaTime / alertTime;
                        howAlert = Mathf.Min(howAlert, 0.05f);
                        enemyMovement.target = hit.point;
                        sawAnything = true;
                    }
                }
            }
            if (!sawAnything)
            {
                howAlert -= Time.deltaTime / alertTime / unalertModifier;
                if (howAlert < 0.1)
                {
                    myState = State.Patrol;
                    audioSource.PlayOneShot(audioClips["Nevermind"]);
                    Debug.Log("nevermind Chase");
                }
            }
        }

        // TODO: kill player
        if (Vector3.Distance(transform.position, goals[0]) < 3)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Patrol()
    {
        agent.speed = 1f * mySpeedMultiplier;
        bool sawAnything = false;
        
        foreach (Vector3 t in goals)
        {
            Vector3 directionToRat = (t - transform.position).normalized;
                float angleToTarget = Mathf.Min(Vector3.Angle(transform.forward, directionToRat), Vector3.Angle(-transform.up, directionToRat)); // always also looking down lol
                
                // Debug.Log(directionToRat + "," + angleToTarget + ", " +  transform.forward);

                if (angleToTarget < myFOV / 2)
                {
                Physics.Raycast(transform.position, directionToRat, out RaycastHit hit, viewDistance, layerMask);
                // Debug.Log(hit.collider);
                if (hit.collider == playerCollider)
                {
                    sawAnything = true;
                    // Debug.Log("HYPEPEPEP");
                    myState = State.Alert;
                    audioSource.PlayOneShot(audioClips["Alert"], volumeScale:0.1f);
                    Debug.Log("HEY! patrol");
                    howAlert += Time.deltaTime / alertTime;
                    enemyMovement.target = hit.point;
                    break;
                    
                }
            }
        }

        if (!sawAnything)
        {
            enemyMovement.target = patrolPoints[patrolPointIndex];
            if (Vector3.Distance(transform.position, patrolPoints[patrolPointIndex]) < howCloseToSwitchPatrolPoint)
            {
                patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Length;
                enemyMovement.target = patrolPoints[patrolPointIndex];
            }
        }
    }
}
