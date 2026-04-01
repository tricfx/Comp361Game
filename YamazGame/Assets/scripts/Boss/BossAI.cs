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

    [Header("Roam Settings")]
    [SerializeField] private float roamRadius = 5f;
    [SerializeField] private float moveDuration = 3f;   // how long it moves before stopping
    [SerializeField] private float minRoamWait = 1f;
    [SerializeField] private float maxRoamWait = 2f;

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
        if (health.IsDead) return;

        Roam();
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
            // Count down move duration
            moveTimer -= Time.deltaTime;

            Vector2 toTarget = roamTarget - (Vector2)transform.position;

            // Stop either when time is up or when we've arrived
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

    // ---- Phase stubs (fill in later) ----

    private void Phase1()
    {
        // TODO: chase player, trigger close range attack when in range
    }

    private void Phase2()
    {
        // TODO: enraged behaviour, tail swing added to close range combo, new patterns
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)origin : transform.position, roamRadius);
    }
}
