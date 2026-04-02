using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHealth health;
    [SerializeField] private BossMovement movement;
    [SerializeField] private BossAttack attack;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private Transform player;

    [Header("Chase & Attack")]
    [SerializeField] private float chaseStopDistance = 7f;
    [SerializeField] private float attackRange = 4f;

    [Header("Roam Settings")]
    [SerializeField] private float roamRadius = 5f;
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float minRoamWait = 1f;
    [SerializeField] private float maxRoamWait = 2f;

    private enum State { Roam, Chase, Attack }
    private State currentState = State.Roam;

    private Vector2 origin;
    private Vector2 roamTarget;
    private float roamWaitTimer = 0f;
    private float moveTimer = 0f;
    private bool isWaiting = false;

    private void Awake()
    {
        if (!health) health = GetComponent<BossHealth>();
        if (!movement) movement = GetComponent<BossMovement>();
        if (!attack) attack = GetComponent<BossAttack>();

        if (player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        origin = transform.position;
        PickNewRoamTarget();
    }

    private void Update()
    {
        if (health.IsDead || player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Roam:
                Roam();
                if (distToPlayer <= detectionRadius)
                    EnterChase();
                break;

            case State.Chase:
                if (distToPlayer > detectionRadius)
                {
                    EnterRoam();
                    break;
                }
                Chase(distToPlayer);
                // Only enter attack state from Chase — never from within Attack
                if (distToPlayer <= attackRange && attack.CanAttack)
                    EnterAttack();
                break;

            case State.Attack:
                // Just wait for the attack to finish, then go back to Chase
                // PerformCloseRangeAttack handles its own loop internally
                if (!attack.IsAttacking)
                {
                    // Attack fully done — go back to chase, which will re-enter attack if still in range
                    EnterChase();
                }
                break;
        }
    }

    // ---- State enter methods (no recursion risk) ----

    private void EnterRoam()
    {
        if (attack.IsAttacking) attack.CancelAttack();
        currentState = State.Roam;
        PickNewRoamTarget();
    }

    private void EnterChase()
    {
        if (attack.IsAttacking) attack.CancelAttack();
        currentState = State.Chase;
        movement.Stop();
    }

    private void EnterAttack()
    {
        currentState = State.Attack;
        movement.Stop();
        attack.PerformCloseRangeAttack();
    }

    // ---- Roam ----

    private void Roam()
    {
        if (isWaiting)
        {
            roamWaitTimer -= Time.deltaTime;
            if (roamWaitTimer <= 0f)
                PickNewRoamTarget();
        }
        else
        {
            moveTimer -= Time.deltaTime;
            Vector2 toTarget = roamTarget - (Vector2)transform.position;

            if (moveTimer <= 0f || toTarget.magnitude <= 0.15f)
            {
                movement.Stop();
                isWaiting = true;
                roamWaitTimer = Random.Range(minRoamWait, maxRoamWait);
            }
            else
            {
                movement.SetMoveDirection(toTarget.normalized);
            }
        }
    }

    private void PickNewRoamTarget()
    {
        Vector2 offset = Random.insideUnitCircle * roamRadius;
        roamTarget = origin + offset;
        moveTimer = moveDuration;
        isWaiting = false;
    }

    // ---- Chase ----

    private void Chase(float distToPlayer)
    {
        if (distToPlayer <= chaseStopDistance)
            movement.Stop();
        else
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
            movement.SetMoveDirection(dir);
        }
    }

    // ---- Phase stubs ----

    private void Phase2()
    {
        // TODO: enraged behaviour, tail swing added to close range combo, new patterns
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chaseStopDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)origin : transform.position, roamRadius);
    }
}
