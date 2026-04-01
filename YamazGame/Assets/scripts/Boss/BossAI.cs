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
    [SerializeField] private float roamRadius = 5f;       // how far from origin it can wander
    [SerializeField] private float minRoamWait = 1f;      // min pause between roam steps
    [SerializeField] private float maxRoamWait = 3f;      // max pause between roam steps

    private Vector2 origin;           // where the boss started
    private Vector2 roamTarget;       // current destination
    private float roamWaitTimer = 0f; // counts down before picking next target
    private bool isRoaming = false;

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
        if (isRoaming)
        {
            // Move toward target
            Vector2 toTarget = roamTarget - (Vector2)transform.position;

            if (toTarget.magnitude <= 0.15f)
            {
                // Arrived — stop and start waiting
                movement.Stop();
                isRoaming = false;
                roamWaitTimer = Random.Range(minRoamWait, maxRoamWait);
            }
            else
            {
                movement.SetMoveDirection(toTarget.normalized);
            }
        }
        else
        {
            // Waiting
            roamWaitTimer -= Time.deltaTime;
            if (roamWaitTimer <= 0f)
                PickNewRoamTarget();
        }
    }

    private void PickNewRoamTarget()
    {
        // Pick a random point within roamRadius of the spawn origin
        Vector2 offset = Random.insideUnitCircle * roamRadius;
        roamTarget = origin + offset;
        isRoaming = true;
    }

    // ---- Phase stubs (fill in later) ----

    private void Phase1()
    {
        // TODO: patrol, basic attacks
    }

    private void Phase2()
    {
        // TODO: enraged behaviour, new attack patterns
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Show roam boundary
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)origin : transform.position, roamRadius);
    }
}