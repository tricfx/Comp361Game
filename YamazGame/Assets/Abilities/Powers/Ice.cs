using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class Ice : MonoBehaviour, IAbility
{
    [SerializeField] private Collider2D mapBounds;
    [SerializeField] private float spawnOffset = 1f;
    [SerializeField] private float boundMargin = 3f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float hitRadius = 3f;
    [SerializeField] private float damageMultiplier = 2f;
    [SerializeField] private float slowDuration = 5f;
    [SerializeField] private float slowMultiplier = 0.3f; // 0.3 = 30% of original speed
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerHurtbox playerHurtbox;
    [SerializeField] private BossMovement bossMovement = null;
    [SerializeField] private Vector3 effectRotationEuler = new Vector3(10.93f, -10.05f, -13.7f);
    [SerializeField] private Vector3 effectScale = new Vector3(0.5f, 0.83f, 1f);
    public ParticleSystem ice;
    private ParticleSystem iceInstance;



    private void Awake()
    {
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();
        if (!playerHurtbox)
            playerHurtbox = playerHealth.transform.root.GetComponentInChildren<PlayerHurtbox>();
        if (playerHurtbox == null)
            Debug.LogError("Ice: Could not find PlayerHurtbox on player!");

        iceInstance = Instantiate(ice, GetSpawnPosition(), Quaternion.Euler(effectRotationEuler));
        iceInstance.transform.localScale = effectScale;
    }

    public void Do()
    {

        if (iceInstance != null)
        {
            iceInstance.Play();
            if (iceInstance != null)
                iceInstance.transform.position = GetSpawnPosition();
        }
        StartCoroutine(CharmOverTime());
    }

    public void Dispose()
    {
        StopAllCoroutines();
        if (iceInstance != null)
            iceInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Destroy(gameObject);
    }

    private IEnumerator CharmOverTime()
    {
        Vector3 effectPos = iceInstance.transform.position;

        // One big burst of damage immediately on cast
        int burstDamage = Mathf.Max(1, Mathf.RoundToInt(playerHurtbox.attackDamage * damageMultiplier));
        Collider2D[] hits = Physics2D.OverlapCircleAll(effectPos, hitRadius);
        foreach (var col in hits)
        {
            if (col.CompareTag("EnemyHitbox"))
            {
                NewEnemy enemy = col.GetComponentInParent<NewEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(burstDamage);
                    StartCoroutine(SlowEnemy(enemy));
                }
                else
                {
                    BossHealth boss = col.GetComponentInParent<BossHealth>();
                    bossMovement = col.GetComponentInParent<BossMovement>();
                    if (boss != null)
                    {
                        boss.TakeDamage(burstDamage);
                        StartCoroutine(SlowBoss(bossMovement.moveSpeed));
                    }
                }
            }
        }

        // Wait for visual effect to finish
        float total = 0f;
        while (total < duration)
        {
            total += Time.deltaTime;
            yield return null;
        }

        if (iceInstance != null)
            iceInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private IEnumerator SlowEnemy(NewEnemy enemy)
    {
        enemy.SlowMultiplier = slowMultiplier;
        yield return new WaitForSeconds(slowDuration);
        if (enemy != null)
            enemy.SlowMultiplier = 1f;
    }

    private IEnumerator SlowBoss(float originalSpeed)
    {
        if (bossMovement != null)
        {
            bossMovement.moveSpeed = originalSpeed * slowMultiplier;
            yield return new WaitForSeconds(slowDuration);
            bossMovement.moveSpeed = originalSpeed;
        }
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

