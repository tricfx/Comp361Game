using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] public float attackCooldown = 2.25f;

    [SerializeField] private BossAnimatorController anim;
    [SerializeField] private BossMovement movement;

    private bool isPhase2 = false;

    private Transform player;
    private float lastAttackTime = -Mathf.Infinity;
    private bool stageComplete = false;
    private float stageStartTime = 0f;

    [Header("Attack Timing")]
    [SerializeField] private float stageTimeout = 10f;

    [Header("Ranged Attack")]
    [SerializeField] private GameObject swordProjectilePrefab;
    [SerializeField] private Transform swordSpawnPoint;
    [SerializeField] private float swordFireInterval = 0.2f;
    [SerializeField] private float chargeUpDuration = 1.5f;
    [SerializeField] private float rangedAttackDuration = 3.5f;
    [SerializeField] private float swordSpeed = 25f;
    [SerializeField] private float rangedCooldown = 4f;
    [SerializeField] private float safeZoneRadius = 5f;

    private float lastRangedAttackTime = -Mathf.Infinity;
    public bool IsRangedAttacking { get; private set; } = false;
    public bool CanRangedAttack => Time.time >= lastRangedAttackTime + rangedCooldown && !IsAttacking && !IsRangedAttacking;

    public bool IsAttacking { get; private set; } = false;
    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown && !IsAttacking;

    private void Awake()
    {
        if (!anim) anim = GetComponent<BossAnimatorController>();
        if (!movement) movement = GetComponent<BossMovement>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        isPhase2 = SceneManager.GetActiveScene().name == "Boss-Phase-2";
    }

    public void PerformCloseRangeAttack()
    {


        if (!CanAttack)
        {

            return;
        }


        StopAllCoroutines();

        IsAttacking = true;
        lastAttackTime = Time.time;
        movement?.Stop();

        StartCoroutine(CloseRangeRoutine());
    }

    public void CancelAttack()
    {

        StopAllCoroutines();
        anim.SetAttackStage(0);
        IsAttacking = false;

        lastAttackTime = -Mathf.Infinity;
    }

    private IEnumerator CloseRangeRoutine()
    {
        anim.SetFacing(GetCardinalToPlayer());

        int totalStages = isPhase2 ? 4 : 3;
        for (int stage = 1; stage <= totalStages; stage++)
        {
            Debug.Log($"[BossAttack] CloseRangeRoutine: entering stage {stage}/{totalStages}");
            anim.SetAttackStage(stage);
            stageComplete = false;
            stageStartTime = Time.time;
            yield return new WaitUntil(() => stageComplete || WaitTimeout(stageTimeout));


        }

        OnStageComplete();


        anim.SetAttackStage(0);

        IsAttacking = false;
    }

    private Vector2 GetCardinalToPlayer()
    {
        if (player == null) return Vector2.down;
        Vector2 dir = (player.position - transform.position).normalized;
        return Mathf.Abs(dir.x) >= Mathf.Abs(dir.y)
            ? (dir.x >= 0 ? Vector2.right : Vector2.left)
            : (dir.y >= 0 ? Vector2.up : Vector2.down);
    }

    // Called by animation event on last frame of each stage clip
    public void OnStageComplete()
    {

        stageComplete = true;
    }

    private bool WaitTimeout(float timeout)
    {
        return Time.time - stageStartTime >= timeout;
    }

    public void PerformRangedAttack()
    {


        if (!CanRangedAttack)
        {

            return;
        }

        IsRangedAttacking = true;
        lastRangedAttackTime = Time.time;
        movement?.Stop();

        StartCoroutine(RangedAttackRoutine());
    }

    public void CancelRangedAttack()
    {

        StopCoroutine(nameof(RangedAttackRoutine));
        anim?.SetCharging(false);
        anim?.SetCastIdle(false);
        IsRangedAttacking = false;
    }

    private IEnumerator RangedAttackRoutine()
    {

        anim?.SetCharging(true);
        yield return new WaitForSeconds(chargeUpDuration);
        anim?.SetCharging(false);


        anim?.SetCastIdle(true);

        float elapsed = 0f;
        while (elapsed < rangedAttackDuration)
        {
            float distToPlayer = player != null
                ? Vector2.Distance(transform.position, player.position)
                : float.MaxValue;

            if (distToPlayer > safeZoneRadius)
            {

                FireSwordAtPlayer();
            }
            else
            {

            }

            yield return new WaitForSeconds(swordFireInterval);
            elapsed += swordFireInterval;
        }


        anim?.SetCastIdle(false);
        IsRangedAttacking = false;
    }

    private void FireSwordAtPlayer()
    {
        if (swordProjectilePrefab == null || player == null) return;

        anim?.SetFacing(GetCardinalToPlayer());

        Transform spawnFrom = swordSpawnPoint != null ? swordSpawnPoint : transform;
        Vector2 dir = ((Vector2)player.position - (Vector2)spawnFrom.position).normalized;

        GameObject sword = Instantiate(swordProjectilePrefab, spawnFrom.position, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        sword.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Rigidbody2D swordRb = sword.GetComponent<Rigidbody2D>();
        if (swordRb != null)
            swordRb.linearVelocity = dir * swordSpeed;
    }
}