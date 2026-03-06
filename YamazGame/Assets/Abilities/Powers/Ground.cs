using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class Ground : MonoBehaviour, IAbility
{
    [SerializeField] private Collider2D mapBounds;
    [SerializeField] private float spawnOffset = 5f;
    [SerializeField] private float boundMargin = 3f;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float hitRadius = 3f;          // Area of effect radius
    [SerializeField] private float damageMultiplier = 1.5f; // Multiplier on player attackDamage
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerHurtbox playerHurtbox;
    [SerializeField] private Vector3 effectRotationEuler = new Vector3(33.31f, -0.84f, 0f);

    public ParticleSystem ground;
    private ParticleSystem groundInstance;



    private void Awake()
    {
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();
        if (!playerHurtbox)
            playerHurtbox = playerHealth.transform.root.GetComponentInChildren<PlayerHurtbox>();
        if (playerHurtbox == null)
            Debug.LogError("Ground: Could not find PlayerHurtbox on player!");

        Quaternion effectRotation = Quaternion.Euler(effectRotationEuler);
        groundInstance = Instantiate(ground, GetSpawnPosition(), effectRotation);
    }

    public void Do()
    {

        if (groundInstance != null)
        {
            groundInstance.Play();
            if (groundInstance != null)
            {
                groundInstance.transform.rotation = Quaternion.Euler(effectRotationEuler);
                groundInstance.transform.position = GetSpawnPosition();
            }
        }
        StartCoroutine(CharmOverTime());
    }

    public void Dispose()
    {
        StopAllCoroutines();
        if (groundInstance != null)
            groundInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Destroy(gameObject);
    }

    private IEnumerator CharmOverTime()
    {
        Vector3 effectPos = groundInstance.transform.position;
        float total = 0f;
        float tickTimer = 0f;
        int burstDamage = Mathf.Max(1, Mathf.RoundToInt(playerHurtbox.attackDamage * damageMultiplier));

        while (total < duration)
        {
            tickTimer += Time.deltaTime;

            // Burst damage once per second
            if (tickTimer >= 1f)
            {
                tickTimer = 0f;

                Collider2D[] hits = Physics2D.OverlapCircleAll(effectPos, hitRadius);
                foreach (var col in hits)
                {
                    if (col.CompareTag("EnemyHitbox"))
                    {
                        NewEnemy enemy = col.GetComponentInParent<NewEnemy>();
                        enemy?.TakeDamage(burstDamage);
                    }
                }
            }

            total += Time.deltaTime;
            yield return null;
        }

        if (groundInstance != null)
            groundInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    private Vector3 GetSpawnPosition()
    {
        var controller = playerHealth.GetComponentInParent<PlayerController2D>();
        Vector2 dir2D = controller != null ? controller.MoveDir : Vector2.right;

        Vector3 dir = new Vector3(dir2D.x, dir2D.y, 0f).normalized;
        Vector3 origin = playerHealth.transform.position;

        float maxCheck = spawnOffset + boundMargin;

        // Raycast and look for the closest "map" collider automatically
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, maxCheck);

        float nearest = float.PositiveInfinity;
        foreach (var hit in hits)
        {
            if (!hit.collider) continue;

            // ignore player�s own colliders
            if (hit.collider.transform.IsChildOf(playerHealth.transform.root)) continue;

            // treat these as map bounds (tilemap walls/floor composites)
            if (hit.collider is TilemapCollider2D || hit.collider is CompositeCollider2D)
            {
                if (hit.distance < nearest) nearest = hit.distance;
            }
        }

        float travel = spawnOffset;

        // If we hit a bound, stop 2 units before it
        if (nearest < float.PositiveInfinity)
            travel = Mathf.Max(0f, nearest - boundMargin);

        return origin + dir * travel;
    }
}

