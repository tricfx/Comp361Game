using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class Venti : MonoBehaviour, IAbility
{
    [SerializeField] private Collider2D mapBounds;
    [SerializeField] private float spawnOffset = 4f;
    [SerializeField] private float boundMargin = 3f;
    [SerializeField] private float duration = 7f;
    [SerializeField] private float pullRadius = 6f;
    [SerializeField] private float pullForce = 8f;
    [SerializeField] private float tickRate = 0.5f;
    [SerializeField] private float damageMultiplier = 0.5f;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerHurtbox playerHurtbox;
    [SerializeField] private BossMovement bossMovement = null;

    [SerializeField] private ParticleSystem venti;
    private ParticleSystem ventiInstance;



    private void Awake()
    {
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();
        if (!playerHurtbox)
            playerHurtbox = playerHealth.transform.root.GetComponentInChildren<PlayerHurtbox>();
        if (playerHurtbox == null)
            Debug.LogError("Venti: Could not find PlayerHurtbox on player!");

        Vector3 spawnPosition = playerHealth.transform.position;

        ventiInstance = Instantiate(venti, GetSpawnPosition(), Quaternion.identity);
    }

    public void Do()
    {

        if (ventiInstance != null)
        {
            ventiInstance.Play();
            if (ventiInstance != null)
                ventiInstance.transform.position = GetSpawnPosition();
        }
        StartCoroutine(CharmOverTime());
    }

    public void Dispose()
    {
        StopAllCoroutines();
        if (ventiInstance != null)
            ventiInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Destroy(gameObject);
    }

    private IEnumerator CharmOverTime()
    {
        Vector3 core = ventiInstance.transform.position;
        float total = 0f;
        float tickTimer = 0f;
        int tickDamage = Mathf.Max(1, Mathf.RoundToInt(playerHurtbox.attackDamage * damageMultiplier));


        while (total < duration)
        {
            // Pull all enemies in radius toward core every frame
            Collider2D[] hits = Physics2D.OverlapCircleAll(core, pullRadius);
            foreach (var col in hits)
            {
                if (col.CompareTag("EnemyHitbox"))
                {
                    NewEnemy enemy = col.GetComponentInParent<NewEnemy>();
                    if (enemy != null && enemy.IsAlive)
                    {
                        Vector2 pullDir = (core - enemy.transform.position).normalized;
                        enemy.rb.AddForce(pullDir * pullForce, ForceMode2D.Force);
                    }
                    else
                    {
                        Rigidbody2D bossRb = col.GetComponentInParent<Rigidbody2D>();
                        bossMovement = col.GetComponentInParent<BossMovement>();
                        if (bossRb != null)
                        {
                            Vector2 pullDir = ((Vector2)core - (Vector2)col.transform.position).normalized;
                            bossRb.AddForce(pullDir * pullForce, ForceMode2D.Force);
                            bossMovement.isInVertex = true; // prevent boss from moving itself and fighting the pull


                        }
                    }
                }
            }

            // Tick damage to enemies near the core
            tickTimer += Time.deltaTime;
            if (tickTimer >= tickRate)
            {
                tickTimer = 0f;
                Collider2D[] coreHits = Physics2D.OverlapCircleAll(core, 1.5f);
                foreach (var col in coreHits)
                {
                    if (col.CompareTag("EnemyHitbox"))
                    {
                        NewEnemy enemy = col.GetComponentInParent<NewEnemy>();
                        if (enemy != null)
                            enemy.TakeDamage(tickDamage);
                        else
                            col.GetComponentInParent<BossHitbox>()?.TakeDamage(tickDamage);
                    }
                }
            }

            total += Time.deltaTime;
            yield return null;
        }

        if (ventiInstance != null)
            ventiInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (bossMovement != null)
            bossMovement.isInVertex = false; // allow boss to move again after effect ends

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

