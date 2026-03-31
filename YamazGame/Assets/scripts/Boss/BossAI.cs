using UnityEngine;

// Stub AI — wire up phases and behaviour here later
public class BossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHealth health;
    [SerializeField] private BossMovement movement;
    [SerializeField] private BossAttack attack;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private Transform player;

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
    }

    private void Update()
    {
        if (health.IsDead || player == null) return;

        // TODO: implement AI phases here
        // Example structure:
        // Phase1() if health > 50%
        // Phase2() if health <= 50%
    }

    // ---- Phase stubs ----

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
    }
}