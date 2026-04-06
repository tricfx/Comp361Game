using UnityEngine.SceneManagement;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHealth health;
    [SerializeField] private BossMovement movement;
    [SerializeField] private BossAttack attack;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Chase & Attack")]
    [SerializeField] private float chaseStopDistance = 3f;
    [SerializeField] private float attackRange = 3f;

    [Header("Ranged Attack Zones")]
    [SerializeField] private float rangedOuterZone = 20f;
    [SerializeField] private float rangedInnerZone = 5f;
    [Range(0f, 1f)]
    [SerializeField] private float rangedAttackProbability = 0.18f;

    [Header("Roam Settings")]
    [SerializeField] private float roamRadius = 5f;
    [SerializeField] private float moveDuration = 3f;
    [SerializeField] private float minRoamWait = 1f;
    [SerializeField] private float maxRoamWait = 3f;

    private enum State { Roam, Chase, Attack, Ranged }
    private bool rangedRollDone = false;
    private State currentState = State.Roam;
    private State lastLoggedState = (State)(-1); // for logging state changes only

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
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerHealth = playerObj.GetComponent<PlayerHealth>();
            }
        }

        origin = transform.position;
        PickNewRoamTarget();
    }

    private void Update()
    {
        // Log state changes so we can see flicker transitions
        if (currentState != lastLoggedState)
        {

            lastLoggedState = currentState;
        }

        if (health.IsDead || player == null) return;

        if (playerHealth != null && playerHealth.IsDead)
        {
            if (attack.IsAttacking)
            {

                attack.CancelAttack();
            }
            movement.Stop();
            currentState = State.Roam;
            isWaiting = true;
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Roam:
                Roam();
                if (distToPlayer <= detectionRadius)
                {
                    EnterChase();
                }
                break;

            case State.Chase:
                if (distToPlayer > detectionRadius)
                {

                    EnterRoam();
                    break;
                }

                bool rangedAvailable = SceneManager.GetActiveScene().name == "Boss-Phase-2";

                if (rangedAvailable && distToPlayer > rangedInnerZone && distToPlayer <= rangedOuterZone && attack.CanRangedAttack)
                {
                    if (!rangedRollDone)
                    {
                        rangedRollDone = true;
                        float roll = Random.value;

                        if (roll < rangedAttackProbability)
                        {

                            EnterRanged();
                            break;
                        }

                    }
                }
                else
                {
                    rangedRollDone = false;
                }

                Chase(distToPlayer);

                if (distToPlayer <= attackRange && attack.CanAttack)
                {
                    Debug.Log($"[BossAI] In melee range (dist={distToPlayer:F2}) and CanAttack=true — EnterAttack");
                    EnterAttack();
                }
                else if (distToPlayer <= attackRange && !attack.CanAttack)
                {
                    Debug.Log($"[BossAI] In melee range (dist={distToPlayer:F2}) but CanAttack=false (IsAttacking={attack.IsAttacking}, IsRangedAttacking={attack.IsRangedAttacking}) — waiting for cooldown");
                }
                break;

            case State.Attack:
                if (!attack.IsAttacking)
                {

                    EnterChase();
                }
                break;

            case State.Ranged:
                if (!attack.IsRangedAttacking)
                {

                    EnterChase();
                }
                break;
        }
    }

    // ---- State transitions ----

    private void EnterRoam()
    {

        if (attack.IsAttacking) attack.CancelAttack();
        currentState = State.Roam;
        PickNewRoamTarget();
    }

    private void EnterChase()
    {

        currentState = State.Chase;
    }

    private void EnterAttack()
    {

        currentState = State.Attack;
        movement.Stop();
        attack.PerformCloseRangeAttack();
    }

    private void EnterRanged()
    {

        currentState = State.Ranged;
        rangedRollDone = false;
        movement.Stop();
        attack.PerformRangedAttack();
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
        if (!attack.CanAttack)
        {
            movement.Stop();
            movement.CanDash = false;
            return;
        }

        movement.CanDash = true;

        if (distToPlayer <= chaseStopDistance)
        {
            movement.Stop();
            return;
        }

        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float dashChancePerSecond = 0.4f;

        if (distToPlayer > 3f && Random.value < dashChancePerSecond * Time.deltaTime)
            movement.Dash(dir);
        else
            movement.SetMoveDirection(dir);
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

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, rangedOuterZone);

        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, rangedInnerZone);
    }
}