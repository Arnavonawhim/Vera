using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform visionPoint;
    
    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 15f;
    [SerializeField] private float visionAngle = 60f;
    [SerializeField] private float visionCheckRate = 10f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask playerMask;
    
    [Header("Alert Settings")]
    [SerializeField] private float alertDuration = 10f;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseTime = 5f;
    [SerializeField] private float losePlayerDistance = 20f;
    
    [Header("Patrol Settings")]
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float patrolRadius = 10f;
    
    private NavMeshAgent agent;
    private Vector3 lastKnownPlayerPosition;
    private float chaseTimer;
    private bool canSeePlayer;
    private float patrolTimer;
    private Vector3 patrolDestination;
    private float visionCheckTimer;
    private bool isAlerted = false;
    private Vector3 alertPosition;
    private float alertTimer;
    
    public enum MonsterState
    {
        Patrol,
        Chase,
        Search
    }
    
    private MonsterState currentState = MonsterState.Patrol;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent not found on Monster!");
            enabled = false;
            return;
        }
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player == null)
            {
                Debug.LogError("Player not found! Make sure Player has 'Player' tag.");
            }
        }
        
        if (visionPoint == null)
        {
            visionPoint = transform;
        }
        
        agent.speed = patrolSpeed;
        agent.acceleration = 10f;
        agent.angularSpeed = 200f;
        
        Invoke(nameof(InitializePatrol), 0.1f);
    }
    
    void InitializePatrol()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            SetNewPatrolDestination();
        }
        else
        {
            Debug.LogError("Monster is not on NavMesh! Make sure you baked the NavMesh.");
        }
    }
    
    void Update()
    {
        if (player == null || agent == null || !agent.isOnNavMesh) return;
        
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            agent.isStopped = true;
            return;
        }
        
        if (isAlerted)
        {
            alertTimer -= Time.deltaTime;
            
            if (alertTimer <= 0)
            {
                isAlerted = false;
            }
            else
            {
                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(alertPosition);
                }
            }
        }
        
        visionCheckTimer += Time.deltaTime;
        if (visionCheckTimer >= 1f / visionCheckRate)
        {
            CheckVision();
            visionCheckTimer = 0f;
        }
        
        switch (currentState)
        {
            case MonsterState.Patrol:
                HandlePatrol();
                break;
            case MonsterState.Chase:
                HandleChase();
                break;
            case MonsterState.Search:
                HandleSearch();
                break;
        }
    }

    void UpdateAnimation()
{
    if (animator == null || agent == null) return;

    // how fast agent is moving
    float speed = agent.velocity.magnitude;

    bool isMoving = speed > 0.1f; // small threshold to avoid jitter

    animator.SetBool("isrunning", isMoving);
}

    
    void CheckVision()
    {
        canSeePlayer = false;
        
        if (player == null) return;
        
        Vector3 directionToPlayer = (player.position - visionPoint.position).normalized;
        float distanceToPlayer = Vector3.Distance(visionPoint.position, player.position);
        
        if (distanceToPlayer <= visionRange)
        {
            float angleToPlayer = Vector3.Angle(visionPoint.forward, directionToPlayer);
            
            if (angleToPlayer <= visionAngle / 2)
            {
                if (!Physics.Raycast(visionPoint.position, directionToPlayer, distanceToPlayer, obstacleMask))
                {
                    canSeePlayer = true;
                    OnPlayerSpotted();
                }
            }
        }
    }
    
    void OnPlayerSpotted()
    {
        if (currentState != MonsterState.Chase)
        {
            currentState = MonsterState.Chase;
            agent.speed = chaseSpeed;
            chaseTimer = chaseTime;
        }
    }
    
    void HandlePatrol()
    {
        if (canSeePlayer)
        {
            return;
        }
        
        if (agent.enabled && agent.isOnNavMesh && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    patrolTimer -= Time.deltaTime;
                    
                    if (patrolTimer <= 0)
                    {
                        SetNewPatrolDestination();
                    }
                }
            }
        }
    }
    
    void HandleChase()
    {
        if (canSeePlayer)
        {
            lastKnownPlayerPosition = player.position;
            chaseTimer = chaseTime;
            
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(player.position);
            }
        }
        else
        {
            chaseTimer -= Time.deltaTime;
            
            if (chaseTimer <= 0)
            {
                currentState = MonsterState.Search;
                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(lastKnownPlayerPosition);
                }
            }
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > losePlayerDistance && !canSeePlayer)
        {
            currentState = MonsterState.Search;
        }
    }
    
    void HandleSearch()
    {
        if (canSeePlayer)
        {
            return;
        }
        
        if (agent.enabled && agent.isOnNavMesh && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    patrolTimer -= Time.deltaTime;
                    
                    if (patrolTimer <= 0)
                    {
                        currentState = MonsterState.Patrol;
                        agent.speed = patrolSpeed;
                        SetNewPatrolDestination();
                    }
                }
            }
        }
    }
    
    void SetNewPatrolDestination()
    {
        if (!agent.isOnNavMesh) return;
        
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolDestination = hit.position;
            agent.SetDestination(patrolDestination);
            patrolTimer = patrolWaitTime;
        }
        else
        {
            Invoke(nameof(SetNewPatrolDestination), 1f);
        }
    }
    
    public bool CanSeePlayer() => canSeePlayer;
    public bool IsChasing() => currentState == MonsterState.Chase;
    public MonsterState GetCurrentState() => currentState;
    
    private void OnDrawGizmosSelected()
    {
        if (visionPoint == null) visionPoint = transform;
        
        Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(visionPoint.position, visionRange);
        
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2, 0) * visionPoint.forward * visionRange;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2, 0) * visionPoint.forward * visionRange;
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(visionPoint.position, visionPoint.position + leftBoundary);
        Gizmos.DrawLine(visionPoint.position, visionPoint.position + rightBoundary);
        
        if (Application.isPlaying && player != null)
        {
            Gizmos.color = canSeePlayer ? Color.green : Color.red;
            Gizmos.DrawLine(visionPoint.position, player.position);
        }
        
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lastKnownPlayerPosition, 1f);
        }
    }

    public void AlertMonster(Vector3 position)
    {
        isAlerted = true;
        alertPosition = position;
        alertTimer = alertDuration;
        
        currentState = MonsterState.Chase;
        agent.speed = chaseSpeed;
        
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(alertPosition);
        }
        
        Debug.Log("Monster alerted to position: " + position);
    }
}
